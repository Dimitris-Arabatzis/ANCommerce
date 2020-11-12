﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Rocky_Models
{
    public class InquiryDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string InquiryHeaderId { get; set; }
        [ForeignKey("InquiryHeaderId")]
        public InquiryHeader InquiryHeader { get; set; }

        [Required]
        public string ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

    }
}