namespace WebViewCameraDemo
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            RequestCameraPermission();
        }

        private async Task RequestCameraPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status != PermissionStatus.Granted)
                await Permissions.RequestAsync<Permissions.Camera>();
        }
    }

}
