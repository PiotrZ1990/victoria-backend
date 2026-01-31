using Victoria.Mobile.Models;
using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

public partial class CoursesPage : ContentPage
{
    private readonly LanguageCoursesService _service;

    public CoursesPage()
    {
        InitializeComponent();
        var api = new ApiClient();
        _service = new LanguageCoursesService(api);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            ErrorLabel.Text = "";
            Loader.IsVisible = true;
            Loader.IsRunning = true;

            var list = await _service.GetAllAsync();
            CoursesList.ItemsSource = list;
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
        }
        finally
        {
            Loader.IsRunning = false;
            Loader.IsVisible = false;
        }
    }

    private async void OnRefreshClicked(object sender, EventArgs e) => await LoadAsync();

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.CurrentSelection?.FirstOrDefault() as LanguageCourseListModel;
        if (selected == null) return;

        ((CollectionView)sender).SelectedItem = null;

        await Shell.Current.GoToAsync($"course-details?id={selected.Id}");
    }
}
