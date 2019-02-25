using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSerialize
{
    [Serializable]
    class Base
    {
        public Base()
        {
            //Code = Guid.NewGuid().ToString();
        }
        [TxtSerializable]
        public string Name { get; set; }
        //[TxtSerializable]
        public string Code { get; private set; }
        [TxtSerializable]
        public int Count { get; set; }
        [TxtSerializable]
        public int Weight { get; set; }
    }
}
