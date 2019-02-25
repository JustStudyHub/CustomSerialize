using System;

namespace CustomSerialize
{
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
