using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryR.Models
{
    public class Author
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        [DisplayName("First name")]
        [Required]
        public string FirstName { get; set; }
        [DisplayName("Last name")]
        public string? LastName { get; set; }
        [DisplayName("Born year")]
        public int? BornYear { get; set; }

        
        public ICollection<Book> Books { get; set; }
    }
}
