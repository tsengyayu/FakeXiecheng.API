using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FakeXiecheng.API.Controllers
{

	[ApiController]
	[Route("auth")]
	public class AuthenticateController : ControllerBase
	{
        private readonly IConfiguration _configuration;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ITouristRouteRepository _touristRouteRepository;

		public AuthenticateController(
			IConfiguration configuration,
			UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITouristRouteRepository touristRouteRepository
        )
		{
			_configuration = configuration;
			_userManager = userManager;
			_signInManager = signInManager;
			_touristRouteRepository = touristRouteRepository;

        }

        [AllowAnonymous]
		[HttpPost("login")]
		public async Task<IActionResult> login([FromBody] LoinDto loginDto)
		{
			//1. 驗證用戶名密碼
			var loginResult = await _signInManager.PasswordSignInAsync(
				loginDto.Email,
				loginDto.Password,
				false,
				false
				);
			if (!loginResult.Succeeded)
			{
				return BadRequest();
			}

			var user = await _userManager.FindByNameAsync(loginDto.Email);
            //2. 創建JWT（header、payload、signature）
            //header
            var signingAlogrithm = SecurityAlgorithms.HmacSha256;
			//payload
			var claims = new List<Claim>
			{
				//sub
				new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(ClaimTypes.Role, "Admin")
			};
			var roleNames = await _userManager.GetRolesAsync(user);
			foreach(var roleName in roleNames)
			{
				var roleClaim = new Claim(ClaimTypes.Role, roleName);
				claims.Add(roleClaim);
			}


            //signiture(此方法SecretKey必須至少有32 個字元)
            var secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
			var signingKey = new SymmetricSecurityKey(secretByte);
			var signingCrecentials = new SigningCredentials(signingKey, signingAlogrithm);
			
			var token = new JwtSecurityToken(
				issuer: _configuration["Authentication:Issuer"],
				audience: _configuration["Authentication:Audience"],
				claims,
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddDays(1),
				signingCrecentials
				);

			var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

			//3. return 200 ok + JWT
			return Ok(tokenStr);
        }

		[AllowAnonymous]
		[HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        //public async Task<IdentityResult> Register([FromBody] RegisterDto registerDto)
        {
			////1. 使用用戶名創建用戶對象
			var user = new ApplicationUser()
			{
				UserName = registerDto.Email,
				Email = registerDto.Email,
				Address = registerDto.Email //預備修改(將dotnet執行的登陸信息能夠用api執行存入)

				//EmailConfirmed = true
			};


			//2.hash密碼，保存用戶
			var result = await _userManager.CreateAsync(user, registerDto.Password);
			if (!result.Succeeded)
			{
				return BadRequest();
			}

			//3. 初始化購物車
			var shoppingCart = new ShoppingCart()
			{
				Id = Guid.NewGuid(),
				UserId = user.Id
			};
			await _touristRouteRepository.CreateShoppingCart(shoppingCart);
			await _touristRouteRepository.SaveAsync();

			//4. return
			return Ok();
			//除錯用(保留user定義，其他註解)
			//return await _userManager.CreateAsync(user, registerDto.Password);
		}

	}

}

