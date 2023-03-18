namespace Adoler.Dtos
{
    public class PagedList<T>
    {
        public PagedList()
        {

        }
        public IEnumerable<T> Items { get; set; }
        public PagedList(IEnumerable<T> source, int pageNumber, int pageSize, int count)
        {
            Items = source;
        }
    }
}
