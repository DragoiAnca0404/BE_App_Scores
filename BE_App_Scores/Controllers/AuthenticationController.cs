﻿using BE_App_Scores.Models.Authentication.SignUp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BE_App_Scores.Models;

namespace BE_App_Scores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _configuration=configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {
            //Check user exist 

            //Check user exist
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);

            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User already Exists!" });
            }

            //Add the user in the database

            IdentityUser user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.Username
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);


            return result.Succeeded
                ? StatusCode(StatusCodes.Status201Created,
                new Response { Status = "Success", Message = "User Created Successfully!" })
                : StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status ="Error", Message="User Failed to Create" });
                //Assign a role

        }


    }
}
