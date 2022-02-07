using System.ComponentModel.DataAnnotations;

namespace UserModule.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required]
        public string TeamName { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
