using Duplex.Infrastructure.Data.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duplex.Infrastructure.Data.Models
{
    public class Region
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2), MaxLength(40)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(2), MaxLength(6)]
        public string Code { get; set; } = null!;
    }
}
