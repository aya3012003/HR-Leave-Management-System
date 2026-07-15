using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.Models
{
    public class Department : BaseEntity
    {
        [Required , MaxLength(50)]
        public string Name { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
