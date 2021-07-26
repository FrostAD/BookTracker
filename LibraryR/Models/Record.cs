using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryR.Models
{
    public class Record
    {
        [Required]
        public string BookId { get; set; }
        public string UserId { get; set; }
        public string StatusTypeId { get; set; }
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        public DateTime RecordDate { get; set; }

        
    }
}
