using MouseTagProject.Interfaces;

namespace MouseTagProject.Services
{
    public class Page : IPage
    {
        public int PageSize(int records, int pageSize)
        {
            decimal pages = (decimal) records / pageSize;
            pages = Math.Ceiling(pages);
            int pagesResult = Convert.ToInt32(pages);
            return pagesResult;
        }
    }
}
