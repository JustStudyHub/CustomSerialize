using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CustomSerialize
{
    class CustomFormater
    {
        Dictionary<string, string> _header;
        List<string> _info;
        List<string> _deserializeInfo;
        public CustomFormater()
        {
            _header = new Dictionary<string, string>();
            _info = new List<string>();
            _deserializeInfo = new List<string>();
        }       
        public void Serialize(FileStream fs, object obj)
        {
            ObjFormat(obj);
            WriteToStream(fs);
        }

        public void Serialize(FileStream fs, object[] objects)
        {
            foreach(object obj in objects)
            {
                ObjFormat(obj);
            }
            WriteToStream(fs);
        }
        public List<object> Deserialize(StreamReader sr)
        {
            ReadFromStream(sr);
            List<object> res = new List<object>();
            Dictionary<string, string[]> typeDescript = new Dictionary<string, string[]>();
            string objDescription = null;
            string[] objProperties;
            int i = 0;
            int tempInt = 0;
            double tempDouble = 0;
            bool tempBool = false;
            while (objDescription != "")
            {
                objDescription = _deserializeInfo[i];
                objProperties = objDescription.Split('\t');
                typeDescript.Add(objProperties[0], objProperties);
                i++;
            }
            //i++;
            for (; i<_deserializeInfo.Count; ++i)
            {
                objDescription = _deserializeInfo[i];
                objProperties = objDescription.Split('\t');
                if (typeDescript.ContainsKey(objProperties[0]))
                {
                    string[] properties = typeDescript[objProperties[0]];
                    object temp = Activator.CreateInstance(null, "CustomSerialize." + objProperties[0]).Unwrap();
                    Type type = temp.GetType();
                    for (int j = 1; j < properties.Length; j++)
                    {
                        PropertyInfo prop = type.GetProperty(properties[j], BindingFlags.Public | BindingFlags.Instance);
                        if (null != prop && prop.CanWrite)
                        {
                            if(Int32.TryParse(objProperties[j], out tempInt))
                            {
                                prop.SetValue(temp, tempInt, null);
                            }
                            else if(double.TryParse(objProperties[j], out tempDouble))
                            {
                                prop.SetValue(temp, tempDouble, null);
                            }
                            else if (bool.TryParse(objProperties[j], out tempBool))
                            {
                                prop.SetValue(temp, tempBool, null);
                            }
                            else
                            {
                                prop.SetValue(temp, objProperties[j], null);
                            }
                        }
                    }
                    res.Add(temp);
                }
            }
            return res;
        }
        private void ObjFormat(object obj)
        {
            Type type = obj.GetType();
            var properties = type.GetProperties();
            var propertiesWithAttr = properties.Select(pi => new
            { Property = pi, Attribute = pi.GetCustomAttributes(typeof(TxtSerializableAttribute), true).FirstOrDefault() as TxtSerializableAttribute })
            .Where(x => x.Attribute != null)
            .ToList();
            StringBuilder sb = new StringBuilder();
            if (!_header.ContainsKey(type.Name))
            {
                sb.AppendFormat(type.Name);
                foreach (var pa in propertiesWithAttr)
                {
                    sb.AppendFormat("\t{0}", pa.Property.Name);
                }
                sb.AppendLine();
                _header.Add(type.Name, sb.ToString());
            }
            sb.Clear();
            sb.AppendFormat(type.Name);
            foreach (var pa in propertiesWithAttr)
            {
                object value = pa.Property.GetValue(obj, null);
                sb.AppendFormat("\t{0}", value);
            }
            sb.AppendLine();
            _info.Add(sb.ToString());
        }
        private void WriteToStream(FileStream fs)
        {
            byte[] array;
            foreach (var h in _header)
            {
                array = Encoding.Default.GetBytes(h.Value);
                fs.Write(array, 0, array.Length);
            }
            array = Encoding.Default.GetBytes(Environment.NewLine);
            fs.Write(array, 0, array.Length);
            foreach (var i in _info)
            {
                array = Encoding.Default.GetBytes(i);
                fs.Write(array, 0, array.Length);
            }
        }

        private void ReadFromStream(StreamReader sr)
        {
            string line;
            while((line = sr.ReadLine()) != null)
            {
                _deserializeInfo.Add(line);
            }
        }
    }
}
