using System;
using FakeXiecheng.API.Models;

namespace FakeXiecheng.API.Dtos
{
	public class OrderDto
	{
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ICollection<LineItem> OrderItems { get; set; }
        public OrderStateEnum State { get; set; }
        public DateTime CreateDateUTC { get; set; }
        public string TransactionMetadata { get; set; }
    }
}

