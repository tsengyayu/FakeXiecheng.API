//數據倉庫接口
using System;
using FakeXiecheng.API.Models;
using System.Threading.Tasks;
using FakeXiecheng.API.Helper;

namespace FakeXiecheng.API.Services
{
	public interface ITouristRouteRepository
	{
		Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(
			string keyword, string ratingOperator, int? ratingValue,
			int pageSize, int pageNumber, string orderBy); //返回所有旅遊路線
		Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId); //返回一組旅遊路線
		Task<bool> TouristRouteExistsAsync(Guid touristRouteId);
		Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId);
		Task<TouristRoutePicture> GetPictureAsync(int pictureId);
		void AddTouristRoute(TouristRoute touristRoute);
		Task<bool> SaveAsync();
		void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture);
		void DeleteTouristRoute(TouristRoute touristRoute);
		void DeleteTouristRoutePicture(TouristRoutePicture picture);
		Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids);
		void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
		Task<ShoppingCart> GetShoppingCartByUserId(string userId);
		Task CreateShoppingCart(ShoppingCart shoppingCart);
		Task AddShoppingCartItem(LineItem lineItem);
		Task<LineItem> GetShoppingCartItemByItemId(int lineItemId);
		void DeleteShoppingCartItem(LineItem lineItem);
		Task<IEnumerable<LineItem>> GetShoppingCartByIdListAsync(IEnumerable<int> ids);
		void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems);
		Task AddOrderAsync(Order order);
		Task<PaginationList<Order>> GetOrdersByUserId(string userId,int pageSize, int pageNumber);
		Task<Order> GetOrderById(Guid orderId);
		
	}
}

