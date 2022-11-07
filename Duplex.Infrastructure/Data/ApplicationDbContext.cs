﻿using Duplex.Infrastructure.Data.Models;
using Duplex.Infrastructure.Data.Models.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Duplex.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Prize> Prizes { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}