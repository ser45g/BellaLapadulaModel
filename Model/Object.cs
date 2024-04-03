using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.Model
{
    public class Object
    {
        [Required]
        public int Id { get; set; }

        public string? Path { get; set; } = null;

        public string? Name { get; set; } = null;

        [Required]
        public SecurityMark SecurityMark { get; set; }
        [Required]
        public Category SecurityCategory { get; set; }
       
    }
}
