using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.Models
{
    public class Holiday : BaseEntity
    {

        public DateOnly Date { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(5)]
        public string CountryCode { get; set; } = "EG";

        public int Year { get; set; }

    }
}
