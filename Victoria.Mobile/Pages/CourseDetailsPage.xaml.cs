using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

[QueryProperty(nameof(CourseId), "id")]
public partial class CourseDetailsPage : ContentPage
{
    private readonly LanguageCoursesService _courses;
    private readonly CourseGroupsService _groups;
    private readonly EnrollmentsService _enrollments;

    private int _courseId;

    public string CourseId
    {
        set
        {
            if (int.TryParse(value, out var parsed))
                _courseId = parsed;
        }
    }

    public CourseDetailsPage()
    {
        InitializeComponent();

        var api = new ApiClient();
        _courses = new LanguageCoursesService(api);
        _groups = new CourseGroupsService(api);
        _enrollments = new EnrollmentsService(api);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_courseId <= 0)
        {
            ErrorLabel.Text = "Missing course id.";
            return;
        }

        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            ErrorLabel.Text = "";
            Loader.IsVisible = true;
            Loader.IsRunning = true;

            var course = await _courses.GetByIdAsync(_courseId);

            TitleLabel.Text = course.Name;
            MetaLabel.Text = $"Level: {course.Level ?? "-"} | Price: {course.Price:0.00} {course.Currency}";
            DescLabel.Text = course.Description ?? "";

            var list = await _groups.GetByCourseAsync(_courseId);
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

    private async void OnEnrollClicked(object sender, EventArgs e)
    {
        try
        {
            var btn = (Button)sender;

            if (btn.CommandParameter == null)
                throw new Exception("Missing group id.");

            var groupId = Convert.ToInt32(btn.CommandParameter);

            await _enrollments.EnrollMyAsync(groupId);

            await DisplayAlert("OK", "You are enrolled ✅", "Close");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Close");
        }
    }
}
