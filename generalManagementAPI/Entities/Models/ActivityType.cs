using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class ActivityType
    {
        [Key]
        public int ActivityTypeId { get; set; }
        public string ActivityTypeName { get; set; }
    }
}