﻿using BE_App_Scores.Models.Authentication.SignUp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BE_App_Scores.Models;
using BE_App_Scores.Service.Models;
using BE_App_Scores.Service.Services;
namespace BE_App_Scores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;


        public AuthenticationController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {

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
            if (await _roleManager.RoleExistsAsync(role))
            {

                var result = await _userManager.CreateAsync(user, registerUser.Password);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User Failed to Create" });
                }

                //Add role to the user ...
                await _userManager.AddToRoleAsync(user, role);


                //Add Token to Verify the email...
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email!}, "Confirmation email link", confirmationLink!);
                _emailService.SendEmail(message);



                return StatusCode(StatusCodes.Status200OK,
                         new Response { Status = "Success", Message = $"User Created $ Email Sent to {user.Email} Successfully!" });

            }
            else {
                return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "This Role doesn't exist" });
            }


            //Assign a role

        }

        [HttpGet]
        public IActionResult TestEmail()
        {
            // var message = new Message(new string[] { "dragoiancaa@gmail.com" }, "Test", "<h1>Altceva</h1>");

            var message = new Message(new string[] { "dragoiancaa@gmail.com" }, "Test", "<h1>Altceva</h1>");


            _emailService.SendEmail(message);
            return StatusCode(StatusCodes.Status200OK,
                   new Response { Status = "Success", Message = "Email Success sent!" });
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, 
                        new Response { Status = "Success", Message = "Email Verified Successfully" });
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError, 
                new Response { Status = "Error", Message = "This user doesn't exist!" });
        }
    }
}
