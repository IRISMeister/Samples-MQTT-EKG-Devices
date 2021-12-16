Class Solution.SimpleClass Extends (%Persistent, %XML.Adaptor, Ens.Util.RequestBodyMethods)
{

Property topic As %String;

Property seq As %BigInt;

Property myInt As %Library.Integer;

Property myLong As %Library.BigInt;

Property myBool As %Library.Boolean;

Property myDouble As %Library.Double;

Property myFloat As %Library.Float;

Property myBytes As %Library.RawString;

Property myString As %Library.String;

Property myArray As list Of %Library.RawString(STORAGEDEFAULT = "list");

Property myMap As array Of %Library.String(STORAGEDEFAULT = "list");

Index idxseq On seq;

Method %OnNew(topic As %String, seq As %BigInt) As %Status [ Private, ServerOnly = 1 ]
{
    Set ..topic=topic
    Set ..seq=seq
    Return $$$OK
}
ClassMethod GETNEWID() As %Integer
{
    Quit $INCREMENT(^SIMPLECLASSUNIQ)
}

}
