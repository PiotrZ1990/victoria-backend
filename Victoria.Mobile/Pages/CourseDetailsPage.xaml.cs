using Victoria.Mobile.Models;
using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

[QueryProperty(nameof(CourseId), "id")]
public partial class CourseDetailsPage : ContentPage
{
    private int _id;
    private readonly LanguageCoursesService _courses;
    private readonly CourseGroupsService _groups;

    public string CourseId
    {
        set
        {
            if (int.TryParse(value, out var id))
                _id = id;
        }
    }

    public CourseDetailsPage()
    {
        InitializeComponent();
        var api = new ApiClient();
        _courses = new LanguageCoursesService(api);
        _groups = new CourseGroupsService(api);
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

            var course = await _courses.GetByIdAsync(_id);
            TitleLabel.Text = course.Name;
            MetaLabel.Text = $"Level: {course.Level ?? "-"} | Price: {course.Price:0.00} {course.Currency}";
            DescLabel.Text = course.Description ?? "";

            var list = await _groups.GetByCourseAsync(_id);
            GroupsList.ItemsSource = list;
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

    private async void OnGroupSelected(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.CurrentSelection?.FirstOrDefault() as CourseGroupListModel;
        if (selected == null) return;

        ((CollectionView)sender).SelectedItem = null;

        await Shell.Current.GoToAsync($"group-details?id={selected.Id}");
    }
}
