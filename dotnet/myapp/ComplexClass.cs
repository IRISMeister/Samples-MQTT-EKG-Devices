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


    public static Z Populate()
    {
        Z z = new Z()
        {
            myUInt = 1,
            myULong = 2L,
            myUBool = true,
            myUDouble = 3.14,
            myUFloat = (float)1.59E-3,
            myUBytes = new byte[3] { 0x01, 0x02, 0x03 },
            myUString = "abc",
            myInt = 1,
            myLong = 2L,
            myBool = true,
            myDouble = 3.14,
            myFloat = (float)1.59E-2,
            myBytes = new byte[3] { 0x01, 0x02, 0x03 },
            myString = "def",
            myNull = null,
            myFixed = new byte[16] { 0x01, 0x02, 0x03, 0x04, 0x01, 0x02, 0x03, 0x04, 0x01, 0x02, 0x03, 0x04, 0x01, 0x02, 0x03, 0x04 },
            myA = new A() { f1 = 3L },
            myNullableA = new A() { f1 = 4L },
            myE = MyEnum.B,
            myArray = new List<byte[]>() { new byte[] { 0x01, 0x02, 0x03, 0x04 } },
            myArray2 = new List<newRec>() { new newRec() { f1 = 4L } },
            myMap = new Dictionary<string, string>()
            {
                ["abc"] = "123"
            },
            myMap2 = new Dictionary<string, newRec>()
            {
                ["abc"] = new newRec() { f1 = 5L }
            },
            myObject = new A() { f1 = 6L },
            myArray3 = new List<List<object>>() { new List<object>() { 7.0, "def" } }
        };
        return z;
    } 

}
