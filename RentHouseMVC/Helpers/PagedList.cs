using System;
using System.Collections.Generic;
using System.Linq;

namespace RentHouseMVC.Helpers
{
    /// <summary>
    /// Danh sách đã phân trang (tự viết, không dùng thư viện ngoài).
    /// Chỉ lấy từ database đúng số dòng của trang hiện tại bằng Skip/Take.
    /// </summary>
    public class PagedList<T> : List<T>
    {
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalItemCount { get; private set; }
        public int PageCount { get; private set; }

        public bool HasPreviousPage
        {
            get { return PageNumber > 1; }
        }

        public bool HasNextPage
        {
            get { return PageNumber < PageCount; }
        }

        public int FirstItemOnPage
        {
            get { return TotalItemCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1; }
        }

        public int LastItemOnPage
        {
            get { return Math.Min(PageNumber * PageSize, TotalItemCount); }
        }

        /// <param name="source">Truy vấn ĐÃ sắp xếp (OrderBy) - bắt buộc với Skip trong EF</param>
        /// <param name="pageNumber">Trang hiện tại, bắt đầu từ 1</param>
        /// <param name="pageSize">Số phần tử mỗi trang</param>
        public PagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (pageSize < 1)
            {
                pageSize = 10;
            }

            PageSize = pageSize;
            TotalItemCount = source.Count();
            PageCount = (int)Math.Ceiling(TotalItemCount / (double)pageSize);

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            if (PageCount > 0 && pageNumber > PageCount)
            {
                pageNumber = PageCount;
            }
            PageNumber = pageNumber;

            if (TotalItemCount > 0)
            {
                int skip = (pageNumber - 1) * pageSize;
                AddRange(source.Skip(skip).Take(pageSize).ToList());
            }
        }
    }
}
