using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace UserModule.Models
{
    public class User
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Please fill-in username.")]
        [StringLength(15, MinimumLength = 8)]
        [RegularExpression(@"[^\s]+", ErrorMessage = "Username must be letters and numbers.")]
        public string Username { get; set; }


        [Display(Name = "Full Name")]
        [StringLength(20)]
        public string? Fullname { get; set; }

        [Required(ErrorMessage = "Please fill-in password.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,15}$", ErrorMessage = "Username must be 8-15 characters, and include letters and numbers.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please fill-in confirm password.")]
        [Compare("Password", ErrorMessage = "The password fields did not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please fill-in valid email address.")]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please fill-in join date.")]
        [Display(Name = "Join Date")]
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }

        [Required(ErrorMessage = "Please select a position.")]
        [Display(Name = "Position Name")]
        [ForeignKey("Position")]
        public int PositionRefId { get; set; }
        public Position Position { get; set; }

        [Required(ErrorMessage = "Please select a team.")]
        [Display(Name = "Team Name")]
        [ForeignKey("Team")]
        public int TeamRefId { get; set; }
        public Team Team { get; set; }

        [Required]
        [Display(Name = "Status Name")]
        [ForeignKey("Status")]
        public int StatusRefId { get; set; }
        public Status Status { get; set; }

        [Required]
        [StringLength(40)]
        public string SecurityPhrase { get; set; }


    }
}
