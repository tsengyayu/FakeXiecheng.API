using System;
using System.ComponentModel.DataAnnotations;
using FakeXiecheng.API.ValidationAttributes;

namespace FakeXiecheng.API.Dtos
{
    [TouristRouteTitleMustBeDifferentFromDescriptionAttribute]
    public abstract class TouristRouteForMainPulationDto
	{
        [Required(ErrorMessage = "title不可為空")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(1500)]
        public virtual string Description { get; set; }
        //計算方式：原價*折扣
        public decimal Price { get; set; }

        //public decimal OriginalPrice { get; set; }

        //public double? DiscountPresent { get; set; }

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

        public ICollection<TouristRoutePictureForCreationDto> TouristRoutePictures { get; set; }
            = new List<TouristRoutePictureForCreationDto>();
    }
}

