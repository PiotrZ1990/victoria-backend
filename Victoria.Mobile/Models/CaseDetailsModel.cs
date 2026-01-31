using System.Text.Json.Serialization;
using Victoria.Mobile.Helpers;

namespace Victoria.Mobile.Models;

public class CaseDetailsModel
{
    public int Id { get; set; }
    public string? ClientName { get; set; }
    public string? Status { get; set; }

    // ✅ Stage jako string (Application/Visa/itd)
    [JsonConverter(typeof(FlexibleStringJsonConverter))]
    public string? Stage { get; set; }

    public DateTime CreatedAt { get; set; }
}
