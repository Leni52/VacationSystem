using System.ComponentModel.DataAnnotations;

namespace WorkForceManagement.DTO.RequestDTO
{
    public class UserRequestDTO
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
        [MinLength(6)]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        public bool IsAdmin { get; set; }
    }
}
