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
using Android.Annotation;
using Android.App;
using AndroidNet = Android.Net;
using Android.Graphics.Drawables;
using AndroidGraphics = Android.Graphics;
using Android.Views;
using AndroidWidget = Android.Widget;
using Android.Widget;

namespace WebViewCameraDemo.Platforms.Android
{
    internal class MyWebChromeClient : MauiWebChromeClient
    {
        public MyWebChromeClient(IWebViewHandler handler) : base(handler)
        {

        }

        public static IValueCallback mUploadCallbackAboveL;
        string _photoPath;
        public override bool OnShowFileChooser(global::Android.Webkit.WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        {
            #region Method 1
            var context = Platform.CurrentActivity;

            AlertDialog.Builder alertDialog = new AlertDialog.Builder(MainActivity.Instance); //, Resource.Style.CustomAlertDialogTheme //, global::WebViewCameraDemo.Resource.Style.CustomAlertDialogTheme
            alertDialog.SetTitle("Take picture or choose photo.");
            
            alertDialog.SetNeutralButton("Camera", async (sender, alertArgs) =>
            {
                try
                {
                    var photo = await MediaPicker.CapturePhotoAsync();
                    var uri = await LoadPhotoAsync(photo);
                    filePathCallback.OnReceiveValue(uri);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
                }
            });

            alertDialog.SetNegativeButton("Photo", async (sender, alertArgs) =>
            {
                try
                {
                    var photo = await MediaPicker.PickPhotoAsync();
                    var uri = await LoadPhotoAsync(photo);
                    filePathCallback.OnReceiveValue(uri);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"PickPhotoAsync THREW: {ex.Message}");
                }
            });
            alertDialog.SetPositiveButton("Cancel", (sender, alertArgs) =>
            {
                filePathCallback.OnReceiveValue(null);
            });

            var dialog = alertDialog.Create();
            dialog.Show();

            #region button icons
            // Access and style the buttons
            var camButton = dialog.GetButton((int)DialogButtonType.Neutral);            
            camButton.SetBackgroundColor(AndroidGraphics.Color.Transparent); // Set custom background color
            camButton.SetTextColor(AndroidGraphics.Color.DarkGray); // Set custom text color
            Drawable iconCam = context.GetDrawable(Microsoft.Maui.Resource.Drawable.camera);
            iconCam.SetBounds(0, 0, iconCam.IntrinsicWidth, iconCam.IntrinsicHeight);
            camButton.SetCompoundDrawables(iconCam, null, null, null);

            var photoButton = dialog.GetButton((int)DialogButtonType.Negative);
            photoButton.SetBackgroundColor(AndroidGraphics.Color.Transparent); // Set custom background color
            photoButton.SetTextColor(AndroidGraphics.Color.DarkGray); // Set custom text color
            Drawable iconPhoto = context.GetDrawable(Microsoft.Maui.Resource.Drawable.gallery);
            iconPhoto.SetBounds(0, 0, iconCam.IntrinsicWidth, iconCam.IntrinsicHeight);
            photoButton.SetCompoundDrawables(iconPhoto, null, null, null);

            var cancelButton = dialog.GetButton((int)DialogButtonType.Positive);
            cancelButton.SetBackgroundColor(AndroidGraphics.Color.Transparent); // Set custom background color
            cancelButton.SetTextColor(AndroidGraphics.Color.DarkGray); // Set custom text color

            #endregion

            #endregion

            return true;
        }
        async Task<AndroidNet.Uri[]> LoadPhotoAsync(FileResult photo)
        {
            // cancelled
            if (photo == null)
            {
                _photoPath = null;
                return null;
            }
            // save the file into local storage
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = System.IO.File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);
            _photoPath = newFile;
            AndroidNet.Uri uri = AndroidNet.Uri.FromFile(new Java.IO.File(_photoPath));
            return new AndroidNet.Uri[] { uri };
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
