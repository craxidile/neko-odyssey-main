﻿namespace NekoOdyssey.Scripts.Database.Domains.Sites
{
    public class SitesDbContext : BaseDbContext
    {
        public SitesDbContext(DbContextOptions options): base("sites", options)
        {
        }
    }
}