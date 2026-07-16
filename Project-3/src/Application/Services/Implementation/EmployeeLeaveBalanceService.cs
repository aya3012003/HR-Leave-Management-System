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
            var balances = await _unitOfWork.LeaveBalances.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<EmployeeLeaveBalanceDto>>(balances);
        }
        public async Task<EmployeeLeaveBalanceDto> CreateAsync(CreateEmployeeLeaveBalanceDto dto)
        {
            var exists = await _unitOfWork.LeaveBalances.AnyAsync(x =>
                x.UserId == dto.UserId &&
                x.LeaveTypeId == dto.LeaveTypeId);

            if (exists)
                throw new DuplicateEmployeeLeaveBalanceException();

            var balance = _mapper.Map<EmployeeLeaveBalance>(dto);

            await _unitOfWork.LeaveBalances.AddAsync(balance);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<EmployeeLeaveBalanceDto>(balance);
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

            return _mapper.Map<EmployeeLeaveBalanceDto>(balance);
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
