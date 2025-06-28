using System.Collections.ObjectModel;
using System.Collections.Specialized;

using XIVUniPF_Core;

namespace XIVUniPF.Classes
{
    // 针对 PartyInfo 的特化，支持批量添加/替换和排序
    public class PartyCollection : ObservableCollection<PartyInfo>
    {
        private readonly IList<PartyInfo> origin;

        private bool suppressNotification;

        private PartySortOption sortOption;

        public PartySortOption SortOption
        {
            get => sortOption;
            set
            {
                if (value != sortOption)
                {
                    sortOption = value;
                    Sort();
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        public PartyCollection()
        {
            suppressNotification = false;
            origin = [];
            sortOption = PartySortOptions.TimeLeft;
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

            Sort();
        }

        private void Sort()
        {
            suppressNotification = true;

            Clear();
            var sortedItems = origin.ToList();
            sortedItems.Sort((a, b) => SortOption.Comparison(a, b));
            foreach (var item in sortedItems)
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