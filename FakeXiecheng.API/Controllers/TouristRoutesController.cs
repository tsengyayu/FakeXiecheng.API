using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Text.RegularExpressions;
using FakeXiecheng.API.ResourceParameters;
using FakeXiecheng.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using FakeXiecheng.API.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FakeXiecheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;

        public TouristRoutesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IPropertyMappingService propertyMappingService)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _propertyMappingService = propertyMappingService;
        }

        private string GenerateTouristRouteResourceURL(
            TouristRouteResourceParameters parameters,
            PaginationResourceParameters parameters2,
            ResourceUrlType type)
        {
            return type switch
            {
                ResourceUrlType.PreviousPage => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters2.PageNumber - 1,
                        pageSize = parameters2.PageSize
                    }),
                ResourceUrlType.NextPage => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters2.PageNumber + 1,
                        pageSize = parameters2.PageSize
                    }),
                _ => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = parameters.Fields,
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters2.PageNumber,
                        pageSize = parameters2.PageSize
                    })
            };
        } 

        //api/touristRoutes?keyword=傳入的參數
        //1.application/json -> 旅遊路線資源
        //2.application/vnd.aleks.hateoas+json
        [HttpGet(Name ="GetTouristRoutes")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutes(
            [FromQuery] TouristRouteResourceParameters parameters,
            [FromQuery] PaginationResourceParameters parameters2,
            [FromQuery(Name ="Accept")] string mediaType)
        {
            
            //Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
            //string operatorType = "";
            //int ratingValue = -1;
            //if(parameters.Rating != null)
            //{
            //    Match match = regex.Match(parameters.Rating);
            //    if (match.Success)
            //    {
            //    operatorType = match.Groups[1].Value;
            //    ratingValue = Int32.Parse(match.Groups[2].Value);
            //    }
            //}
            if(!MediaTypeHeaderValue.
                TryParse(mediaType, out MediaTypeHeaderValue parsedMediatype))
            {
                return BadRequest();
            }
            if (!_propertyMappingService.IsMappingExists<TouristRouteDto, TouristRoute>(parameters.OrderBy))
            {
                return BadRequest("請輸入正確的排序參數");
            }
            if (!_propertyMappingService.IsPropertiesExists<TouristRouteDto>(parameters.Fields))
            {
                return BadRequest("請輸入正確的塑形參數");
            }
            var touristRoutesFromRepo = await _touristRouteRepository
                .GetTouristRoutesAsync(
                parameters.Keyword,
                parameters.RatingOperator,
                parameters.RatingValue,
                parameters2.PageSize,
                parameters2.PageNumber,
                parameters.OrderBy
                );
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("沒有旅遊路線");
            }
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);

            var previousPageLink = touristRoutesFromRepo.HasPrevious
                ? GenerateTouristRouteResourceURL(
                    parameters, parameters2, ResourceUrlType.PreviousPage)
                : null;

            var nextPageLink = touristRoutesFromRepo.HasNext
                ? GenerateTouristRouteResourceURL(
                    parameters, parameters2, ResourceUrlType.PreviousPage)
                : null;

            // x-pagination
            var paginationMetadata = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = touristRoutesFromRepo.TotalCount,
                pageSize = touristRoutesFromRepo.PageSize,
                currentPage = touristRoutesFromRepo.CurrentPage,
                totalPages = touristRoutesFromRepo.TotalPages
            };

            Response.Headers.Add("x-pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var shapedDtoList = touristRoutesDto.ShapeData(parameters.Fields);

            if(parsedMediatype.MediaType == "application/vnd.aleks.hateoas+json")
            {
                var linkDto = CreateLinksForTouristRouteList(parameters, parameters2);

                var shapedDtoWithLinklist = shapedDtoList.Select(t =>
                {
                    var touristRouteDictionary = t as IDictionary<string, object>;
                    var links = CreateLinkForTouristRoute(
                        (Guid)touristRouteDictionary["Id"], null);
                    touristRouteDictionary.Add("links", links);
                    return touristRouteDictionary;
                });

                var result = new
                {
                    value = shapedDtoWithLinklist,
                    links = linkDto
                };

                return Ok(result);
            }
            return Ok(shapedDtoList);
            

        }

        private IEnumerable<LinkDto> CreateLinkForTouristRoute(
            Guid touristRouteId,
            string fields)
        {
            var links = new List<LinkDto>();
            //獲取路線
            links.Add(
                new LinkDto(
                    Url.Link("GetTouristRouteById", new { touristRouteId, fields }),
                    "self",
                    "GET"
                    )
                );

            // 更新
            links.Add(
                new LinkDto(
                    Url.Link("UpdateTouristRoute", new { touristRouteId }),
                    "update",
                    "PUT"
                    )
                );

            // 局部更新 
            links.Add(
                new LinkDto(
                    Url.Link("PartiallyUpdateTouristRoute", new { touristRouteId }),
                    "partially_update",
                    "PATCH")
                );

            // 删除
            links.Add(
                new LinkDto(
                    Url.Link("DeleteTouristRoute", new { touristRouteId }),
                    "delete",
                    "DELETE")
                );

            // 獲取路線圖片
            links.Add(
                new LinkDto(
                    Url.Link("GetPictureListForTouristRoute", new { touristRouteId }),
                    "get_pictures",
                    "GET")
                );

            // 添加新圖片
            links.Add(
                new LinkDto(
                    Url.Link("CreateTouristRoutePicture", new { touristRouteId }),
                    "create_picture",
                    "POST")
                );

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForTouristRouteList(
            TouristRouteResourceParameters paramaters,
            PaginationResourceParameters paramaters2)
        {
            var links = new List<LinkDto>();
            // 添加self，自我連結
            links.Add(new LinkDto(
                    GenerateTouristRouteResourceURL(
                        paramaters, paramaters2, ResourceUrlType.CurrentPage),
                    "self",
                    "GET"
                ));

            // "api/touristRoutes"
            // 添加創建旅遊路線
            links.Add(new LinkDto(
                    Url.Link("CreateTouristRoute", null),
                    "create_tourist_route",
                    "POST"
                ));

            return links;
        }

        [HttpGet("{touristRouteId}", Name = "GetTouristRouteById")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRouteById(
            Guid touristRouteId,
            string fields
        )
        {
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRouteFromRepo == null)
            {
                return NotFound($"旅遊路線{touristRouteId}找不到");
            }
            //var touristRouteDto = new TouristRouteDto()
            //{
            //    Id = touristRouteFromRepo.Id,
            //    Title = touristRouteFromRepo.Title,
            //    Description = touristRouteFromRepo.Description,
            //    Price = touristRouteFromRepo.OriginalPrice * (decimal)(touristRouteFromRepo.DiscountPresent ?? 1),
            //    CreateTime = touristRouteFromRepo.CreateTime,
            //    UpdateTime = touristRouteFromRepo.UpdateTime,
            //    Features = touristRouteFromRepo.Features,
            //    Fees = touristRouteFromRepo.Fees,
            //    Notes = touristRouteFromRepo.Notes,
            //    Rating = touristRouteFromRepo.Rating,
            //    TravelDays = touristRouteFromRepo.TravelDays.ToString(),
            //    TripType = touristRouteFromRepo.TripType.ToString(),
            //    DepartureCity = touristRouteFromRepo.DepartureCity.ToString()

            //};
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            //return Ok(touristRouteDto.ShapeData(fields));

            var linkDtos = CreateLinkForTouristRoute(touristRouteId, fields);

            var result = touristRouteDto.ShapeData(fields)
                as IDictionary<string, object>;
            result.Add("links", linkDtos);

            return Ok(result);
        }

        [HttpPost(Name = "CreateTouristRoute")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);

            var links = CreateLinkForTouristRoute(touristRouteModel.Id, null);
            var result = touristRouteToReturn.ShapeData(null)
                as IDictionary<string, object>;

            result.Add("links", links);


            return CreatedAtAction(
                "GetTouristRouteById",
                new { touristRouteId = result["Id"] },
                result
                );
        }

        [HttpPut("{touristRouteId}", Name ="UpdateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTouristRoute(
            [FromRoute]Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto
        )
        {
            if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅遊路線找不到");
            }

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            //1.映射dto
            //2.更新dto
            //3.映射model
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);

            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{touristRouteId}", Name = "PartiallyUpdateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute(
            [FromRoute]Guid touristRouteId,
            [FromBody]JsonPatchDocument<TouristRouteForUpdateDto> patchDocument
        )
        {
            if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅遊路線找不到");
            }

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPatch, ModelState);
            if (!TryValidateModel(touristRouteToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(touristRouteToPatch, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{touristRouteId}", Name = "DeleteTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅遊路線找不到");
            }
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRoute);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("({touristIDs})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteByIDs(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))][FromRoute]IEnumerable<Guid> touristIDs
        )
        {
            if(touristIDs == null)
            {
                return BadRequest();
            }

            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesByIDListAsync(touristIDs);
            _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
    }
}

