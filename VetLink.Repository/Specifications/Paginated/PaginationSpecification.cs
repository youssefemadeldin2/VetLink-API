using System;
using System.Collections.Generic;
using System.Text;

namespace VetLink.Repository.Specifications.Paginated
{
    public class PaginationSpecification
    {
        public int PageIndex { get; set; } = 1;
        private int _PageSize = 6;
        private const int MAXPAGESIZE = 30;
        public int PageSize
        {
            get => _PageSize;
            set => _PageSize = (value> MAXPAGESIZE)? MAXPAGESIZE : value;
        }
    }
}
