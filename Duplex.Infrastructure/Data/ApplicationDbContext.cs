using Duplex.Infrastructure.Data.Models;
using Duplex.Infrastructure.Data.Models.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Duplex.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Bet> Bets { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Region> Regions { get; set; } = null!;
        public DbSet<Prize> Prizes { get; set; } = null!;
        public DbSet<EventUser> EventsUsers { get; set; } = null!;
        public DbSet<UserBet> UsersBets { get; set; } = null!;
        public DbSet<UserPrize> UsersPrizes { get; set; } = null!;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<EventUser>()
                .HasKey(k => new { k.UserId, k.EventId });

            builder.Entity<UserBet>()
                .HasKey(k => new { k.UserId, k.BetId });

            builder.Entity<Region>().HasData(
                new Region() { Id = 1, Code = "Unset", Name = "Unknown" },
                new Region() { Id = 2, Code = "EUNE", Name = "Europe Nordic & East" },
                new Region() { Id = 3, Code = "NA", Name = "North America" });

            base.OnModelCreating(builder);
    }
}
}