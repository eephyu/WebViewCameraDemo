namespace WebViewCameraDemo
{
    public partial class App : Application
    {
        public string _url = "https://saas.cloudapp.com.sg/WebViewCameraDemo/employee/index";

        public string Url
        {
            get
            {
                string result = _url;
                Preferences.Get("Url", result);
                return result;
            }

            set
            {
                _url = value;
                Preferences.Set("Url", value);
            }
        }
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
