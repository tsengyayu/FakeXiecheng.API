using System;
using System.Data;
using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
	[Route("api/touristRoutes/{touristRouteId}/pictures")]
	[ApiController]
	public class TouristRoutePicturesController : ControllerBase
	{
		private ITouristRouteRepository _touristRouteRepository;
		private IMapper _mapper;

		public TouristRoutePicturesController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
		{
			_touristRouteRepository = touristRouteRepository ?? throw new ArgumentNullException(nameof(touristRouteRepository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(touristRouteRepository));
        }

		[HttpGet(Name = "GetPictureListForTouristRoute")]
		public async Task<IActionResult> GetPictureListForTouristRoute(Guid touristRouteId)
		{
			if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
			{
				return NotFound("旅遊路線不存在");
			}

			var picturesFromRepo = await _touristRouteRepository.GetPicturesByTouristRouteIdAsync(touristRouteId);
			if (picturesFromRepo == null || picturesFromRepo.Count() <= 0)
			{
				return NotFound("照片不存在");
			}
			return Ok(_mapper.Map<IEnumerable<TouristRoutePictrueDto>>(picturesFromRepo));
		}

		[HttpGet("{pictureId}", Name = "GetPicture")]
		public async Task<IActionResult> GetPicture(Guid touristRouteId,int pictureId)
		{
            if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅遊路線不存在");
            }

			var pictureFromRepo = await _touristRouteRepository.GetPictureAsync(pictureId);
			if (pictureFromRepo == null)
			{
				return NotFound("照片不存在");
			}
			return Ok(_mapper.Map<TouristRoutePictrueDto>(pictureFromRepo));
        }

		[HttpPost(Name = "CreateTouristRoutePicture")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoutePicture(
			[FromRoute] Guid touristRouteId,
			[FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto)
		{
            if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅遊路線不存在");
            }
			var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
			_touristRouteRepository.AddTouristRoutePicture(touristRouteId, pictureModel);
			await _touristRouteRepository.SaveAsync();
			var pictureToReturn = _mapper.Map<TouristRoutePictrueDto>(pictureModel);
			return CreatedAtRoute("GetPicture",
				new
				{
					touristRouteId = pictureModel.TouristRouteId,
					pictureId = pictureModel.Id
				},
				pictureToReturn
				);
        }

		[HttpDelete("{pictureId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeletePicture(
			[FromRoute]Guid touristRouteId,
			[FromRoute]int pictureId
		)
		{
            if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅遊路線不存在");
            }

			var picture = await _touristRouteRepository.GetPictureAsync(pictureId);
			_touristRouteRepository.DeleteTouristRoutePicture(picture);
			await _touristRouteRepository.SaveAsync();
			return NoContent();
        }


	}
}

