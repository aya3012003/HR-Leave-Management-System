using Project_3.src.Application.Services.Interfaces;

namespace Project_3.src.Application.Services.Implementation
{
    public class LeaveCalculationService : ILeaveCalculationService
    {
        private readonly IHolidayService _holidayService;

        public LeaveCalculationService(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        public async Task<int> CalculateLeaveDaysAsync(
            DateOnly startDate,
            DateOnly endDate,
            string countryCode = "EG")
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be before start date.");

            var holidayDates = new HashSet<DateOnly>();

            for (int year = startDate.Year; year <= endDate.Year; year++)
            {
                var holidays = await _holidayService.GetByYearAsync(year, countryCode);

                foreach (var holiday in holidays)
                {
                    holidayDates.Add(holiday.Date);
                }
            }

            int workingDays = 0;

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Friday ||
                    date.DayOfWeek == DayOfWeek.Saturday)
                {
                    continue;
                }

                if (holidayDates.Contains(date))
                {
                    continue;
                }

                workingDays++;
            }

            return workingDays;
        }
    }
}
