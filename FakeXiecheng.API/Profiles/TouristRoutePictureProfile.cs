﻿using System;
using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;

namespace FakeXiecheng.API.Profiles
{
	public class TouristRoutePictureProfile : Profile
	{
		public TouristRoutePictureProfile()
		{
			CreateMap<TouristRoutePicture, TouristRoutePictrueDto>();
			CreateMap<TouristRoutePictureForCreationDto, TouristRoutePicture>();
			CreateMap<TouristRoutePicture, TouristRoutePictureForCreationDto>();

		}
	}
}

