using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Serverr.Data.Models;

namespace Serverr.Data;

public class AppDbContext : DbContext
{
    public DbSet<Parent> Parents { get; set; }
    public DbSet<Child> Children { get; set; }
    public DbSet<BlockRule> BlockRules { get; set; }
    public DbSet<BrowsingLog> BrowsingLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        string conn = Environment.GetEnvironmentVariable("DB_CONNECTION");
        options.UseNpgsql(conn);
    }
}
