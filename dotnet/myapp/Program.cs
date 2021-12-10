using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//for ADO.NET
using InterSystems.Data.IRISClient;

//for NAtive API
using InterSystems.Data.IRISClient.ADO;

using Avro;
using Avro.IO;
using Avro.Generic;

namespace adonet
{
    class Program
    {
        static void Main(string[] args)
        {
         
            int iris_object_id=1;
            if(args.Length>0) iris_object_id=int.Parse(args[0]);

            /*
             * How to handle Generic
             */ 
            HandleGeneric();

            /*
             * How to deserialize MQTT message.
             */ 
            dc.MyLibrary my = new dc.MyLibrary();
            try {
                IRISObject msg = my.GetEnsLibMQTT(iris_object_id);
                byte[] b = msg.GetBytes("StringValue");
                Console.WriteLine(b.Length+ " bytes received.");
                MemoryStream myms = new MemoryStream();
                myms.Write(b, 0, b.Length);
                myms.Position = 0;
                // may garble your console...
                //Console.WriteLine((new System.IO.StreamReader(myms)).ReadToEnd());

                string path = "/share/SimpleClass.avsc";
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                var reader = new StreamReader(fs, Encoding.UTF8);
                string schema = reader.ReadToEnd();
                fs.Close();

                Schema rs;
                Schema ws;
                rs = Schema.Parse(schema); //c# cannot parse from a File.
                ws = Schema.Parse(schema);
                myms.Position = 0;

                SimpleClass s=ReflectReader.deserialize<SimpleClass>(myms,ws,rs);

                Console.WriteLine("Received ToString():"+s.ToString());
                Console.WriteLine("Received GetType():"+s.GetType());

                Console.WriteLine("Values read from EnsLib.MQTT.Message.");
                Console.WriteLine("myString:{0} ", s.myString);
                Console.Write("myBytes: ");
                for (int i = 0; i < s.myBytes.Length; i++)
                {
                    Console.Write("[{0}] ", s.myBytes[i].ToString());
                }                
                
                Console.WriteLine(" ");
/*
                // use GenericReader to decode very simple array of integers
                string path = "/share/BinaryEncoder.avsc";
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                var reader = new StreamReader(fs, Encoding.UTF8);
                //string schema="{\"type\": \"array\", \"items\": \"int\"}";
                string schema = reader.ReadToEnd();
                fs.Close();
                Console.WriteLine(schema);

                Schema rs;
                Schema ws;
                rs = Schema.Parse(schema); //c# cannot parse from a File.
                ws = Schema.Parse(schema);
                myms.Position = 0;
                GenericReader<object> r = new GenericReader<object>(ws, rs);
                Avro.IO.Decoder d = new BinaryDecoder(myms);

                object reuse = default( object );
                Object values=r.Read( reuse,d);
                Console.WriteLine(values.ToString());
                Console.WriteLine("Received :"+values.GetType());

                Console.WriteLine("Values read from EnsLib.MQTT.Message.");
                Object[] objs = (Object[])values;
                for (int i = 0; i < objs.Length; i++)
                {
                    if (i==0) { Console.WriteLine("Class={0} Values=...", objs[i].GetType().FullName); }
                    Console.Write("{0} ", objs[i].ToString());
                }
                Console.WriteLine(" ");
*/

            }
            catch (Exception e) {
                Console.WriteLine("No EnsLib.MQTT.Message found.");
                Console.WriteLine("Exception Source:"+e.Source);
                Console.WriteLine(e.StackTrace);
                //throw;
            }

            /*
             * How to use Native APIs
             */ 
            IRISObject input = my.DoSomethingNative("MyTopic", "MyData");
            IRISObject input2 = my.DoSomethingSQL("MyTopic", "MyData");

            /*
             * general AVRO tests
             */ 
            AVROTest();

            Console.WriteLine("Hit any key");
            Console.ReadLine();

        }

        static void HandleGeneric() {
            dc.GenericList<int> list1 = new dc.GenericList<int>();
            list1.Add(1);

            // Declare a list of type string.
            dc.GenericList<string> list2 = new dc.GenericList<string>();
            list2.Add("abc");

            // Declare a list of type ExampleClass.
            dc.GenericList<dc.MyLibrary> list3 = new dc.GenericList<dc.MyLibrary>();
            list3.Add(new dc.MyLibrary());
        }

        static void AVROTest() {
            string schema;
            Object[] objs;

            schema = "[{\"type\": \"array\", \"items\": \"string\"}, \"string\"]";
            object value = new string[] { "aaa", "bbb" };
            var r=GenericReader.test<object>(schema, value);
            objs = (Object[])r;
            Console.WriteLine(objs.Length);
            for (int i = 0; i < objs.Length; i++)
            {
                Console.WriteLine("Class={0}, Value={1}", objs[i].GetType().FullName, objs[i].ToString());
            }

            schema = "[{\"type\": \"array\", \"items\": \"float\"}, \"double\"]";
            object fvalue = new float[] { 23.67f, 22.78f };
            r=GenericReader.test<object>(schema, fvalue);
            objs = (Object[])r;
            for (int i = 0; i < objs.Length; i++)
            {
                Console.WriteLine("Class={0}, Value={1}", objs[i].GetType().FullName, objs[i].ToString());
            }

            schema = "[{\"type\": \"array\", \"items\": \"int\"}, \"int\"]";
            object intvalue = new int[] { 1,2,3,4,5 };
            r = GenericReader.test<object>(schema, intvalue);
            objs = (Object[])r;
            for (int i = 0; i < objs.Length; i++)
            {
                Console.WriteLine("Class={0}, Value={1}", objs[i].GetType().FullName, objs[i].ToString());
            }

            schema = ComplexClass.SCHEMA;
            ComplexClass z= ComplexClass.Populate();
            r=ReflectReader.protocol<ComplexClass>(schema, z);
            Console.WriteLine(((ComplexClass)r).myString);

            SimpleClass s= SimpleClass.Populate();
            schema=SimpleClass.SCHEMA;

            r=ReflectReader.schema<SimpleClass>(schema, s);
            Console.WriteLine(((SimpleClass)r).myString);

        }
    }


}
