namespace Project_3.src.Application.Services.Interfaces
{

        public interface ILeaveCalculationService
        {
            Task<int> CalculateLeaveDaysAsync(
                      DateOnly startDate,
                      DateOnly endDate,
                      string countryCode = "EG");
        }
    
}
