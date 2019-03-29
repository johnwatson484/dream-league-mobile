using Android.App;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using Android.Views;
using Android.Content;
using Android.Runtime;
using System;

namespace DreamLeague.Mobile
{
    [Activity(Label = "Dream League", MainLauncher = true, Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity
    {
        WebView webview;
        public IValueCallback mUploadMessage;
        public ProgressBar oSpinner;
        private static int FILECHOOSER_RESULTCODE = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            webview = FindViewById<WebView>(Resource.Id.webview);
            webview.Settings.JavaScriptEnabled = true;
            webview.Settings.AllowFileAccess = true;
            webview.SetWebViewClient(new DLWebViewClient());
            webview.LoadUrl("https://dreamleaguefantasyfootball.co.uk");
            webview.SetWebChromeClient(new DLWebChromeClient(this));
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == FILECHOOSER_RESULTCODE)
            {
                if (null == mUploadMessage) return;
                Android.Net.Uri[] result = data == null || resultCode != Result.Ok ? null : new Android.Net.Uri[] { data.Data };
                try
                {
                    mUploadMessage.OnReceiveValue(result);

                }
                catch (Exception)
                {
                }

                mUploadMessage = null;
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        public class DLWebChromeClient : WebChromeClient
        {
            MainActivity mainActivity;

            public DLWebChromeClient(MainActivity activity)
            {
                mainActivity = activity;

            }

            public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
            {
                mainActivity.mUploadMessage = filePathCallback;
                Intent i = new Intent(Intent.ActionGetContent);
                i.AddCategory(Intent.CategoryOpenable);
                i.SetType("*/*");
                mainActivity.StartActivityForResult(Intent.CreateChooser(i, "File Chooser"), MainActivity.FILECHOOSER_RESULTCODE);

                return true;
            }
        }

        public class DLWebViewClient : WebViewClient
        {
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                view.LoadUrl(url);
                return false;
            }
        }

        public override bool OnKeyDown(Android.Views.Keycode keyCode, Android.Views.KeyEvent e)
        {
            if (keyCode == Keycode.Back && webview.CanGoBack())
            {
                webview.GoBack();
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }
    }
}

