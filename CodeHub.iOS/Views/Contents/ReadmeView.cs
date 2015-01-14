using System;
using MonoTouch.UIKit;
using System.Reactive.Linq;
using ReactiveUI;
using CodeHub.Views.Contents;
using CodeHub.Core.ViewModels.Contents;
using CodeHub.Core.Services;

namespace CodeHub.iOS.Views.Contents
{
    public class ReadmeView : BaseWebView<ReadmeViewModel>
    {
        public ReadmeView(INetworkActivityService networkActivityService)
            : base(networkActivityService)
        {
            Web.ScalesPageToFit = true;

            this.WhenAnyValue(x => x.ViewModel.ContentText).IsNotNull().Subscribe(x =>
                LoadContent(new ReadmeRazorView { Model = x }.GenerateString()));

            this.WhenAnyValue(x => x.ViewModel.ShowMenuCommand).IsNotNull().Subscribe(x =>
                NavigationItem.RightBarButtonItem = x.ToBarButtonItem(UIBarButtonSystemItem.Action));
        }

		protected override bool ShouldStartLoad(MonoTouch.Foundation.NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
		    if (request.Url.AbsoluteString.StartsWith("file://", StringComparison.Ordinal))
		        return base.ShouldStartLoad(request, navigationType);
            ViewModel.GoToLinkCommand.ExecuteIfCan(request.Url.AbsoluteString);
		    return false;
		}
    }
}

