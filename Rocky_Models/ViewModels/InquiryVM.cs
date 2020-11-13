using System;
using System.Collections.Generic;
using System.Text;

namespace Rocky_Models.ViewModels
{
    public class InquiryVM
    {
        public InquiryHeader InquiryHeader { get; set; }
        public IEnumerable<InquiryDetail> InquiryDetails{ get; set; }
    }
}
