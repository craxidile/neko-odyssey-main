namespace NekoOdyssey.Scripts.Database.Domains
{
    public enum DbCopyMode
    {
        DoNotCopy = 0,
        ForceCopy = 1,
        CopyIfNotExists = 2,
    }
    
    public class DbContextOptions
    {
        public bool ReadOnly { get; set; } = false;
        public DbCopyMode CopyMode { get; set; } = DbCopyMode.DoNotCopy;
    }
}