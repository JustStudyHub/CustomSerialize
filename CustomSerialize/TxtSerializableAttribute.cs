using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSerialize
{
    [AttributeUsage(AttributeTargets.Property)]
    class TxtSerializableAttribute : Attribute
    {
        public TxtSerializableAttribute()
        {

        }
    }
}
