using System.ComponentModel.DataAnnotations;

namespace LdapAPI.Core.DTOs
{
    public class UserDTO
    {
        [Required]
        [DataType(DataType.Text, ErrorMessage ="Please enter the user name")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage ="Please Enter the password")]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDTO : UserDTO
    {
        [Required(ErrorMessage = "cn is required")]
        public string Commonname { get; set; } = string.Empty;

        [Required(ErrorMessage = "sn is required")]
        public string Surename { get; set; } = string.Empty;

        [Required(ErrorMessage = "employeeType is required")]
        public string Employeetype { get; set; } = string.Empty;

        [Required(ErrorMessage = "employeeNumber is required")]
        public uint Employeenumber { get; set; } 
    }

    public class ModifiedDTO : RegisterDTO
    {
        public string ModifiedCommonname { get; set; } = string.Empty;
        public string ModifiedSurename { get; set; } = string.Empty;
        public string ModifiedEmployeetype { get; set; } = string.Empty;
        public uint ModifiedEmployeenumber { get; set; }

    }

    public class DeleteDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Employeetype { get; set; } = string.Empty;
    }
}
