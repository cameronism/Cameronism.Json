# Unsafe JSON ;)

Serialization only (no deserialization). Entirely built around `byte*`

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

### JSON -> .NET

No deserialization (use JIL or Newtonsoft)

- string
	- from `System.String`  
	  writer done
	- from `System.Char`  
	  writer done
	- from `System.Guid`  
	  writer done
	- from `System.DateTime`  
	  writer done
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
	- from `System.Double`  
	  TODO
	- from `System.Float`  
	  TODO
	- from `System.Decimal`  
	  TODO
	- from `System.DateTime`  
	  TODO
	- from enum  
	  TODO
- object
	- from type composed of other supported types  
	  TODO
	- from `IDictionary<TKey, TValue>`  
	  TODO, requires TKey to be string or enum
- array
	- from array  
	  TODO
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
	  TODO

## Optimizations

- Not TextWriter based, `byte*` based
- Member order  
  TODO
- Whitespace for 8 byte alignment  
  TODO