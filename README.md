# Cameronism.Json

*Very* fast JSON serializer for .NET

This serializer gains much of its speed by writing directly to byte pointers `byte*`.
By avoiding the overhead of TextWriter and Stream this JSON serializer is the speed of
binary serializers.

Deserialization is not currently supported, I recommend JIL for your deserialization needs.

## Benchmarks

** Array with integers 0 through 1024, 1024 times **

| Serializer      | Average Milliseconds |
| --------------  | --------------------:|
| Cameronism.Json |                    9 |
| Jil             |                   41 |
| ProtoBuf        |                  170 |
| Newtonsoft      |                  179 |


** Array with integers 0 through 1024 as strings, 1024 times **

| Serializer      | Average Milliseconds |
| --------------  | --------------------:|
| Cameronism.Json |                   24 |
| Jil             |                   85 |
| ProtoBuf        |                  107 |
| Newtonsoft      |                  134 |


** Dictionary with 1 million small objects **

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

- Add unit test for (enumerable of) memberless object
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

