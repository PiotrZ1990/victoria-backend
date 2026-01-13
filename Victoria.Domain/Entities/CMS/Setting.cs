namespace Victoria.Domain.Entities.Cms
{
    public class Setting
    {
        public int Id { get; set; }
        public string Key { get; set; } = default!;
        public string? Value { get; set; }
        public string? Description { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
