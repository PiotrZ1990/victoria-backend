namespace Victoria.Web.Models.Admin;

public class AdminUserListVm
{
    public string Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public int? StudentId { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class AdminUserEditVm
{
    public string Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public int? StudentId { get; set; }

    public List<string> AllRoles { get; set; } = new();
    public List<string> SelectedRoles { get; set; } = new();
}
