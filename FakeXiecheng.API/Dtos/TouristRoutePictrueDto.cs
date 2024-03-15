using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FakeXiecheng.API.Dtos
{
	public class TouristRoutePictrueDto
	{
		public TouristRoutePictrueDto()
		{
		}

        public int Id { get; set; }

        public string Url { get; set; }

        public Guid TouristRouteId { get; set; }
    }
}

