using AutoMapper;
using Project_3.src.Application.DTOs.DepartmentDTOs;
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
        }

    }
}
