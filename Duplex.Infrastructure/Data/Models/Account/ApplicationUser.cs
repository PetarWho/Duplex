﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Duplex.Infrastructure.Data.Models.Account
{
    public class ApplicationUser:IdentityUser
    {
        public string? Image { get; set; } = @"https://drive.google.com/thumbnail?id=1dcQndrPP6Ehv0bC1tDOA1DItnJIjH9qD";

        [Required]
        [ForeignKey(nameof(Region))]
        public int RegionId { get; set; }
        public Region Region { get; set; } = null!;
        public int Coins { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Loses { get; set; } = 0;
        public ICollection<Bet> Bets { get; set; } = new HashSet<Bet>();
        public ICollection<Prize> Prizes { get; set; } = new HashSet<Prize>();
        public ICollection<EventUser> Events { get; set; } = new HashSet<EventUser>();
    }
}
