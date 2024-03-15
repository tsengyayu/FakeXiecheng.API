﻿using System;
using FakeXiecheng.API.Models;

namespace FakeXiecheng.API.Dtos
{
	public class ShoppingCartDto
	{
        public Guid Id { get; set; }
        public string UserId { get; set; }
        //public ApplicationUser User { get; set; }
        public ICollection<LineItemDto> ShoppingCartItems { get; set; }
    }
}

