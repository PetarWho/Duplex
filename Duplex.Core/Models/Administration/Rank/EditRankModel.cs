﻿using System.ComponentModel.DataAnnotations;

namespace Duplex.Core.Models.Administration.Rank
{
    public class EditRankModel
    {
        public string Id { get; set; } = null!;

        [Required]
        [MinLength(3), MaxLength(20)]
        public string Name { get; set; } = null!;
    }
}
