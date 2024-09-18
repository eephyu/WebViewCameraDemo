using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;
using WebViewCameraDemo.Platforms.Android;
using Android;
using Android.Content;

namespace WebViewCameraDemo
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {

        public static int REQUEST_CODE = 10004556;
        public static Android.Net.Uri imageUri;
        public static MainActivity Instance;
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Instance = this;
            // set the path of save the photo file
            string filePath = Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator
            + Android.OS.Environment.DirectoryPictures + Java.IO.File.Separator;
            string fileName = "IMG_" + DateTime.Now.Millisecond + ".jpg";
            imageUri = Android.Net.Uri.FromFile(new Java.IO.File(filePath + fileName));
            ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.Camera, Manifest.Permission.RecordAudio, Manifest.Permission.ModifyAudioSettings }, 0);

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == REQUEST_CODE)
            {
                chooseAbove(requestCode, data);
            }
        }
        private void chooseAbove(int resultCode, Intent data)
        {

            if (resultCode == REQUEST_CODE)
            {
                updatePhotos();

                if (data != null)
                {
                    // handle the picture
                    Android.Net.Uri[] results;
                    Android.Net.Uri uriData = data.Data;
                    if (uriData != null)
                    {
                        results = new Android.Net.Uri[] { uriData };
                        foreach (Android.Net.Uri uri in results)
                        {
                            Log.Info("test", "return uri：" + uri.ToString());
                        }
                        MyWebChromeClient.mUploadCallbackAboveL.OnReceiveValue(results);
                    }
                    else
                    {
                        MyWebChromeClient.mUploadCallbackAboveL.OnReceiveValue(null);
                    }
                }
                else
                {
                    MyWebChromeClient.mUploadCallbackAboveL.OnReceiveValue(new Android.Net.Uri[] { imageUri });
                }
            }
            MyWebChromeClient.mUploadCallbackAboveL = null;
        }

        private void updatePhotos()
        {
            //Send the broadcast to notify the system to reflash file
            Intent intent = new Intent(Intent.ActionMediaScannerScanFile);
            intent.SetData(imageUri);
            SendBroadcast(intent);
        }
    }
}
