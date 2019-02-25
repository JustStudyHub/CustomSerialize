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
        public CustomFormater()
        {
            _header = new Dictionary<string, string>();
            _info = new List<string>();
        }
        public void Serialize(FileStream fs, object obj)
        {
            SerializeObj(obj);
            WriteToStream(fs);
        }

        public void Serialize(FileStream fs, object[] objects)
        {
            foreach (object obj in objects)
            {
                SerializeObj(obj);
            }
            WriteToStream(fs);
        }
        public List<object> Deserialize(StreamReader sr)
        {
            List<string> deserializeInfo = ReadFromStream(sr);
            List<object> res = new List<object>();
            Dictionary<string, string[]> typeDescript = new Dictionary<string, string[]>();
            string objDescription = string.Empty;
            string[] objProperties;
            string typeName = string.Empty;
            int i = 0;

            while (!string.IsNullOrEmpty(deserializeInfo[i]))
            {
                objDescription = deserializeInfo[i];
                objProperties = objDescription.Split('\t');
                typeName = objProperties[0];
                typeDescript.Add(typeName, objProperties);
                i++;
            }
            for (; i < deserializeInfo.Count; ++i)
            {
                objDescription = deserializeInfo[i];
                objProperties = objDescription.Split('\t');
                if (typeDescript.ContainsKey(typeName))
                {
                    string[] properties = typeDescript[typeName];
                    string assName = Assembly.GetExecutingAssembly().GetName().Name;
                    object temp = Activator.CreateInstance(assName, assName + "." + typeName).Unwrap();
                    for (int j = 1; j < properties.Length; j++)
                    {
                        PropertyInfo prop = temp.GetType().GetProperty(properties[j], BindingFlags.Public | BindingFlags.Instance);
                        if (null != prop && prop.CanWrite)
                        {
                            if (int.TryParse(objProperties[j], out int tempInt))
                            {
                                prop.SetValue(temp, tempInt, null);
                            }
                            else if (double.TryParse(objProperties[j], out double tempDouble))
                            {
                                prop.SetValue(temp, tempDouble, null);
                            }
                            else if (bool.TryParse(objProperties[j], out bool tempBool))
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
        private void SerializeObj(object obj)
        {
            Type type = obj.GetType();
            var properties = type.GetProperties();
            var propertiesWithAttr = properties.Select(
                    pi => new
                    {
                        Property = pi,
                        Attribute = pi.GetCustomAttributes(typeof(TxtSerializableAttribute), true).FirstOrDefault() as TxtSerializableAttribute
                    }
                )
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

        private List<string> ReadFromStream(StreamReader sr)
        {
            List<string> deserializeInfo = new List<string>();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                deserializeInfo.Add(line);
            }
            return deserializeInfo;
        }
    }
}
