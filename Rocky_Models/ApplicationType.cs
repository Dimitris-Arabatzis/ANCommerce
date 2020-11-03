using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky_Models
{
    public class ApplicationType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MinLength(2)]
        public string Name { get; set; }
    }
}
