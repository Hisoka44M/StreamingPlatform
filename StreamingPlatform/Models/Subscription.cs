using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StreamingPlatform.Models
{
    public class Subscription
    {
        [Key]
        public int SubscriptionID { get; set; }
        public string? Name { get; set; }        // "Free" / "Premium"
        public decimal Price { get; set; }
        public int MaxDevices { get; set; }
        public string? AudioQuality { get; set; }
    }
}
