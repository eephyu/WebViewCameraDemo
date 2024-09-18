using Android.OS;
using Android.Provider;
using Android.Webkit;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.Content;
using Android.Icu.Text;
using Android.Icu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WebViewCameraDemo.Platforms.Android
{
    internal class MyWebChromeClient : MauiWebChromeClient
    {
        public MyWebChromeClient(IWebViewHandler handler) : base(handler)
        {

        }

        public static IValueCallback mUploadCallbackAboveL;
        public override bool OnShowFileChooser(global::Android.Webkit.WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        {
            mUploadCallbackAboveL = filePathCallback;
            takePhoto();
            return true;
        }

        private void takePhoto()
        {
            //close the policy, if not you will get Android.OS.FileUriExposedException:exposed beyond app through ClipData.Item.getUri()'
            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());

            Intent intent = new Intent(MediaStore.ActionImageCapture);
            intent.PutExtra(MediaStore.ExtraOutput, MainActivity.imageUri);
            MainActivity.Instance.StartActivityForResult(intent, MainActivity.REQUEST_CODE);


            // Select the picture from the local file
            //        Intent i = new Intent(Intent.ACTION_GET_CONTENT);
            //        i.addCategory(Intent.CATEGORY_OPENABLE);
            //        i.setType("image/*");
            //        startActivityForResult(Intent.createChooser(i, "Image Chooser"), REQUEST_CODE);
        }

        public override void OnPermissionRequest(PermissionRequest request)
        {
            // Process each request
            foreach (var resource in request.GetResources())
            {
                // Check if the web page is requesting permission to the camera
                if (resource.Equals(PermissionRequest.ResourceVideoCapture, StringComparison.OrdinalIgnoreCase))
                {
                    // Get the status of the .NET MAUI app's access to the camera
                    //PermissionStatus status = Permissions.CheckStatusAsync<Permissions.Camera>().Result;

                    //// Deny the web page's request if the app's access to the camera is not "Granted"
                    //if (status != PermissionStatus.Granted)
                    //    request.Deny();
                    //else
                    //    request.Grant(request.GetResources());

                    try
                    {
                        request.Grant(request.GetResources());
                        base.OnPermissionRequest(request);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    return;
                }
            }

            base.OnPermissionRequest(request);
        }
    }
}
