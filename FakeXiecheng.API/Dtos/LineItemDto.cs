﻿using System;
using FakeXiecheng.API.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FakeXiecheng.API.Dtos
{
	public class LineItemDto
	{
        public int Id { get; set; }

        public Guid TouristRouteId { get; set; }

        public TouristRouteDto TouristRoute { get; set; }

        public Guid? ShoppingCartId { get; set; }

        public decimal OriginalPrice { get; set; }

        public double? DiscountPresent { get; set; }
    }
}

