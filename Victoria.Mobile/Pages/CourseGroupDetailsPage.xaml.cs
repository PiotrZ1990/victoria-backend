using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

[QueryProperty(nameof(GroupId), "id")]
public partial class CourseGroupDetailsPage : ContentPage
{
    private int _id;
    private readonly CourseGroupsService _service;

    public string GroupId
    {
        set
        {
            if (int.TryParse(value, out var id))
                _id = id;
        }
    }

    public CourseGroupDetailsPage()
    {
        InitializeComponent();
        var api = new ApiClient();
        _service = new CourseGroupsService(api);
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

            var g = await _service.GetByIdAsync(_id);

            GroupNameLabel.Text = g.GroupName;
            DatesLabel.Text = $"Dates: {g.StartDate:yyyy-MM-dd} → {g.EndDate:yyyy-MM-dd}";
            CapacityLabel.Text = $"Capacity: {g.Capacity}";
            ActiveLabel.Text = $"Active: {(g.IsActive ? "Yes" : "No")}";
            CreatedLabel.Text = $"Created: {g.CreatedAt:yyyy-MM-dd HH:mm}";
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
}
