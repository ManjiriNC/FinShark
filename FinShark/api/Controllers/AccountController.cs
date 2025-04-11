using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userMannager;
        private readonly ITokenService _tokenservice;
        public AccountController(UserManager<AppUser> userMannager, ITokenService tokenservice)
        {
            _userMannager = userMannager;
            _tokenservice = tokenservice;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try{
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };

                var createdUser = await _userMannager.CreateAsync(appUser, registerDto.Password);

                if(createdUser.Succeeded)
                {
                    var roleResult = await _userMannager.AddToRoleAsync(appUser, "User");
                    if(roleResult.Succeeded)
                    {
                        return Ok(
                            new  NewUserDto
                            {
                                UserName = appUser.UserName,
                                Email= appUser.Email,
                                Token = _tokenservice.CreateToken(appUser)
                            }
                        );
                    }else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }

            }catch(Exception e)
            {
                return StatusCode(500,e);
            }
        }

        
    }
}