using Foundation;
using Microsoft.Identity.Client;
using UIKit;

namespace ReminderProX
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            if (AuthenticationContinuationHelper.IsBrokerResponse(null))
            {
                AuthenticationContinuationHelper.SetBrokerContinuationEventArgs(url);
                return true;
            }

            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
            return base.OpenUrl(app, url, options);
        }
    }
}