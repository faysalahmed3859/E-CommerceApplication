using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handy.Extensions
{
    public class Pager
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public int SerialNumber { get; set; }
        public Pager()
        {

        }
        public Pager(int totalItem,int page,int pageSize=10)
        {
            int totalPages=(int)Math.Ceiling((decimal)totalItem / (decimal)pageSize);
            int currentPage = page;
            int startPage = currentPage - 5;
            int endPage = currentPage + 4;
            if (startPage<=0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }
            if (endPage>totalPages)
            {
                endPage = totalPages;
                if (endPage>10)
                {
                    startPage = endPage - 9;
                }
            }
            TotalItems = totalItem;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;

        }
    }
}
