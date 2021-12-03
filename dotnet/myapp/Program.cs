using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//for ADO.NET
using InterSystems.Data.IRISClient;

//for NAtive API
using InterSystems.Data.IRISClient.ADO;

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
            IRISObject msg = my.GetEnsLibMQTT(1);
            byte[] b = msg.GetBytes("StringValue");

            Console.WriteLine("");

            //IRISObject input = my.DoSomethingNative("MyTopic", "MyData");
            //IRISObject input2 = my.DoSomethingSQL("MyTopic", "MyData");
        }
    }


}
