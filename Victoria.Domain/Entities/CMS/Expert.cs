namespace Victoria.Domain.Entities.CMS
{
    public class Expert
    {
        public int Id { get; set; }

        public string FullName { get; set; }
        public string Position { get; set; }   // Education Consultant
        public string Bio { get; set; }

        public bool IsActive { get; set; }
    }
}
