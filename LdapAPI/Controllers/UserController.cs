using Azure.Messaging;
using LDAP.Authuntication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LdapAPI.Core.DTOs;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LdapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LDAPAuth _ldapAuth;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(LDAPAuth ldapAuth, IHttpContextAccessor httpContextAccessor)
        {
            _ldapAuth = ldapAuth;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                Members.members.Clear();

                bool result = _ldapAuth.Login(userDTO.Email, userDTO.Password);
                if(!result)
                {
                    return Unauthorized();
                }

                //var token = CreateJwt(userDTO);

                //return Ok(new TokenApiDTO()
                //{
                //    AccessToken = token
                //});

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userDTO.Email),
                    new Claim(ClaimTypes.Role, LoggedUser.employeeType)
                };

                var identity = new ClaimsIdentity(
                    claims,
                    "LDAP",
                    ClaimTypes.Name,
                    ClaimTypes.Role
                    );

                var principal = new ClaimsPrincipal(identity);

                if (_httpContextAccessor.HttpContext != null)
                {
                    try
                    {
                        await _httpContextAccessor.HttpContext.SignInAsync("LDAP", principal, properties: new AuthenticationProperties() { IsPersistent= true});

                        var loggedEmployee = new 
                        { 
                            commonname = LoggedUser.cn, 
                            surename = LoggedUser.sn, 
                            employeenumber = LoggedUser.employeeNumber,
                            employeetype = LoggedUser.employeeType 
                        };
                        // new { message = "User loged in" , commonname = LoggedUser.cn, surename = LoggedUser.sn, employeenumber = LoggedUser.employeeNumber, role = LoggedUser.employeeType }
                        return Accepted( new {message= "User loged in" , loggedEmployee });
                        //return Accepted(new { users = Members.members.ToArray() });
                        //return Accepted(new { Message = $"Role= {UserConfig.employeeType}" });
                        //return Accepted(new { Groups = UserConfig.Groups.ToArray() });
                    }
                    catch (Exception ex)
                    {
                        return Problem($"Somtinghing went wrong Adding Claim {nameof(Login)} {ex.Message}", statusCode: 500);
                    }
                }
                else
                {
                    return Problem("HttpContext is null");
                }


            }
            catch(Exception ex)
            {
                return Problem($"Somtinghing went wrong in the {nameof(Login)} {ex.Message}", statusCode: 500);
            }
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    await _httpContextAccessor.HttpContext.SignOutAsync("LDAP");
                    return Ok(new { message = "User logged out" });
                }
                else
                {
                    throw new Exception(
                        "For some reasons, HTTP context is null, signing out cannot be performed"
                        );
                }
            }
            catch (Exception ex) 
            {
                return Problem($"Signing-out failed: {nameof(Logout)} {ex.Message}", statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<UserConfig>> GetAllMembers()
        {
            _ldapAuth.GetAllMembers();
            var mbrs = Members.members.ToArray();
            return Ok(mbrs);
        }

    }
}
