using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XIVUniPF_Core;

namespace XIVUniPF.Classes
{
    // 针对 PartyInfo 的特化，支持替换、排序、过滤
    // 内部实现：
    // 1. origin 存储原始数据
    // 2. filteredAndSorted 存储经过过滤和排序后的数据作为缓存
    // 3. 截取 filteredAndSorted 的一部分作为当前显示的数据
    public class PartyCollection : ObservableCollection<PartyInfo>
    {
        // Public
        public delegate void PageChangedEventHandler(PartyCollection sender);

        public event PageChangedEventHandler? PageChanged;

        // Private
        private readonly IList<PartyInfo> origin;

        private bool suppressNotification;

        private PartySortOption sortOption;

        /// <summary>
        /// 存储一组过滤器函数，用于筛选 PartyInfo
        /// 返回 true 表示通过筛选
        /// </summary>
        private IList<Func<PartyInfo, bool>> filters;

        private int pageSize;

        private int page;

        private int pageCount;


        // Properties
        public PartySortOption SortOption
        {
            get => sortOption;
            set
            {
                if (value != sortOption)
                {
                    sortOption = value;
                    Notify();
                    Update();
                }
            }
        }

        public int Page
        {
            get => page;
            set
            {
                if (value != page)
                {
                    page = value;
                    Notify();
                    Update();
                }
            }
        }

        public int PageCount => pageCount;

        public PartyCollection()
        {
            suppressNotification = false;
            origin = [];
            sortOption = PartySortOptions.TimeLeft;
            filters = [];
            pageSize = 100;
            pageCount = 0;
            page = 0;
        }

        public void Replace(IEnumerable<PartyInfo> items)
        {
            if (items == null)
                return;

            suppressNotification = true;

            origin.Clear();
            foreach (var item in items)
                origin.Add(item);
            page = 1;

            suppressNotification = false;
            Update();
        }

        public void AddFilter(Func<PartyInfo, bool> filter)
        {
            if (filter == null || filters.Contains(filter))
                return;

            filters.Add(filter);
            Update();
        }

        public void Update()
        {
            if (origin.Count == 0)
                return;

            suppressNotification = true;

            // 应用过滤器
            var tmp = origin.AsEnumerable();
            foreach (var filter in filters)
                tmp = tmp.Where(filter);

            // 排序
            var filtered = tmp.ToList();
            filtered.Sort(sortOption.Comparison);

            // 计算总页数
            pageCount = filtered.Count / pageSize + (filtered.Count % pageSize > 0 ? 1 : 0);

            // 分页
            var paged = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 添加到最终显示的列表中
            Clear();
            foreach (var item in paged)
                Add(item);

            suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            PageChanged?.Invoke(this);
        }


        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!suppressNotification)
                base.OnCollectionChanged(e);
        }

        private void Notify([CallerMemberName] string? prop = null)
        {
            if (!suppressNotification)
                OnPropertyChanged(new PropertyChangedEventArgs(prop));
        }
    }
}