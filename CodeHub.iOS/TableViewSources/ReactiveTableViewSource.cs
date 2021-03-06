using System;
using UIKit;
using System.Reactive.Subjects;
using System.Reactive;
using CodeHub.Core.ViewModels;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;

namespace CodeHub.iOS.TableViewSources
{
    public abstract class ReactiveTableViewSource<TViewModel> : ReactiveUI.ReactiveTableViewSource<TViewModel>, IInformsEnd, IInformsEmpty
    {
        private readonly ISubject<Unit> _requestMoreSubject = new Subject<Unit>();
        private readonly ISubject<bool> _isEmptySubject = new BehaviorSubject<bool>(true);

        public IObservable<Unit> RequestMore
        {
            get { return _requestMoreSubject; }
        }

        public IObservable<bool> IsEmpty
        {
            get { return _isEmptySubject; }
        }

        protected ReactiveTableViewSource(UITableView tableView, nfloat sizeHint)
            : base(tableView)
        {
            tableView.RowHeight = sizeHint;
            tableView.EstimatedRowHeight = sizeHint;
            this.WhenAnyValue(x => x.Data).IsNotNull().Select(x => x.Count == 0).Subscribe(_isEmptySubject.OnNext);
        }

        protected ReactiveTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<TViewModel> collection, 
            Foundation.NSString cellKey, nfloat sizeHint, Action<UITableViewCell> initializeCellAction = null) 
            : base(tableView, collection, cellKey, (float)sizeHint, initializeCellAction)
        {
            tableView.RowHeight = sizeHint;
            tableView.EstimatedRowHeight = sizeHint;
            collection.CountChanged.Select(x => x == 0).Subscribe(_isEmptySubject.OnNext);
            (collection as IReactiveCollection<TViewModel>).Do(x => _isEmptySubject.OnNext(!x.Any()));
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, Foundation.NSIndexPath indexPath)
        {
            if (indexPath.Section == (NumberOfSections(tableView) - 1) &&
                indexPath.Row == (RowsInSection(tableView, indexPath.Section) - 1))
            {
                // We need to skip an event loop to stay out of trouble
                BeginInvokeOnMainThread(() => _requestMoreSubject.OnNext(Unit.Default));
            }
        }

        public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var item = ItemAt(indexPath) as ICanGoToViewModel;
            if (item != null)
                item.GoToCommand.ExecuteIfCan();

            base.RowSelected(tableView, indexPath);
        }
    }

    public interface IInformsEnd
    {
        IObservable<Unit> RequestMore { get; }
    }

    public interface IInformsEmpty
    {
        IObservable<bool> IsEmpty { get; }
    }
}

