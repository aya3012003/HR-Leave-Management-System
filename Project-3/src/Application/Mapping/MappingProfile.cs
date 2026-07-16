using AutoMapper;
using Project_3.src.Application.DTOs.DepartmentDTOs;
using Project_3.src.Application.DTOs.EmployeeLeaveBalanceDtos;
using Project_3.src.Application.DTOs.LeaveTypeDto;
using Project_3.src.Application.Models;

namespace Project_3.src.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<CreateDepartmentDto, Department>().ReverseMap();
            CreateMap<UpdateDepartmentDto, Department>().ReverseMap();
            CreateMap<EmployeeLeaveBalance, EmployeeLeaveBalanceDto>()
                .ForMember(dest => dest.EmployeeName,
                    opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.LeaveTypeName,
                    opt => opt.MapFrom(src => src.LeaveType.Name));

            CreateMap<CreateEmployeeLeaveBalanceDto, EmployeeLeaveBalance>();

            CreateMap<UpdateEmployeeLeaveBalanceDto, EmployeeLeaveBalance>();

            CreateMap<LeaveType, LeaveTypeDto>();
            CreateMap<CreateLeaveTypeDto, LeaveType>();
            CreateMap<UpdateLeaveTypeDto, LeaveType>();

        }

    }
}
