﻿using System;
using FakeXiecheng.API.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FakeXiecheng.API.Dtos
{
	public class TouristRouteDto
	{
		public TouristRouteDto()
		{
		}

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        //計算方式：原價*折扣
        public decimal Price { get; set; }

        public decimal OriginalPrice { get; set; }

        public double? DiscountPresent { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public DateTime? DepartureTime { get; set; }

        public string Features { get; set; }

        public string Fees { get; set; }

        public string Notes { get; set; }

        public double Rating { get; set; }

        public string TravelDays { get; set; }

        public string TripType { get; set; }

        public string DepartureCity { get; set; }

        public ICollection<TouristRoutePictrueDto> TouristRoutePictures { get; set; }
    }
}

