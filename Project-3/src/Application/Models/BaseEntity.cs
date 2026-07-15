using System.ComponentModel.DataAnnotations;

namespace Project_3.src.Application.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

    }
}
