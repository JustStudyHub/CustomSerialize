using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Reflection;

namespace CustomSerialize
{
    class Program
    {
        static void Main(string[] args)
        {
            //Stream stream = File.Open("data.xml", FileMode.Create);
            //XmlSerializer serializer =  new XmlSerializer(typeof(Int16));
            //object obj = new Int32();
            //Type type = obj.GetType();
            //Console.WriteLine(type);

            Base testObj0 = new Base
            {
                Name = "testName0",
                Count = 0,
                Weight = 700
            };

            Base testObj1 = new Base
            {
                Name = "testName1",
                Count = 1,
                Weight = 300
            };

            CustomFormater cf= new CustomFormater();
            Child testObj2 = new Child
            {
                Name = "testName2",
                Count = 2,
                Weight = 600,
                IsOpen = true
            };
            Child testObj3 = new Child
            {
                Name = "testName3",
                Count = 3,
                Weight = 500,
                IsOpen = true
            };
            Child testObj4 = new Child
            {
                Name = "testName4",
                Count = 4,
                Weight = 200,
                IsOpen = true
            };

            object[] objBuff = { testObj0, testObj1, testObj2, testObj3, testObj4 };
            //cf.Serialize(testObj);
            //cf.Serialize(testObj2);
            FileStream fs = new FileStream("note.txt", FileMode.OpenOrCreate);
            cf.Serialize(fs, objBuff);
            fs.Close();

            StreamReader sr = new StreamReader("note.txt");
            List<object> objList = cf.Deserialize(sr);
            sr.Close();
            foreach(object obj in objList)
            {
                Type type = obj.GetType();
                PropertyInfo[] propInfo = type.GetProperties();
                Console.Write("{0}\t", type.Name);
                foreach (var p in propInfo)
                {
                    Console.Write("{0} - {1}\t", p.Name, p.GetValue(obj));
                }
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
