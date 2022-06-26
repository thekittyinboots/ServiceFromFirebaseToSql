using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPanelToSql.Models
{
    public class ServiceContext : DbContext
    {
        protected readonly IConfiguration Configuration;
        public ServiceContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        }
        public DbSet<UserPanelInfo> UserPanelInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.Entity<UserPanelInfo>().ToTable("UserPanelInfo");
        }
    }
}
