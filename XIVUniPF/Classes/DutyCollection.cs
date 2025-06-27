using System.Collections.ObjectModel;
using System.Collections.Specialized;

using XIVUniPF_Core;

namespace XIVUniPF.Classes
{
    // 针对 PartyInfo 的特化，支持批量添加/替换和排序
    public class DutyCollection : ObservableCollection<PartyInfo>
    {
        private bool suppressNotification = false;

        public void AddRange(IEnumerable<PartyInfo> items)
        {
            if (items == null)
                return;

            suppressNotification = true;
            foreach (var item in items)
                Add(item);
            suppressNotification = false;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Replace(IEnumerable<PartyInfo> items)
        {
            if (items == null)
                return;

            suppressNotification = true;
            Clear();
            foreach (var item in items)
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