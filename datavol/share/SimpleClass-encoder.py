import io,os
import avro.schema
import avro.io

schema = avro.schema.Parse(open("SimpleClass.avsc", "rb").read())

data = {'myUInt': 1, 'myULong': 2, 'myUBool': True, 'myUDouble': 3.14, 'myUFloat': 0.0015899999998509884, 'myUBytes': b'\x01\x02\x03', 'myUString': 'abc', 'myInt': 1, 'myLong': 2, 'myBool': True, 'myDouble': 3.14, 'myFloat': 0.01590000092983246, 'myBytes': b'\x01\x02\x03', 'myString': 'this is a SimpleClass', 'myArray': [b'\x01\x02\x03\x04'], 'myMap': {'abc': '123'}}

writer = avro.io.DatumWriter(schema)
bytes_writer = io.BytesIO()
encoder = avro.io.BinaryEncoder(bytes_writer)
writer.write(data,encoder)

raw_bytes = bytes_writer.getvalue()
print(len(raw_bytes))
print(type(raw_bytes))

fd = os.open('SimpleClass.avro', os.O_CREAT|os.O_WRONLY)
os.write(fd, raw_bytes)
os.close(fd)

