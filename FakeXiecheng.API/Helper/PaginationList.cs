using System;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;

namespace FakeXiecheng.API.Helper
{
	public class PaginationList<T>:List<T>
	{
		public int TotalPages { get; set; }
		public int TotalCount { get; set; }
		public bool HasPrevious => CurrentPage > 1;
		public bool HasNext => CurrentPage < TotalPages;

		public int CurrentPage { get; set; }
		public int PageSize { get; set; }

		public PaginationList(int totalCount, int currentPage, int pageSize, List<T> items)
		{
			CurrentPage = currentPage;
			PageSize = pageSize;
			AddRange(items);
			TotalCount = totalCount;
			TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
		}

		public static async Task<PaginationList<T>> CreateAsync(
			int currentPage,
			int pageSize,
			IQueryable<T> result
		)
		{
			var totalCount = await result.CountAsync();
            //pagination分頁
            //skip一定量的數據
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            //以pagesize為標準顯示一定量的數據
            result = result.Take(pageSize);

			var items = await result.ToListAsync();

            return new PaginationList<T>(totalCount, currentPage, pageSize, items);
		}
	}
}

