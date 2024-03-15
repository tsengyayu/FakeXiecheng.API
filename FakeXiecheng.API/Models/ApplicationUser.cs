using System;
using Microsoft.AspNetCore.Identity;

namespace FakeXiecheng.API.Models
{
	public class ApplicationUser: IdentityUser
	{
		public string Address { get; set; }
		//shoppingCart
		public ShoppingCart ShoppingCart { get; set; }
		//Orders
		public ICollection<Order> Orders { get; set; }
		public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }
	}
}

