using System.ComponentModel.DataAnnotations;

namespace WorkForceManagement.DTO.RequestModels
{
    public class UserRequestModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        public bool IsAdmin { get; set; }
    }
}
