using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

[QueryProperty(nameof(PostId), "id")]
public partial class NewsDetailsPage : ContentPage
{
    private readonly NewsPostsService _service;
    private int _id;

    public string PostId
    {
        set
        {
            if (int.TryParse(value, out var id))
                _id = id;
        }
    }

    public NewsDetailsPage()
    {
        InitializeComponent();
        var api = new ApiClient();
        _service = new NewsPostsService(api);
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

            var dto = await _service.GetByIdAsync(_id);

            TitleLabel.Text = dto.Title;
            DateLabel.Text = $"Created: {dto.CreatedAt:yyyy-MM-dd HH:mm}";
            SummaryLabel.Text = dto.Summary ?? "";
            ContentLabel.Text = dto.Content ?? "";
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
