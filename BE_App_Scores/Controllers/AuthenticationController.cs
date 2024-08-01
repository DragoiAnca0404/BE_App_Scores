﻿using BE_App_Scores.Models.Authentication.SignUp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BE_App_Scores.Models;
using BE_App_Scores.Service.Models;
using BE_App_Scores.Service.Services;
using BE_App_Scores.Models.Authentication.Login;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.Data;

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
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AuthenticationController> _logger; // Add this line



        public AuthenticationController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            IConfiguration configuration,
             SignInManager<IdentityUser> signInManager,
             ILogger<AuthenticationController> logger
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _configuration = configuration;
            _signInManager = signInManager;
            _logger = logger; // Initialize the logger

        }

        [HttpPost]
        [Route("register")]
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
                UserName = registerUser.Username,
                TwoFactorEnabled = true
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

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                if (user.TwoFactorEnabled)
                {
                    await _signInManager.SignOutAsync();
                    await _signInManager.PasswordSignInAsync(user, model.Password, false, true);


                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                    var message = new Message(new string[] { user.Email! }, "OTP Confrimation", token);
                    _emailService.SendEmail(message);

                    return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });
                }


                    var jwtToken = GetToken(authClaims);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                });
            }
            return Unauthorized();
        }


        [HttpPost]
        [Route("login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("User not found: {username}", username);
                return Unauthorized(new Response { Status = "Error", Message = "User not found." });
            }

            _logger.LogInformation("User found: {username}", username);

            _logger.LogInformation("Attempting 2FA sign in for user: {username} with code: {code}", username, code);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);

            if (!signIn.Succeeded)
            {
                _logger.LogWarning("2FA sign in failed for user: {username}, code: {code}", username, code);

                if (signIn.IsLockedOut)
                {
                    _logger.LogWarning("User is locked out.");
                    return StatusCode(StatusCodes.Status423Locked, new Response { Status = "Error", Message = "User is locked out." });
                }
                if (signIn.IsNotAllowed)
                {
                    _logger.LogWarning("User is not allowed to sign in.");
                    return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = "Error", Message = "User is not allowed to sign in." });
                }
                _logger.LogWarning("2FA sign in failed due to invalid code.");
                return Unauthorized(new Response { Status = "Error", Message = "Invalid 2FA code." });
            }

            _logger.LogInformation("2FA sign in succeeded for user: {username}", username);

            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtToken = GetToken(authClaims);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration = jwtToken.ValidTo
            });
        }



        [HttpPost]
        [AllowAnonymous]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user  = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgotPasswordLink = Url.Action( nameof(ResetPassword), "Authentication", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email! }, "Forgot Password Link!", forgotPasswordLink! );
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                   new Response { Status = "Success", Message = $"Password Changed request is sent on Email {user.Email}. Please check your email." });
            }

            return StatusCode(StatusCodes.Status400BadRequest,
                              new Response { Status = "Error", Message = $"Couldn't send link to e-mail, in order to reset your password" });
        }

        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Token = token, Email = email };

            return Ok(new { model });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);

            if (user != null)
            {

                var resetPassresult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (!resetPassresult.Succeeded) {
                
                foreach (var error in resetPassresult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Ok(ModelState);
                }

                return StatusCode(StatusCodes.Status200OK,
                   new Response { Status = "Success", Message = $"Password has been changed" });
            }

            return StatusCode(StatusCodes.Status400BadRequest,
                              new Response { Status = "Error", Message = $"Couldn't send link to e-mail, try again" });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}
