namespace Victoria.Mobile.Pages;

public partial class PublicHomePage : ContentPage
{
    public PublicHomePage()
    {
        InitializeComponent();
    }

    private async void OnCoursesClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//public/courses");

    private async void OnNewsClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//public/news");

    private async void OnApplyClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//public/apply");

    private async void OnLoginClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//login");
}
