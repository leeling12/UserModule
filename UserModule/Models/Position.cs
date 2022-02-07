using System.ComponentModel.DataAnnotations;

namespace UserModule.Models
{
    public class Position
    {
        [Key]
        public int PositionId { get; set; }

        [Required]
        public string PositionName { get; set; }

        public ICollection<User> Users { get; set; }

    }
}
