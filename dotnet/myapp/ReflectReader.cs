/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.Collections.Generic;
using Avro;
using Avro.IO;
using Avro.Generic;
using Avro.Specific;
using Avro.Reflect;


    public enum MyEnum
    {
        A,
        B,
        C
    }
    public class A
    {
        public long f1 { get; set; }
    }
    public class newRec
    {
        public long f1 { get; set; }
    }

    public class Z
    {
        public int? myUInt { get; set; }

        public long? myULong { get; set; }

        public bool? myUBool { get; set; }

        public double? myUDouble { get; set; }

        public float? myUFloat { get; set; }

        public byte[] myUBytes { get; set; }

        public string myUString { get; set; }

        public int myInt { get; set; }

        public long myLong { get; set; }

        public bool myBool { get; set; }

        public double myDouble { get; set; }

        public float myFloat { get; set; }

        public byte[] myBytes { get; set; }

        public string myString { get; set; }

        public object myNull { get; set; }

        public byte[] myFixed { get; set; }

        public A myA { get; set; }

        public A myNullableA { get; set; }

        public MyEnum myE { get; set; }

        public List<byte[]> myArray { get; set; }

        public List<newRec> myArray2 { get; set; }

        public Dictionary<string, string> myMap { get; set; }

        public Dictionary<string, newRec> myMap2 { get; set; }

        public object myObject { get; set; }

        public List<List<object>> myArray3 { get; set; }
    }
    public class ReflectReader
    {
    
    public static T test<T>(string s, T value)
    {
        Stream stream;
        Avro.Schema ws;
        serialize<T>(s, value, out stream, out ws);

        // save to a file.
        string path = "Reflect.avro";
        FileStream fs = new FileStream(
            path, FileMode.Create, FileAccess.Write);
        stream.CopyTo(fs);
        fs.Close();
        stream.Position = 0;
        

        T output = deserialize<T>(stream, ws, ws);
        return output;
    }


    private static void serialize<T>(string writerSchema, T actual, out Stream stream, out Avro.Schema ws)
    {

        Protocol protocol = Protocol.Parse(writerSchema);
        ws = null;
        foreach (var s in protocol.Types)
        {
            if (s.Name == "Z")
            {
                ws = s;
            }
        }

        var ms = new MemoryStream();
        Encoder e = new BinaryEncoder(ms);

        ReflectWriter<T> w  = new ReflectWriter<T>(ws);

        w.Write(actual, e);
        ms.Flush();
        ms.Position = 0;
        stream = ms;
    }

    private static S deserialize<S>(Stream ms, Avro.Schema ws, Avro.Schema rs)
    {
        //S deserialized = null;
        long initialPos = ms.Position;

        ReflectReader<S> r = new ReflectReader<S>(ws, rs);
        Decoder d = new BinaryDecoder(ms);
        return r.Read(default(S), new BinaryDecoder(ms));
    }
    
    }
