using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky_Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1,int.MaxValue,ErrorMessage = "Display Order for Category must be bigger than 0.")]
        [DisplayName("Display Order")]
        public int DisplayOrder{ get; set; }
    }
}
