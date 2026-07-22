using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.LeaveRequestDTOs;
using Project_3.src.Application.ExceptionHandling;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Interfaces;
using Project_3.src.Infrastructure.Repositories.Interfaces;
using Project_3.src.Infrastructure.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using static Project_3.src.Application.Services.Implementation.LeaveRequestService;

namespace Project_3.src.Application.Services.Implementation
{
        public class LeaveRequestService : ILeaveRequestService
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly UserManager<User> _userManager;
            private readonly ILeaveCalculationService _leaveCalculationService;
            private readonly IEmailService _emailService;
        public LeaveRequestService(IUnitOfWork unitOfWork,IMapper mapper,UserManager<User> userManager,ILeaveCalculationService leaveCalculationService,IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _leaveCalculationService = leaveCalculationService;
            _emailService = emailService;
        }

        public async Task<PagedResult<LeaveRequestDto>> GetAllAsync(LeaveRequestQueryParams query)
            {
                var requests = await _unitOfWork.LeaveRequests.GetPagedAsync(query);
                return new PagedResult<LeaveRequestDto>
                {
                    Items = _mapper.Map<List<LeaveRequestDto>>(requests.Items),
                    TotalCount = requests.TotalCount,
                    PageNumber = requests.PageNumber,
                    PageSize = requests.PageSize
                };
            }

            public async Task<PagedResult<LeaveRequestDto>> GetMyRequestsAsync(string userId, LeaveRequestQueryParams query)
            {
                query.UserId = userId;
                return await GetAllAsync(query);
            }

            public async Task<LeaveRequestDto> GetByIdAsync(int id)
            {
                var request = await _unitOfWork.LeaveRequests.GetByIdAsync(id, r => r.User, r => r.LeaveType);
                if (request == null) throw new LeaveRequestNotFoundException(id);
                return _mapper.Map<LeaveRequestDto>(request);
            }

