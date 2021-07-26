using LibraryR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryR.ViewModels
{
    public class RecordVM
    {
        public Record Record { get; set; }
        public IEnumerable<SelectListItem> BookDropDown { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> StatusDropDown { get; set; } = new List<SelectListItem>();
    }
}
