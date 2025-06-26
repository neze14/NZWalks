using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NZWalks.API.Data
{
    public class NZWalksAuthAbContext : IdentityDbContext
    {
        public NZWalksAuthAbContext(DbContextOptions<NZWalksAuthAbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var readerRoleId = "a71a55d6-99d7-4123-b4e0-1218ecb90e3e";
            var writerRoleId = "4874c8b4-c9a3-45df-9d91-79dd1c965574";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerRoleId,
                    ConcurrencyStamp = readerRoleId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper()
                },
                new IdentityRole
                {
                    Id = writerRoleId,
                    ConcurrencyStamp = writerRoleId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper()
                },
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
