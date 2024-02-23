using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.Model
{
    public class Subject
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public SecurityMark SecurityMark { get; set; }
    }
}
