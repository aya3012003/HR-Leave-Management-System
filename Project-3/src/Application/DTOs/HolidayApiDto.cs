namespace Project_3.src.Application.DTOs
{
    public class HolidayApiDto
    {
        public DateOnly Date { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CountryCode { get; set; } = string.Empty;
    }
}
