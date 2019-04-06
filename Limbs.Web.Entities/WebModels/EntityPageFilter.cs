namespace Limbs.Web.Entities.WebModels
{
    public class EntityPageFilter
    {
        private int _index;
        private int _pageSize;
        public const int MaxPageSize = 24;

        public EntityPageFilter(int index = 0, int pageSize = MaxPageSize)
        {
            Index = index;
            PageSize = pageSize;
        }

        public int Index
        {
            get => _index < 0 ? 0 : _index;
            set => _index = value;
        }

        public int Skip => PageSize * Index;

        public int PageSize
        {
            get => _pageSize > MaxPageSize ? MaxPageSize : _pageSize;
            set => _pageSize = value;
        }
    }
}
