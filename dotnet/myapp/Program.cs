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
         
            dc.GenericList<int> list1 = new dc.GenericList<int>();
            list1.Add(1);

            // Declare a list of type string.
            dc.GenericList<string> list2 = new dc.GenericList<string>();
            list2.Add("abc");

            // Declare a list of type ExampleClass.
            dc.GenericList<dc.MyLibrary> list3 = new dc.GenericList<dc.MyLibrary>();
            list3.Add(new dc.MyLibrary());


            dc.MyLibrary my = new dc.MyLibrary();
            try {
                IRISObject msg = my.GetEnsLibMQTT(1);
                byte[] b = msg.GetBytes("StringValue");
                MemoryStream myms = new MemoryStream();
                myms.Write(b, 0, b.Length);
                myms.Position = 0;
                // may garble your console...
                Console.WriteLine((new System.IO.StreamReader(myms)).ReadToEnd());

                /*
                Unhandled Exception: System.InvalidCastException: Unable to cast object of type 'System.Object[]' to type 'System.Int32[]'.
                at Avro.Generic.DefaultReader.Read[T](T reuse, Decoder decoder)
                at adonet.Program.Main(String[] args) in /source/myapp/Program.cs:line 54

                string schema="{\"type\": \"array\", \"items\": \"int\"}";
                Schema rs;
                Schema ws;
                rs = Schema.Parse(schema);
                ws = Schema.Parse(schema);
                myms.Position = 0;
                GenericReader<int[]> r = new GenericReader<int[]>(ws, rs);
                Avro.IO.Decoder d = new BinaryDecoder(myms);

                int[] reuse = default( int[] );
                Console.WriteLine("Now Read");
                r.Read( reuse, d );
                */
            }
            catch (Exception e) {
                if (e.Source=="Avro") throw;
                Console.WriteLine("No EnsLib.MQTT.Message found.");
                Console.WriteLine(e.Source);
            }

            //IRISObject input = my.DoSomethingNative("MyTopic", "MyData");
            //IRISObject input2 = my.DoSomethingSQL("MyTopic", "MyData");

            Console.WriteLine("Hit any key");
            Console.ReadLine();

        }
    }


}
