﻿using System.ComponentModel.DataAnnotations;

namespace Duplex.Models.Prize
{
    public class AddPrizeViewModel
    {
        [Required]
        [MinLength(3), MaxLength(30)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(1, 9999, ConvertValueInInvariantCulture = true)]
        public int Cost { get; set; }

        [Required]
        [MinLength(5), MaxLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public DateTime CreatedOnUTC { get; set; }
    }
}
