# Unsafe JSON ;)

Serialization only (no deserialization). Entirely built around `byte*`

## Misc TODO

- switch back to nuget for sigil once [LoadIndirect supports char and bool](https://github.com/kevin-montrose/Sigil/pull/21)
- Better return value when insufficient space
  + object serializer
    * currently just doubles original avail param
    * `member result - minLength` might be sufficient
    * should decrease available before serializing member, save some room for following members
- Minimum length (and maybe reserved length) per schema  
  DateTime and Guid are JSON type string but we know a lot more about their length than the general string case
- Inline constant properties (see optimizations)
- Call Dispose on enumerators

## Supports

### .NET -> JSON

(list from [JIL](https://github.com/kevin-montrose/Jil))

- Strings 
	- String **(writer done)**
	- Char **(TODO)**
- Boolean **(writer done)**
- Integer numbers (int, long, byte, etc.)
	- int, uint **(writer done)**
	- long, ulong **(TODO)**
- Floating point numbers (float, double, and decimal) **(TODO)**
- DateTimes & DateTimeOffsets
	- DateTime **(writer done)**
	- DateTimeOffset **(TODO)**
- Nullable types **(TODO)**
- Enumerations **(TODO)**
	- Including [Flags]
- Guids **(writer done)**
	- Only the "D" format
- IList&lt;T&gt; implementations **(TODO)**
- IDictionary&lt;TKey, TValue&gt; **(TODO)**
	- implementations where TKey is a string or enumeration

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
	  TODO
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
	  TODO, requires TKey to be string or enum
- array
	- from array  
	  handled by delegate generator
	- from `IEnumerable<>`  
	  TODO
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

