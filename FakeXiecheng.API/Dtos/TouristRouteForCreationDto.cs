using System;
using System.ComponentModel.DataAnnotations;
using FakeXiecheng.API.ValidationAttributes;

namespace FakeXiecheng.API.Dtos
{

	public class TouristRouteForCreationDto : TouristRouteForMainPulationDto// : IValidatableObject
	{
		public TouristRouteForCreationDto()
		{
		}
        

        //public IEnumerable<ValidationResult> Validate(
        //    ValidationContext validationContext)
        //{
        //    if(Title == Description)
        //    {
        //        yield return new ValidationResult(
        //            "路線名稱必須與路線描述不同",
        //            new[] { "TouristRouteForCreationDto" });
        //    }
        //}
    }
}

