using Victoria.Mobile.Models;
using Victoria.Mobile.Services;

namespace Victoria.Mobile.Pages;

public partial class NewsPage : ContentPage
{
    private readonly NewsPostsService _service;

    public NewsPage()
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

            var list = await _service.GetAllAsync();
            NewsList.ItemsSource = list;
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

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadAsync();
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.CurrentSelection?.FirstOrDefault() as NewsPostListModel;
        if (selected == null) return;

        ((CollectionView)sender).SelectedItem = null;

        await Shell.Current.GoToAsync($"news-details?id={selected.Id}");
    }
}
