using System;
using System.ComponentModel.DataAnnotations;

namespace FakeXiecheng.API.Dtos
{
	public class LoinDto
	{
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
	}
}

