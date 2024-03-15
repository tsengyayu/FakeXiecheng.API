﻿using System;
using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;

namespace FakeXiecheng.API.Profiles
{
	public class OrderProfile:Profile
	{
		public OrderProfile()
		{
			CreateMap<Order, OrderDto>()
				.ForMember(
					dest => dest.State,
					opt =>
					{
						opt.MapFrom(src => src.State.ToString());
					}
				);
		}
		
	}
}

