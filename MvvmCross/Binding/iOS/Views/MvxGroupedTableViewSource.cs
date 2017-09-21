using System;
using Foundation;
using MvvmCross.Platform.Core;
using MvvmCross.Platform.iOS.Platform;
using UIKit;

namespace MvvmCross.Binding.iOS.Views
{
    public class MvxGroupedTableViewSource : MvxBaseGroupedTableViewSource<object, object>
    {
        private readonly MvxIosMajorVersionChecker _iosVersion6Checker = new MvxIosMajorVersionChecker(6);

        private readonly NSString _cellIdentifier;
        private readonly NSString _headerIdentifier;

        protected virtual NSString CellIdentifier => _cellIdentifier;
        protected virtual NSString HeaderIdentifier => _headerIdentifier;

        public MvxGroupedTableViewSource(UITableView tableView, string nibName, string headerNibName, NSBundle bundle = null)
            : base(tableView)
        {
            _cellIdentifier = new NSString(nibName);
            tableView.RegisterNibForCellReuse(UINib.FromName(nibName, bundle ?? NSBundle.MainBundle), nibName);

            _headerIdentifier = new NSString(headerNibName);
            tableView.RegisterNibForHeaderFooterViewReuse(UINib.FromName(headerNibName, bundle ?? NSBundle.MainBundle), headerNibName);
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            if (_iosVersion6Checker.IsVersionOrHigher)
                return tableView.DequeueReusableCell(CellIdentifier, indexPath);

            return tableView.DequeueReusableCell(CellIdentifier);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var header = tableView.DequeueReusableHeaderFooterView(HeaderIdentifier);
            if (header is IMvxDataConsumer dataConsumerHeaderView)
            {
                dataConsumerHeaderView.DataContext = GetElementForHeader((int)section);
            }

            return header;
        }
    }
}
