namespace tuan3.Pagination
{
    public class PagedResult<T>
    {
        public List<T> ? Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages
        {
            get
            {

                if (PageSize <= 0)
                {
                    return 0;
                }

                double result = (double)TotalItems / PageSize;
                return (int)Math.Ceiling(result);
            }



        }
    }
}
