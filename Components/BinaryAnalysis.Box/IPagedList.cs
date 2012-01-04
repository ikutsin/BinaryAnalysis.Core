using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Box
{
    public interface IPagedList<TM> : IList<TM>
    {
        int TotalItemCount { get; }
        int PageIndex { get; }
        int PageSize { get; }
    }

    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public PagedList(IQueryable<T> superset, int index, int pageSize)
        {
            Init(superset, index, pageSize);
        }

        public PagedList(IEnumerable<T> superset, int index, int pageSize, int total = -1)
        {
            // set source to blank list if superset is null to prevent exceptions
            var source = superset == null
                                    ? new List<T>().AsQueryable()
                                    : superset.AsQueryable();

            Init(source, index, pageSize);
        }

        void Init(IQueryable<T> superset, int index, int pageSize, int total = -1)
        {
            TotalItemCount = total > 0 ? total : superset.Count();
            PageSize = pageSize;
            PageIndex = index;

            if (index < 0) throw new ArgumentOutOfRangeException("index", index, "PageIndex cannot be below 0.");
            if (pageSize < 1) throw new ArgumentOutOfRangeException("pageSize", pageSize, "PageSize cannot be less than 1.");

            // add items to internal list
            if (TotalItemCount > 0)
                if (index == 0)
                    AddRange(superset.Take(pageSize).ToList());
                else
                    AddRange(superset.Skip((index) * pageSize).Take(pageSize).ToList());
        }

        public int TotalItemCount { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }

    }
}
