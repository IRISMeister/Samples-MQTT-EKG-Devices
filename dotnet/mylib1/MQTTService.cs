using System;
using System.IO;
using System.Collections.Generic;
using InterSystems.Data.IRISClient.Gateway;
using InterSystems.Data.IRISClient.ADO;

namespace dc
{
    public class MQTTService : InterSystems.EnsLib.PEX.BusinessService
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

            // Repeat it until ms depleted.
            do { items.Add((dc.SimpleClass)dc.ReflectReader.get<dc.SimpleClass>(ms,schema)); }
            while (ms.Position<ms.Length);

            const int columncount = 4;  // Number of columns (p1,p2,p3,p4...) Solution.RAWDATA has
            IRIS iris = GatewayContext.GetIRIS();
            IRISList list = new IRISList();
            IRISObject newrequest;
            int elementcount;
            foreach (dc.SimpleClass simple in items)
            {
                elementcount=simple.myBytes.Length;
                // get unique value via Native API
                seqno = (long)iris.ClassMethodLong("Solution.RAWDATA", "GETNEWID");

                //Split received array into rows.
                for (int i = 0; i < elementcount; i += columncount)
                {
                    list.Clear();
                    for (int j = 0; j < columncount; j++) {
                        if ((i+j+1)==elementcount) { break; };
                        list.Add(simple.myBytes[i+j]);
                    }
                    iris.ClassMethodStatusCode("Solution.RAWDATA", "INSERT", seqno, list);
                }

                newrequest = (IRISObject)iris.ClassMethodObject("Solution.RAWDATAC", "%New", seqno);

                // Iterate through target business components and send request message
                string[] targetNames = TargetConfigNames.Split(',');
                foreach (string name in targetNames)
                {
                    LOGINFO("Target:" + name);
                    SendRequestAsync(name, newrequest);
                }
            }
            return null;
        }

    }
}
