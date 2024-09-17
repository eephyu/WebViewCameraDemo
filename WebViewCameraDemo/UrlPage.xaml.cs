using System;

namespace WebViewCameraDemo;

public partial class UrlPage : ContentPage
{
    public UrlPage()
    {
        InitializeComponent();
        txtUrl.Text = (Application.Current as App).Url;
    }

    private void btnNext_Clicked(object sender, EventArgs e)
    {

        (Application.Current as App).Url = txtUrl.Text;

        Navigation.PushAsync(new MainPage());
    }
}