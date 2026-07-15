using AutoMapper;
using Project_3.src.API.Extensions;
using Project_3.src.Application.DTOs.DepartmentDTOs;
using Project_3.src.Application.ExceptionHandling;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Interfaces;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.Application.Services.Implementation
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DepartmentService( IRepository<Department> repository,IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }
        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<(IEnumerable<DepartmentDto> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var (departments, totalCount) = await _repository.GetPagedAsync(pageNumber, pageSize);
            var dtos = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
            return (dtos, totalCount);
        }

        public async Task<DepartmentDto> GetByIdAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null)
                throw new DepartmentNotFoundException(id);

            return _mapper.Map<DepartmentDto>(department);
        }
        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {
            if (await _repository.AnyAsync(d => d.Name == dto.Name))
            {
                throw new DuplicateDepartmentException(dto.Name);
            }

            var department = _mapper.Map<Department>(dto);

            await _repository.AddAsync(department);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DepartmentDto>(department);
        }
        public async Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null)
                throw new DepartmentNotFoundException(id);

            if (await _repository.AnyAsync(d =>
                d.Name == dto.Name &&
                d.Id != id))
            {
                throw new DuplicateDepartmentException(dto.Name);
            }

            _mapper.Map(dto, department);

            _repository.Update(department);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);
            if (department == null)
            {
                throw new DepartmentNotFoundException(id);
            }
            _repository.Delete(department);
            await _unitOfWork.SaveChangesAsync();
        }

        
    }
}
