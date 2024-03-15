using System;
using System.ComponentModel.DataAnnotations;
using FakeXiecheng.API.ValidationAttributes;

namespace FakeXiecheng.API.Dtos
{

    public class TouristRouteForUpdateDto : TouristRouteForMainPulationDto
	{
        
        [Required(ErrorMessage ="更新必備")]
        [MaxLength(1500)]
        public override string Description { get; set; }

    }
}

