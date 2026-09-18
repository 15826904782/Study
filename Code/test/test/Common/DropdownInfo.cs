using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Common
{
    public class DropdownInfo
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public override string ToString()
        {
            return Key;
        }
    }
}
