using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Database> Databases { get; set; }
        public DbSet<RegisteredScript> RegisteredScripts { get; set; }
        public DbSet<SearchFolder> SearchFolders { get; set; }
        public DbSet<ServerSetting> ServerSettings { get; set; }
        public DbSet<AppSetting> AppSettings{ get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
