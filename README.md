# Cameronism.Json

*Very* fast JSON serializer for .NET

This serializer gains much of its speed by writing directly to byte pointers `byte*`.
By avoiding the overhead of TextWriter and Stream this JSON serializer is the speed of
binary serializers.

Deserialization is not currently supported, I recommend JIL for your deserialization needs.

## Benchmarks

Array with integers 0 through 1024, 1024 times

| Serializer      | Average Milliseconds |
| --------------  | --------------------:|
| Cameronism.Json |                    9 |
| Jil             |                   41 |
| ProtoBuf        |                  170 |
| Newtonsoft      |                  179 |

---

Array with integers 0 through 1024 as strings, 1024 times

| Serializer      | Average Milliseconds |
| --------------  | --------------------:|
| Cameronism.Json |                   24 |
| Jil             |                   85 |
| ProtoBuf        |                  107 |
| Newtonsoft      |                  134 |

---

Dictionary with 1 million small objects

| Serializer      | Average Milliseconds |
| --------------  | --------------------:|
| Cameronism.Json |                  415 |
| ProtoBuf        |                  889 |
| Jil             |                 1230 |
| Newtonsoft      |                 3195 |

Small object:

```csharp
new
{
	SessionId = Guid.NewGuid(),
	Timestamp = DateTime.UtcNow,
	UserAgent = i + "/" + i,
	PageId = (uint)i,
	Options = new Dictionary<string,string>(),
	Groups = new List<GroupInfo>(),
}
```

## Usage

```csharp
var stuffToSerialize = GetSomeStuff();
var buffer = new byte[4096]; // for performance buffers like this should be reused
int position;
unsafe
{
	fixed (byte* ptr = buffer)
	{
		position = Serializer.Serialize(stuffToSerialize, ptr, buffer.Length);
	}
}

if (position < 0) return; // buffer was too small

/* buffer now has the JSON */

// copy it elsewhere
otherStream.Write(buffer, 0, position);

// or create a stream of your own
var myStream = new MemoryStream(buffer, 0, position);

// or get the string version of the JSON 
string debug = Encoding.UTF8.GetString(buffer, 0, position);
// if you need a string for more than debugging, use a TextWriter based serializer like Jil
```

---

Serialize may be also be called with an [UnmanagedMemoryStream](http://msdn.microsoft.com/en-us/library/system.io.unmanagedmemorystream%28v=vs.110%29.aspx).
[MemoryMappedFile](http://msdn.microsoft.com/en-us/library/system.io.memorymappedfiles.memorymappedfile%28v=vs.110%29.aspx) can be used to get an unmanaged
stream for a file on disk (or not on disk at all).

## Install Cameronism.Json

	PM> Install-Package Cameronism.Json


### JSON types from .NET types

No deserialization (use JIL or Newtonsoft)

- string
	- from `System.String`  
	  writer done
	- from `System.Char`  
	  writer done
	- from `System.Guid`  
	  writer done, only the "D" format
	- from `System.DateTime`  
	  writer done, only ISO8601
	- from `System.DateTimeOffset`  
	  TODO
	- from `System.Net.IPAddress`  
	  handled by delegate generator, very Windows specific
	- from enum
		- FlagAttribute  
		  TODO
		- non flag  
		  TODO
- number
	- from `System.Int32`  
	  writer done
	- from `System.UInt32`  
	  writer done
	- from `System.Int16`  
	  cast to `Int32`, probably not worth special case
	- from `System.UInt16`  
	  cast to `UInt32`, probably not worth special case
	- from `System.Byte`  
	  cast to `UInt32`, probably not worth special case
	- from `System.SByte`  
	  cast to `Int32`, probably not worth special case
	- from `System.Int64`  
	  writer done
	- from `System.UInt64`  
	  writer done
	- from `System.Double`  
	  writer done, wastefully uses ToString
	- from `System.Single`  
	  writer done, wastefully uses ToString
	- from `System.Decimal`  
	  writer done, wastefully uses ToString
	- from `System.DateTime`  
	  TODO
	- from enum  
	  handled by delegate generator
- object
	- from type composed of other supported types  
	  handled by delegate generator
	- from `IDictionary<TKey, TValue>`  
	  - string TKey  
	    handled by delegate generator
	  - enum TKey  
	    TODO
- array
	- from array  
	  handled by delegate generator
	- from `IEnumerable<>`  
	  handled by delegate generator
- true
	- from `System.Boolean`  
	  writer done
- false
	- from `System.Boolean`  
	  writer done
- null
	- from `System.String`  
	  writer done
	- from `System.Nullable<>`  
	  handled by delegate generator for primitives and non-dictionary objects

## Optimizations

- Not TextWriter based, `byte*` based
- Member order  
  TODO
- Whitespace for 8 byte alignment  
  TODO
- Inline constant properties
  TODO
  Properties that return a constant will not be called, the serialized value
  will be calculated when generating serialization method

## TODO

- Submit Sigil bug (and hopefully test + fix) for incorrect SigilVerificationException : MarkLabel expects a value on the stack, but it was empty

  The following fails with doVerify: true, it seems to work fine with doVerify: false

		ldarg.3
		ldlen
		conv.i4
		stloc.0 // System.Int32 available
		ldarg.0
		dup
		call Boolean get_HasValue()

		brtrue HasValue_true
		pop
		ldarg.1
		ldloc.0 // System.Int32 available
		call Int32 WriteNull(Byte*, Int32)
		ldc.i4.m1
		mul
		ldloc.0 // System.Int32 available
		add
		stloc.0 // System.Int32 available
		ldarg.2
		ldarg.3
		ldc.i4.0
		ldarg.3
		ldlen
		conv.i4
		ldloc.0 // System.Int32 available
		sub
		callvirt Void Write(Byte[], Int32, Int32)
		ret

		HasValue_true:
		call Int32 GetValueOrDefault()
		ldarg.1
		ldloc.0 // System.Int32 available
		call Int32 WriteInt32(Int32, Byte*, Int32)
		ldc.i4.m1
		mul
		ldloc.0 // System.Int32 available
		add
		stloc.0 // System.Int32 available
		ldarg.2
		ldarg.3
		ldc.i4.0
		ldarg.3
		ldlen
		conv.i4
		ldloc.0 // System.Int32 available
		sub
		callvirt Void Write(Byte[], Int32, Int32)
		ret

- Support serializing to Stream
  + It won't be as fast but will be much more accessible
  + So far, looks like it will be slower by less than 10%
  + Caller will be responsible for providing `byte[]` buffer  
    `Serializer.Serialize<T>(T value, Stream destination, byte[] buffer)`
- Better return value when insufficient space
  + object serializer
    * currently just doubles original avail param
    * `member result - minLength` might be sufficient
    * should decrease available before serializing member, save some room for following members
- Minimum length (and maybe reserved length) per schema  
  DateTime and Guid are JSON type string but we know a lot more about their length than the general string case
- Inline constant properties (see optimizations)
- Call Dispose on enumerators
- Performance test array with unrolled first iteration

