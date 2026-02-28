namespace Printpress.Application
{
    public record Paging
    {

        public Paging(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        private int _pageNumber = 1;
        private int _pageSize = 20;
        private int _maxPageSize = 200;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = (value > 0) ? value : 1;
        }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 0 && value < _maxPageSize) ? value : 20;
        }
    }
}
