using AutoMapper;
using Project_3.src.Application.DTOs;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.EmployeeLeaveBalanceDtos;
using Project_3.src.Application.ExceptionHandling;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Interfaces;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.Application.Services.Implementation
{
    public class EmployeeLeaveBalanceService : IEmployeeLeaveBalanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeLeaveBalanceService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<EmployeeLeaveBalanceDto>> GetPagedAsync(EmployeeLeaveBalanceQueryParams query)
        {
            var result = await _unitOfWork.LeaveBalances.GetPagedAsync(query);

            return new PagedResult<EmployeeLeaveBalanceDto>
            {
                Items = _mapper.Map<List<EmployeeLeaveBalanceDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }
        public async Task<EmployeeLeaveBalanceDto> GetByIdAsync(int id)
        {
            var balance = await _unitOfWork.LeaveBalances.GetByIdAsync(
                id,
                x => x.User,
                x => x.LeaveType);

            if (balance == null)
                throw new EmployeeLeaveBalanceNotFoundException(id);

            return _mapper.Map<EmployeeLeaveBalanceDto>(balance);
        }
        public async Task<IEnumerable<EmployeeLeaveBalanceDto>> GetMyBalancesAsync(string userId)
        {
            // Fetch the user's current balances
            var balances = await _unitOfWork.LeaveBalances.GetByUserIdAsync(userId);

            // Fetch all leave types configured in the system
            // (Assuming you have a method like GetAllAsync in your LeaveTypes repository)
            var allLeaveTypes = await _unitOfWork.LeaveTypes.GetAllAsync();

            // Find any leave types the user DOES NOT have a balance for yet
            var missingLeaveTypes = allLeaveTypes
                .Where(lt => !balances.Any(b => b.LeaveTypeId == lt.Id))
                .ToList();

            // If they are missing balances, create them instantly
            if (missingLeaveTypes.Any())
            {
                foreach (var lt in missingLeaveTypes)
                {
                    var newBalance = new EmployeeLeaveBalance
                    {
                        UserId = userId,
                        LeaveTypeId = lt.Id,
                        RemainingDays = lt.DefaultDays // Grant default quota
                    };
                    await _unitOfWork.LeaveBalances.AddAsync(newBalance);
                }

                await _unitOfWork.SaveChangesAsync();

                // Re-fetch to ensure we have the new records (with Navigation properties included)
                balances = await _unitOfWork.LeaveBalances.GetByUserIdAsync(userId);
            }

            return _mapper.Map<IEnumerable<EmployeeLeaveBalanceDto>>(balances);
        }

        public async Task<EmployeeLeaveBalanceDto> UpdateAsync(
            int id,
            UpdateEmployeeLeaveBalanceDto dto)
        {
            var balance = await _unitOfWork.LeaveBalances.GetByIdAsync(id);

            if (balance == null)
                throw new EmployeeLeaveBalanceNotFoundException(id);

            _mapper.Map(dto, balance);

            _unitOfWork.LeaveBalances.Update(balance);
            await _unitOfWork.SaveChangesAsync();

            balance = await _unitOfWork.LeaveBalances.GetByIdAsync(
                id,
                x => x.User,
                x => x.LeaveType);

            return _mapper.Map<EmployeeLeaveBalanceDto>(balance!);
        }

        public async Task DeleteAsync(int id)
        {
            var balance = await _unitOfWork.LeaveBalances.GetByIdAsync(id);

            if (balance == null)
                throw new EmployeeLeaveBalanceNotFoundException(id);

            _unitOfWork.LeaveBalances.Delete(balance);
            await _unitOfWork.SaveChangesAsync();
        }

       
    }
}
