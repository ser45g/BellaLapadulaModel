using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.Utilities
{
    static public class Extensions
    {
        public static string ReplaceFirstInstance(this string source, string find, string replace)
        {
            int index = source.IndexOf(find);
            return index < 0 ? source : source.Substring(0, index) + replace +
                 source.Substring(index + find.Length);
        }
    }
}
