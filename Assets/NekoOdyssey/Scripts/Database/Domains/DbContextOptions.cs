namespace NekoOdyssey.Scripts.Database.Domains
{
    public class DbContextOptions
    {
        public bool ReadOnly { get; set; } = false;
        public bool CopyRequired { get; set; } = false;
    }
}