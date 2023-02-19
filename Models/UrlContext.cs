using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class UrlContext : DbContext
    {
        public virtual DbSet<UrlInfo> Urls { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public UrlContext(DbContextOptions<UrlContext> options) : base(options)
        {

        }
    }
}
