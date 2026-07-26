namespace tuan3.Pagination
{
    public class PaginationQuery
    {
        private const int MaxPageSize= 50;
        private int _pageSize = 10;
        public string? Keyword { get; set; }
        public int PageNumber { get; set; } = 1;


        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value > MaxPageSize) _pageSize = MaxPageSize;
                else if (value < 1) _pageSize = 1;
                else value=_pageSize;
            }
        }
    }
}
