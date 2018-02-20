using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagnumPhotos.API.Helpers
{
    public class PhotographersResourceParameters
    {
        const int maxPageSize = 20;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string Genre { get; set; }

        public string SearchQuery { get; set; }

        public string OrderBy { get; set; } = "Name";
    }
}
