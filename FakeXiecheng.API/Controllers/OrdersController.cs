using System;
using System.Security.Claims;
using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.ResourceParameters;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FakeXiecheng.API.Controllers
{
	[ApiController]
	[Route("api/orders")]
	public class OrdersController : ControllerBase
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ITouristRouteRepository _touristRouteRepository;
		private readonly IMapper _mapper;
		private readonly IHttpClientFactory _httpClientFactory;

		public OrdersController(IHttpContextAccessor httpContextAccessor,
			ITouristRouteRepository touristRouteRepository,
			IMapper mapper,
            IHttpClientFactory httpClientFactory
        )
		{
			_httpContextAccessor = httpContextAccessor;
			_touristRouteRepository = touristRouteRepository;
			_mapper = mapper;
			_httpClientFactory = httpClientFactory;
			
		}

		[HttpGet(Name = "GetOrders")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> GetOrders(
			[FromQuery] PaginationResourceParameters parameters
		)
		{
			//1. 獲得當前用戶
			var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

			//2. 使用用戶id來獲取訂單歷史紀錄

			var orders = await _touristRouteRepository.GetOrdersByUserId(userId, parameters.PageSize,parameters.PageNumber);

			return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
		}

		[HttpGet("{orderId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
		{
            //1. 獲得當前用戶
            var userId = _httpContextAccessor
				.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

			var order = await _touristRouteRepository.GetOrderById(orderId);

			return Ok(_mapper.Map<OrderDto>(order));
        }

		[HttpPost("{orderId}/placeOrder")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> PlaceOrder([FromRoute] Guid orderId)
		{
            //1. 獲得當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			//2. 開始處理支付
			var order = await _touristRouteRepository.GetOrderById(orderId);
			order.PaymentProcessing();
			await _touristRouteRepository.SaveAsync();

			//3. 向第三方提交支付請求，等待第三方響應
			var httpClient = _httpClientFactory.CreateClient();
			string url = @"https://localhost:7028/api/FakeVanderPaymentProcess?orderNumber={0}&returnFault=true";
			var response = await httpClient.PostAsync(
				string.Format(url, order.Id, false),
				null
				);
			//4. 提取支付結果，以及支付信息
			bool isApproved = false;
			string transactionMetadata = "";
			if (response.IsSuccessStatusCode)
			{
				transactionMetadata = await response.Content.ReadAsStringAsync();
				var jsonObject = (JObject)JsonConvert.DeserializeObject(transactionMetadata);
				isApproved = jsonObject["approved"].Value<bool>();
			}
			//5. 如果第三方支付成功，完成訂單
			if (isApproved)
			{
				order.PaymentApprove();
			}
			else
			{
				order.PaymentReject();
			}
			order.TransactionMetadata = transactionMetadata;
			await _touristRouteRepository.SaveAsync();

			return Ok(_mapper.Map <OrderDto>(order));
        }
	}
}

