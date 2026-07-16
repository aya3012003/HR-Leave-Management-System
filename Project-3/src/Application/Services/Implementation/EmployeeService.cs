using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_3.src.Application.DTOs.Common;
using Project_3.src.Application.DTOs.EmployeeDTOs;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Interfaces;
using Project_3.src.Infrastructure.Data.Context;

namespace Project_3.src.Application.Services.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public EmployeeService(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(int page, int pageSize, int? deptId, string? search)
        {
            var query = _userManager.Users.Include(u => u.Department).AsQueryable();

            if (deptId.HasValue)
                query = query.Where(u => u.DepartmentId == deptId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u => u.FirstName.Contains(search) ||
                                         u.LastName.Contains(search) ||
                                         u.Email.Contains(search));

            var totalCount = await query.CountAsync();
            var users = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var dtos = new List<EmployeeDto>();
            foreach (var user in users)
            {
                dtos.Add(await MapToDtoAsync(user));
            }

            return new PagedResult<EmployeeDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(string id)
        {
            var user = await _userManager.Users.Include(u => u.Department).FirstOrDefaultAsync(u => u.Id == id);
            return user == null ? null : await MapToDtoAsync(user);
        }

        public async Task<EmployeeDto?> GetEmployeeProfileAsync(string userId)
        {
            return await GetEmployeeByIdAsync(userId);
        }

        public async Task<EmployeeDto?> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DepartmentId = dto.DepartmentId,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return null;

            var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user); // Rollback
                return null;
            }

            return await MapToDtoAsync(await _userManager.Users.Include(u => u.Department).FirstAsync(u => u.Id == user.Id));
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(string id, UpdateEmployeeDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.FirstName)) user.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.LastName)) user.LastName = dto.LastName;
            if (dto.DepartmentId.HasValue) user.DepartmentId = dto.DepartmentId.Value;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return null;

            return await MapToDtoAsync(await _userManager.Users.Include(u => u.Department).FirstAsync(u => u.Id == user.Id));
        }

        public async Task<bool> DeleteEmployeeAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        // Helper method to map User entity to EmployeeDto
        private async Task<EmployeeDto> MapToDtoAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new EmployeeDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email ?? string.Empty,
                DepartmentId = user.DepartmentId,
                DepartmentName = user.Department?.Name,
                Roles = roles.ToList()
            };
        }
    }
}