using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportERP.Models.Entities
{
    public class Driver : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(15)]
        public string Mobile { get; set; } = string.Empty;

        [StringLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;
    }
}
