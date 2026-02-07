using Victoria.Mobile.Models;
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

            var groups = await _groups.GetByCourseAsync(_courseId);
            var my = await _enrollments.GetMyAsync();

            // mapa: CourseGroupId -> Enrollment
            var myMap = my.ToDictionary(x => x.CourseGroupId, x => x);

            // wrappery UI
            var ui = groups.Select(g =>
            {
                var enrolled = myMap.TryGetValue(g.Id, out var enr);

                return new CourseGroupUiModel
                {
                    Id = g.Id,
                    GroupName = g.GroupName,
                    StartDate = g.StartDate,
                    EndDate = g.EndDate,
                    Capacity = g.Capacity,
                    IsActive = g.IsActive,

                    IsEnrolled = enrolled,
                    EnrollmentId = enrolled ? enr!.Id : null
                };
            }).ToList();

            GroupsList.ItemsSource = ui;
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
            var groupId = Convert.ToInt32(btn.CommandParameter);

            await _enrollments.EnrollMyAsync(groupId);

            await DisplayAlert("OK", "You are enrolled ✅", "Close");
            await LoadAsync(); // przełącz UI
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Close");
        }
    }

    private async void OnUnenrollClicked(object sender, EventArgs e)
    {
        try
        {
            var btn = (Button)sender;
            if (btn.CommandParameter == null)
                throw new Exception("Missing enrollment id.");

            var enrollmentId = Convert.ToInt32(btn.CommandParameter);

            var ok = await DisplayAlert("Confirm", "Unenroll from this course group?", "Yes", "No");
            if (!ok) return;

            await _enrollments.UnenrollMyAsync(enrollmentId);

            await DisplayAlert("OK", "Unenrolled ✅", "Close");
            await LoadAsync(); // przełącz UI
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Close");
        }
    }
}
