using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ScrumFlix.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        [MaxLength(100)]
        public string LocationName { get; set; }
    }
}
