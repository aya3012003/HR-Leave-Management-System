using AutoMapper;
using Project_3.src.Application.DTOs;
using Project_3.src.Application.Models;
using Project_3.src.Application.Services.Interfaces;
using Project_3.src.Infrastructure.Repositories.Interfaces;

namespace Project_3.src.Application.Services.Implementation
{
    public class HolidayService : IHolidayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHolidayApiService _holidayApiService;
        private readonly IMapper _mapper;

        public HolidayService(IUnitOfWork unitOfWork, IHolidayApiService holidayApiService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _holidayApiService = holidayApiService;
            _mapper = mapper;
        }
        public async Task<IEnumerable<HolidayApiDto>> GetByYearAsync(int year, string countryCode = "EG")
        {
            if (await _unitOfWork.Holidays.ExistsForYearAsync( countryCode , year))
            {
                var cached = await _unitOfWork.Holidays.GetByYearAsync( countryCode, year);
                return _mapper.Map<IEnumerable<HolidayApiDto>>(cached);
            }

            var apiHolidays = await _holidayApiService.GetHolidaysAsync(countryCode, year);

            var holidays = apiHolidays.Select(x => new Holiday
            {
                Date = x.Date,
                Name = x.Name,
                CountryCode = x.CountryCode,
                Year = x.Date.Year
            }).ToList();

                foreach (var holiday in holidays)
                {
                    await _unitOfWork.Holidays.AddAsync(holiday);
                }

            await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<IEnumerable<HolidayApiDto>>(holidays);
            

        }
    }
}
