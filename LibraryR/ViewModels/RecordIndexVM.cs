using LibraryR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryR.ViewModels
{
    public class RecordIndexVM
    {
        public Record Record { get; set; }
        [DisplayName("Book Title")]
        public string BookTitle { get; set; }
        [DisplayName("Status")]
        public string StatusString { get; set; }
    }
}
