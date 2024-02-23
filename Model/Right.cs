using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.Model
{
    public enum Right { None,Read,Write,Execute,All}
    public class SecurityRight
    {
        public Object Object { get; set; }
        public Subject Subject { get; set; }
        public Right Right { get; set; }
    }
}
