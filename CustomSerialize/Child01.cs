
namespace CustomSerialize
{
    class Child : Base
    {
        [TxtSerializable]
        public bool IsOpen { get; set; }
    }
}
