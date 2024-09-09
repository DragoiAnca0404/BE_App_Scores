using BE_App_Scores.Models.Authentication.SignUp;
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
using BE_App_Scores.Utils;
using System.Web;

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
            // Verifică dacă utilizatorul există deja
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);

            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User already Exists!" });
            }

            // Adaugă utilizatorul în baza de date
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

                // Atribuie rolul utilizatorului
                await _userManager.AddToRoleAsync(user, role);

                // Generează token pentru confirmarea emailului
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme);

                var emailBody = $@"
    <h1>Bine ai venit, {user.UserName}!</h1>
    <p>Te rugăm să confirmi adresa ta de email făcând click pe linkul de mai jos:</p>
    <p><a href='{confirmationLink}'>Confirmă Emailul</a></p>
    <br/>
    <p>Suntem încântați să te avem cu noi!</p>
    <p>Între timp, iată un GIF pentru tine:</p>
    <img src='https://i.imgur.com/Eftpra2.gif' alt='GIF de bun venit' />
    <br/>
    <p>Mulțumim și bine ai venit la platforma noastră!</p>
";
                var message = new Message(new string[] { user.Email }, "Bine ai venit la platforma noastră!", emailBody);
                _emailService.SendEmail(message);



                return StatusCode(StatusCodes.Status200OK,
                         new Response { Status = "Success", Message = $"User Created & Email Sent to {user.Email} Successfully!" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "This Role doesn't exist" });
            }
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
            IdentityUser user = null;

            // Verifică dacă input-ul este un e-mail
            if (IsValidEmail(model.UsernameOrEmail))
            {
                user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(model.UsernameOrEmail);
            }

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid username or password." });
            }

            if (!user.EmailConfirmed)
            {
                return Unauthorized(new Response { Status = "Error", Message = "Email not confirmed. Please verify your email." });
            }

            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                // Generate and send the 2FA code
                //var message = new Message(new string[] { user.Email }, "Bine ai venit la platforma noastră!", emailBody);

                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");


                var emailBody = $@"
    <h1>Salutari, {user.UserName}!</h1>
    <p>Mai jos vei regasi codul tau:</p>
    <p>{token}</p>
    <br/>
    <p>Suntem încântați să te avem cu noi!</p>
    <p>Între timp, iată un GIF pentru tine:</p>
    <img src='https://i.imgur.com/Eftpra2.gif' alt='GIF de bun venit' />
    <br/>
    <p>Mulțumim și bine ai venit la platforma noastră!</p>
";

                var message = new Message(new string[] { user.Email }, "Mesaj confirmare - Aplicatie Scoruri", emailBody);
                _emailService.SendEmail(message);

                return Ok(new Response
                {
                    Status = "Success",
                    Message = $"2FA code sent to {user.Email}."
                });
            }

            // Generate JWT if 2FA is not enabled
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
            var jwtToken = GetToken(authClaims);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration = jwtToken.ValidTo
            });
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        [Route("login-2FA")]
        public async Task<IActionResult> LoginWithOTP([FromBody] TwoFactorLoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return Unauthorized(new Response { Status = "Error", Message = "User not found." });
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", model.Code);
            if (!isValid)
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invalid 2FA code." });
            }

            // Generate JWT after successful 2FA validation
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
                var resetPasswordUrl = $"http://localhost:8100/reset-password?token={Uri.EscapeDataString(token)}&email={user.Email}";


                //  var forgotPasswordLink = Url.Action( nameof(ResetPassword), "Authentication", new { token, email = user.Email }, Request.Scheme);
                // var message = new Message(new string[] { user.Email! }, "Forgot Password Link!", forgotPasswordLink! );

                var emailBody = $@"
    <h1>Salutari, {user.UserName}!</h1>
    <p>Te rugăm să confirmi adresa ta de email făcând click pe linkul de mai jos:</p>
    <p><a href='{resetPasswordUrl}'>Link reseteaza parola</a></p>
    <br/>
    <p>Suntem încântați să te avem cu noi!</p>
    <p>Între timp, iată un GIF pentru tine:</p>
    <img src='https://i.imgur.com/Eftpra2.gif' alt='GIF de bun venit' />
    <br/>
    <p>Mulțumim și bine ai venit la platforma noastră!</p>
";

                var message = new Message(new string[] { user.Email! }, "Buna, ti-ai uitat parola.", emailBody);

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
