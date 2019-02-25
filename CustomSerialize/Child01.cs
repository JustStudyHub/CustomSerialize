using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSerialize
{
    class Child : Base
    {
        [TxtSerializable]
        public bool IsOpen { get; set; }
    }
}
