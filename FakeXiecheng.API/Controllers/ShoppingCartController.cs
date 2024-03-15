using System;
using System.Security.Claims;
using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Helper;
using FakeXiecheng.API.Models;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
	[ApiController]
	[Route("api/shoppingCart")]
	public class ShoppingCartController:ControllerBase
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ITouristRouteRepository _touristRouteRepository;
		private readonly IMapper _mapper;

		public ShoppingCartController(IHttpContextAccessor httpContextAccessor,
			ITouristRouteRepository touristRouteRepository,
            IMapper mapper
        )
		{
			_httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
			_mapper = mapper;

        }

		[HttpGet(Name = "GetShoppingCart")]
		[Authorize(AuthenticationSchemes ="Bearer")]
		public async Task<IActionResult> GetShoppingCart()
		{
			//1. 獲得當前用戶
			var userId = _httpContextAccessor
				.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

			//2. 使用userid獲得購物車
			var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);

			return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
		}

		[HttpPost("item")]
		[Authorize(AuthenticationSchemes ="Bearer")]
		public async Task<IActionResult> AddShoppingCardItem([FromBody] AddShoppingCartItemDto addShoppingCartItemDto)
		{
            //1. 獲得當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //2. 使用userid獲得購物車
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);

			//3. 創建lineItem
			var touristRoute = await _touristRouteRepository
				.GetTouristRouteAsync(addShoppingCartItemDto.TouristRouteId);

			if(touristRoute == null)
			{
				return NotFound("旅遊路線不存在");
			}

			var lineItem = new LineItem()
			{
				TouristRouteId = addShoppingCartItemDto.TouristRouteId,
				ShoppingCartId = shoppingCart.Id,
				OriginalPrice = touristRoute.OriginalPrice,
				DiscountPresent = touristRoute.DiscountPresent
			};

			//4. 添加LineItem，並保存數據庫
			await _touristRouteRepository.AddShoppingCartItem(lineItem);
			await _touristRouteRepository.SaveAsync();

			return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }

		[HttpDelete("items/{itemId}")]
		[Authorize(AuthenticationSchemes ="Bearer")]
		public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] int itemId)
		{
			var lineItem = await _touristRouteRepository
				.GetShoppingCartItemByItemId(itemId);

			if(lineItem == null)
			{
				return NotFound("購物車商品找不到");
			}

			_touristRouteRepository.DeleteShoppingCartItem(lineItem);
			await _touristRouteRepository.SaveAsync();

			return NoContent();

		}

		[HttpDelete("items/({itemIDs})")]
		[Authorize(AuthenticationSchemes ="Bearer")]
		public async Task<IActionResult> RemoveShoppingCartItems(
			[ModelBinder(BinderType = typeof(ArrayModelBinder))]
			[FromRoute] IEnumerable<int> itemIDs
		)
		{
			var lineitems = await _touristRouteRepository
				.GetShoppingCartByIdListAsync(itemIDs);

			_touristRouteRepository.DeleteShoppingCartItems(lineitems);
			await _touristRouteRepository.SaveAsync();

			return NoContent();
		}

		[HttpPost("checkout")]
		[Authorize(AuthenticationSchemes ="Bearer")]
		public async Task<IActionResult> Checkout()
		{
            //1. 獲得當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //2. 使用userid獲得購物車
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);

			//3. 創建訂單
			var order = new Order()
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				State = OrderStateEnum.Pending,
				OrderItems = shoppingCart.ShoppingCartItems,
				CreateDateUTC = DateTime.UtcNow,
				TransactionMetadata = "111"
			};

			shoppingCart.ShoppingCartItems = null;

			//4. 保存數據
			await _touristRouteRepository.AddOrderAsync(order);
			await _touristRouteRepository.SaveAsync();

			//5. return
			return Ok(_mapper.Map<OrderDto>(order));
        }
	}
}

