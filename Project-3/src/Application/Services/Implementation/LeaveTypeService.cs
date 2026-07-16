using AutoMapper;
using Project_3.src.Application.DTOs;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.LeaveTypeDto;
using Project_3.src.Application.ExceptionHandling;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Interfaces;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.Application.Services.Implementation
{
    public class LeaveTypeService : ILeaveTypeService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LeaveTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<LeaveTypeDto>> GetPagedAsync(QueryParams query)
        {
            var result = await _unitOfWork.LeaveTypes.GetPagedAsync(query);

            return new PagedResult<LeaveTypeDto>
            {
                Items = _mapper.Map<List<LeaveTypeDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<LeaveTypeDto> GetByIdAsync(int id)
        {
            var leaveType = await _unitOfWork.LeaveTypes.GetByIdAsync(id);

            if (leaveType == null)
                throw new LeaveTypeNotFoundException(id);

            return _mapper.Map<LeaveTypeDto>(leaveType);
        }

        public async Task<LeaveTypeDto> CreateAsync(CreateLeaveTypeDto dto)
        {
            if (await _unitOfWork.LeaveTypes.AnyAsync(x => x.Name == dto.Name))
                throw new DuplicateLeaveTypeException(dto.Name);

            var leaveType = _mapper.Map<LeaveType>(dto);

            await _unitOfWork.LeaveTypes.AddAsync(leaveType);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LeaveTypeDto>(leaveType);
        }

        public async Task<LeaveTypeDto> UpdateAsync(int id, UpdateLeaveTypeDto dto)
        {
            var leaveType = await _unitOfWork.LeaveTypes.GetByIdAsync(id);

            if (leaveType == null)
                throw new LeaveTypeNotFoundException(id);

            if (await _unitOfWork.LeaveTypes.AnyAsync(x =>
                x.Name == dto.Name &&
                x.Id != id))
            {
                throw new DuplicateLeaveTypeException(dto.Name);
            }

            _mapper.Map(dto, leaveType);

            _unitOfWork.LeaveTypes.Update(leaveType);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LeaveTypeDto>(leaveType);
        }

        public async Task DeleteAsync(int id)
        {
            var leaveType = await _unitOfWork.LeaveTypes.GetByIdAsync(id);

            if (leaveType == null)
                throw new LeaveTypeNotFoundException(id);

            _unitOfWork.LeaveTypes.Delete(leaveType);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}

