using System;
using FakeXiecheng.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
	[Route("api")]
	[ApiController]
	public class RootController : ControllerBase
	{
		[HttpGet(Name ="GetRoot")]
		public IActionResult GetRoot()
		{
			var links = new List<LinkDto>();

			//自我連結
			links.Add(
				new LinkDto(
					Url.Link("GetRoot", null),
					"self",
					"GET"
					));
            // 一級連結 旅遊路線 “GET api/touristRoutes”
            links.Add(
                new LinkDto(
                    Url.Link("GetTouristRoutes", null),
                    "get_tourist_routes",
                    "GET"
                ));

            // 一級連結 旅遊路線 “POST api/touristRoutes”
            links.Add(
                new LinkDto(
                    Url.Link("CreateTouristRoute", null),
                    "create_tourist_route",
                    "POST"
                ));

            // 一級連結 購物車 “GET api/orders”
            links.Add(
                new LinkDto(
                    Url.Link("GetShoppingCart", null),
                    "get_shopping_cart",
                    "GET"
                ));

            // 一級連結 訂單 “GET api/shoppingCart”
            links.Add(
                new LinkDto(
                    Url.Link("GetOrders", null),
                    "get_orders",
                    "GET"
                ));

            return Ok(links);
        }
	}
}

