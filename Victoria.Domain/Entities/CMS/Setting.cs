namespace Victoria.Domain.Entities.CMS
{
    public class Setting
    {
        public int Id { get; set; }

        public string Key { get; set; }     // SiteName, ContactEmail
        public string Value { get; set; }
    }
}
