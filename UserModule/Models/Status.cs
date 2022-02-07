using System.ComponentModel.DataAnnotations;

namespace UserModule.Models
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }

        public string CurrentStatus { get; set; }

      
    }
}
