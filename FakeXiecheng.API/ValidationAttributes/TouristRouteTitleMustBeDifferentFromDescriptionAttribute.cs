using System;
using System.ComponentModel.DataAnnotations;
using FakeXiecheng.API.Dtos;

namespace FakeXiecheng.API.ValidationAttributes
{
	public class TouristRouteTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
	{
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
            )
        {
            var touristRouteDto = (TouristRouteForMainPulationDto)validationContext.ObjectInstance;
            if (touristRouteDto.Title == touristRouteDto.Description)
            {
                return new ValidationResult(
                    "路線名稱必須與路線描述不同",
                    new[] { "TouristRouteForCreationDto" });
            }
            return ValidationResult.Success;
        }
    }
}