            public async Task<LeaveRequestDto> CreateAsync(string userId, CreateLeaveRequestDto dto)
            {
            if (dto.EndDate < dto.StartDate )
                    throw new ValidationException("End date cannot be before start date.");
            if (dto.StartDate < DateOnly.FromDateTime(DateTime.Today))
                throw new ValidationException("Start date must be in future.");

            int workingDays = await _leaveCalculationService.CalculateLeaveDaysAsync(
                dto.StartDate,
                dto.EndDate); 
            if (workingDays <= 0)
                    throw new ValidationException("Leave request must contain at least one working day.");
            bool hasOverlap = await _unitOfWork.LeaveRequests.HasOverlappingRequestAsync(userId,dto.StartDate,dto.EndDate);

            if (hasOverlap)
                throw new ValidationException(
                    "You already have a leave request during this period.");
            var balance = await _unitOfWork.LeaveBalances.GetByUserAndLeaveTypeAsync(userId, dto.LeaveTypeId);
                if (balance == null || balance.RemainingDays < workingDays)
                    throw new InsufficientLeaveBalanceException();

                var request = new LeaveRequest
                {
                    UserId = userId,
                    LeaveTypeId = dto.LeaveTypeId,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    WorkingDays = workingDays,
                    Reason = dto.Reason,
                    Status = LeaveStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.LeaveRequests.AddAsync(request);
                await _unitOfWork.SaveChangesAsync();

                return await GetByIdAsync(request.Id);
            }

            public async Task<LeaveRequestDto> ApproveAsync(int id, string managerId, LeaveRequestActionDto dto)
            {
                var request = await _unitOfWork.LeaveRequests.GetByIdAsync(id, r => r.User);
                if (request == null) throw new LeaveRequestNotFoundException(id);

                await ValidateManagerDepartmentAccess(managerId, request.User.DepartmentId);

                if (request.Status != LeaveStatus.Pending)
                    throw new InvalidLeaveRequestActionException("Only pending requests can be approved.");

                var balance = await _unitOfWork.LeaveBalances.GetByUserAndLeaveTypeAsync(request.UserId, request.LeaveTypeId);
                if (balance == null || balance.RemainingDays < request.WorkingDays)
                    throw new InsufficientLeaveBalanceException();

                balance.RemainingDays -= request.WorkingDays;
                _unitOfWork.LeaveBalances.Update(balance);

                request.Status = LeaveStatus.Approved;
                request.ManagerComment = dto.ManagerComment;
                _unitOfWork.LeaveRequests.Update(request);
                await _unitOfWork.SaveChangesAsync();
                await _emailService.SendEmailAsync(
                        request.User.Email!,
                        "Leave Request Approved",
                        $"""
                        Hello {request.User.UserName},

                        Your leave request has been approved.

                        Leave Type: {request.LeaveTypeId}
                        From: {request.StartDate}
                        To: {request.EndDate}

                        Manager Comment:
                        {dto.ManagerComment}

                        HR Leave Management System
                        """);

                return await GetByIdAsync(id);
            }

            public async Task<LeaveRequestDto> RejectAsync(int id, string managerId, LeaveRequestActionDto dto)
            {
                var request = await _unitOfWork.LeaveRequests.GetByIdAsync(id, r => r.User);
                if (request == null) throw new LeaveRequestNotFoundException(id);

                await ValidateManagerDepartmentAccess(managerId, request.User.DepartmentId);

                if (request.Status != LeaveStatus.Pending)
                    throw new InvalidLeaveRequestActionException("Only pending requests can be rejected.");

                if (string.IsNullOrWhiteSpace(dto.ManagerComment))
                    throw new ValidationException("Comment required for rejection.");

                request.Status = LeaveStatus.Rejected;
                request.ManagerComment = dto.ManagerComment;
                _unitOfWork.LeaveRequests.Update(request);
                await _unitOfWork.SaveChangesAsync();
                await _emailService.SendEmailAsync(
                    request.User.Email!,
                    "Leave Request Rejected",
                    $"""
                        Hello {request.User.UserName},

                        Unfortunately your leave request has been rejected.

                        Leave Type: {request.LeaveTypeId}
                        From: {request.StartDate}
                        To: {request.EndDate}

                        Manager Comment:
                        {dto.ManagerComment}

                        HR Leave Management System
                        """);
                return await GetByIdAsync(id);
            }

            public async Task<LeaveRequestDto> CancelAsync(int id, string userId)
            {
                var request = await _unitOfWork.LeaveRequests.GetByIdAsync(id);
                if (request == null) throw new LeaveRequestNotFoundException(id);

                if (request.UserId != userId)
                    throw new InvalidLeaveRequestActionException("Can only cancel your own requests.");

                if (request.Status == LeaveStatus.Cancelled || request.Status == LeaveStatus.Rejected)
                    throw new InvalidLeaveRequestActionException("Already cancelled or rejected.");

                if (request.Status == LeaveStatus.Approved)
                {
                    var balance = await _unitOfWork.LeaveBalances.GetByUserAndLeaveTypeAsync(userId, request.LeaveTypeId);
                    if (balance != null)
                    {
                        balance.RemainingDays += request.WorkingDays;
                        _unitOfWork.LeaveBalances.Update(balance);
                    }
                }

                request.Status = LeaveStatus.Cancelled;
                _unitOfWork.LeaveRequests.Update(request);
                await _unitOfWork.SaveChangesAsync();

                return await GetByIdAsync(id);
            }

            private async Task ValidateManagerDepartmentAccess(string managerId, int? employeeDeptId)
            {
                var manager = await _userManager.FindByIdAsync(managerId);
                var roles = await _userManager.GetRolesAsync(manager!);

                if (roles.Contains("Manager") && !roles.Contains("Admin"))
                {
                    if (manager?.DepartmentId != employeeDeptId)
                        throw new UnauthorizedAccessException("You can only review requests for your own department.");
                }
            }

            private int CalculateWorkingDays(DateOnly start, DateOnly end)
            {
                int workingDays = 0;
                for (var date = start; date <= end; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != DayOfWeek.Friday && date.DayOfWeek != DayOfWeek.Saturday)
                        workingDays++;
                }
                return workingDays;
            }
        }
    }
