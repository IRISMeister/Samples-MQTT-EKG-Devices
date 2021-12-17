using System;
using System.IO;
using System.Collections.Generic;
using InterSystems.Data.IRISClient.Gateway;
using InterSystems.Data.IRISClient.ADO;

namespace dc
{
    public class MQTTServicePEX4 : InterSystems.EnsLib.PEX.BusinessService
    {
        public string TargetConfigNames;

        public override void OnTearDown() { } // Abstract method in PEX superclass. Must override.
        public override void OnInit() { } // Abstract method in PEX superclass. Must override.

        public override object OnProcessInput(object request)
        {
            long seqno;

            IRISObject req = (IRISObject)request;
            LOGINFO("Received object: " + req.InvokeString("%ClassName", 1));

            String value = req.GetString("StringValue");
            LOGINFO("Received StringValue: " + value);

            String topic = req.GetString("Topic");
            LOGINFO("Received topic: " + topic);

            // Decode AVRO
            byte[] b = req.GetBytes("StringValue");
            MemoryStream ms = new MemoryStream();
            ms.Write(b, 0, b.Length);
            ms.Position = 0;

            string schema;
            schema=dc.SimpleClass.SCHEMA;
            
            // Add all record(s) into a list
            var items = new List<dc.SimpleClass>();
            dc.SimpleClass s;

            // Repeat it until ms depleted.
            do {
                s = (dc.SimpleClass)dc.ReflectReader.get<dc.SimpleClass>(ms,schema);
                items.Add(s);
            }
            while (ms.Position<ms.Length);

            // Can't send XEP(dotnet) class as is.  So we need to create a simple message class via Native API to hold it.
            IRIS iris = GatewayContext.GetIRIS();
            IRISObject newrequest;
            dc.MyLibrary mylib = new dc.MyLibrary();
            foreach (dc.SimpleClass simple in items)
            {
                // Save decoded data into IRIS via XEP
                mylib.XEP("dc.SimpleClass", simple);

                // Use myLong as an unique key. 
                seqno = simple.myLong;
                // Save a container message into IRIS via Native API
                newrequest = (IRISObject)iris.ClassMethodObject("Solution.SimpleClassC", "%New", topic,seqno);
                
                // Iterate through target business components and send request message
                string[] targetNames = TargetConfigNames.Split(',');
                foreach (string name in targetNames)
                {
                    SendRequestAsync(name, newrequest);
                    LOGINFO("Target:" + name);
                }
            }
            return null;
        }

    }
}
