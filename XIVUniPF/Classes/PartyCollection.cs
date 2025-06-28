using System.Collections.ObjectModel;
using System.Collections.Specialized;

using XIVUniPF_Core;

namespace XIVUniPF.Classes
{
    // 针对 PartyInfo 的特化，支持替换、排序、过滤
    public class PartyCollection : ObservableCollection<PartyInfo>
    {
        private readonly IList<PartyInfo> origin;

        private bool suppressNotification;

        private PartySortOption sortOption;

        /// <summary>
        /// 存储一组过滤器函数，用于筛选 PartyInfo
        /// 返回 true 表示通过筛选
        /// </summary>
        private List<Func<PartyInfo, bool>> filters;

        public PartySortOption SortOption
        {
            get => sortOption;
            set
            {
                if (value != sortOption)
                {
                    sortOption = value;
                    Refresh();
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        public PartyCollection()
        {
            suppressNotification = false;
            origin = [];
            sortOption = PartySortOptions.TimeLeft;
            filters = [];
        }

        public void Replace(IEnumerable<PartyInfo> items)
        {
            if (items == null)
                return;

            suppressNotification = true;
            origin.Clear();
            foreach (var item in items)
                origin.Add(item);
            suppressNotification = false;

            Refresh();
        }

        public void AddFilter(Func<PartyInfo, bool> filter)
        {
            if (filter == null || filters.Contains(filter))
                return;

            filters.Add(filter);
            Refresh();
        }

        public void Refresh()
        {
            suppressNotification = true;

            // 应用过滤器
            var tmp = origin.AsEnumerable();
            foreach (var filter in filters)
                tmp = tmp.Where(filter);

            // 排序
            var filtered = tmp.ToList();
            filtered.Sort(sortOption.Comparison);

            // 添加到最终显示的列表中
            Clear();
            foreach (var item in filtered)
                Add(item);

            suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!suppressNotification)
                base.OnCollectionChanged(e);
        }
    }
}