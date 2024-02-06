using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LdapAPI.Core.DTOs;
using LDAP.Authuntication;

namespace LdapAPI.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly LDAPAuth _ldapAuth;
        public AdminController(LDAPAuth ldapAuth)
        {
            _ldapAuth = ldapAuth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                RegisterUser.Rcn = registerDTO.Commonname;
                RegisterUser.Rsn = registerDTO.Surename;
                RegisterUser.RemployeeType = registerDTO.Employeetype;
                RegisterUser.RuserPassword = registerDTO.Password;
                RegisterUser.Ruid = registerDTO.Email;
                RegisterUser.RemployeeNumber = registerDTO.Employeenumber;

                bool result = _ldapAuth.Register();
                if (!result)
                {
                    return Problem($"Member registation went wrong", statusCode: 500);
                }
                else
                {
                    return Ok(new { Message = "Member Register Success!" });
                }
            }
            catch (Exception ex)
            {
                return Problem($"Somtinghing went wrong in the {nameof(Register)} {ex.Message}", statusCode: 500);
            }
        }

        [HttpPost("modify")]
        public async Task<IActionResult> Modify([FromBody] ModifiedDTO modifyDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                modifiedMember.uid = modifyDTO.Email;
                modifiedMember.password = modifyDTO.Password;
                modifiedMember.cn = modifyDTO.Commonname;
                modifiedMember.sn = modifyDTO.Surename;
                modifiedMember.employeeNumber = modifyDTO.Employeenumber;
                modifiedMember.employeeType = modifyDTO.Employeetype;
                modifiedMember.ModifiedSurename = modifyDTO.ModifiedSurename;
                modifiedMember.ModifiedCommonname = modifyDTO.ModifiedCommonname;
                modifiedMember.ModifiedEmployeenumber = modifyDTO.ModifiedEmployeenumber;
                modifiedMember.ModifiedEmployeetype = modifyDTO.ModifiedEmployeetype;

                bool result = _ldapAuth.Modify();
                if (!result)
                {
                    return Problem($"Member Modification went wrong", statusCode: 500);
                }
                else
                {
                    return Ok(new { Message = "Member Modification Success!" });
                }
            }
            catch (Exception ex)
            {
                return Problem($"Somtinghing went wrong in the {nameof(Modify)} {ex.Message}", statusCode: 500);
            }

        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteDTO deleteDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                _ldapAuth.DeleteMember(deleteDTO.Email,deleteDTO.Employeetype);
                return Ok(new { Message = "Member Delete Success!" });
            }
            catch (Exception ex)
            {
                return Problem($"Somtinghing went wrong in the {nameof(Delete)} {ex.Message}", statusCode: 500);
            }

        }


    }
}
