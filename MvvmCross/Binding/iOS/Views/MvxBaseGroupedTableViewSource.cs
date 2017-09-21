using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Foundation;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.Platform.WeakSubscription;
using UIKit;

namespace MvvmCross.Binding.iOS.Views
{
    public abstract class MvxBaseGroupedTableViewSource<TKey, TItem> : MvxBaseTableViewSource
    {
        private List<IDisposable> _subscriptions;
        private IEnumerable<IGrouping<TKey, TItem>> _itemsSource;

        protected MvxBaseGroupedTableViewSource(UITableView tableView)
            : base(tableView)
        {
        }

        protected MvxBaseGroupedTableViewSource(IntPtr handle) : base(handle)
        {
        }

        public bool ReloadOnAllItemsSourceSets { get; set; }
        public bool UseAnimations { get; set; }
        public UITableViewRowAnimation AddAnimation { get; set; }
        public UITableViewRowAnimation RemoveAnimation { get; set; }
        public UITableViewRowAnimation ReplaceAnimation { get; set; }

        public IEnumerable<IGrouping<TKey, TItem>> ItemsSource
        {
            get => _itemsSource;
            set
            {
                if (ReferenceEquals(_itemsSource, value)
                    && !ReloadOnAllItemsSourceSets)
                    return;

                if (_subscriptions != null)
                {
                    _subscriptions.ForEach(x => x.Dispose());
                    _subscriptions = null;
                }

                _itemsSource = value;

                var collectionChanged = _itemsSource as INotifyCollectionChanged;
                _subscriptions = new List<IDisposable>();

                if (collectionChanged != null)
                {
                    _subscriptions.Add(collectionChanged.WeakSubscribe(OnItemsSourceCollectionChanged));
                }

                foreach (var rowsInSection in _itemsSource)
                {
                    if (rowsInSection is INotifyCollectionChanged notifyCollectionChanged)
                    {
                        _subscriptions.Add(notifyCollectionChanged.WeakSubscribe(RowsInSectionChanged));
                    }
                }

                ReloadTableData();
            }
        }

        private void RowsInSectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ReloadTableData();
            //TODO: Animation Support 
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ReloadTableData();
            //TODO: Animation Support 
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            var sectionPosition = ItemsSource?.ElementAt((int)section);
            return sectionPosition?.Count() ?? 0;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return ItemsSource?.Count() ?? 0;
        }

        protected TKey GetElementForHeader(int index)
        {
            return ItemsSource.ElementAt(index).Key;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            return ItemsSource.ElementAt(indexPath.Section).ElementAt(indexPath.Row);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_subscriptions != null)
                {
                    _subscriptions.ForEach(x => x.Dispose());
                    _subscriptions = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
