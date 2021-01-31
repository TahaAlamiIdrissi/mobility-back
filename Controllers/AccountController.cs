using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Core;

namespace AuthService.Controllers
{
    [Route("/api/[controller]/")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AccountController(SignInManager<IdentityUser> signInManager,
                                 UserManager<IdentityUser> userManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IEmailService emailService,
                                 IConfiguration config)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._emailService = emailService;
            this._roleManager = roleManager;
            this._config = config;
        }
        [Route("[action]")]
        public string Index()
        {
            return "Hello";
        }

        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register([FromBody] RegisterModel model)
        {
            List<IdentityError> errors = new List<IdentityError>();
            // verify the data 
            if (ModelState.IsValid)
            {
                //if the data are valid create the user
                var user = new IdentityUser()
                {
                    UserName = model.Username,
                    Email = model.Email,
                    EmailConfirmed = false,
                };
                var result = await this._userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var role = await this._roleManager.FindByIdAsync(model.Role);
                    if (role == null)
                    {
                        await this._roleManager.CreateAsync(new IdentityRole() { Name = model.Role, NormalizedName = model.Role.ToUpper() });
                    }
                    await this._userManager.AddToRoleAsync(user, role.NormalizedName);
                    /* send email confirmation */
                    // Generate Email token 
                    var mailToken = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                    // Generate Url
                    var link = Url.ActionLink(nameof(VerifyEmail), "account", new { userId = user.Id, mailToken }, Request.Scheme, Request.Host.ToString());
                    // Send the mail
                    await this._emailService.SendAsync("test@test.com", "Email Verification", $"<a href=\"{link}\">Verify Email</a>", true);
                    // and return a 201 created user
                    return CreatedAtAction(nameof(Register), new { Usename = user.UserName, Email = user.Email, EmailConfirmed = user.EmailConfirmed,role });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        errors.Add(error);
                    }
                    // else return a bad request 
                    return BadRequest(errors);
                }
            }
            else
            {
                return BadRequest("Check user info ! error occured when creating the user");
            }
        }
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> VerifyEmail(string userId, string mailToken)
        {
            var user = await this._userManager.FindByIdAsync(userId);
            var result = await this._userManager.ConfirmEmailAsync(user, mailToken);
            List<IdentityError> errors = new List<IdentityError>();

            if (user == null)
                return NotFound();

            if (result.Succeeded)
            {
                // return Redirect("http://localhost:8080/welcome");
                return Ok("Email COnfirmed SUcsseefully");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    errors.Add(error);
                }
                // else return a bad request 
                return BadRequest(errors);
            }
        }

        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
            // check if the user exist
            var user = await this._userManager.FindByNameAsync(model.Username);

            if (user == null)
                return NotFound("User with given username is not found ! ");

            // if the data is valid log the user 
            if (ModelState.IsValid)
            {
                var result = await this._signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded)
                {
                    var roles = this._userManager.GetRolesAsync(user);
                    var token = GenerateJwtSecurityToken(user,roles);
                   /*  HttpContext.Response.Cookies.Append(
                            "token",
                            token,
                            new CookieOptions
                            {
                                HttpOnly = true
                            }
                    ); */
                    return Ok(new { user, token,roles});
                }
                else
                {
                    // else return a bad request 
                    return BadRequest("User with given Password is not Found ! ");
                }
            }
            else
            {
                return BadRequest("Check user info ! error occured when creating the user");
            }

        }
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Logout()
        {
            await this._signInManager.SignOutAsync();
            return Ok("Signed out Successfully !");
        }

        // FOR TESTING PURPOSE ONLY *********************************************************************************
        [HttpGet]
        [Route("user/info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var jwt = Request.Headers["Authorization"].ToString().Split(" ").Skip(1).FirstOrDefault();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt) as JwtSecurityToken;
            var email = token.Claims.First(claim => claim.Type == "email").Value;

            // returining in the user info a data referencing the curent user and a list of roles 
            var user = await this._userManager.FindByEmailAsync(email);
            var roles = await this._userManager.GetRolesAsync(user);
            var element = roles.FirstOrDefault();
            if (user != null)
                return Ok(new {user,element});
                
            else
                return NotFound("The given token is Invalid !");
        }
        // ******************************** *********************************************************************************

        public string GenerateJwtSecurityToken(IdentityUser user,Task<IList<string>> roles)
        {
            // generate the jwt token and send it in the response 
            /* string issuer = null, string audience = null, IEnumerable<Claim> claims = null, DateTime? notBefore = null, DateTime? expires = null, SigningCredentials signingCredentials = null */
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(ClaimTypes.Role,roles.Result.FirstOrDefault())
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._config["Jwt:SecretKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                this._config["Jwt:Issuer"],
                this._config["Jwt:Audience"],
                claims,
                DateTime.Now,
                DateTime.Now.AddHours(2),
                signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }
    }
}