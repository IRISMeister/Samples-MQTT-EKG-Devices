using System;
using System.IO;
using InterSystems.Data.IRISClient.Gateway;
using InterSystems.Data.IRISClient.ADO;

namespace dc
{
    public class MQTTServicePEX3 : InterSystems.EnsLib.PEX.BusinessService
    {
        public string TargetConfigNames;

        public override void OnTearDown() { } // Abstract method in PEX superclass. Must override.
        public override void OnInit() { } // Abstract method in PEX superclass. Must override.

        public override object OnProcessInput(object request)
        {
            long seqno;

            LOGINFO("Message Received");
            IRISObject req = (IRISObject)request;
            LOGINFO("Received object: " + req.InvokeString("%ClassName", 1));

            String value = req.GetString("StringValue");
            LOGINFO("Received StringValue: " + value);

            String topic = req.GetString("Topic");
            LOGINFO("Received topic: " + topic);

            // ++Write your code here++
            // Decode AVRO
            byte[] b = req.GetBytes("StringValue");
            MemoryStream ms = new MemoryStream();
            ms.Write(b, 0, b.Length);
            ms.Position = 0;

            string schema;
            schema=dc.SimpleClass.SCHEMA;
            
            var r=dc.ReflectReader.get<dc.SimpleClass>(ms,schema);
            dc.SimpleClass simple = (dc.SimpleClass)r;

            IRISList myarray = new IRISList();
            for (int j = 0; j < simple.myArray.Count; j++) {
                myarray.Add(String.Join(",",simple.myArray[j]));
            }
            // --Write your code here--

            IRIS iris = GatewayContext.GetIRIS();

            // Save decoded values into IRIS via Native API
            seqno = (long)iris.ClassMethodLong("Solution.SimpleClass", "GETNEWID");
            // Pass an array as a comma separated String value.
            IRISObject newrequest = (IRISObject)iris.ClassMethodObject("Solution.SimpleClass", "%New", topic,seqno,simple.myInt,simple.myLong,simple.myBool,simple.myDouble,simple.myFloat,String.Join(",",simple.myBytes),simple.myString,myarray);

            // Iterate through target business components and send request message
            string[] targetNames = TargetConfigNames.Split(',');
            foreach (string name in targetNames)
            {
                SendRequestAsync(name, newrequest);
                LOGINFO("Target:" + name);

            }
            return null;
        }

    }
}
