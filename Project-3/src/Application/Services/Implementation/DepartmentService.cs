using AutoMapper;
using Project_3.src.Application.DTOs;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.DepartmentDTOs;
using Project_3.src.Application.ExceptionHandling;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Interfaces;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.Application.Services.Implementation
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<PagedResult<DepartmentDto>> GetPagedAsync( QueryParams query)
        {
            var result = await _unitOfWork.Departments.GetPagedAsync(query);

            return new PagedResult<DepartmentDto>
            {
                Items = _mapper.Map<List<DepartmentDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<DepartmentDto> GetByIdAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);

            if (department == null)
                throw new DepartmentNotFoundException(id);

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {
            if (await _unitOfWork.Departments.AnyAsync(d => d.Name == dto.Name))
            {
                throw new DuplicateDepartmentException(dto.Name);
            }

            var department = _mapper.Map<Department>(dto);

            await _unitOfWork.Departments.AddAsync(department);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);

            if (department == null)
                throw new DepartmentNotFoundException(id);

            if (await _unitOfWork.Departments.AnyAsync(d =>
                d.Name == dto.Name &&
                d.Id != id))
            {
                throw new DuplicateDepartmentException(dto.Name);
            }

            _mapper.Map(dto, department);

            _unitOfWork.Departments.Update(department);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);

            if (department == null)
                throw new DepartmentNotFoundException(id);

            _unitOfWork.Departments.Delete(department);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}