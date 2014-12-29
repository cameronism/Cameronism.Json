using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cameronism.Json.Tests.Util
{
	internal static class CorEnums
	{
		public enum System_Enum_ParseFailureKind : int
		{
			None = 0,
			Argument = 1,
			ArgumentNull = 2,
			ArgumentWithParameter = 3,
			UnhandledException = 4,
		}
		public enum System_Exception_ExceptionMessageKind : int
		{
			ThreadAbort = 1,
			ThreadInterrupted = 2,
			OutOfMemory = 3,
		}
		public enum System_ExceptionArgument : int
		{
			obj = 0,
			dictionary = 1,
			dictionaryCreationThreshold = 2,
			array = 3,
			info = 4,
			key = 5,
			collection = 6,
			list = 7,
			match = 8,
			converter = 9,
			queue = 10,
			stack = 11,
			capacity = 12,
			index = 13,
			startIndex = 14,
			value = 15,
			count = 16,
			arrayIndex = 17,
			name = 18,
			mode = 19,
			item = 20,
			options = 21,
			view = 22,
		}
		public enum System_ExceptionResource : int
		{
			Argument_ImplementIComparable = 0,
			Argument_InvalidType = 1,
			Argument_InvalidArgumentForComparison = 2,
			Argument_InvalidRegistryKeyPermissionCheck = 3,
			ArgumentOutOfRange_NeedNonNegNum = 4,
			Arg_ArrayPlusOffTooSmall = 5,
			Arg_NonZeroLowerBound = 6,
			Arg_RankMultiDimNotSupported = 7,
			Arg_RegKeyDelHive = 8,
			Arg_RegKeyStrLenBug = 9,
			Arg_RegSetStrArrNull = 10,
			Arg_RegSetMismatchedKind = 11,
			Arg_RegSubKeyAbsent = 12,
			Arg_RegSubKeyValueAbsent = 13,
			Argument_AddingDuplicate = 14,
			Serialization_InvalidOnDeser = 15,
			Serialization_MissingKeys = 16,
			Serialization_NullKey = 17,
			Argument_InvalidArrayType = 18,
			NotSupported_KeyCollectionSet = 19,
			NotSupported_ValueCollectionSet = 20,
			ArgumentOutOfRange_SmallCapacity = 21,
			ArgumentOutOfRange_Index = 22,
			Argument_InvalidOffLen = 23,
			Argument_ItemNotExist = 24,
			ArgumentOutOfRange_Count = 25,
			ArgumentOutOfRange_InvalidThreshold = 26,
			ArgumentOutOfRange_ListInsert = 27,
			NotSupported_ReadOnlyCollection = 28,
			InvalidOperation_CannotRemoveFromStackOrQueue = 29,
			InvalidOperation_EmptyQueue = 30,
			InvalidOperation_EnumOpCantHappen = 31,
			InvalidOperation_EnumFailedVersion = 32,
			InvalidOperation_EmptyStack = 33,
			ArgumentOutOfRange_BiggerThanCollection = 34,
			InvalidOperation_EnumNotStarted = 35,
			InvalidOperation_EnumEnded = 36,
			NotSupported_SortedListNestedWrite = 37,
			InvalidOperation_NoValue = 38,
			InvalidOperation_RegRemoveSubKey = 39,
			Security_RegistryPermission = 40,
			UnauthorizedAccess_RegistryNoWrite = 41,
			ObjectDisposed_RegKeyClosed = 42,
			NotSupported_InComparableType = 43,
			Argument_InvalidRegistryOptionsCheck = 44,
			Argument_InvalidRegistryViewCheck = 45,
		}
		[Flags]
		public enum System_StringSplitOptions : int
		{
			None = 0,
			RemoveEmptyEntries = 1,
		}
		public enum System_StringComparison : int
		{
			CurrentCulture = 0,
			CurrentCultureIgnoreCase = 1,
			InvariantCulture = 2,
			InvariantCultureIgnoreCase = 3,
			Ordinal = 4,
			OrdinalIgnoreCase = 5,
		}
		public enum System_DateTimeKind : int
		{
			Unspecified = 0,
			Utc = 1,
			Local = 2,
		}
		public enum System_DelegateBindingFlags : int
		{
			StaticMethodOnly = 1,
			InstanceMethodOnly = 2,
			OpenDelegateOnly = 4,
			ClosedDelegateOnly = 8,
			NeverCloseOverNull = 16,
			CaselessMatching = 32,
			SkipSecurityChecks = 64,
			RelaxedSignature = 128,
		}
		public enum System_LogLevel : int
		{
			Trace = 0,
			Status = 20,
			Warning = 40,
			Error = 50,
			Panic = 100,
		}
		[Flags]
		public enum System_AppDomain_APPX_FLAGS : int
		{
			APPX_FLAGS_UNKNOWN = 0,
			APPX_FLAGS_INITIALIZED = 1,
			APPX_FLAGS_APPX_MODEL = 2,
			APPX_FLAGS_APPX_DESIGN_MODE = 4,
			APPX_FLAGS_APPX_NGEN = 8,
			APPX_FLAGS_APPX_MASK = 14,
			APPX_FLAGS_API_CHECK = 16,
		}
		[Flags]
		public enum System_AppDomainManagerInitializationOptions : int
		{
			None = 0,
			RegisterWithHost = 1,
		}
		public enum System_AppDomainSetup_LoaderInformation : int
		{
			ApplicationBaseValue = 0,
			ConfigurationFileValue = 1,
			DynamicBaseValue = 2,
			DevPathValue = 3,
			ApplicationNameValue = 4,
			PrivateBinPathValue = 5,
			PrivateBinPathProbeValue = 6,
			ShadowCopyDirectoriesValue = 7,
			ShadowCopyFilesValue = 8,
			CachePathValue = 9,
			LicenseFileValue = 10,
			DisallowPublisherPolicyValue = 11,
			DisallowCodeDownloadValue = 12,
			DisallowBindingRedirectsValue = 13,
			DisallowAppBaseProbingValue = 14,
			ConfigurationBytesValue = 15,
			ManifestFilePathValue = 16,
			VersioningManifestBaseValue = 17,
			LoaderMaximum = 18,
		}
		public enum System_LoaderOptimization : int
		{
			NotSpecified = 0,
			SingleDomain = 1,
			MultiDomain = 2,
			MultiDomainHost = 3,
			DomainMask = 3,
			DisallowBindings = 4,
		}
		public enum System_ActivationContext_ContextForm : int
		{
			Loose = 0,
			StoreBounded = 1,
		}
		public enum System_ActivationContext_ApplicationState : int
		{
			Undefined = 0,
			Starting = 1,
			Running = 2,
		}
		public enum System_ActivationContext_ApplicationStateDisposition : int
		{
			Undefined = 0,
			Starting = 1,
			Running = 2,
			StartingMigrated = 65537,
			RunningFirstTime = 131074,
		}
		[Flags]
		public enum System_AttributeTargets : int
		{
			Assembly = 1,
			Module = 2,
			Class = 4,
			Struct = 8,
			Enum = 16,
			Constructor = 32,
			Method = 64,
			Property = 128,
			Field = 256,
			Event = 512,
			Interface = 1024,
			Parameter = 2048,
			Delegate = 4096,
			ReturnValue = 8192,
			GenericParameter = 16384,
			All = 32767,
		}
		public enum System_ConfigEvents : int
		{
			StartDocument = 0,
			StartDTD = 1,
			EndDTD = 2,
			StartDTDSubset = 3,
			EndDTDSubset = 4,
			EndProlog = 5,
			StartEntity = 6,
			EndEntity = 7,
			EndDocument = 8,
			DataAvailable = 9,
			LastEvent = 9,
		}
		public enum System_ConfigNodeType : int
		{
			Element = 1,
			Attribute = 2,
			Pi = 3,
			XmlDecl = 4,
			DocType = 5,
			DTDAttribute = 6,
			EntityDecl = 7,
			ElementDecl = 8,
			AttlistDecl = 9,
			Notation = 10,
			Group = 11,
			IncludeSect = 12,
			PCData = 13,
			CData = 14,
			IgnoreSect = 15,
			Comment = 16,
			EntityRef = 17,
			Whitespace = 18,
			Name = 19,
			NMToken = 20,
			String = 21,
			Peref = 22,
			Model = 23,
			ATTDef = 24,
			ATTType = 25,
			ATTPresence = 26,
			DTDSubset = 27,
			LastNodeType = 28,
		}
		public enum System_ConfigNodeSubType : int
		{
			Version = 28,
			Encoding = 29,
			Standalone = 30,
			NS = 31,
			XMLSpace = 32,
			XMLLang = 33,
			System = 34,
			Public = 35,
			NData = 36,
			AtCData = 37,
			AtId = 38,
			AtIdref = 39,
			AtIdrefs = 40,
			AtEntity = 41,
			AtEntities = 42,
			AtNmToken = 43,
			AtNmTokens = 44,
			AtNotation = 45,
			AtRequired = 46,
			AtImplied = 47,
			AtFixed = 48,
			PentityDecl = 49,
			Empty = 50,
			Any = 51,
			Mixed = 52,
			Sequence = 53,
			Choice = 54,
			Star = 55,
			Plus = 56,
			Questionmark = 57,
			LastSubNodeType = 58,
		}
		[Flags]
		public enum System_Console_ControlKeyState : int
		{
			RightAltPressed = 1,
			LeftAltPressed = 2,
			RightCtrlPressed = 4,
			LeftCtrlPressed = 8,
			ShiftPressed = 16,
			NumLockOn = 32,
			ScrollLockOn = 64,
			CapsLockOn = 128,
			EnhancedKey = 256,
		}
		public enum System_ConsoleColor : int
		{
			Black = 0,
			DarkBlue = 1,
			DarkGreen = 2,
			DarkCyan = 3,
			DarkRed = 4,
			DarkMagenta = 5,
			DarkYellow = 6,
			Gray = 7,
			DarkGray = 8,
			Blue = 9,
			Green = 10,
			Cyan = 11,
			Red = 12,
			Magenta = 13,
			Yellow = 14,
			White = 15,
		}
		public enum System_ConsoleKey : int
		{
			Backspace = 8,
			Tab = 9,
			Clear = 12,
			Enter = 13,
			Pause = 19,
			Escape = 27,
			Spacebar = 32,
			PageUp = 33,
			PageDown = 34,
			End = 35,
			Home = 36,
			LeftArrow = 37,
			UpArrow = 38,
			RightArrow = 39,
			DownArrow = 40,
			Select = 41,
			Print = 42,
			Execute = 43,
			PrintScreen = 44,
			Insert = 45,
			Delete = 46,
			Help = 47,
			D0 = 48,
			D1 = 49,
			D2 = 50,
			D3 = 51,
			D4 = 52,
			D5 = 53,
			D6 = 54,
			D7 = 55,
			D8 = 56,
			D9 = 57,
			A = 65,
			B = 66,
			C = 67,
			D = 68,
			E = 69,
			F = 70,
			G = 71,
			H = 72,
			I = 73,
			J = 74,
			K = 75,
			L = 76,
			M = 77,
			N = 78,
			O = 79,
			P = 80,
			Q = 81,
			R = 82,
			S = 83,
			T = 84,
			U = 85,
			V = 86,
			W = 87,
			X = 88,
			Y = 89,
			Z = 90,
			LeftWindows = 91,
			RightWindows = 92,
			Applications = 93,
			Sleep = 95,
			NumPad0 = 96,
			NumPad1 = 97,
			NumPad2 = 98,
			NumPad3 = 99,
			NumPad4 = 100,
			NumPad5 = 101,
			NumPad6 = 102,
			NumPad7 = 103,
			NumPad8 = 104,
			NumPad9 = 105,
			Multiply = 106,
			Add = 107,
			Separator = 108,
			Subtract = 109,
			Decimal = 110,
			Divide = 111,
			F1 = 112,
			F2 = 113,
			F3 = 114,
			F4 = 115,
			F5 = 116,
			F6 = 117,
			F7 = 118,
			F8 = 119,
			F9 = 120,
			F10 = 121,
			F11 = 122,
			F12 = 123,
			F13 = 124,
			F14 = 125,
			F15 = 126,
			F16 = 127,
			F17 = 128,
			F18 = 129,
			F19 = 130,
			F20 = 131,
			F21 = 132,
			F22 = 133,
			F23 = 134,
			F24 = 135,
			BrowserBack = 166,
			BrowserForward = 167,
			BrowserRefresh = 168,
			BrowserStop = 169,
			BrowserSearch = 170,
			BrowserFavorites = 171,
			BrowserHome = 172,
			VolumeMute = 173,
			VolumeDown = 174,
			VolumeUp = 175,
			MediaNext = 176,
			MediaPrevious = 177,
			MediaStop = 178,
			MediaPlay = 179,
			LaunchMail = 180,
			LaunchMediaSelect = 181,
			LaunchApp1 = 182,
			LaunchApp2 = 183,
			Oem1 = 186,
			OemPlus = 187,
			OemComma = 188,
			OemMinus = 189,
			OemPeriod = 190,
			Oem2 = 191,
			Oem3 = 192,
			Oem4 = 219,
			Oem5 = 220,
			Oem6 = 221,
			Oem7 = 222,
			Oem8 = 223,
			Oem102 = 226,
			Process = 229,
			Packet = 231,
			Attention = 246,
			CrSel = 247,
			ExSel = 248,
			EraseEndOfFile = 249,
			Play = 250,
			Zoom = 251,
			NoName = 252,
			Pa1 = 253,
			OemClear = 254,
		}
		[Flags]
		public enum System_ConsoleModifiers : int
		{
			Alt = 1,
			Shift = 2,
			Control = 4,
		}
		public enum System_ConsoleSpecialKey : int
		{
			ControlC = 0,
			ControlBreak = 1,
		}
		[Flags]
		public enum System_Base64FormattingOptions : int
		{
			None = 0,
			InsertLineBreaks = 1,
		}
		public enum System_DayOfWeek : int
		{
			Sunday = 0,
			Monday = 1,
			Tuesday = 2,
			Wednesday = 3,
			Thursday = 4,
			Friday = 5,
			Saturday = 6,
		}
		public enum System_EnvironmentVariableTarget : int
		{
			Process = 0,
			User = 1,
			Machine = 2,
		}
		public enum System_Environment_OSName : int
		{
			Invalid = 0,
			Unknown = 1,
			WinNT = 128,
			Nt4 = 129,
			Win2k = 130,
			MacOSX = 256,
			Tiger = 257,
			Leopard = 258,
		}
		public enum System_Environment_SpecialFolderOption : int
		{
			None = 0,
			DoNotVerify = 16384,
			Create = 32768,
		}
		public enum System_Environment_SpecialFolder : int
		{
			Desktop = 0,
			Programs = 2,
			MyDocuments = 5,
			Personal = 5,
			Favorites = 6,
			Startup = 7,
			Recent = 8,
			SendTo = 9,
			StartMenu = 11,
			MyMusic = 13,
			MyVideos = 14,
			DesktopDirectory = 16,
			MyComputer = 17,
			NetworkShortcuts = 19,
			Fonts = 20,
			Templates = 21,
			CommonStartMenu = 22,
			CommonPrograms = 23,
			CommonStartup = 24,
			CommonDesktopDirectory = 25,
			ApplicationData = 26,
			PrinterShortcuts = 27,
			LocalApplicationData = 28,
			InternetCache = 32,
			Cookies = 33,
			History = 34,
			CommonApplicationData = 35,
			Windows = 36,
			System = 37,
			ProgramFiles = 38,
			MyPictures = 39,
			UserProfile = 40,
			SystemX86 = 41,
			ProgramFilesX86 = 42,
			CommonProgramFiles = 43,
			CommonProgramFilesX86 = 44,
			CommonTemplates = 45,
			CommonDocuments = 46,
			CommonAdminTools = 47,
			AdminTools = 48,
			CommonMusic = 53,
			CommonPictures = 54,
			CommonVideos = 55,
			Resources = 56,
			LocalizedResources = 57,
			CommonOemLinks = 58,
			CDBurning = 59,
		}
		public enum System_GCCollectionMode : int
		{
			Default = 0,
			Forced = 1,
			Optimized = 2,
		}
		public enum System_InternalGCCollectionMode : int
		{
			NonBlocking = 1,
			Blocking = 2,
			Optimized = 4,
		}
		public enum System_GCNotificationStatus : int
		{
			Succeeded = 0,
			Failed = 1,
			Canceled = 2,
			Timeout = 3,
			NotApplicable = 4,
		}
		public enum System_MidpointRounding : int
		{
			ToEven = 0,
			AwayFromZero = 1,
		}
		public enum System_PlatformID : int
		{
			Win32S = 0,
			Win32Windows = 1,
			Win32NT = 2,
			WinCE = 3,
			Unix = 4,
			Xbox = 5,
			MacOSX = 6,
		}
		public enum System_TypeNameFormatFlags : int
		{
			FormatBasic = 0,
			FormatNamespace = 1,
			FormatFullInst = 2,
			FormatAssembly = 4,
			FormatSignature = 8,
			FormatNoVersion = 16,
			FormatAngleBrackets = 64,
			FormatStubInfo = 128,
			FormatGenericParam = 256,
			FormatSerialization = 259,
		}
		public enum System_TypeNameKind : int
		{
			Name = 0,
			ToString = 1,
			SerializationName = 2,
			FullName = 3,
		}
		public enum System_RuntimeType_MemberListType : int
		{
			All = 0,
			CaseSensitive = 1,
			CaseInsensitive = 2,
			HandleToInfo = 3,
		}
		public enum System_RuntimeType_RuntimeTypeCache_WhatsCached : int
		{
			Nothing = 0,
			EnclosingType = 1,
		}
		public enum System_RuntimeType_RuntimeTypeCache_CacheType : int
		{
			Method = 0,
			Constructor = 1,
			Field = 2,
			Property = 3,
			Event = 4,
			Interface = 5,
			NestedType = 6,
		}
		public enum System_Signature_MdSigCallingConvention : byte
		{
			Default = 0,
			C = 1,
			StdCall = 2,
			ThisCall = 3,
			FastCall = 4,
			Vararg = 5,
			Field = 6,
			LocalSig = 7,
			Property = 8,
			Unmgd = 9,
			GenericInst = 10,
			Max = 11,
			CallConvMask = 15,
			Generics = 16,
			HasThis = 32,
			ExplicitThis = 64,
		}
		public enum System_StubHelpers_AsAnyMarshaler_BackPropAction : int
		{
			None = 0,
			Array = 1,
			Layout = 2,
			StringBuilderAnsi = 3,
			StringBuilderUnicode = 4,
		}
		public enum System_StubHelpers_TypeKind : int
		{
			Primitive = 0,
			Metadata = 1,
			Projection = 2,
		}
		[Flags]
		public enum System_TimeZoneInfoOptions : int
		{
			None = 1,
			NoThrowOnInvalidTime = 2,
		}
		public enum System_TypeCode : int
		{
			Empty = 0,
			Object = 1,
			DBNull = 2,
			Boolean = 3,
			Char = 4,
			SByte = 5,
			Byte = 6,
			Int16 = 7,
			UInt16 = 8,
			Int32 = 9,
			UInt32 = 10,
			Int64 = 11,
			UInt64 = 12,
			Single = 13,
			Double = 14,
			Decimal = 15,
			DateTime = 16,
			String = 18,
		}
		public enum System_Version_ParseFailureKind : int
		{
			ArgumentNullException = 0,
			ArgumentException = 1,
			ArgumentOutOfRangeException = 2,
			FormatException = 3,
		}
		public enum System_Threading_WaitHandle_OpenExistingResult : int
		{
			Success = 0,
			NameNotFound = 1,
			PathNotFound = 2,
			NameInvalid = 3,
		}
		[Flags]
		public enum System_Threading_SynchronizationContextProperties : int
		{
			None = 0,
			RequireWaitNotification = 1,
		}
		public enum System_Threading_EventResetMode : int
		{
			AutoReset = 0,
			ManualReset = 1,
		}
		public enum System_Threading_ExecutionContext_Flags : int
		{
			None = 0,
			IsNewCapture = 1,
			IsFlowSuppressed = 2,
			IsPreAllocatedDefault = 4,
		}
		[Flags]
		public enum System_Threading_ExecutionContext_CaptureOptions : int
		{
			None = 0,
			IgnoreSyncCtx = 1,
			OptimizeDefaultCase = 2,
		}
		public enum System_Diagnostics_Tracing_EventProvider_WriteEventErrorCode : int
		{
			NoError = 0,
			NoFreeBuffers = 1,
			EventTooBig = 2,
			NullInput = 3,
			TooManyArgs = 4,
			Other = 5,
		}
		public enum System_Threading_StackCrawlMark : int
		{
			LookForMe = 0,
			LookForMyCaller = 1,
			LookForMyCallersCaller = 2,
			LookForThread = 3,
		}
		public enum System_Threading_ThreadPriority : int
		{
			Lowest = 0,
			BelowNormal = 1,
			Normal = 2,
			AboveNormal = 3,
			Highest = 4,
		}
		[Flags]
		public enum System_Threading_ThreadState : int
		{
			Running = 0,
			StopRequested = 1,
			SuspendRequested = 2,
			Background = 4,
			Unstarted = 8,
			Stopped = 16,
			WaitSleepJoin = 32,
			Suspended = 64,
			AbortRequested = 128,
			Aborted = 256,
		}
		public enum System_Threading_ApartmentState : int
		{
			STA = 0,
			MTA = 1,
			Unknown = 2,
		}
		public enum System_Threading_LazyThreadSafetyMode : int
		{
			None = 0,
			PublicationOnly = 1,
			ExecutionAndPublication = 2,
		}
		public enum System_Threading_Tasks_TaskStatus : int
		{
			Created = 0,
			WaitingForActivation = 1,
			WaitingToRun = 2,
			Running = 3,
			WaitingForChildrenToComplete = 4,
			RanToCompletion = 5,
			Canceled = 6,
			Faulted = 7,
		}
		[Flags]
		public enum System_Threading_Tasks_TaskCreationOptions : int
		{
			None = 0,
			PreferFairness = 1,
			LongRunning = 2,
			AttachedToParent = 4,
			DenyChildAttach = 8,
			HideScheduler = 16,
		}
		[Flags]
		public enum System_Threading_Tasks_InternalTaskOptions : int
		{
			None = 0,
			ChildReplica = 256,
			ContinuationTask = 512,
			PromiseTask = 1024,
			SelfReplicating = 2048,
			LazyCancellation = 4096,
			QueuedByRuntime = 8192,
			DoNotDispose = 16384,
			InternalOptionsMask = 65280,
		}
		[Flags]
		public enum System_Threading_Tasks_TaskContinuationOptions : int
		{
			None = 0,
			PreferFairness = 1,
			LongRunning = 2,
			AttachedToParent = 4,
			DenyChildAttach = 8,
			HideScheduler = 16,
			LazyCancellation = 32,
			NotOnRanToCompletion = 65536,
			NotOnFaulted = 131072,
			OnlyOnCanceled = 196608,
			NotOnCanceled = 262144,
			OnlyOnFaulted = 327680,
			OnlyOnRanToCompletion = 393216,
			ExecuteSynchronously = 524288,
		}
		public enum Windows_Foundation_Diagnostics_CausalityRelation : int
		{
			AssignDelegate = 0,
			Join = 1,
			Choice = 2,
			Cancel = 3,
			Error = 4,
		}
		public enum Windows_Foundation_Diagnostics_CausalitySource : int
		{
			Application = 0,
			Library = 1,
			System = 2,
		}
		public enum Windows_Foundation_Diagnostics_CausalitySynchronousWork : int
		{
			CompletionNotification = 0,
			ProgressNotification = 1,
			Execution = 2,
		}
		public enum Windows_Foundation_Diagnostics_CausalityTraceLevel : int
		{
			Required = 0,
			Important = 1,
			Verbose = 2,
		}
		public enum Windows_Foundation_Diagnostics_AsyncCausalityStatus : int
		{
			Started = 0,
			Completed = 1,
			Canceled = 2,
			Error = 3,
		}
		public enum System_Threading_Tasks_CausalityTraceLevel : int
		{
			Required = 0,
			Important = 1,
			Verbose = 2,
		}
		public enum System_Threading_Tasks_AsyncCausalityStatus : int
		{
			Started = 0,
			Completed = 1,
			Canceled = 2,
			Error = 3,
		}
		public enum System_Threading_Tasks_CausalityRelation : int
		{
			AssignDelegate = 0,
			Join = 1,
			Choice = 2,
			Cancel = 3,
			Error = 4,
		}
		public enum System_Threading_Tasks_CausalitySynchronousWork : int
		{
			CompletionNotification = 0,
			ProgressNotification = 1,
			Execution = 2,
		}
		[Flags]
		public enum System_Threading_Tasks_ConcurrentExclusiveSchedulerPair_ProcessingMode : byte
		{
			NotCurrentlyProcessing = 0,
			ProcessingExclusiveTask = 1,
			ProcessingConcurrentTasks = 2,
			Completing = 4,
			Completed = 8,
		}
		public enum System_Threading_Tasks_TplEtwProvider_ForkJoinOperationType : int
		{
			ParallelInvoke = 1,
			ParallelFor = 2,
			ParallelForEach = 3,
		}
		public enum System_Threading_Tasks_TplEtwProvider_TaskWaitBehavior : int
		{
			Synchronous = 1,
			Asynchronous = 2,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMSSECTIONID : int
		{
			CMSSECTIONID_FILE_SECTION = 1,
			CMSSECTIONID_CATEGORY_INSTANCE_SECTION = 2,
			CMSSECTIONID_COM_REDIRECTION_SECTION = 3,
			CMSSECTIONID_PROGID_REDIRECTION_SECTION = 4,
			CMSSECTIONID_CLR_SURROGATE_SECTION = 5,
			CMSSECTIONID_ASSEMBLY_REFERENCE_SECTION = 6,
			CMSSECTIONID_WINDOW_CLASS_SECTION = 8,
			CMSSECTIONID_STRING_SECTION = 9,
			CMSSECTIONID_ENTRYPOINT_SECTION = 10,
			CMSSECTIONID_PERMISSION_SET_SECTION = 11,
			CMSSECTIONENTRYID_METADATA = 12,
			CMSSECTIONID_ASSEMBLY_REQUEST_SECTION = 13,
			CMSSECTIONID_REGISTRY_KEY_SECTION = 16,
			CMSSECTIONID_DIRECTORY_SECTION = 17,
			CMSSECTIONID_FILE_ASSOCIATION_SECTION = 18,
			CMSSECTIONID_COMPATIBLE_FRAMEWORKS_SECTION = 19,
			CMSSECTIONID_EVENT_SECTION = 101,
			CMSSECTIONID_EVENT_MAP_SECTION = 102,
			CMSSECTIONID_EVENT_TAG_SECTION = 103,
			CMSSECTIONID_COUNTERSET_SECTION = 110,
			CMSSECTIONID_COUNTER_SECTION = 111,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_ASSEMBLY_DEPLOYMENT_FLAG : int
		{
			CMS_ASSEMBLY_DEPLOYMENT_FLAG_BEFORE_APPLICATION_STARTUP = 4,
			CMS_ASSEMBLY_DEPLOYMENT_FLAG_RUN_AFTER_INSTALL = 16,
			CMS_ASSEMBLY_DEPLOYMENT_FLAG_INSTALL = 32,
			CMS_ASSEMBLY_DEPLOYMENT_FLAG_TRUST_URL_PARAMETERS = 64,
			CMS_ASSEMBLY_DEPLOYMENT_FLAG_DISALLOW_URL_ACTIVATION = 128,
			CMS_ASSEMBLY_DEPLOYMENT_FLAG_MAP_FILE_EXTENSIONS = 256,
			CMS_ASSEMBLY_DEPLOYMENT_FLAG_CREATE_DESKTOP_SHORTCUT = 512,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_ASSEMBLY_REFERENCE_FLAG : int
		{
			CMS_ASSEMBLY_REFERENCE_FLAG_OPTIONAL = 1,
			CMS_ASSEMBLY_REFERENCE_FLAG_VISIBLE = 2,
			CMS_ASSEMBLY_REFERENCE_FLAG_FOLLOW = 4,
			CMS_ASSEMBLY_REFERENCE_FLAG_IS_PLATFORM = 8,
			CMS_ASSEMBLY_REFERENCE_FLAG_CULTURE_WILDCARDED = 16,
			CMS_ASSEMBLY_REFERENCE_FLAG_PROCESSOR_ARCHITECTURE_WILDCARDED = 32,
			CMS_ASSEMBLY_REFERENCE_FLAG_PREREQUISITE = 128,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_ASSEMBLY_REFERENCE_DEPENDENT_ASSEMBLY_FLAG : int
		{
			CMS_ASSEMBLY_REFERENCE_DEPENDENT_ASSEMBLY_FLAG_OPTIONAL = 1,
			CMS_ASSEMBLY_REFERENCE_DEPENDENT_ASSEMBLY_FLAG_VISIBLE = 2,
			CMS_ASSEMBLY_REFERENCE_DEPENDENT_ASSEMBLY_FLAG_PREREQUISITE = 4,
			CMS_ASSEMBLY_REFERENCE_DEPENDENT_ASSEMBLY_FLAG_RESOURCE_FALLBACK_CULTURE_INTERNAL = 8,
			CMS_ASSEMBLY_REFERENCE_DEPENDENT_ASSEMBLY_FLAG_INSTALL = 16,
			CMS_ASSEMBLY_REFERENCE_DEPENDENT_ASSEMBLY_FLAG_ALLOW_DELAYED_BINDING = 32,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_FILE_FLAG : int
		{
			CMS_FILE_FLAG_OPTIONAL = 1,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_ENTRY_POINT_FLAG : int
		{
			CMS_ENTRY_POINT_FLAG_HOST_IN_BROWSER = 1,
			CMS_ENTRY_POINT_FLAG_CUSTOMHOSTSPECIFIED = 2,
			CMS_ENTRY_POINT_FLAG_CUSTOMUX = 4,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_COM_SERVER_FLAG : int
		{
			CMS_COM_SERVER_FLAG_IS_CLR_CLASS = 1,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_USAGE_PATTERN : int
		{
			CMS_USAGE_PATTERN_SCOPE_APPLICATION = 1,
			CMS_USAGE_PATTERN_SCOPE_PROCESS = 2,
			CMS_USAGE_PATTERN_SCOPE_MACHINE = 3,
			CMS_USAGE_PATTERN_SCOPE_MASK = 7,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_SCHEMA_VERSION : int
		{
			CMS_SCHEMA_VERSION_V1 = 1,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_FILE_HASH_ALGORITHM : int
		{
			CMS_FILE_HASH_ALGORITHM_SHA1 = 1,
			CMS_FILE_HASH_ALGORITHM_SHA256 = 2,
			CMS_FILE_HASH_ALGORITHM_SHA384 = 3,
			CMS_FILE_HASH_ALGORITHM_SHA512 = 4,
			CMS_FILE_HASH_ALGORITHM_MD5 = 5,
			CMS_FILE_HASH_ALGORITHM_MD4 = 6,
			CMS_FILE_HASH_ALGORITHM_MD2 = 7,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_TIME_UNIT_TYPE : int
		{
			CMS_TIME_UNIT_TYPE_HOURS = 1,
			CMS_TIME_UNIT_TYPE_DAYS = 2,
			CMS_TIME_UNIT_TYPE_WEEKS = 3,
			CMS_TIME_UNIT_TYPE_MONTHS = 4,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_FILE_WRITABLE_TYPE : int
		{
			CMS_FILE_WRITABLE_TYPE_NOT_WRITABLE = 1,
			CMS_FILE_WRITABLE_TYPE_APPLICATION_DATA = 2,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_HASH_TRANSFORM : int
		{
			CMS_HASH_TRANSFORM_IDENTITY = 1,
			CMS_HASH_TRANSFORM_MANIFESTINVARIANT = 2,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CMS_HASH_DIGESTMETHOD : int
		{
			CMS_HASH_DIGESTMETHOD_SHA1 = 1,
			CMS_HASH_DIGESTMETHOD_SHA256 = 2,
			CMS_HASH_DIGESTMETHOD_SHA384 = 3,
			CMS_HASH_DIGESTMETHOD_SHA512 = 4,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_MuiResourceIdLookupMapEntryFieldId : int
		{
			MuiResourceIdLookupMap_Count = 0,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_MuiResourceTypeIdStringEntryFieldId : int
		{
			MuiResourceTypeIdString_StringIds = 0,
			MuiResourceTypeIdString_StringIdsSize = 1,
			MuiResourceTypeIdString_IntegerIds = 2,
			MuiResourceTypeIdString_IntegerIdsSize = 3,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_MuiResourceTypeIdIntEntryFieldId : int
		{
			MuiResourceTypeIdInt_StringIds = 0,
			MuiResourceTypeIdInt_StringIdsSize = 1,
			MuiResourceTypeIdInt_IntegerIds = 2,
			MuiResourceTypeIdInt_IntegerIdsSize = 3,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_MuiResourceMapEntryFieldId : int
		{
			MuiResourceMap_ResourceTypeIdInt = 0,
			MuiResourceMap_ResourceTypeIdIntSize = 1,
			MuiResourceMap_ResourceTypeIdString = 2,
			MuiResourceMap_ResourceTypeIdStringSize = 3,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_HashElementEntryFieldId : int
		{
			HashElement_Transform = 0,
			HashElement_TransformMetadata = 1,
			HashElement_TransformMetadataSize = 2,
			HashElement_DigestMethod = 3,
			HashElement_DigestValue = 4,
			HashElement_DigestValueSize = 5,
			HashElement_Xml = 6,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_FileEntryFieldId : int
		{
			File_HashAlgorithm = 0,
			File_LoadFrom = 1,
			File_SourcePath = 2,
			File_ImportPath = 3,
			File_SourceName = 4,
			File_Location = 5,
			File_HashValue = 6,
			File_HashValueSize = 7,
			File_Size = 8,
			File_Group = 9,
			File_Flags = 10,
			File_MuiMapping = 11,
			File_WritableType = 12,
			File_HashElements = 13,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_FileAssociationEntryFieldId : int
		{
			FileAssociation_Description = 0,
			FileAssociation_ProgID = 1,
			FileAssociation_DefaultIcon = 2,
			FileAssociation_Parameter = 3,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CategoryMembershipDataEntryFieldId : int
		{
			CategoryMembershipData_Xml = 0,
			CategoryMembershipData_Description = 1,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_SubcategoryMembershipEntryFieldId : int
		{
			SubcategoryMembership_CategoryMembershipData = 0,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CategoryMembershipEntryFieldId : int
		{
			CategoryMembership_SubcategoryMembership = 0,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_COMServerEntryFieldId : int
		{
			COMServer_Flags = 0,
			COMServer_ConfiguredGuid = 1,
			COMServer_ImplementedClsid = 2,
			COMServer_TypeLibrary = 3,
			COMServer_ThreadingModel = 4,
			COMServer_RuntimeVersion = 5,
			COMServer_HostFile = 6,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_ProgIdRedirectionEntryFieldId : int
		{
			ProgIdRedirection_RedirectedGuid = 0,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CLRSurrogateEntryFieldId : int
		{
			CLRSurrogate_RuntimeVersion = 0,
			CLRSurrogate_ClassName = 1,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_AssemblyReferenceDependentAssemblyEntryFieldId : int
		{
			AssemblyReferenceDependentAssembly_Group = 0,
			AssemblyReferenceDependentAssembly_Codebase = 1,
			AssemblyReferenceDependentAssembly_Size = 2,
			AssemblyReferenceDependentAssembly_HashValue = 3,
			AssemblyReferenceDependentAssembly_HashValueSize = 4,
			AssemblyReferenceDependentAssembly_HashAlgorithm = 5,
			AssemblyReferenceDependentAssembly_Flags = 6,
			AssemblyReferenceDependentAssembly_ResourceFallbackCulture = 7,
			AssemblyReferenceDependentAssembly_Description = 8,
			AssemblyReferenceDependentAssembly_SupportUrl = 9,
			AssemblyReferenceDependentAssembly_HashElements = 10,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_AssemblyReferenceEntryFieldId : int
		{
			AssemblyReference_Flags = 0,
			AssemblyReference_DependentAssembly = 1,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_WindowClassEntryFieldId : int
		{
			WindowClass_HostDll = 0,
			WindowClass_fVersioned = 1,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_ResourceTableMappingEntryFieldId : int
		{
			ResourceTableMapping_FinalStringMapped = 0,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_EntryPointEntryFieldId : int
		{
			EntryPoint_CommandLine_File = 0,
			EntryPoint_CommandLine_Parameters = 1,
			EntryPoint_Identity = 2,
			EntryPoint_Flags = 3,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_PermissionSetEntryFieldId : int
		{
			PermissionSet_XmlSegment = 0,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_AssemblyRequestEntryFieldId : int
		{
			AssemblyRequest_permissionSetID = 0,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_DescriptionMetadataEntryFieldId : int
		{
			DescriptionMetadata_Publisher = 0,
			DescriptionMetadata_Product = 1,
			DescriptionMetadata_SupportUrl = 2,
			DescriptionMetadata_IconFile = 3,
			DescriptionMetadata_ErrorReportUrl = 4,
			DescriptionMetadata_SuiteName = 5,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_DeploymentMetadataEntryFieldId : int
		{
			DeploymentMetadata_DeploymentProviderCodebase = 0,
			DeploymentMetadata_MinimumRequiredVersion = 1,
			DeploymentMetadata_MaximumAge = 2,
			DeploymentMetadata_MaximumAge_Unit = 3,
			DeploymentMetadata_DeploymentFlags = 4,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_DependentOSMetadataEntryFieldId : int
		{
			DependentOSMetadata_SupportUrl = 0,
			DependentOSMetadata_Description = 1,
			DependentOSMetadata_MajorVersion = 2,
			DependentOSMetadata_MinorVersion = 3,
			DependentOSMetadata_BuildNumber = 4,
			DependentOSMetadata_ServicePackMajor = 5,
			DependentOSMetadata_ServicePackMinor = 6,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_CompatibleFrameworksMetadataEntryFieldId : int
		{
			CompatibleFrameworksMetadata_SupportUrl = 0,
		}
		public enum System_Deployment_Internal_Isolation_Manifest_MetadataSectionEntryFieldId : int
		{
			MetadataSection_SchemaVersion = 0,
			MetadataSection_ManifestFlags = 1,
			MetadataSection_UsagePatterns = 2,
			MetadataSection_CdfIdentity = 3,
			MetadataSection_LocalPath = 4,
			MetadataSection_HashAlgorithm = 5,
			MetadataSection_ManifestHash = 6,
			MetadataSection_ManifestHashSize = 7,
			MetadataSection_ContentType = 8,
			MetadataSection_RuntimeImageVersion = 9,
			MetadataSection_MvidValue = 10,
			MetadataSection_MvidValueSize = 11,
			MetadataSection_DescriptionData = 12,
			MetadataSection_DeploymentData = 13,
			MetadataSection_DependentOSData = 14,
			MetadataSection_defaultPermissionSetID = 15,
			MetadataSection_RequestedExecutionLevel = 16,
			MetadataSection_RequestedExecutionLevelUIAccess = 17,
			MetadataSection_ResourceTypeResourcesDependency = 18,
			MetadataSection_ResourceTypeManifestResourcesDependency = 19,
			MetadataSection_KeyInfoElement = 20,
			MetadataSection_CompatibleFrameworksData = 21,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_STORE_ASSEMBLY_STATUS_FLAGS : int
		{
			STORE_ASSEMBLY_STATUS_MANIFEST_ONLY = 1,
			STORE_ASSEMBLY_STATUS_PAYLOAD_RESIDENT = 2,
			STORE_ASSEMBLY_STATUS_PARTIAL_INSTALL = 4,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_STORE_ASSEMBLY_FILE_STATUS_FLAGS : int
		{
			STORE_ASSEMBLY_FILE_STATUS_FLAG_PRESENT = 1,
		}
		public enum System_Deployment_Internal_Isolation_IIDENTITYAUTHORITY_DEFINITION_IDENTITY_TO_TEXT_FLAGS : int
		{
			IIDENTITYAUTHORITY_DEFINITION_IDENTITY_TO_TEXT_FLAG_CANONICAL = 1,
		}
		public enum System_Deployment_Internal_Isolation_IIDENTITYAUTHORITY_REFERENCE_IDENTITY_TO_TEXT_FLAGS : int
		{
			IIDENTITYAUTHORITY_REFERENCE_IDENTITY_TO_TEXT_FLAG_CANONICAL = 1,
		}
		public enum System_Deployment_Internal_Isolation_IIDENTITYAUTHORITY_DOES_DEFINITION_MATCH_REFERENCE_FLAGS : int
		{
			IIDENTITYAUTHORITY_DOES_DEFINITION_MATCH_REFERENCE_FLAG_EXACT_MATCH_REQUIRED = 1,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_IAPPIDAUTHORITY_ARE_DEFINITIONS_EQUAL_FLAGS : int
		{
			IAPPIDAUTHORITY_ARE_DEFINITIONS_EQUAL_FLAG_IGNORE_VERSION = 1,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_IAPPIDAUTHORITY_ARE_REFERENCES_EQUAL_FLAGS : int
		{
			IAPPIDAUTHORITY_ARE_REFERENCES_EQUAL_FLAG_IGNORE_VERSION = 1,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_ISTORE_BIND_REFERENCE_TO_ASSEMBLY_FLAGS : int
		{
			ISTORE_BIND_REFERENCE_TO_ASSEMBLY_FLAG_FORCE_LIBRARY_SEMANTICS = 1,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_ISTORE_ENUM_ASSEMBLIES_FLAGS : int
		{
			ISTORE_ENUM_ASSEMBLIES_FLAG_LIMIT_TO_VISIBLE_ONLY = 1,
			ISTORE_ENUM_ASSEMBLIES_FLAG_MATCH_SERVICING = 2,
			ISTORE_ENUM_ASSEMBLIES_FLAG_FORCE_LIBRARY_SEMANTICS = 4,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_ISTORE_ENUM_FILES_FLAGS : int
		{
			ISTORE_ENUM_FILES_FLAG_INCLUDE_INSTALLED_FILES = 1,
			ISTORE_ENUM_FILES_FLAG_INCLUDE_MISSING_FILES = 2,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_StoreOperationStageComponent_OpFlags : int
		{
			Nothing = 0,
		}
		public enum System_Deployment_Internal_Isolation_StoreOperationStageComponent_Disposition : int
		{
			Failed = 0,
			Installed = 1,
			Refreshed = 2,
			AlreadyInstalled = 3,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_StoreOperationStageComponentFile_OpFlags : int
		{
			Nothing = 0,
		}
		public enum System_Deployment_Internal_Isolation_StoreOperationStageComponentFile_Disposition : int
		{
			Failed = 0,
			Installed = 1,
			Refreshed = 2,
			AlreadyInstalled = 3,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_StoreApplicationReference_RefFlags : int
		{
			Nothing = 0,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_StoreOperationPinDeployment_OpFlags : int
		{
			Nothing = 0,
			NeverExpires = 1,
		}
		public enum System_Deployment_Internal_Isolation_StoreOperationPinDeployment_Disposition : int
		{
			Failed = 0,
			Pinned = 1,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_StoreOperationUnpinDeployment_OpFlags : int
		{
			Nothing = 0,
		}
		public enum System_Deployment_Internal_Isolation_StoreOperationUnpinDeployment_Disposition : int
		{
			Failed = 0,
			Unpinned = 1,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_StoreOperationInstallDeployment_OpFlags : int
		{
			Nothing = 0,
			UninstallOthers = 1,
		}
		public enum System_Deployment_Internal_Isolation_StoreOperationInstallDeployment_Disposition : int
		{
			Failed = 0,
			AlreadyInstalled = 1,
			Installed = 2,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_StoreOperationUninstallDeployment_OpFlags : int
		{
			Nothing = 0,
		}
		public enum System_Deployment_Internal_Isolation_StoreOperationUninstallDeployment_Disposition : int
		{
			Failed = 0,
			DidNotExist = 1,
			Uninstalled = 2,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_StoreOperationSetDeploymentMetadata_OpFlags : int
		{
			Nothing = 0,
		}
		public enum System_Deployment_Internal_Isolation_StoreOperationSetDeploymentMetadata_Disposition : int
		{
			Failed = 0,
			Set = 2,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_StoreOperationSetCanonicalizationContext_OpFlags : int
		{
			Nothing = 0,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_StoreOperationScavenge_OpFlags : int
		{
			Nothing = 0,
			Light = 1,
			LimitSize = 2,
			LimitTime = 4,
			LimitCount = 8,
		}
		public enum System_Deployment_Internal_Isolation_StoreTransactionOperationType : int
		{
			Invalid = 0,
			SetCanonicalizationContext = 14,
			StageComponent = 20,
			PinDeployment = 21,
			UnpinDeployment = 22,
			StageComponentFile = 23,
			InstallDeployment = 24,
			UninstallDeployment = 25,
			SetDeploymentMetadata = 26,
			Scavenge = 27,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_Store_EnumAssembliesFlags : int
		{
			Nothing = 0,
			VisibleOnly = 1,
			MatchServicing = 2,
			ForceLibrarySemantics = 4,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_Store_EnumAssemblyFilesFlags : int
		{
			Nothing = 0,
			IncludeInstalled = 1,
			IncludeMissing = 2,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_Store_EnumApplicationPrivateFiles : int
		{
			Nothing = 0,
			IncludeInstalled = 1,
			IncludeMissing = 2,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_Store_EnumAssemblyInstallReferenceFlags : int
		{
			Nothing = 0,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_Store_EnumCategoriesFlags : int
		{
			Nothing = 0,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_Store_EnumSubcategoriesFlags : int
		{
			Nothing = 0,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_Store_EnumCategoryInstancesFlags : int
		{
			Nothing = 0,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_Store_GetPackagePropertyFlags : int
		{
			Nothing = 0,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_IsolationInterop_CreateActContextParameters_CreateFlags : int
		{
			Nothing = 0,
			StoreListValid = 1,
			CultureListValid = 2,
			ProcessorFallbackListValid = 4,
			ProcessorValid = 8,
			SourceValid = 16,
			IgnoreVisibility = 32,
		}
		[Flags]
		public enum System_Deployment_Internal_Isolation_IsolationInterop_CreateActContextParametersSource_SourceFlags : int
		{
			Definition = 1,
			Reference = 2,
		}
		public enum System_Deployment_Internal_Isolation_StateManager_RunningState : int
		{
			Undefined = 0,
			Starting = 1,
			Running = 2,
		}
		[Flags]
		public enum System_Collections_Concurrent_EnumerablePartitionerOptions : int
		{
			None = 0,
			NoBuffering = 1,
		}
		public enum System_Diagnostics_AssertFilters : int
		{
			FailDebug = 0,
			FailIgnore = 1,
			FailTerminate = 2,
			FailContinueFilter = 3,
		}
		[Flags]
		public enum System_Diagnostics_DebuggableAttribute_DebuggingModes : int
		{
			None = 0,
			Default = 1,
			IgnoreSymbolStoreSequencePoints = 2,
			EnableEditAndContinue = 4,
			DisableOptimizations = 256,
		}
		public enum System_Diagnostics_DebuggerBrowsableState : int
		{
			Never = 0,
			Collapsed = 2,
			RootHidden = 3,
		}
		public enum System_Diagnostics_LoggingLevels : int
		{
			TraceLevel0 = 0,
			TraceLevel1 = 1,
			TraceLevel2 = 2,
			TraceLevel3 = 3,
			TraceLevel4 = 4,
			StatusLevel0 = 20,
			StatusLevel1 = 21,
			StatusLevel2 = 22,
			StatusLevel3 = 23,
			StatusLevel4 = 24,
			WarningLevel = 40,
			ErrorLevel = 50,
			PanicLevel = 100,
		}
		public enum System_Diagnostics_StackTrace_TraceFormat : int
		{
			Normal = 0,
			TrailingNewLine = 1,
			NoResourceLookup = 2,
		}
		public enum System_Diagnostics_Tracing_ControllerCommand : int
		{
			Update = 0,
			Disable = -3,
			Enable = -2,
			SendManifest = -1,
		}
		public enum System_Diagnostics_Tracing_EventCommand : int
		{
			Update = 0,
			Disable = -3,
			Enable = -2,
			SendManifest = -1,
		}
		public enum System_Diagnostics_Tracing_ManifestEnvelope_ManifestFormats : byte
		{
			SimpleXmlFormat = 1,
		}
		public enum System_Diagnostics_Tracing_EventLevel : int
		{
			LogAlways = 0,
			Critical = 1,
			Error = 2,
			Warning = 3,
			Informational = 4,
			Verbose = 5,
		}
		public enum System_Diagnostics_Tracing_EventTask : int
		{
			None = 0,
		}
		public enum System_Diagnostics_Tracing_EventOpcode : int
		{
			Info = 0,
			Start = 1,
			Stop = 2,
			DataCollectionStart = 3,
			DataCollectionStop = 4,
			Extension = 5,
			Reply = 6,
			Resume = 7,
			Suspend = 8,
			Send = 9,
			Receive = 240,
		}
		[Flags]
		public enum System_Diagnostics_Tracing_EventKeywords : long
		{
			None = 0,
			WdiContext = 562949953421312,
			WdiDiagnostic = 1125899906842624,
			Sqm = 2251799813685248,
			CorrelationHint = 4503599627370496,
			AuditFailure = 4503599627370496,
			AuditSuccess = 9007199254740992,
			EventLogClassic = 36028797018963968,
		}
		public enum System_Diagnostics_Contracts_ContractFailureKind : int
		{
			Precondition = 0,
			Postcondition = 1,
			PostconditionOnException = 2,
			Invariant = 3,
			Assert = 4,
			Assume = 5,
		}
		public enum System_Diagnostics_SymbolStore_SymAddressKind : int
		{
			ILOffset = 1,
			NativeRVA = 2,
			NativeRegister = 3,
			NativeRegisterRelative = 4,
			NativeOffset = 5,
			NativeRegisterRegister = 6,
			NativeRegisterStack = 7,
			NativeStackRegister = 8,
			BitField = 9,
			NativeSectionOffset = 10,
		}
		public enum System_Reflection_LoadContext : int
		{
			DEFAULT = 0,
			LOADFROM = 1,
			UNKNOWN = 2,
			HOSTED = 3,
		}
		public enum System_Reflection_RuntimeAssembly_ASSEMBLY_FLAGS : uint
		{
			ASSEMBLY_FLAGS_UNKNOWN = 0,
			ASSEMBLY_FLAGS_TOKEN_MASK = 16777215,
			ASSEMBLY_FLAGS_INITIALIZED = 16777216,
			ASSEMBLY_FLAGS_FRAMEWORK = 33554432,
			ASSEMBLY_FLAGS_SAFE_REFLECTION = 67108864,
		}
		[Flags]
		public enum System_Reflection_AssemblyNameFlags : int
		{
			None = 0,
			PublicKey = 1,
			Retargetable = 256,
			EnableJITcompileOptimizer = 16384,
			EnableJITcompileTracking = 32768,
		}
		public enum System_Reflection_AssemblyContentType : int
		{
			Default = 0,
			WindowsRuntime = 1,
		}
		public enum System_Reflection_ProcessorArchitecture : int
		{
			None = 0,
			MSIL = 1,
			X86 = 2,
			IA64 = 3,
			Amd64 = 4,
			Arm = 5,
		}
		[Flags]
		public enum System_Reflection_Associates_Attributes : int
		{
			ComposedOfAllVirtualMethods = 1,
			ComposedOfAllPrivateMethods = 2,
			ComposedOfNoPublicMembers = 4,
			ComposedOfNoStaticMembers = 8,
		}
		[Flags]
		public enum System_Reflection_BindingFlags : int
		{
			Default = 0,
			IgnoreCase = 1,
			DeclaredOnly = 2,
			Instance = 4,
			Static = 8,
			Public = 16,
			NonPublic = 32,
			FlattenHierarchy = 64,
			InvokeMethod = 256,
			CreateInstance = 512,
			GetField = 1024,
			SetField = 2048,
			GetProperty = 4096,
			SetProperty = 8192,
			PutDispProperty = 16384,
			PutRefDispProperty = 32768,
			ExactBinding = 65536,
			SuppressChangeType = 131072,
			OptionalParamBinding = 262144,
			IgnoreReturn = 16777216,
		}
		[Flags]
		public enum System_Reflection_CallingConventions : int
		{
			Standard = 1,
			VarArgs = 2,
			Any = 3,
			HasThis = 32,
			ExplicitThis = 64,
		}
		public enum System_Reflection_CustomAttributeEncoding : int
		{
			Undefined = 0,
			Boolean = 2,
			Char = 3,
			SByte = 4,
			Byte = 5,
			Int16 = 6,
			UInt16 = 7,
			Int32 = 8,
			UInt32 = 9,
			Int64 = 10,
			UInt64 = 11,
			Float = 12,
			Double = 13,
			String = 14,
			Array = 29,
			Type = 80,
			Object = 81,
			Field = 83,
			Property = 84,
			Enum = 85,
		}
		[Flags]
		public enum System_Reflection_EventAttributes : int
		{
			None = 0,
			SpecialName = 512,
			ReservedMask = 1024,
			RTSpecialName = 1024,
		}
		[Flags]
		public enum System_Reflection_FieldAttributes : int
		{
			PrivateScope = 0,
			Private = 1,
			FamANDAssem = 2,
			Assembly = 3,
			Family = 4,
			FamORAssem = 5,
			Public = 6,
			FieldAccessMask = 7,
			Static = 16,
			InitOnly = 32,
			Literal = 64,
			NotSerialized = 128,
			HasFieldRVA = 256,
			SpecialName = 512,
			RTSpecialName = 1024,
			HasFieldMarshal = 4096,
			PinvokeImpl = 8192,
			HasDefault = 32768,
			ReservedMask = 38144,
		}
		[Flags]
		public enum System_Reflection_GenericParameterAttributes : int
		{
			None = 0,
			Covariant = 1,
			Contravariant = 2,
			VarianceMask = 3,
			ReferenceTypeConstraint = 4,
			NotNullableValueTypeConstraint = 8,
			DefaultConstructorConstraint = 16,
			SpecialConstraintMask = 28,
		}
		[Flags]
		public enum System_Reflection_ResourceLocation : int
		{
			Embedded = 1,
			ContainedInAnotherAssembly = 2,
			ContainedInManifestFile = 4,
		}
		public enum System_Reflection_CorElementType : byte
		{
			End = 0,
			Void = 1,
			Boolean = 2,
			Char = 3,
			I1 = 4,
			U1 = 5,
			I2 = 6,
			U2 = 7,
			I4 = 8,
			U4 = 9,
			I8 = 10,
			U8 = 11,
			R4 = 12,
			R8 = 13,
			String = 14,
			Ptr = 15,
			ByRef = 16,
			ValueType = 17,
			Class = 18,
			Var = 19,
			Array = 20,
			GenericInst = 21,
			TypedByRef = 22,
			I = 24,
			U = 25,
			FnPtr = 27,
			Object = 28,
			SzArray = 29,
			MVar = 30,
			CModReqd = 31,
			CModOpt = 32,
			Internal = 33,
			Max = 34,
			Modifier = 64,
			Sentinel = 65,
			Pinned = 69,
		}
		[Flags]
		public enum System_Reflection_MdSigCallingConvention : byte
		{
			Default = 0,
			C = 1,
			StdCall = 2,
			ThisCall = 3,
			FastCall = 4,
			Vararg = 5,
			Field = 6,
			LocalSig = 7,
			Property = 8,
			Unmgd = 9,
			GenericInst = 10,
			CallConvMask = 15,
			Generic = 16,
			HasThis = 32,
			ExplicitThis = 64,
		}
		[Flags]
		public enum System_Reflection_PInvokeAttributes : int
		{
			ThrowOnUnmappableCharUseAssem = 0,
			CharSetNotSpec = 0,
			BestFitUseAssem = 0,
			NoMangle = 1,
			CharSetAnsi = 2,
			CharSetUnicode = 4,
			CharSetAuto = 6,
			CharSetMask = 6,
			BestFitEnabled = 16,
			BestFitDisabled = 32,
			BestFitMask = 48,
			SupportsLastError = 64,
			CallConvWinapi = 256,
			CallConvCdecl = 512,
			CallConvStdcall = 768,
			CallConvThiscall = 1024,
			CallConvFastcall = 1280,
			CallConvMask = 1792,
			ThrowOnUnmappableCharEnabled = 4096,
			ThrowOnUnmappableCharDisabled = 8192,
			ThrowOnUnmappableCharMask = 12288,
			MaxValue = 65535,
		}
		[Flags]
		public enum System_Reflection_MethodSemanticsAttributes : int
		{
			Setter = 1,
			Getter = 2,
			Other = 4,
			AddOn = 8,
			RemoveOn = 16,
			Fire = 32,
		}
		public enum System_Reflection_MetadataTokenType : int
		{
			Module = 0,
			TypeRef = 16777216,
			TypeDef = 33554432,
			FieldDef = 67108864,
			MethodDef = 100663296,
			ParamDef = 134217728,
			InterfaceImpl = 150994944,
			MemberRef = 167772160,
			CustomAttribute = 201326592,
			Permission = 234881024,
			Signature = 285212672,
			Event = 335544320,
			Property = 385875968,
			ModuleRef = 436207616,
			TypeSpec = 452984832,
			Assembly = 536870912,
			AssemblyRef = 587202560,
			File = 637534208,
			ExportedType = 654311424,
			ManifestResource = 671088640,
			GenericPar = 704643072,
			MethodSpec = 721420288,
			String = 1879048192,
			Name = 1895825408,
			BaseType = 1912602624,
			Invalid = 2147483647,
		}
		[Flags]
		public enum System_Reflection_MemberTypes : int
		{
			Constructor = 1,
			Event = 2,
			Field = 4,
			Method = 8,
			Property = 16,
			TypeInfo = 32,
			Custom = 64,
			NestedType = 128,
			All = 191,
		}
		[Flags]
		public enum System_Reflection_MethodAttributes : int
		{
			ReuseSlot = 0,
			PrivateScope = 0,
			Private = 1,
			FamANDAssem = 2,
			Assembly = 3,
			Family = 4,
			FamORAssem = 5,
			Public = 6,
			MemberAccessMask = 7,
			UnmanagedExport = 8,
			Static = 16,
			Final = 32,
			Virtual = 64,
			HideBySig = 128,
			NewSlot = 256,
			VtableLayoutMask = 256,
			CheckAccessOnOverride = 512,
			Abstract = 1024,
			SpecialName = 2048,
			RTSpecialName = 4096,
			PinvokeImpl = 8192,
			HasSecurity = 16384,
			RequireSecObject = 32768,
			ReservedMask = 53248,
		}
		[Flags]
		public enum System_Reflection_INVOCATION_FLAGS : uint
		{
			INVOCATION_FLAGS_UNKNOWN = 0,
			INVOCATION_FLAGS_INITIALIZED = 1,
			INVOCATION_FLAGS_NO_INVOKE = 2,
			INVOCATION_FLAGS_NEED_SECURITY = 4,
			INVOCATION_FLAGS_NO_CTOR_INVOKE = 8,
			INVOCATION_FLAGS_IS_CTOR = 16,
			INVOCATION_FLAGS_SPECIAL_FIELD = 16,
			INVOCATION_FLAGS_RISKY_METHOD = 32,
			INVOCATION_FLAGS_FIELD_SPECIAL_CAST = 32,
			INVOCATION_FLAGS_NON_W8P_FX_API = 64,
			INVOCATION_FLAGS_IS_DELEGATE_CTOR = 128,
			INVOCATION_FLAGS_CONTAINS_STACK_POINTERS = 256,
			INVOCATION_FLAGS_CONSTRUCTOR_INVOKE = 268435456,
		}
		public enum System_Reflection_MethodImplAttributes : int
		{
			Managed = 0,
			IL = 0,
			Native = 1,
			OPTIL = 2,
			Runtime = 3,
			CodeTypeMask = 3,
			Unmanaged = 4,
			ManagedMask = 4,
			NoInlining = 8,
			ForwardRef = 16,
			Synchronized = 32,
			NoOptimization = 64,
			PreserveSig = 128,
			AggressiveInlining = 256,
			InternalCall = 4096,
			MaxMethodImplVal = 65535,
		}
		[Flags]
		public enum System_Reflection_PortableExecutableKinds : int
		{
			NotAPortableExecutableImage = 0,
			ILOnly = 1,
			Required32Bit = 2,
			PE32Plus = 4,
			Unmanaged32Bit = 8,
			Preferred32Bit = 16,
		}
		public enum System_Reflection_ImageFileMachine : int
		{
			I386 = 332,
			ARM = 452,
			IA64 = 512,
			AMD64 = 34404,
		}
		[Flags]
		public enum System_Reflection_ExceptionHandlingClauseOptions : int
		{
			Clause = 0,
			Filter = 1,
			Finally = 2,
			Fault = 4,
		}
		[Flags]
		public enum System_Reflection_ParameterAttributes : int
		{
			None = 0,
			In = 1,
			Out = 2,
			Lcid = 4,
			Retval = 8,
			Optional = 16,
			HasDefault = 4096,
			HasFieldMarshal = 8192,
			Reserved3 = 16384,
			Reserved4 = 32768,
			ReservedMask = 61440,
		}
		[Flags]
		public enum System_Reflection_PropertyAttributes : int
		{
			None = 0,
			SpecialName = 512,
			RTSpecialName = 1024,
			HasDefault = 4096,
			Reserved2 = 8192,
			Reserved3 = 16384,
			Reserved4 = 32768,
			ReservedMask = 62464,
		}
		[Flags]
		public enum System_Reflection_ResourceAttributes : int
		{
			Public = 1,
			Private = 2,
		}
		[Flags]
		public enum System_Reflection_TypeAttributes : int
		{
			NotPublic = 0,
			AutoLayout = 0,
			AnsiClass = 0,
			Class = 0,
			Public = 1,
			NestedPublic = 2,
			NestedPrivate = 3,
			NestedFamily = 4,
			NestedAssembly = 5,
			NestedFamANDAssem = 6,
			NestedFamORAssem = 7,
			VisibilityMask = 7,
			SequentialLayout = 8,
			ExplicitLayout = 16,
			LayoutMask = 24,
			Interface = 32,
			ClassSemanticsMask = 32,
			Abstract = 128,
			Sealed = 256,
			SpecialName = 1024,
			RTSpecialName = 2048,
			Import = 4096,
			Serializable = 8192,
			WindowsRuntime = 16384,
			UnicodeClass = 65536,
			AutoClass = 131072,
			StringFormatMask = 196608,
			CustomFormatClass = 196608,
			HasSecurity = 262144,
			ReservedMask = 264192,
			BeforeFieldInit = 1048576,
			CustomFormatMask = 12582912,
		}
		[Flags]
		public enum System_Runtime_Serialization_StreamingContextStates : int
		{
			CrossProcess = 1,
			CrossMachine = 2,
			File = 4,
			Persistence = 8,
			Remoting = 16,
			Other = 32,
			Clone = 64,
			CrossAppDomain = 128,
			All = 255,
		}
		public enum System_Globalization_BidiCategory : int
		{
			LeftToRight = 0,
			LeftToRightEmbedding = 1,
			LeftToRightOverride = 2,
			RightToLeft = 3,
			RightToLeftArabic = 4,
			RightToLeftEmbedding = 5,
			RightToLeftOverride = 6,
			PopDirectionalFormat = 7,
			EuropeanNumber = 8,
			EuropeanNumberSeparator = 9,
			EuropeanNumberTerminator = 10,
			ArabicNumber = 11,
			CommonNumberSeparator = 12,
			NonSpacingMark = 13,
			BoundaryNeutral = 14,
			ParagraphSeparator = 15,
			SegmentSeparator = 16,
			Whitespace = 17,
			OtherNeutrals = 18,
		}
		public enum System_Globalization_CalendarAlgorithmType : int
		{
			Unknown = 0,
			SolarCalendar = 1,
			LunarCalendar = 2,
			LunisolarCalendar = 3,
		}
		public enum System_Globalization_CalendarWeekRule : int
		{
			FirstDay = 0,
			FirstFullWeek = 1,
			FirstFourDayWeek = 2,
		}
		[Flags]
		public enum System_Globalization_CompareOptions : int
		{
			None = 0,
			IgnoreCase = 1,
			IgnoreNonSpace = 2,
			IgnoreSymbols = 4,
			IgnoreKanaType = 8,
			IgnoreWidth = 16,
			OrdinalIgnoreCase = 268435456,
			StringSort = 536870912,
			Ordinal = 1073741824,
		}
		[Flags]
		public enum System_Globalization_CultureTypes : int
		{
			NeutralCultures = 1,
			SpecificCultures = 2,
			InstalledWin32Cultures = 4,
			AllCultures = 7,
			UserCustomCulture = 8,
			ReplacementCultures = 16,
			WindowsOnlyCultures = 32,
			FrameworkCultures = 64,
		}
		public enum System_DateTimeParse_DTT : int
		{
			End = 0,
			NumEnd = 1,
			NumAmpm = 2,
			NumSpace = 3,
			NumDatesep = 4,
			NumTimesep = 5,
			MonthEnd = 6,
			MonthSpace = 7,
			MonthDatesep = 8,
			NumDatesuff = 9,
			NumTimesuff = 10,
			DayOfWeek = 11,
			YearSpace = 12,
			YearDateSep = 13,
			YearEnd = 14,
			TimeZone = 15,
			Era = 16,
			NumUTCTimeMark = 17,
			Unk = 18,
			NumLocalTimeMark = 19,
			Max = 20,
		}
		public enum System_DateTimeParse_TM : int
		{
			AM = 0,
			PM = 1,
			NotSet = -1,
		}
		public enum System_DateTimeParse_DS : int
		{
			BEGIN = 0,
			N = 1,
			NN = 2,
			D_Nd = 3,
			D_NN = 4,
			D_NNd = 5,
			D_M = 6,
			D_MN = 7,
			D_NM = 8,
			D_MNd = 9,
			D_NDS = 10,
			D_Y = 11,
			D_YN = 12,
			D_YNd = 13,
			D_YM = 14,
			D_YMd = 15,
			D_S = 16,
			T_S = 17,
			T_Nt = 18,
			T_NNt = 19,
			ERROR = 20,
			DX_NN = 21,
			DX_NNN = 22,
			DX_MN = 23,
			DX_NM = 24,
			DX_MNN = 25,
			DX_DS = 26,
			DX_DSN = 27,
			DX_NDS = 28,
			DX_NNDS = 29,
			DX_YNN = 30,
			DX_YMN = 31,
			DX_YN = 32,
			DX_YM = 33,
			TX_N = 34,
			TX_NN = 35,
			TX_NNN = 36,
			TX_TS = 37,
			DX_NNY = 38,
		}
		public enum System_DTSubStringType : int
		{
			Unknown = 0,
			Invalid = 1,
			Number = 2,
			End = 3,
			Other = 4,
		}
		public enum System_ParseFailureKind : int
		{
			None = 0,
			ArgumentNull = 1,
			Format = 2,
			FormatWithParameter = 3,
			FormatBadDateTimeCalendar = 4,
		}
		[Flags]
		public enum System_ParseFlags : int
		{
			HaveYear = 1,
			HaveMonth = 2,
			HaveDay = 4,
			HaveHour = 8,
			HaveMinute = 16,
			HaveSecond = 32,
			HaveTime = 64,
			HaveDate = 128,
			TimeZoneUsed = 256,
			TimeZoneUtc = 512,
			ParsedMonthName = 1024,
			CaptureOffset = 2048,
			YearDefault = 4096,
			Rfc1123Pattern = 8192,
			UtcSortPattern = 16384,
		}
		public enum System_TokenType : int
		{
			NumberToken = 1,
			YearNumberToken = 2,
			Am = 3,
			Pm = 4,
			MonthToken = 5,
			EndOfString = 6,
			DayOfWeekToken = 7,
			TimeZoneToken = 8,
			EraToken = 9,
			DateWordToken = 10,
			UnknownToken = 11,
			HebrewNumber = 12,
			JapaneseEraToken = 13,
			TEraToken = 14,
			IgnorableSymbol = 15,
			RegularTokenMask = 255,
			SEP_Unk = 256,
			SEP_End = 512,
			SEP_Space = 768,
			SEP_Am = 1024,
			SEP_Pm = 1280,
			SEP_Date = 1536,
			SEP_Time = 1792,
			SEP_YearSuff = 2048,
			SEP_MonthSuff = 2304,
			SEP_DaySuff = 2560,
			SEP_HourSuff = 2816,
			SEP_MinuteSuff = 3072,
			SEP_SecondSuff = 3328,
			SEP_LocalTimeMark = 3584,
			SEP_DateOrOffset = 3840,
			SeparatorTokenMask = 65280,
		}
		[Flags]
		public enum System_Globalization_DateTimeStyles : int
		{
			None = 0,
			AllowLeadingWhite = 1,
			AllowTrailingWhite = 2,
			AllowInnerWhite = 4,
			AllowWhiteSpaces = 7,
			NoCurrentDateDefault = 8,
			AdjustToUniversal = 16,
			AssumeLocal = 32,
			AssumeUniversal = 64,
			RoundtripKind = 128,
		}
		[Flags]
		public enum System_Globalization_MonthNameStyles : int
		{
			Regular = 0,
			Genitive = 1,
			LeapYear = 2,
		}
		[Flags]
		public enum System_Globalization_DateTimeFormatFlags : int
		{
			None = 0,
			UseGenitiveMonth = 1,
			UseLeapYearMonth = 2,
			UseSpacesInMonthNames = 4,
			UseHebrewRule = 8,
			UseSpacesInDayNames = 16,
			UseDigitPrefixInTokens = 32,
			NotInitialized = -1,
		}
		public enum System_Globalization_FORMATFLAGS : int
		{
			None = 0,
			UseGenitiveMonth = 1,
			UseLeapYearMonth = 2,
			UseSpacesInMonthNames = 4,
			UseHebrewParsing = 8,
			UseSpacesInDayNames = 16,
			UseDigitPrefixInTokens = 32,
		}
		public enum System_Globalization_CalendarId : ushort
		{
			GREGORIAN = 1,
			GREGORIAN_US = 2,
			JAPAN = 3,
			TAIWAN = 4,
			KOREA = 5,
			HIJRI = 6,
			THAI = 7,
			HEBREW = 8,
			GREGORIAN_ME_FRENCH = 9,
			GREGORIAN_ARABIC = 10,
			GREGORIAN_XLIT_ENGLISH = 11,
			GREGORIAN_XLIT_FRENCH = 12,
			JULIAN = 13,
			JAPANESELUNISOLAR = 14,
			CHINESELUNISOLAR = 15,
			SAKA = 16,
			LUNAR_ETO_CHN = 17,
			LUNAR_ETO_KOR = 18,
			LUNAR_ETO_ROKUYOU = 19,
			KOREANLUNISOLAR = 20,
			TAIWANLUNISOLAR = 21,
			PERSIAN = 22,
			UMALQURA = 23,
			LAST_CALENDAR = 23,
		}
		public enum System_Globalization_DateTimeFormatInfoScanner_FoundDatePattern : int
		{
			None = 0,
			FoundYearPatternFlag = 1,
			FoundMonthPatternFlag = 2,
			FoundDayPatternFlag = 4,
			FoundYMDPatternFlag = 7,
		}
		public enum System_Globalization_DigitShapes : int
		{
			Context = 0,
			None = 1,
			NativeNational = 2,
		}
		public enum System_Globalization_GregorianCalendarTypes : int
		{
			Localized = 1,
			USEnglish = 2,
			MiddleEastFrench = 9,
			Arabic = 10,
			TransliteratedEnglish = 11,
			TransliteratedFrench = 12,
		}
		public enum System_Globalization_TimeSpanFormat_Pattern : int
		{
			None = 0,
			Minimum = 1,
			Full = 2,
		}
		[Flags]
		public enum System_Globalization_TimeSpanStyles : int
		{
			None = 0,
			AssumeNegative = 1,
		}
		public enum System_Globalization_TimeSpanParse_TTT : int
		{
			None = 0,
			End = 1,
			Num = 2,
			Sep = 3,
			NumOverflow = 4,
		}
		[Flags]
		public enum System_Globalization_NumberStyles : int
		{
			None = 0,
			AllowLeadingWhite = 1,
			AllowTrailingWhite = 2,
			AllowLeadingSign = 4,
			Integer = 7,
			AllowTrailingSign = 8,
			AllowParentheses = 16,
			AllowDecimalPoint = 32,
			AllowThousands = 64,
			Number = 111,
			AllowExponent = 128,
			Float = 167,
			AllowCurrencySymbol = 256,
			Currency = 383,
			Any = 511,
			AllowHexSpecifier = 512,
			HexNumber = 515,
		}
		public enum System_Globalization_UnicodeCategory : int
		{
			UppercaseLetter = 0,
			LowercaseLetter = 1,
			TitlecaseLetter = 2,
			ModifierLetter = 3,
			OtherLetter = 4,
			NonSpacingMark = 5,
			SpacingCombiningMark = 6,
			EnclosingMark = 7,
			DecimalDigitNumber = 8,
			LetterNumber = 9,
			OtherNumber = 10,
			SpaceSeparator = 11,
			LineSeparator = 12,
			ParagraphSeparator = 13,
			Control = 14,
			Format = 15,
			Surrogate = 16,
			PrivateUse = 17,
			ConnectorPunctuation = 18,
			DashPunctuation = 19,
			OpenPunctuation = 20,
			ClosePunctuation = 21,
			InitialQuotePunctuation = 22,
			FinalQuotePunctuation = 23,
			OtherPunctuation = 24,
			MathSymbol = 25,
			CurrencySymbol = 26,
			ModifierSymbol = 27,
			OtherSymbol = 28,
			OtherNotAssigned = 29,
		}
		public enum System_Globalization_HebrewNumberParsingState : int
		{
			InvalidHebrewNumber = 0,
			NotHebrewDigit = 1,
			FoundEndOfHebrewNumber = 2,
			ContinueParsing = 3,
		}
		public enum System_Globalization_HebrewNumber_HebrewToken : int
		{
			Digit400 = 0,
			Digit200_300 = 1,
			Digit100 = 2,
			Digit10 = 3,
			Digit1 = 4,
			Digit6_7 = 5,
			Digit7 = 6,
			Digit9 = 7,
			SingleQuote = 8,
			DoubleQuote = 9,
			Invalid = -1,
		}
		public enum System_Globalization_HebrewNumber_HS : int
		{
			Start = 0,
			S400 = 1,
			S400_400 = 2,
			S400_X00 = 3,
			S400_X0 = 4,
			X00_DQ = 5,
			S400_X00_X0 = 6,
			X0_DQ = 7,
			X = 8,
			X0 = 9,
			X00 = 10,
			S400_DQ = 11,
			S400_400_DQ = 12,
			S400_400_100 = 13,
			S9 = 14,
			X00_S9 = 15,
			S9_DQ = 16,
			END = 100,
			_err = -1,
		}
		public enum System_Text_NormalizationForm : int
		{
			FormC = 1,
			FormD = 2,
			FormKC = 5,
			FormKD = 6,
		}
		public enum System_Text_ExtendedNormalizationForms : int
		{
			FormC = 1,
			FormD = 2,
			FormKC = 5,
			FormKD = 6,
			FormIdna = 13,
			FormCDisallowUnassigned = 257,
			FormDDisallowUnassigned = 258,
			FormKCDisallowUnassigned = 261,
			FormKDDisallowUnassigned = 262,
			FormIdnaDisallowUnassigned = 269,
		}
		public enum System_Text_ISO2022Encoding_ISO2022Modes : int
		{
			ModeHalfwidthKatakana = 0,
			ModeJIS0208 = 1,
			ModeKR = 5,
			ModeHZ = 6,
			ModeGB2312 = 7,
			ModeCNS11643_1 = 9,
			ModeCNS11643_2 = 10,
			ModeASCII = 11,
			ModeNOOP = -3,
			ModeInvalidEscape = -2,
			ModeIncompleteEscape = -1,
		}
		public enum System_Resources_ResourceTypeCode : int
		{
			Null = 0,
			String = 1,
			Boolean = 2,
			Char = 3,
			Byte = 4,
			SByte = 5,
			Int16 = 6,
			UInt16 = 7,
			Int32 = 8,
			UInt32 = 9,
			Int64 = 10,
			UInt64 = 11,
			Single = 12,
			Double = 13,
			Decimal = 14,
			DateTime = 15,
			TimeSpan = 16,
			LastPrimitive = 16,
			ByteArray = 32,
			Stream = 33,
			StartOfUserTypes = 64,
		}
		public enum System_Resources_UltimateResourceFallbackLocation : int
		{
			MainAssembly = 0,
			Satellite = 1,
		}
		[Flags]
		public enum Microsoft_Win32_Win32Native_Color : short
		{
			Black = 0,
			ForegroundBlue = 1,
			ForegroundGreen = 2,
			ForegroundRed = 4,
			ForegroundYellow = 6,
			ForegroundIntensity = 8,
			ForegroundMask = 15,
			BackgroundBlue = 16,
			BackgroundGreen = 32,
			BackgroundRed = 64,
			BackgroundYellow = 96,
			BackgroundIntensity = 128,
			BackgroundMask = 240,
			ColorMask = 255,
		}
		public enum Microsoft_Win32_Win32Native_SECURITY_IMPERSONATION_LEVEL : int
		{
			Anonymous = 0,
			Identification = 1,
			Impersonation = 2,
			Delegation = 3,
		}
		public enum Microsoft_Win32_RegistryHive : int
		{
			ClassesRoot = -2147483648,
			CurrentUser = -2147483647,
			LocalMachine = -2147483646,
			Users = -2147483645,
			PerformanceData = -2147483644,
			CurrentConfig = -2147483643,
			DynData = -2147483642,
		}
		[Flags]
		public enum Microsoft_Win32_RegistryValueOptions : int
		{
			None = 0,
			DoNotExpandEnvironmentNames = 1,
		}
		public enum Microsoft_Win32_RegistryKeyPermissionCheck : int
		{
			Default = 0,
			ReadSubTree = 1,
			ReadWriteSubTree = 2,
		}
		[Flags]
		public enum Microsoft_Win32_RegistryOptions : int
		{
			None = 0,
			Volatile = 1,
		}
		public enum Microsoft_Win32_RegistryValueKind : int
		{
			Unknown = 0,
			String = 1,
			ExpandString = 2,
			Binary = 3,
			DWord = 4,
			MultiString = 7,
			QWord = 11,
			None = -1,
		}
		public enum Microsoft_Win32_RegistryView : int
		{
			Default = 0,
			Registry64 = 256,
			Registry32 = 512,
		}
		public enum Microsoft_Win32_UnsafeNativeMethods_ManifestEtw_ActivityControl : uint
		{
			EVENT_ACTIVITY_CTRL_GET_ID = 1,
			EVENT_ACTIVITY_CTRL_SET_ID = 2,
			EVENT_ACTIVITY_CTRL_CREATE_ID = 3,
			EVENT_ACTIVITY_CTRL_GET_SET_ID = 4,
			EVENT_ACTIVITY_CTRL_CREATE_SET_ID = 5,
		}
		public enum Microsoft_Win32_UnsafeNativeMethods_ManifestEtw_TRACE_QUERY_INFO_CLASS : int
		{
			TraceGuidQueryList = 0,
			TraceGuidQueryInfo = 1,
			TraceGuidQueryProcess = 2,
			TraceStackTracingInfo = 3,
			MaxTraceSetInfoClass = 4,
		}
		[Flags]
		public enum System_Security_Util_QuickCacheEntryType : int
		{
			FullTrustZoneMyComputer = 16777216,
			FullTrustZoneIntranet = 33554432,
			FullTrustZoneInternet = 67108864,
			FullTrustZoneTrusted = 134217728,
			FullTrustZoneUntrusted = 268435456,
			FullTrustAll = 536870912,
		}
		public enum System_Security_Policy_ApplicationVersionMatch : int
		{
			MatchExactVersion = 0,
			MatchAllVersions = 1,
		}
		public enum System_Security_Policy_Evidence_DuplicateEvidenceAction : int
		{
			Throw = 0,
			Merge = 1,
			SelectNewObject = 2,
		}
		public enum System_Security_Policy_TrustManagerUIContext : int
		{
			Install = 0,
			Upgrade = 1,
			Run = 2,
		}
		public enum System_Security_Policy_EvidenceTypeGenerated : int
		{
			AssemblySupplied = 0,
			Gac = 1,
			Hash = 2,
			PermissionRequest = 3,
			Publisher = 4,
			Site = 5,
			StrongName = 6,
			Url = 7,
			Zone = 8,
		}
		public enum System_Security_Policy_ConfigId : int
		{
			None = 0,
			MachinePolicyLevel = 1,
			UserPolicyLevel = 2,
			EnterprisePolicyLevel = 3,
		}
		[Flags]
		public enum System_Security_Policy_PolicyStatementAttribute : int
		{
			Nothing = 0,
			Exclusive = 1,
			LevelFinal = 2,
			All = 3,
		}
		public enum System_Security_Principal_PrincipalPolicy : int
		{
			UnauthenticatedPrincipal = 0,
			NoPrincipal = 1,
			WindowsPrincipal = 2,
		}
		[Flags]
		public enum System_Security_Principal_TokenAccessLevels : int
		{
			AssignPrimary = 1,
			Duplicate = 2,
			Impersonate = 4,
			Query = 8,
			QuerySource = 16,
			AdjustPrivileges = 32,
			AdjustGroups = 64,
			AdjustDefault = 128,
			AdjustSessionId = 256,
			Read = 131080,
			Write = 131296,
			AllAccess = 983551,
			MaximumAllowed = 33554432,
		}
		public enum System_Security_Principal_TokenImpersonationLevel : int
		{
			None = 0,
			Anonymous = 1,
			Identification = 2,
			Impersonation = 3,
			Delegation = 4,
		}
		public enum System_Security_Principal_WindowsAccountType : int
		{
			Normal = 0,
			Guest = 1,
			System = 2,
			Anonymous = 3,
		}
		public enum System_Security_Principal_WinSecurityContext : int
		{
			Thread = 1,
			Process = 2,
			Both = 3,
		}
		public enum System_Security_Principal_ImpersonationQueryResult : int
		{
			Impersonated = 0,
			NotImpersonated = 1,
			Failed = 2,
		}
		public enum System_Security_Principal_KerbLogonSubmitType : int
		{
			KerbInteractiveLogon = 2,
			KerbSmartCardLogon = 6,
			KerbWorkstationUnlockLogon = 7,
			KerbSmartCardUnlockLogon = 8,
			KerbProxyLogon = 9,
			KerbTicketLogon = 10,
			KerbTicketUnlockLogon = 11,
			KerbS4ULogon = 12,
		}
		public enum System_Security_Principal_SecurityLogonType : int
		{
			Interactive = 2,
			Network = 3,
			Batch = 4,
			Service = 5,
			Proxy = 6,
			Unlock = 7,
		}
		public enum System_Security_Principal_TokenType : int
		{
			TokenPrimary = 1,
			TokenImpersonation = 2,
		}
		public enum System_Security_Principal_TokenInformationClass : int
		{
			TokenUser = 1,
			TokenGroups = 2,
			TokenPrivileges = 3,
			TokenOwner = 4,
			TokenPrimaryGroup = 5,
			TokenDefaultDacl = 6,
			TokenSource = 7,
			TokenType = 8,
			TokenImpersonationLevel = 9,
			TokenStatistics = 10,
			TokenRestrictedSids = 11,
			TokenSessionId = 12,
			TokenGroupsAndPrivileges = 13,
			TokenSessionReference = 14,
			TokenSandBoxInert = 15,
			TokenAuditPolicy = 16,
			TokenOrigin = 17,
			TokenElevationType = 18,
			TokenLinkedToken = 19,
			TokenElevation = 20,
			TokenHasRestrictions = 21,
			TokenAccessInformation = 22,
			TokenVirtualizationAllowed = 23,
			TokenVirtualizationEnabled = 24,
			TokenIntegrityLevel = 25,
			TokenUIAccess = 26,
			TokenMandatoryPolicy = 27,
			TokenLogonSid = 28,
			TokenIsAppContainer = 29,
			TokenCapabilities = 30,
			TokenAppContainerSid = 31,
			TokenAppContainerNumber = 32,
			TokenUserClaimAttributes = 33,
			TokenDeviceClaimAttributes = 34,
			TokenRestrictedUserClaimAttributes = 35,
			TokenRestrictedDeviceClaimAttributes = 36,
			TokenDeviceGroups = 37,
			TokenRestrictedDeviceGroups = 38,
			MaxTokenInfoClass = 39,
		}
		public enum System_Security_Principal_WindowsBuiltInRole : int
		{
			Administrator = 544,
			User = 545,
			Guest = 546,
			PowerUser = 547,
			AccountOperator = 548,
			SystemOperator = 549,
			PrintOperator = 550,
			BackupOperator = 551,
			Replicator = 552,
		}
		public enum System_Runtime_ConstrainedExecution_Consistency : int
		{
			MayCorruptProcess = 0,
			MayCorruptAppDomain = 1,
			MayCorruptInstance = 2,
			WillNotCorruptState = 3,
		}
		public enum System_Runtime_ConstrainedExecution_Cer : int
		{
			None = 0,
			MayFail = 1,
			Success = 2,
		}
		public enum System_Runtime_InteropServices_ComInterfaceType : int
		{
			InterfaceIsDual = 0,
			InterfaceIsIUnknown = 1,
			InterfaceIsIDispatch = 2,
			InterfaceIsIInspectable = 3,
		}
		public enum System_Runtime_InteropServices_ClassInterfaceType : int
		{
			None = 0,
			AutoDispatch = 1,
			AutoDual = 2,
		}
		public enum System_Runtime_InteropServices_IDispatchImplType : int
		{
			SystemDefinedImpl = 0,
			InternalImpl = 1,
			CompatibleImpl = 2,
		}
		[Flags]
		public enum System_Runtime_InteropServices_TypeLibTypeFlags : int
		{
			FAppObject = 1,
			FCanCreate = 2,
			FLicensed = 4,
			FPreDeclId = 8,
			FHidden = 16,
			FControl = 32,
			FDual = 64,
			FNonExtensible = 128,
			FOleAutomation = 256,
			FRestricted = 512,
			FAggregatable = 1024,
			FReplaceable = 2048,
			FDispatchable = 4096,
			FReverseBind = 8192,
		}
		[Flags]
		public enum System_Runtime_InteropServices_TypeLibFuncFlags : int
		{
			FRestricted = 1,
			FSource = 2,
			FBindable = 4,
			FRequestEdit = 8,
			FDisplayBind = 16,
			FDefaultBind = 32,
			FHidden = 64,
			FUsesGetLastError = 128,
			FDefaultCollelem = 256,
			FUiDefault = 512,
			FNonBrowsable = 1024,
			FReplaceable = 2048,
			FImmediateBind = 4096,
		}
		[Flags]
		public enum System_Runtime_InteropServices_TypeLibVarFlags : int
		{
			FReadOnly = 1,
			FSource = 2,
			FBindable = 4,
			FRequestEdit = 8,
			FDisplayBind = 16,
			FDefaultBind = 32,
			FHidden = 64,
			FRestricted = 128,
			FDefaultCollelem = 256,
			FUiDefault = 512,
			FNonBrowsable = 1024,
			FReplaceable = 2048,
			FImmediateBind = 4096,
		}
		public enum System_Runtime_InteropServices_VarEnum : int
		{
			VT_EMPTY = 0,
			VT_NULL = 1,
			VT_I2 = 2,
			VT_I4 = 3,
			VT_R4 = 4,
			VT_R8 = 5,
			VT_CY = 6,
			VT_DATE = 7,
			VT_BSTR = 8,
			VT_DISPATCH = 9,
			VT_ERROR = 10,
			VT_BOOL = 11,
			VT_VARIANT = 12,
			VT_UNKNOWN = 13,
			VT_DECIMAL = 14,
			VT_I1 = 16,
			VT_UI1 = 17,
			VT_UI2 = 18,
			VT_UI4 = 19,
			VT_I8 = 20,
			VT_UI8 = 21,
			VT_INT = 22,
			VT_UINT = 23,
			VT_VOID = 24,
			VT_HRESULT = 25,
			VT_PTR = 26,
			VT_SAFEARRAY = 27,
			VT_CARRAY = 28,
			VT_USERDEFINED = 29,
			VT_LPSTR = 30,
			VT_LPWSTR = 31,
			VT_RECORD = 36,
			VT_FILETIME = 64,
			VT_BLOB = 65,
			VT_STREAM = 66,
			VT_STORAGE = 67,
			VT_STREAMED_OBJECT = 68,
			VT_STORED_OBJECT = 69,
			VT_BLOB_OBJECT = 70,
			VT_CF = 71,
			VT_CLSID = 72,
			VT_VECTOR = 4096,
			VT_ARRAY = 8192,
			VT_BYREF = 16384,
		}
		public enum System_Runtime_InteropServices_UnmanagedType : int
		{
			Bool = 2,
			I1 = 3,
			U1 = 4,
			I2 = 5,
			U2 = 6,
			I4 = 7,
			U4 = 8,
			I8 = 9,
			U8 = 10,
			R4 = 11,
			R8 = 12,
			Currency = 15,
			BStr = 19,
			LPStr = 20,
			LPWStr = 21,
			LPTStr = 22,
			ByValTStr = 23,
			IUnknown = 25,
			IDispatch = 26,
			Struct = 27,
			Interface = 28,
			SafeArray = 29,
			ByValArray = 30,
			SysInt = 31,
			SysUInt = 32,
			VBByRefStr = 34,
			AnsiBStr = 35,
			TBStr = 36,
			VariantBool = 37,
			FunctionPtr = 38,
			AsAny = 40,
			LPArray = 42,
			LPStruct = 43,
			CustomMarshaler = 44,
			Error = 45,
			IInspectable = 46,
			HString = 47,
		}
		[Flags]
		public enum System_Runtime_InteropServices_DllImportSearchPath : int
		{
			LegacyBehavior = 0,
			AssemblyDirectory = 2,
			UseDllDirectoryForDependencies = 256,
			ApplicationDirectory = 512,
			UserDirectories = 1024,
			System32 = 2048,
			SafeDirectories = 4096,
		}
		public enum System_Runtime_InteropServices_CallingConvention : int
		{
			Winapi = 1,
			Cdecl = 2,
			StdCall = 3,
			ThisCall = 4,
			FastCall = 5,
		}
		public enum System_Runtime_InteropServices_CharSet : int
		{
			None = 1,
			Ansi = 2,
			Unicode = 3,
			Auto = 4,
		}
		public enum System_Runtime_InteropServices_GCHandleType : int
		{
			Weak = 0,
			WeakTrackResurrection = 1,
			Normal = 2,
			Pinned = 3,
		}
		public enum System_Runtime_InteropServices_LayoutKind : int
		{
			Sequential = 0,
			Explicit = 2,
			Auto = 3,
		}
		public enum System_Runtime_InteropServices_CustomQueryInterfaceMode : int
		{
			Ignore = 0,
			Allow = 1,
		}
		public enum System_Runtime_InteropServices_PInvokeMap : int
		{
			CharSetNotSpec = 0,
			NoMangle = 1,
			CharSetAnsi = 2,
			CharSetUnicode = 4,
			CharSetAuto = 6,
			CharSetMask = 6,
			BestFitEnabled = 16,
			BestFitDisabled = 32,
			PinvokeOLE = 32,
			BestFitMask = 48,
			BestFitUseAsm = 48,
			SupportsLastError = 64,
			CallConvWinapi = 256,
			CallConvCdecl = 512,
			CallConvStdcall = 768,
			CallConvThiscall = 1024,
			CallConvFastcall = 1280,
			CallConvMask = 1792,
			ThrowOnUnmappableCharEnabled = 4096,
			ThrowOnUnmappableCharDisabled = 8192,
			ThrowOnUnmappableCharMask = 12288,
			ThrowOnUnmappableCharUseAsm = 12288,
		}
		public enum System_Runtime_InteropServices_ComMemberType : int
		{
			Method = 0,
			PropGet = 1,
			PropSet = 2,
		}
		public enum System_Runtime_InteropServices_CustomQueryInterfaceResult : int
		{
			Handled = 0,
			NotHandled = 1,
			Failed = 2,
		}
		[Flags]
		public enum System_Runtime_InteropServices_AssemblyRegistrationFlags : int
		{
			None = 0,
			SetCodeBase = 1,
		}
		[Flags]
		public enum System_Runtime_InteropServices_TypeLibImporterFlags : int
		{
			None = 0,
			PrimaryInteropAssembly = 1,
			UnsafeInterfaces = 2,
			SafeArrayAsSystemArray = 4,
			TransformDispRetVals = 8,
			PreventClassMembers = 16,
			SerializableValueClasses = 32,
			ImportAsX86 = 256,
			ImportAsX64 = 512,
			ImportAsItanium = 1024,
			ImportAsAgnostic = 2048,
			ReflectionOnlyLoading = 4096,
			NoDefineVersionResource = 8192,
			ImportAsArm = 16384,
		}
		[Flags]
		public enum System_Runtime_InteropServices_TypeLibExporterFlags : int
		{
			None = 0,
			OnlyReferenceRegistered = 1,
			CallerResolvedReferences = 2,
			OldNames = 4,
			ExportAs32Bit = 16,
			ExportAs64Bit = 32,
		}
		public enum System_Runtime_InteropServices_ImporterEventKind : int
		{
			NOTIF_TYPECONVERTED = 0,
			NOTIF_CONVERTWARNING = 1,
			ERROR_REFTOINVALIDTYPELIB = 2,
		}
		public enum System_Runtime_InteropServices_ExporterEventKind : int
		{
			NOTIF_TYPECONVERTED = 0,
			NOTIF_CONVERTWARNING = 1,
			ERROR_REFTOINVALIDASSEMBLY = 2,
		}
		[Flags]
		public enum System_Runtime_InteropServices_RegistrationClassContext : int
		{
			InProcessServer = 1,
			InProcessHandler = 2,
			LocalServer = 4,
			InProcessServer16 = 8,
			RemoteServer = 16,
			InProcessHandler16 = 32,
			Reserved1 = 64,
			Reserved2 = 128,
			Reserved3 = 256,
			Reserved4 = 512,
			NoCodeDownload = 1024,
			Reserved5 = 2048,
			NoCustomMarshal = 4096,
			EnableCodeDownload = 8192,
			NoFailureLog = 16384,
			DisableActivateAsActivator = 32768,
			EnableActivateAsActivator = 65536,
			FromDefaultContext = 131072,
		}
		[Flags]
		public enum System_Runtime_InteropServices_RegistrationConnectionType : int
		{
			SingleUse = 0,
			MultipleUse = 1,
			MultiSeparate = 2,
			Suspended = 4,
			Surrogate = 8,
		}
		public enum System_Runtime_InteropServices_DESCKIND : int
		{
			DESCKIND_NONE = 0,
			DESCKIND_FUNCDESC = 1,
			DESCKIND_VARDESC = 2,
			DESCKIND_TYPECOMP = 3,
			DESCKIND_IMPLICITAPPOBJ = 4,
			DESCKIND_MAX = 5,
		}
		public enum System_Runtime_InteropServices_TYPEKIND : int
		{
			TKIND_ENUM = 0,
			TKIND_RECORD = 1,
			TKIND_MODULE = 2,
			TKIND_INTERFACE = 3,
			TKIND_DISPATCH = 4,
			TKIND_COCLASS = 5,
			TKIND_ALIAS = 6,
			TKIND_UNION = 7,
			TKIND_MAX = 8,
		}
		[Flags]
		public enum System_Runtime_InteropServices_TYPEFLAGS : short
		{
			TYPEFLAG_FAPPOBJECT = 1,
			TYPEFLAG_FCANCREATE = 2,
			TYPEFLAG_FLICENSED = 4,
			TYPEFLAG_FPREDECLID = 8,
			TYPEFLAG_FHIDDEN = 16,
			TYPEFLAG_FCONTROL = 32,
			TYPEFLAG_FDUAL = 64,
			TYPEFLAG_FNONEXTENSIBLE = 128,
			TYPEFLAG_FOLEAUTOMATION = 256,
			TYPEFLAG_FRESTRICTED = 512,
			TYPEFLAG_FAGGREGATABLE = 1024,
			TYPEFLAG_FREPLACEABLE = 2048,
			TYPEFLAG_FDISPATCHABLE = 4096,
			TYPEFLAG_FREVERSEBIND = 8192,
			TYPEFLAG_FPROXY = 16384,
		}
		[Flags]
		public enum System_Runtime_InteropServices_IMPLTYPEFLAGS : int
		{
			IMPLTYPEFLAG_FDEFAULT = 1,
			IMPLTYPEFLAG_FSOURCE = 2,
			IMPLTYPEFLAG_FRESTRICTED = 4,
			IMPLTYPEFLAG_FDEFAULTVTABLE = 8,
		}
		[Flags]
		public enum System_Runtime_InteropServices_IDLFLAG : short
		{
			IDLFLAG_NONE = 0,
			IDLFLAG_FIN = 1,
			IDLFLAG_FOUT = 2,
			IDLFLAG_FLCID = 4,
			IDLFLAG_FRETVAL = 8,
		}
		[Flags]
		public enum System_Runtime_InteropServices_PARAMFLAG : short
		{
			PARAMFLAG_NONE = 0,
			PARAMFLAG_FIN = 1,
			PARAMFLAG_FOUT = 2,
			PARAMFLAG_FLCID = 4,
			PARAMFLAG_FRETVAL = 8,
			PARAMFLAG_FOPT = 16,
			PARAMFLAG_FHASDEFAULT = 32,
			PARAMFLAG_FHASCUSTDATA = 64,
		}
		public enum System_Runtime_InteropServices_FUNCKIND : int
		{
			FUNC_VIRTUAL = 0,
			FUNC_PUREVIRTUAL = 1,
			FUNC_NONVIRTUAL = 2,
			FUNC_STATIC = 3,
			FUNC_DISPATCH = 4,
		}
		public enum System_Runtime_InteropServices_INVOKEKIND : int
		{
			INVOKE_FUNC = 1,
			INVOKE_PROPERTYGET = 2,
			INVOKE_PROPERTYPUT = 4,
			INVOKE_PROPERTYPUTREF = 8,
		}
		public enum System_Runtime_InteropServices_CALLCONV : int
		{
			CC_CDECL = 1,
			CC_MSCPASCAL = 2,
			CC_PASCAL = 2,
			CC_MACPASCAL = 3,
			CC_STDCALL = 4,
			CC_RESERVED = 5,
			CC_SYSCALL = 6,
			CC_MPWCDECL = 7,
			CC_MPWPASCAL = 8,
			CC_MAX = 9,
		}
		[Flags]
		public enum System_Runtime_InteropServices_FUNCFLAGS : short
		{
			FUNCFLAG_FRESTRICTED = 1,
			FUNCFLAG_FSOURCE = 2,
			FUNCFLAG_FBINDABLE = 4,
			FUNCFLAG_FREQUESTEDIT = 8,
			FUNCFLAG_FDISPLAYBIND = 16,
			FUNCFLAG_FDEFAULTBIND = 32,
			FUNCFLAG_FHIDDEN = 64,
			FUNCFLAG_FUSESGETLASTERROR = 128,
			FUNCFLAG_FDEFAULTCOLLELEM = 256,
			FUNCFLAG_FUIDEFAULT = 512,
			FUNCFLAG_FNONBROWSABLE = 1024,
			FUNCFLAG_FREPLACEABLE = 2048,
			FUNCFLAG_FIMMEDIATEBIND = 4096,
		}
		[Flags]
		public enum System_Runtime_InteropServices_VARFLAGS : short
		{
			VARFLAG_FREADONLY = 1,
			VARFLAG_FSOURCE = 2,
			VARFLAG_FBINDABLE = 4,
			VARFLAG_FREQUESTEDIT = 8,
			VARFLAG_FDISPLAYBIND = 16,
			VARFLAG_FDEFAULTBIND = 32,
			VARFLAG_FHIDDEN = 64,
			VARFLAG_FRESTRICTED = 128,
			VARFLAG_FDEFAULTCOLLELEM = 256,
			VARFLAG_FUIDEFAULT = 512,
			VARFLAG_FNONBROWSABLE = 1024,
			VARFLAG_FREPLACEABLE = 2048,
			VARFLAG_FIMMEDIATEBIND = 4096,
		}
		public enum System_Runtime_InteropServices_SYSKIND : int
		{
			SYS_WIN16 = 0,
			SYS_WIN32 = 1,
			SYS_MAC = 2,
		}
		[Flags]
		public enum System_Runtime_InteropServices_LIBFLAGS : short
		{
			LIBFLAG_FRESTRICTED = 1,
			LIBFLAG_FCONTROL = 2,
			LIBFLAG_FHIDDEN = 4,
			LIBFLAG_FHASDISKIMAGE = 8,
		}
		public enum System_Runtime_InteropServices_ComTypes_DESCKIND : int
		{
			DESCKIND_NONE = 0,
			DESCKIND_FUNCDESC = 1,
			DESCKIND_VARDESC = 2,
			DESCKIND_TYPECOMP = 3,
			DESCKIND_IMPLICITAPPOBJ = 4,
			DESCKIND_MAX = 5,
		}
		public enum System_Runtime_InteropServices_ComTypes_TYPEKIND : int
		{
			TKIND_ENUM = 0,
			TKIND_RECORD = 1,
			TKIND_MODULE = 2,
			TKIND_INTERFACE = 3,
			TKIND_DISPATCH = 4,
			TKIND_COCLASS = 5,
			TKIND_ALIAS = 6,
			TKIND_UNION = 7,
			TKIND_MAX = 8,
		}
		[Flags]
		public enum System_Runtime_InteropServices_ComTypes_TYPEFLAGS : short
		{
			TYPEFLAG_FAPPOBJECT = 1,
			TYPEFLAG_FCANCREATE = 2,
			TYPEFLAG_FLICENSED = 4,
			TYPEFLAG_FPREDECLID = 8,
			TYPEFLAG_FHIDDEN = 16,
			TYPEFLAG_FCONTROL = 32,
			TYPEFLAG_FDUAL = 64,
			TYPEFLAG_FNONEXTENSIBLE = 128,
			TYPEFLAG_FOLEAUTOMATION = 256,
			TYPEFLAG_FRESTRICTED = 512,
			TYPEFLAG_FAGGREGATABLE = 1024,
			TYPEFLAG_FREPLACEABLE = 2048,
			TYPEFLAG_FDISPATCHABLE = 4096,
			TYPEFLAG_FREVERSEBIND = 8192,
			TYPEFLAG_FPROXY = 16384,
		}
		[Flags]
		public enum System_Runtime_InteropServices_ComTypes_IMPLTYPEFLAGS : int
		{
			IMPLTYPEFLAG_FDEFAULT = 1,
			IMPLTYPEFLAG_FSOURCE = 2,
			IMPLTYPEFLAG_FRESTRICTED = 4,
			IMPLTYPEFLAG_FDEFAULTVTABLE = 8,
		}
		[Flags]
		public enum System_Runtime_InteropServices_ComTypes_IDLFLAG : short
		{
			IDLFLAG_NONE = 0,
			IDLFLAG_FIN = 1,
			IDLFLAG_FOUT = 2,
			IDLFLAG_FLCID = 4,
			IDLFLAG_FRETVAL = 8,
		}
		[Flags]
		public enum System_Runtime_InteropServices_ComTypes_PARAMFLAG : short
		{
			PARAMFLAG_NONE = 0,
			PARAMFLAG_FIN = 1,
			PARAMFLAG_FOUT = 2,
			PARAMFLAG_FLCID = 4,
			PARAMFLAG_FRETVAL = 8,
			PARAMFLAG_FOPT = 16,
			PARAMFLAG_FHASDEFAULT = 32,
			PARAMFLAG_FHASCUSTDATA = 64,
		}
		public enum System_Runtime_InteropServices_ComTypes_VARKIND : int
		{
			VAR_PERINSTANCE = 0,
			VAR_STATIC = 1,
			VAR_CONST = 2,
			VAR_DISPATCH = 3,
		}
		public enum System_Runtime_InteropServices_ComTypes_FUNCKIND : int
		{
			FUNC_VIRTUAL = 0,
			FUNC_PUREVIRTUAL = 1,
			FUNC_NONVIRTUAL = 2,
			FUNC_STATIC = 3,
			FUNC_DISPATCH = 4,
		}
		[Flags]
		public enum System_Runtime_InteropServices_ComTypes_INVOKEKIND : int
		{
			INVOKE_FUNC = 1,
			INVOKE_PROPERTYGET = 2,
			INVOKE_PROPERTYPUT = 4,
			INVOKE_PROPERTYPUTREF = 8,
		}
		public enum System_Runtime_InteropServices_ComTypes_CALLCONV : int
		{
			CC_CDECL = 1,
			CC_MSCPASCAL = 2,
			CC_PASCAL = 2,
			CC_MACPASCAL = 3,
			CC_STDCALL = 4,
			CC_RESERVED = 5,
			CC_SYSCALL = 6,
			CC_MPWCDECL = 7,
			CC_MPWPASCAL = 8,
			CC_MAX = 9,
		}
		[Flags]
		public enum System_Runtime_InteropServices_ComTypes_FUNCFLAGS : short
		{
			FUNCFLAG_FRESTRICTED = 1,
			FUNCFLAG_FSOURCE = 2,
			FUNCFLAG_FBINDABLE = 4,
			FUNCFLAG_FREQUESTEDIT = 8,
			FUNCFLAG_FDISPLAYBIND = 16,
			FUNCFLAG_FDEFAULTBIND = 32,
			FUNCFLAG_FHIDDEN = 64,
			FUNCFLAG_FUSESGETLASTERROR = 128,
			FUNCFLAG_FDEFAULTCOLLELEM = 256,
			FUNCFLAG_FUIDEFAULT = 512,
			FUNCFLAG_FNONBROWSABLE = 1024,
			FUNCFLAG_FREPLACEABLE = 2048,
			FUNCFLAG_FIMMEDIATEBIND = 4096,
		}
		[Flags]
		public enum System_Runtime_InteropServices_ComTypes_VARFLAGS : short
		{
			VARFLAG_FREADONLY = 1,
			VARFLAG_FSOURCE = 2,
			VARFLAG_FBINDABLE = 4,
			VARFLAG_FREQUESTEDIT = 8,
			VARFLAG_FDISPLAYBIND = 16,
			VARFLAG_FDEFAULTBIND = 32,
			VARFLAG_FHIDDEN = 64,
			VARFLAG_FRESTRICTED = 128,
			VARFLAG_FDEFAULTCOLLELEM = 256,
			VARFLAG_FUIDEFAULT = 512,
			VARFLAG_FNONBROWSABLE = 1024,
			VARFLAG_FREPLACEABLE = 2048,
			VARFLAG_FIMMEDIATEBIND = 4096,
		}
		public enum System_Runtime_InteropServices_ComTypes_SYSKIND : int
		{
			SYS_WIN16 = 0,
			SYS_WIN32 = 1,
			SYS_MAC = 2,
			SYS_WIN64 = 3,
		}
		[Flags]
		public enum System_Runtime_InteropServices_ComTypes_LIBFLAGS : short
		{
			LIBFLAG_FRESTRICTED = 1,
			LIBFLAG_FCONTROL = 2,
			LIBFLAG_FHIDDEN = 4,
			LIBFLAG_FHASDISKIMAGE = 8,
		}
		public enum System_Runtime_InteropServices_WindowsRuntime_PropertyType : int
		{
			Empty = 0,
			UInt8 = 1,
			Int16 = 2,
			UInt16 = 3,
			Int32 = 4,
			UInt32 = 5,
			Int64 = 6,
			UInt64 = 7,
			Single = 8,
			Double = 9,
			Char16 = 10,
			Boolean = 11,
			String = 12,
			Inspectable = 13,
			DateTime = 14,
			TimeSpan = 15,
			Guid = 16,
			Point = 17,
			Size = 18,
			Rect = 19,
			Other = 20,
			UInt8Array = 1025,
			Int16Array = 1026,
			UInt16Array = 1027,
			Int32Array = 1028,
			UInt32Array = 1029,
			Int64Array = 1030,
			UInt64Array = 1031,
			SingleArray = 1032,
			DoubleArray = 1033,
			Char16Array = 1034,
			BooleanArray = 1035,
			StringArray = 1036,
			InspectableArray = 1037,
			DateTimeArray = 1038,
			TimeSpanArray = 1039,
			GuidArray = 1040,
			PointArray = 1041,
			SizeArray = 1042,
			RectArray = 1043,
			OtherArray = 1044,
		}
		[Flags]
		public enum System_Runtime_InteropServices_WindowsRuntime_InterfaceForwardingSupport : int
		{
			None = 0,
			IBindableVector = 1,
			IVector = 2,
			IBindableVectorView = 4,
			IVectorView = 8,
			IBindableIterableOrIIterable = 16,
		}
		public enum System_IO_SearchOption : int
		{
			TopDirectoryOnly = 0,
			AllDirectories = 1,
		}
		public enum System_IO_DriveType : int
		{
			Unknown = 0,
			NoRootDirectory = 1,
			Removable = 2,
			Fixed = 3,
			Network = 4,
			CDRom = 5,
			Ram = 6,
		}
		[Flags]
		public enum System_IO_FileAccess : int
		{
			Read = 1,
			Write = 2,
			ReadWrite = 3,
		}
		public enum System_IO_FileMode : int
		{
			CreateNew = 1,
			Create = 2,
			Open = 3,
			OpenOrCreate = 4,
			Truncate = 5,
			Append = 6,
		}
		[Flags]
		public enum System_IO_FileOptions : int
		{
			None = 0,
			Encrypted = 16384,
			DeleteOnClose = 67108864,
			SequentialScan = 134217728,
			RandomAccess = 268435456,
			Asynchronous = 1073741824,
			WriteThrough = -2147483648,
		}
		[Flags]
		public enum System_IO_FileShare : int
		{
			None = 0,
			Read = 1,
			Write = 2,
			ReadWrite = 3,
			Delete = 4,
			Inheritable = 16,
		}
		[Flags]
		public enum System_IO_FileAttributes : int
		{
			ReadOnly = 1,
			Hidden = 2,
			System = 4,
			Directory = 16,
			Archive = 32,
			Device = 64,
			Normal = 128,
			Temporary = 256,
			SparseFile = 512,
			ReparsePoint = 1024,
			Compressed = 2048,
			Offline = 4096,
			NotContentIndexed = 8192,
			Encrypted = 16384,
			IntegrityStream = 32768,
			NoScrubData = 131072,
		}
		public enum System_IO_SeekOrigin : int
		{
			Begin = 0,
			Current = 1,
			End = 2,
		}
		[Flags]
		public enum System_Runtime_CompilerServices_CompilationRelaxations : int
		{
			NoStringInterning = 8,
		}
		[Flags]
		public enum System_Runtime_CompilerServices_MethodImplOptions : int
		{
			Unmanaged = 4,
			NoInlining = 8,
			ForwardRef = 16,
			Synchronized = 32,
			NoOptimization = 64,
			PreserveSig = 128,
			AggressiveInlining = 256,
			InternalCall = 4096,
		}
		public enum System_Runtime_CompilerServices_MethodCodeType : int
		{
			IL = 0,
			Native = 1,
			OPTIL = 2,
			Runtime = 3,
		}
		public enum System_Runtime_CompilerServices_LoadHint : int
		{
			Default = 0,
			Always = 1,
			Sometimes = 2,
		}
		public enum System_Runtime_GCLargeObjectHeapCompactionMode : int
		{
			Default = 1,
			CompactOnce = 2,
		}
		public enum System_Runtime_GCLatencyMode : int
		{
			Batch = 0,
			Interactive = 1,
			LowLatency = 2,
			SustainedLowLatency = 3,
		}
		public enum System_Security_SecurityElementType : int
		{
			Regular = 0,
			Format = 1,
			Comment = 2,
		}
		public enum System_Security_Util_Tokenizer_TokenSource : int
		{
			UnicodeByteArray = 0,
			UTF8ByteArray = 1,
			ASCIIByteArray = 2,
			CharArray = 3,
			String = 4,
			NestedStrings = 5,
			Other = 6,
		}
		public enum System_Security_Util_Tokenizer_ByteTokenEncoding : int
		{
			UnicodeTokens = 0,
			UTF8Tokens = 1,
			ByteTokens = 2,
		}
		[Flags]
		public enum System_Security_Permissions_EnvironmentPermissionAccess : int
		{
			NoAccess = 0,
			Read = 1,
			Write = 2,
			AllAccess = 3,
		}
		[Flags]
		public enum System_Security_Permissions_FileDialogPermissionAccess : int
		{
			None = 0,
			Open = 1,
			Save = 2,
			OpenSave = 3,
		}
		[Flags]
		public enum System_Security_Permissions_FileIOPermissionAccess : int
		{
			NoAccess = 0,
			Read = 1,
			Write = 2,
			Append = 4,
			PathDiscovery = 8,
			AllAccess = 15,
		}
		[Flags]
		public enum System_Security_Permissions_HostProtectionResource : int
		{
			None = 0,
			Synchronization = 1,
			SharedState = 2,
			ExternalProcessMgmt = 4,
			SelfAffectingProcessMgmt = 8,
			ExternalThreading = 16,
			SelfAffectingThreading = 32,
			SecurityInfrastructure = 64,
			UI = 128,
			MayLeakOnAbort = 256,
			All = 511,
		}
		public enum System_Security_Permissions_BuiltInPermissionFlag : int
		{
			EnvironmentPermission = 1,
			FileDialogPermission = 2,
			FileIOPermission = 4,
			IsolatedStorageFilePermission = 8,
			ReflectionPermission = 16,
			RegistryPermission = 32,
			SecurityPermission = 64,
			UIPermission = 128,
			PrincipalPermission = 256,
			PublisherIdentityPermission = 512,
			SiteIdentityPermission = 1024,
			StrongNameIdentityPermission = 2048,
			UrlIdentityPermission = 4096,
			ZoneIdentityPermission = 8192,
			KeyContainerPermission = 16384,
		}
		public enum System_Security_Permissions_IsolatedStorageContainment : int
		{
			None = 0,
			DomainIsolationByUser = 16,
			ApplicationIsolationByUser = 21,
			AssemblyIsolationByUser = 32,
			DomainIsolationByMachine = 48,
			AssemblyIsolationByMachine = 64,
			ApplicationIsolationByMachine = 69,
			DomainIsolationByRoamingUser = 80,
			AssemblyIsolationByRoamingUser = 96,
			ApplicationIsolationByRoamingUser = 101,
			AdministerIsolatedStorageByUser = 112,
			UnrestrictedIsolatedStorage = 240,
		}
		public enum System_Security_Permissions_PermissionState : int
		{
			None = 0,
			Unrestricted = 1,
		}
		public enum System_Security_Permissions_SecurityAction : int
		{
			Demand = 2,
			Assert = 3,
			Deny = 4,
			PermitOnly = 5,
			LinkDemand = 6,
			InheritanceDemand = 7,
			RequestMinimum = 8,
			RequestOptional = 9,
			RequestRefuse = 10,
		}
		[Flags]
		public enum System_Security_Permissions_ReflectionPermissionFlag : int
		{
			NoFlags = 0,
			TypeInformation = 1,
			MemberAccess = 2,
			ReflectionEmit = 4,
			AllFlags = 7,
			RestrictedMemberAccess = 8,
		}
		[Flags]
		public enum System_Security_Permissions_SecurityPermissionFlag : int
		{
			NoFlags = 0,
			Assertion = 1,
			UnmanagedCode = 2,
			SkipVerification = 4,
			Execution = 8,
			ControlThread = 16,
			ControlEvidence = 32,
			ControlPolicy = 64,
			SerializationFormatter = 128,
			ControlDomainPolicy = 256,
			ControlPrincipal = 512,
			ControlAppDomain = 1024,
			RemotingConfiguration = 2048,
			Infrastructure = 4096,
			BindingRedirects = 8192,
			AllFlags = 16383,
		}
		public enum System_Security_Permissions_UIPermissionWindow : int
		{
			NoWindows = 0,
			SafeSubWindows = 1,
			SafeTopLevelWindows = 2,
			AllWindows = 3,
		}
		public enum System_Security_Permissions_UIPermissionClipboard : int
		{
			NoClipboard = 0,
			OwnClipboard = 1,
			AllClipboard = 2,
		}
		[Flags]
		public enum System_Security_Permissions_KeyContainerPermissionFlags : int
		{
			NoFlags = 0,
			Create = 1,
			Open = 2,
			Delete = 4,
			Import = 16,
			Export = 32,
			Sign = 256,
			Decrypt = 512,
			ViewAcl = 4096,
			ChangeAcl = 8192,
			AllFlags = 13111,
		}
		[Flags]
		public enum System_Security_Permissions_RegistryPermissionAccess : int
		{
			NoAccess = 0,
			Read = 1,
			Write = 2,
			Create = 4,
			AllAccess = 7,
		}
		public enum System_Security_PartialTrustVisibilityLevel : int
		{
			VisibleToAllHosts = 0,
			NotVisibleByDefault = 1,
		}
		public enum System_Security_SecurityCriticalScope : int
		{
			Explicit = 0,
			Everything = 1,
		}
		public enum System_Security_SecurityRuleSet : byte
		{
			None = 0,
			Level1 = 1,
			Level2 = 2,
		}
		public enum System_Security_PermissionType : int
		{
			SecurityUnmngdCodeAccess = 0,
			SecuritySkipVerification = 1,
			ReflectionTypeInfo = 2,
			SecurityAssert = 3,
			ReflectionMemberAccess = 4,
			SecuritySerialization = 5,
			ReflectionRestrictedMemberAccess = 6,
			FullTrust = 7,
			SecurityBindingRedirects = 8,
			UIPermission = 9,
			EnvironmentPermission = 10,
			FileDialogPermission = 11,
			FileIOPermission = 12,
			ReflectionPermission = 13,
			SecurityPermission = 14,
			SecurityControlEvidence = 16,
			SecurityControlPrincipal = 17,
		}
		[Flags]
		public enum System_Security_HostSecurityManagerOptions : int
		{
			None = 0,
			HostAppDomainEvidence = 1,
			HostPolicyLevel = 2,
			HostAssemblyEvidence = 4,
			HostDetermineApplicationTrust = 8,
			HostResolvePolicy = 16,
			AllFlags = 31,
		}
		public enum System_Security_PermissionSet_IsSubsetOfType : int
		{
			Normal = 0,
			CheckDemand = 1,
			CheckPermitOnly = 2,
			CheckAssertion = 3,
		}
		public enum System_Security_SpecialPermissionSetFlag : int
		{
			Regular = 0,
			NoSet = 1,
			EmptySet = 2,
			SkipVerification = 3,
		}
		[Flags]
		public enum System_Security_PermissionTokenType : int
		{
			Normal = 1,
			IUnrestricted = 2,
			DontKnow = 4,
			BuiltIn = 8,
		}
		public enum System_Security_SecurityContextSource : int
		{
			CurrentAppDomain = 0,
			CurrentAssembly = 1,
		}
		public enum System_Security_SecurityContextDisableFlow : int
		{
			Nothing = 0,
			WI = 1,
			All = 16383,
		}
		public enum System_Security_WindowsImpersonationFlowMode : int
		{
			IMP_DEFAULT = 0,
			IMP_FASTFLOW = 0,
			IMP_NOFLOW = 1,
			IMP_ALWAYSFLOW = 2,
		}
		public enum System_Security_PolicyLevelType : int
		{
			User = 0,
			Machine = 1,
			Enterprise = 2,
			AppDomain = 3,
		}
		public enum System_Security_SecurityZone : int
		{
			MyComputer = 0,
			Intranet = 1,
			Trusted = 2,
			Internet = 3,
			Untrusted = 4,
			NoZone = -1,
		}
		public enum System_Runtime_Remoting_Channels_RemotingProfilerEvent : int
		{
			ClientSend = 0,
			ClientReceive = 1,
		}
		public enum System_Runtime_Remoting_WellKnownObjectMode : int
		{
			Singleton = 1,
			SingleCall = 2,
		}
		public enum System_Runtime_Remoting_Activation_ActivatorLevel : int
		{
			Construction = 4,
			Context = 8,
			AppDomain = 12,
			Process = 16,
			Machine = 20,
		}
		public enum System_Runtime_Remoting_Channels_ServerProcessing : int
		{
			Complete = 0,
			OneWay = 1,
			Async = 2,
		}
		public enum System_Runtime_Remoting_DuplicateIdentityOption : int
		{
			Unique = 0,
			UseExisting = 1,
		}
		public enum System_Runtime_Remoting_Lifetime_Lease_SponsorState : int
		{
			Initial = 0,
			Waiting = 1,
			Completed = 2,
		}
		public enum System_Runtime_Remoting_Lifetime_LeaseState : int
		{
			Null = 0,
			Initial = 1,
			Active = 2,
			Renewing = 3,
			Expired = 4,
		}
		public enum System_Runtime_Remoting_Proxies_CallType : int
		{
			InvalidCall = 0,
			MethodCall = 1,
			ConstructorCall = 2,
		}
		[Flags]
		public enum System_Runtime_Remoting_Proxies_RealProxyFlags : int
		{
			None = 0,
			RemotingProxy = 1,
			Initialized = 2,
		}
		[Flags]
		public enum System_Runtime_Remoting_Metadata_RemotingMethodCachedData_MethodCacheFlags : int
		{
			None = 0,
			CheckedOneWay = 1,
			IsOneWay = 2,
			CheckedOverloaded = 4,
			IsOverloaded = 8,
			CheckedForAsync = 16,
			CheckedForReturnType = 32,
		}
		[Flags]
		public enum System_Runtime_Remoting_Metadata_SoapOption : int
		{
			None = 0,
			AlwaysIncludeTypes = 1,
			XsdString = 2,
			EmbedAll = 4,
			Option1 = 8,
			Option2 = 16,
		}
		public enum System_Runtime_Remoting_Metadata_XmlFieldOrderOption : int
		{
			All = 0,
			Sequence = 1,
			Choice = 2,
		}
		[Flags]
		public enum System_Runtime_Remoting_Metadata_SoapTypeAttribute_ExplicitlySet : int
		{
			None = 0,
			XmlElementName = 1,
			XmlNamespace = 2,
			XmlTypeName = 4,
			XmlTypeNamespace = 8,
		}
		[Flags]
		public enum System_Runtime_Remoting_Metadata_SoapFieldAttribute_ExplicitlySet : int
		{
			None = 0,
			XmlElementName = 1,
		}
		public enum System_Runtime_Remoting_CustomErrorsModes : int
		{
			On = 0,
			Off = 1,
			RemoteOnly = 2,
		}
		[Flags]
		public enum System_IO_IsolatedStorage_IsolatedStorageScope : int
		{
			None = 0,
			User = 1,
			Domain = 2,
			Assembly = 4,
			Roaming = 8,
			Machine = 16,
			Application = 32,
		}
		public enum System_IO_IsolatedStorage_IsolatedStorageSecurityOptions : int
		{
			IncreaseQuotaForApplication = 4,
		}
		public enum System_Runtime_Serialization_Formatters_FormatterTypeStyle : int
		{
			TypesWhenNeeded = 0,
			TypesAlways = 1,
			XsdString = 2,
		}
		public enum System_Runtime_Serialization_Formatters_FormatterAssemblyStyle : int
		{
			Simple = 0,
			Full = 1,
		}
		public enum System_Runtime_Serialization_Formatters_TypeFilterLevel : int
		{
			Low = 2,
			Full = 3,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_BinaryHeaderEnum : int
		{
			SerializedStreamHeader = 0,
			Object = 1,
			ObjectWithMap = 2,
			ObjectWithMapAssemId = 3,
			ObjectWithMapTyped = 4,
			ObjectWithMapTypedAssemId = 5,
			ObjectString = 6,
			Array = 7,
			MemberPrimitiveTyped = 8,
			MemberReference = 9,
			ObjectNull = 10,
			MessageEnd = 11,
			Assembly = 12,
			ObjectNullMultiple256 = 13,
			ObjectNullMultiple = 14,
			ArraySinglePrimitive = 15,
			ArraySingleObject = 16,
			ArraySingleString = 17,
			CrossAppDomainMap = 18,
			CrossAppDomainString = 19,
			CrossAppDomainAssembly = 20,
			MethodCall = 21,
			MethodReturn = 22,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_BinaryTypeEnum : int
		{
			Primitive = 0,
			String = 1,
			Object = 2,
			ObjectUrt = 3,
			ObjectUser = 4,
			ObjectArray = 5,
			StringArray = 6,
			PrimitiveArray = 7,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_BinaryArrayTypeEnum : int
		{
			Single = 0,
			Jagged = 1,
			Rectangular = 2,
			SingleOffset = 3,
			JaggedOffset = 4,
			RectangularOffset = 5,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_InternalSerializerTypeE : int
		{
			Soap = 1,
			Binary = 2,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_InternalElementTypeE : int
		{
			ObjectBegin = 0,
			ObjectEnd = 1,
			Member = 2,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_InternalParseTypeE : int
		{
			Empty = 0,
			SerializedStreamHeader = 1,
			Object = 2,
			Member = 3,
			ObjectEnd = 4,
			MemberEnd = 5,
			Headers = 6,
			HeadersEnd = 7,
			SerializedStreamHeaderEnd = 8,
			Envelope = 9,
			EnvelopeEnd = 10,
			Body = 11,
			BodyEnd = 12,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_InternalObjectTypeE : int
		{
			Empty = 0,
			Object = 1,
			Array = 2,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_InternalObjectPositionE : int
		{
			Empty = 0,
			Top = 1,
			Child = 2,
			Headers = 3,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_InternalArrayTypeE : int
		{
			Empty = 0,
			Single = 1,
			Jagged = 2,
			Rectangular = 3,
			Base64 = 4,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_InternalMemberTypeE : int
		{
			Empty = 0,
			Header = 1,
			Field = 2,
			Item = 3,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_InternalMemberValueE : int
		{
			Empty = 0,
			InlineValue = 1,
			Nested = 2,
			Reference = 3,
			Null = 4,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_InternalParseStateE : int
		{
			Initial = 0,
			Object = 1,
			Member = 2,
			MemberChild = 3,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_InternalPrimitiveTypeE : int
		{
			Invalid = 0,
			Boolean = 1,
			Byte = 2,
			Char = 3,
			Currency = 4,
			Decimal = 5,
			Double = 6,
			Int16 = 7,
			Int32 = 8,
			Int64 = 9,
			SByte = 10,
			Single = 11,
			TimeSpan = 12,
			DateTime = 13,
			UInt16 = 14,
			UInt32 = 15,
			UInt64 = 16,
			Null = 17,
			String = 18,
		}
		[Flags]
		public enum System_Runtime_Serialization_Formatters_Binary_MessageEnum : int
		{
			NoArgs = 1,
			ArgsInline = 2,
			ArgsIsArray = 4,
			ArgsInArray = 8,
			NoContext = 16,
			ContextInline = 32,
			ContextInArray = 64,
			MethodSignatureInArray = 128,
			PropertyInArray = 256,
			NoReturnValue = 512,
			ReturnValueVoid = 1024,
			ReturnValueInline = 2048,
			ReturnValueInArray = 4096,
			ExceptionInArray = 8192,
			GenericMethod = 32768,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_ValueFixupEnum : int
		{
			Empty = 0,
			Array = 1,
			Header = 2,
			Member = 3,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_InternalNameSpaceE : int
		{
			None = 0,
			Soap = 1,
			XdrPrimitive = 2,
			XdrString = 3,
			UrtSystem = 4,
			UrtUser = 5,
			UserNameSpace = 6,
			MemberName = 7,
			Interop = 8,
			CallElement = 9,
		}
		public enum System_Runtime_Serialization_Formatters_Binary_SoapAttributeType : int
		{
			None = 0,
			SchemaType = 1,
			Embedded = 2,
			XmlElement = 4,
			XmlAttribute = 8,
		}
		[Flags]
		public enum System_Reflection_Emit_DynamicAssemblyFlags : int
		{
			None = 0,
			AllCritical = 1,
			Aptca = 2,
			Critical = 4,
			Transparent = 8,
			TreatAsSafe = 16,
		}
		[Flags]
		public enum System_Reflection_Emit_AssemblyBuilderAccess : int
		{
			Run = 1,
			Save = 2,
			RunAndSave = 3,
			ReflectionOnly = 6,
			RunAndCollect = 9,
		}
		public enum System_Reflection_Emit_TypeNameBuilder_Format : int
		{
			ToString = 0,
			FullName = 1,
			AssemblyQualifiedName = 2,
		}
		[Flags]
		public enum System_Reflection_Emit_DynamicResolver_SecurityControlFlags : int
		{
			Default = 0,
			SkipVisibilityChecks = 1,
			RestrictedSkipVisibilityChecks = 2,
			HasCreationContext = 4,
			CanSkipCSEvaluation = 8,
		}
		public enum System_Reflection_Emit_ScopeAction : int
		{
			Open = 0,
			Close = 1,
		}
		public enum System_Reflection_Emit_TypeKind : int
		{
			IsArray = 1,
			IsPointer = 2,
			IsByRef = 3,
		}
		public enum System_Reflection_Emit_PEFileKinds : int
		{
			Dll = 1,
			ConsoleApplication = 2,
			WindowApplication = 3,
		}
		public enum System_Reflection_Emit_OpCodeValues : int
		{
			Nop = 0,
			Break = 1,
			Ldarg_0 = 2,
			Ldarg_1 = 3,
			Ldarg_2 = 4,
			Ldarg_3 = 5,
			Ldloc_0 = 6,
			Ldloc_1 = 7,
			Ldloc_2 = 8,
			Ldloc_3 = 9,
			Stloc_0 = 10,
			Stloc_1 = 11,
			Stloc_2 = 12,
			Stloc_3 = 13,
			Ldarg_S = 14,
			Ldarga_S = 15,
			Starg_S = 16,
			Ldloc_S = 17,
			Ldloca_S = 18,
			Stloc_S = 19,
			Ldnull = 20,
			Ldc_I4_M1 = 21,
			Ldc_I4_0 = 22,
			Ldc_I4_1 = 23,
			Ldc_I4_2 = 24,
			Ldc_I4_3 = 25,
			Ldc_I4_4 = 26,
			Ldc_I4_5 = 27,
			Ldc_I4_6 = 28,
			Ldc_I4_7 = 29,
			Ldc_I4_8 = 30,
			Ldc_I4_S = 31,
			Ldc_I4 = 32,
			Ldc_I8 = 33,
			Ldc_R4 = 34,
			Ldc_R8 = 35,
			Dup = 37,
			Pop = 38,
			Jmp = 39,
			Call = 40,
			Calli = 41,
			Ret = 42,
			Br_S = 43,
			Brfalse_S = 44,
			Brtrue_S = 45,
			Beq_S = 46,
			Bge_S = 47,
			Bgt_S = 48,
			Ble_S = 49,
			Blt_S = 50,
			Bne_Un_S = 51,
			Bge_Un_S = 52,
			Bgt_Un_S = 53,
			Ble_Un_S = 54,
			Blt_Un_S = 55,
			Br = 56,
			Brfalse = 57,
			Brtrue = 58,
			Beq = 59,
			Bge = 60,
			Bgt = 61,
			Ble = 62,
			Blt = 63,
			Bne_Un = 64,
			Bge_Un = 65,
			Bgt_Un = 66,
			Ble_Un = 67,
			Blt_Un = 68,
			Switch = 69,
			Ldind_I1 = 70,
			Ldind_U1 = 71,
			Ldind_I2 = 72,
			Ldind_U2 = 73,
			Ldind_I4 = 74,
			Ldind_U4 = 75,
			Ldind_I8 = 76,
			Ldind_I = 77,
			Ldind_R4 = 78,
			Ldind_R8 = 79,
			Ldind_Ref = 80,
			Stind_Ref = 81,
			Stind_I1 = 82,
			Stind_I2 = 83,
			Stind_I4 = 84,
			Stind_I8 = 85,
			Stind_R4 = 86,
			Stind_R8 = 87,
			Add = 88,
			Sub = 89,
			Mul = 90,
			Div = 91,
			Div_Un = 92,
			Rem = 93,
			Rem_Un = 94,
			And = 95,
			Or = 96,
			Xor = 97,
			Shl = 98,
			Shr = 99,
			Shr_Un = 100,
			Neg = 101,
			Not = 102,
			Conv_I1 = 103,
			Conv_I2 = 104,
			Conv_I4 = 105,
			Conv_I8 = 106,
			Conv_R4 = 107,
			Conv_R8 = 108,
			Conv_U4 = 109,
			Conv_U8 = 110,
			Callvirt = 111,
			Cpobj = 112,
			Ldobj = 113,
			Ldstr = 114,
			Newobj = 115,
			Castclass = 116,
			Isinst = 117,
			Conv_R_Un = 118,
			Unbox = 121,
			Throw = 122,
			Ldfld = 123,
			Ldflda = 124,
			Stfld = 125,
			Ldsfld = 126,
			Ldsflda = 127,
			Stsfld = 128,
			Stobj = 129,
			Conv_Ovf_I1_Un = 130,
			Conv_Ovf_I2_Un = 131,
			Conv_Ovf_I4_Un = 132,
			Conv_Ovf_I8_Un = 133,
			Conv_Ovf_U1_Un = 134,
			Conv_Ovf_U2_Un = 135,
			Conv_Ovf_U4_Un = 136,
			Conv_Ovf_U8_Un = 137,
			Conv_Ovf_I_Un = 138,
			Conv_Ovf_U_Un = 139,
			Box = 140,
			Newarr = 141,
			Ldlen = 142,
			Ldelema = 143,
			Ldelem_I1 = 144,
			Ldelem_U1 = 145,
			Ldelem_I2 = 146,
			Ldelem_U2 = 147,
			Ldelem_I4 = 148,
			Ldelem_U4 = 149,
			Ldelem_I8 = 150,
			Ldelem_I = 151,
			Ldelem_R4 = 152,
			Ldelem_R8 = 153,
			Ldelem_Ref = 154,
			Stelem_I = 155,
			Stelem_I1 = 156,
			Stelem_I2 = 157,
			Stelem_I4 = 158,
			Stelem_I8 = 159,
			Stelem_R4 = 160,
			Stelem_R8 = 161,
			Stelem_Ref = 162,
			Ldelem = 163,
			Stelem = 164,
			Unbox_Any = 165,
			Conv_Ovf_I1 = 179,
			Conv_Ovf_U1 = 180,
			Conv_Ovf_I2 = 181,
			Conv_Ovf_U2 = 182,
			Conv_Ovf_I4 = 183,
			Conv_Ovf_U4 = 184,
			Conv_Ovf_I8 = 185,
			Conv_Ovf_U8 = 186,
			Refanyval = 194,
			Ckfinite = 195,
			Mkrefany = 198,
			Ldtoken = 208,
			Conv_U2 = 209,
			Conv_U1 = 210,
			Conv_I = 211,
			Conv_Ovf_I = 212,
			Conv_Ovf_U = 213,
			Add_Ovf = 214,
			Add_Ovf_Un = 215,
			Mul_Ovf = 216,
			Mul_Ovf_Un = 217,
			Sub_Ovf = 218,
			Sub_Ovf_Un = 219,
			Endfinally = 220,
			Leave = 221,
			Leave_S = 222,
			Stind_I = 223,
			Conv_U = 224,
			Prefix7 = 248,
			Prefix6 = 249,
			Prefix5 = 250,
			Prefix4 = 251,
			Prefix3 = 252,
			Prefix2 = 253,
			Prefix1 = 254,
			Prefixref = 255,
			Arglist = 65024,
			Ceq = 65025,
			Cgt = 65026,
			Cgt_Un = 65027,
			Clt = 65028,
			Clt_Un = 65029,
			Ldftn = 65030,
			Ldvirtftn = 65031,
			Ldarg = 65033,
			Ldarga = 65034,
			Starg = 65035,
			Ldloc = 65036,
			Ldloca = 65037,
			Stloc = 65038,
			Localloc = 65039,
			Endfilter = 65041,
			Unaligned_ = 65042,
			Volatile_ = 65043,
			Tail_ = 65044,
			Initobj = 65045,
			Constrained_ = 65046,
			Cpblk = 65047,
			Initblk = 65048,
			Rethrow = 65050,
			Sizeof = 65052,
			Refanytype = 65053,
			Readonly_ = 65054,
		}
		public enum System_Reflection_Emit_OpCodeType : int
		{
			Annotation = 0,
			Macro = 1,
			Nternal = 2,
			Objmodel = 3,
			Prefix = 4,
			Primitive = 5,
		}
		public enum System_Reflection_Emit_StackBehaviour : int
		{
			Pop0 = 0,
			Pop1 = 1,
			Pop1_pop1 = 2,
			Popi = 3,
			Popi_pop1 = 4,
			Popi_popi = 5,
			Popi_popi8 = 6,
			Popi_popi_popi = 7,
			Popi_popr4 = 8,
			Popi_popr8 = 9,
			Popref = 10,
			Popref_pop1 = 11,
			Popref_popi = 12,
			Popref_popi_popi = 13,
			Popref_popi_popi8 = 14,
			Popref_popi_popr4 = 15,
			Popref_popi_popr8 = 16,
			Popref_popi_popref = 17,
			Push0 = 18,
			Push1 = 19,
			Push1_push1 = 20,
			Pushi = 21,
			Pushi8 = 22,
			Pushr4 = 23,
			Pushr8 = 24,
			Pushref = 25,
			Varpop = 26,
			Varpush = 27,
			Popref_popi_pop1 = 28,
		}
		public enum System_Reflection_Emit_OperandType : int
		{
			InlineBrTarget = 0,
			InlineField = 1,
			InlineI = 2,
			InlineI8 = 3,
			InlineMethod = 4,
			InlineNone = 5,
			InlinePhi = 6,
			InlineR = 7,
			InlineSig = 9,
			InlineString = 10,
			InlineSwitch = 11,
			InlineTok = 12,
			InlineType = 13,
			InlineVar = 14,
			ShortInlineBrTarget = 15,
			ShortInlineI = 16,
			ShortInlineR = 17,
			ShortInlineVar = 18,
		}
		public enum System_Reflection_Emit_FlowControl : int
		{
			Branch = 0,
			Break = 1,
			Call = 2,
			Cond_Branch = 3,
			Meta = 4,
			Next = 5,
			Phi = 6,
			Return = 7,
			Throw = 8,
		}
		public enum System_Reflection_Emit_PackingSize : int
		{
			Unspecified = 0,
			Size1 = 1,
			Size2 = 2,
			Size4 = 4,
			Size8 = 8,
			Size16 = 16,
			Size32 = 32,
			Size64 = 64,
			Size128 = 128,
		}
		public enum System_Configuration_Assemblies_AssemblyHashAlgorithm : int
		{
			None = 0,
			MD5 = 32771,
			SHA1 = 32772,
			SHA256 = 32780,
			SHA384 = 32781,
			SHA512 = 32782,
		}
		public enum System_Configuration_Assemblies_AssemblyVersionCompatibility : int
		{
			SameMachine = 1,
			SameProcess = 2,
			SameDomain = 3,
		}
		public enum System_Security_Cryptography_CipherMode : int
		{
			CBC = 1,
			ECB = 2,
			OFB = 3,
			CFB = 4,
			CTS = 5,
		}
		public enum System_Security_Cryptography_PaddingMode : int
		{
			None = 1,
			PKCS7 = 2,
			Zeros = 3,
			ANSIX923 = 4,
			ISO10126 = 5,
		}
		public enum System_Security_Cryptography_FromBase64TransformMode : int
		{
			IgnoreWhiteSpaces = 0,
			DoNotIgnoreWhiteSpaces = 1,
		}
		public enum System_Security_Cryptography_CryptoAPITransformMode : int
		{
			Encrypt = 0,
			Decrypt = 1,
		}
		[Flags]
		public enum System_Security_Cryptography_CspProviderFlags : int
		{
			NoFlags = 0,
			UseMachineKeyStore = 1,
			UseDefaultKeyContainer = 2,
			UseNonExportableKey = 4,
			UseExistingKey = 8,
			UseArchivableKey = 16,
			UseUserProtectedKey = 32,
			NoPrompt = 64,
			CreateEphemeralKey = 128,
		}
		public enum System_Security_Cryptography_CryptoStreamMode : int
		{
			Read = 0,
			Write = 1,
		}
		public enum System_Security_Cryptography_KeyNumber : int
		{
			Exchange = 1,
			Signature = 2,
		}
		public enum System_Security_Cryptography_RijndaelManagedTransformMode : int
		{
			Encrypt = 0,
			Decrypt = 1,
		}
		public enum System_Security_Cryptography_CspAlgorithmType : int
		{
			Rsa = 0,
			Dss = 1,
		}
		public enum System_Security_Cryptography_X509Certificates_OidGroup : int
		{
			AllGroups = 0,
			HashAlgorithm = 1,
			EncryptionAlgorithm = 2,
			PublicKeyAlgorithm = 3,
			SignatureAlgorithm = 4,
			Attribute = 5,
			ExtensionOrAttribute = 6,
			EnhancedKeyUsage = 7,
			Policy = 8,
			Template = 9,
			KeyDerivationFunction = 10,
			DisableSearchDS = -2147483648,
		}
		public enum System_Security_Cryptography_X509Certificates_OidKeyType : int
		{
			Oid = 1,
			Name = 2,
			AlgorithmID = 3,
			SignatureID = 4,
			CngAlgorithmID = 5,
			CngSignatureID = 6,
		}
		public enum System_Security_Cryptography_X509Certificates_X509ContentType : int
		{
			Unknown = 0,
			Cert = 1,
			SerializedCert = 2,
			Pfx = 3,
			Pkcs12 = 3,
			SerializedStore = 4,
			Pkcs7 = 5,
			Authenticode = 6,
		}
		[Flags]
		public enum System_Security_Cryptography_X509Certificates_X509KeyStorageFlags : int
		{
			DefaultKeySet = 0,
			UserKeySet = 1,
			MachineKeySet = 2,
			Exportable = 4,
			UserProtected = 8,
			PersistKeySet = 16,
		}
		[Flags]
		public enum System_Security_AccessControl_InheritanceFlags : int
		{
			None = 0,
			ContainerInherit = 1,
			ObjectInherit = 2,
		}
		[Flags]
		public enum System_Security_AccessControl_PropagationFlags : int
		{
			None = 0,
			NoPropagateInherit = 1,
			InheritOnly = 2,
		}
		[Flags]
		public enum System_Security_AccessControl_AuditFlags : int
		{
			None = 0,
			Success = 1,
			Failure = 2,
		}
		[Flags]
		public enum System_Security_AccessControl_SecurityInfos : int
		{
			Owner = 1,
			Group = 2,
			DiscretionaryAcl = 4,
			SystemAcl = 8,
		}
		public enum System_Security_AccessControl_ResourceType : int
		{
			Unknown = 0,
			FileObject = 1,
			Service = 2,
			Printer = 3,
			RegistryKey = 4,
			LMShare = 5,
			KernelObject = 6,
			WindowObject = 7,
			DSObject = 8,
			DSObjectAll = 9,
			ProviderDefined = 10,
			WmiGuidObject = 11,
			RegistryWow6432Key = 12,
		}
		[Flags]
		public enum System_Security_AccessControl_AccessControlSections : int
		{
			None = 0,
			Audit = 1,
			Access = 2,
			Owner = 4,
			Group = 8,
			All = 15,
		}
		[Flags]
		public enum System_Security_AccessControl_AccessControlActions : int
		{
			None = 0,
			View = 1,
			Change = 2,
		}
		public enum System_Security_AccessControl_AceType : byte
		{
			AccessAllowed = 0,
			AccessDenied = 1,
			SystemAudit = 2,
			SystemAlarm = 3,
			AccessAllowedCompound = 4,
			AccessAllowedObject = 5,
			AccessDeniedObject = 6,
			SystemAuditObject = 7,
			SystemAlarmObject = 8,
			AccessAllowedCallback = 9,
			AccessDeniedCallback = 10,
			AccessAllowedCallbackObject = 11,
			AccessDeniedCallbackObject = 12,
			SystemAuditCallback = 13,
			SystemAlarmCallback = 14,
			SystemAuditCallbackObject = 15,
			SystemAlarmCallbackObject = 16,
			MaxDefinedAceType = 16,
		}
		[Flags]
		public enum System_Security_AccessControl_AceFlags : byte
		{
			None = 0,
			ObjectInherit = 1,
			ContainerInherit = 2,
			NoPropagateInherit = 4,
			InheritOnly = 8,
			InheritanceFlags = 15,
			Inherited = 16,
			SuccessfulAccess = 64,
			FailedAccess = 128,
			AuditFlags = 192,
		}
		public enum System_Security_AccessControl_CompoundAceType : int
		{
			Impersonation = 1,
		}
		public enum System_Security_AccessControl_AceQualifier : int
		{
			AccessAllowed = 0,
			AccessDenied = 1,
			SystemAudit = 2,
			SystemAlarm = 3,
		}
		[Flags]
		public enum System_Security_AccessControl_ObjectAceFlags : int
		{
			None = 0,
			ObjectAceTypePresent = 1,
			InheritedObjectAceTypePresent = 2,
		}
		[Flags]
		public enum System_Security_AccessControl_CommonAcl_AF : int
		{
			Invalid = 1,
			NP = 1,
			IO = 2,
			OI = 4,
			CI = 8,
		}
		[Flags]
		public enum System_Security_AccessControl_CommonAcl_PM : int
		{
			GO = 1,
			Invalid = 1,
			GF = 2,
			CO = 4,
			CF = 8,
			F = 16,
		}
		[Flags]
		public enum System_Security_AccessControl_CryptoKeyRights : int
		{
			ReadData = 1,
			WriteData = 2,
			ReadExtendedAttributes = 8,
			WriteExtendedAttributes = 16,
			ReadAttributes = 128,
			WriteAttributes = 256,
			Delete = 65536,
			ReadPermissions = 131072,
			ChangePermissions = 262144,
			TakeOwnership = 524288,
			Synchronize = 1048576,
			FullControl = 2032027,
			GenericAll = 268435456,
			GenericExecute = 536870912,
			GenericWrite = 1073741824,
			GenericRead = -2147483648,
		}
		[Flags]
		public enum System_Security_AccessControl_EventWaitHandleRights : int
		{
			Modify = 2,
			Delete = 65536,
			ReadPermissions = 131072,
			ChangePermissions = 262144,
			TakeOwnership = 524288,
			Synchronize = 1048576,
			FullControl = 2031619,
		}
		[Flags]
		public enum System_Security_AccessControl_FileSystemRights : int
		{
			ListDirectory = 1,
			ReadData = 1,
			WriteData = 2,
			CreateFiles = 2,
			CreateDirectories = 4,
			AppendData = 4,
			ReadExtendedAttributes = 8,
			WriteExtendedAttributes = 16,
			Traverse = 32,
			ExecuteFile = 32,
			DeleteSubdirectoriesAndFiles = 64,
			ReadAttributes = 128,
			WriteAttributes = 256,
			Write = 278,
			Delete = 65536,
			ReadPermissions = 131072,
			Read = 131209,
			ReadAndExecute = 131241,
			Modify = 197055,
			ChangePermissions = 262144,
			TakeOwnership = 524288,
			Synchronize = 1048576,
			FullControl = 2032127,
		}
		[Flags]
		public enum System_Security_AccessControl_MutexRights : int
		{
			Modify = 1,
			Delete = 65536,
			ReadPermissions = 131072,
			ChangePermissions = 262144,
			TakeOwnership = 524288,
			Synchronize = 1048576,
			FullControl = 2031617,
		}
		public enum System_Security_AccessControl_AccessControlModification : int
		{
			Add = 0,
			Set = 1,
			Reset = 2,
			Remove = 3,
			RemoveAll = 4,
			RemoveSpecific = 5,
		}
		[Flags]
		public enum System_Security_AccessControl_RegistryRights : int
		{
			QueryValues = 1,
			SetValue = 2,
			CreateSubKey = 4,
			EnumerateSubKeys = 8,
			Notify = 16,
			CreateLink = 32,
			Delete = 65536,
			ReadPermissions = 131072,
			WriteKey = 131078,
			ExecuteKey = 131097,
			ReadKey = 131097,
			ChangePermissions = 262144,
			TakeOwnership = 524288,
			FullControl = 983103,
		}
		public enum System_Security_AccessControl_AccessControlType : int
		{
			Allow = 0,
			Deny = 1,
		}
		[Flags]
		public enum System_Security_AccessControl_ControlFlags : int
		{
			None = 0,
			OwnerDefaulted = 1,
			GroupDefaulted = 2,
			DiscretionaryAclPresent = 4,
			DiscretionaryAclDefaulted = 8,
			SystemAclPresent = 16,
			SystemAclDefaulted = 32,
			DiscretionaryAclUntrusted = 64,
			ServerSecurity = 128,
			DiscretionaryAclAutoInheritRequired = 256,
			SystemAclAutoInheritRequired = 512,
			DiscretionaryAclAutoInherited = 1024,
			SystemAclAutoInherited = 2048,
			DiscretionaryAclProtected = 4096,
			SystemAclProtected = 8192,
			RMControlValid = 16384,
			SelfRelative = 32768,
		}
		public enum System_Security_Principal_IdentifierAuthority : long
		{
			NullAuthority = 0,
			WorldAuthority = 1,
			LocalAuthority = 2,
			CreatorAuthority = 3,
			NonUniqueAuthority = 4,
			NTAuthority = 5,
			SiteServerAuthority = 6,
			InternetSiteAuthority = 7,
			ExchangeAuthority = 8,
			ResourceManagerAuthority = 9,
		}
		public enum System_Security_Principal_SidNameUse : int
		{
			User = 1,
			Group = 2,
			Domain = 3,
			Alias = 4,
			WellKnownGroup = 5,
			DeletedAccount = 6,
			Invalid = 7,
			Unknown = 8,
			Computer = 9,
		}
		public enum System_Security_Principal_WellKnownSidType : int
		{
			NullSid = 0,
			WorldSid = 1,
			LocalSid = 2,
			CreatorOwnerSid = 3,
			CreatorGroupSid = 4,
			CreatorOwnerServerSid = 5,
			CreatorGroupServerSid = 6,
			NTAuthoritySid = 7,
			DialupSid = 8,
			NetworkSid = 9,
			BatchSid = 10,
			InteractiveSid = 11,
			ServiceSid = 12,
			AnonymousSid = 13,
			ProxySid = 14,
			EnterpriseControllersSid = 15,
			SelfSid = 16,
			AuthenticatedUserSid = 17,
			RestrictedCodeSid = 18,
			TerminalServerSid = 19,
			RemoteLogonIdSid = 20,
			LogonIdsSid = 21,
			LocalSystemSid = 22,
			LocalServiceSid = 23,
			NetworkServiceSid = 24,
			BuiltinDomainSid = 25,
			BuiltinAdministratorsSid = 26,
			BuiltinUsersSid = 27,
			BuiltinGuestsSid = 28,
			BuiltinPowerUsersSid = 29,
			BuiltinAccountOperatorsSid = 30,
			BuiltinSystemOperatorsSid = 31,
			BuiltinPrintOperatorsSid = 32,
			BuiltinBackupOperatorsSid = 33,
			BuiltinReplicatorSid = 34,
			BuiltinPreWindows2000CompatibleAccessSid = 35,
			BuiltinRemoteDesktopUsersSid = 36,
			BuiltinNetworkConfigurationOperatorsSid = 37,
			AccountAdministratorSid = 38,
			AccountGuestSid = 39,
			AccountKrbtgtSid = 40,
			AccountDomainAdminsSid = 41,
			AccountDomainUsersSid = 42,
			AccountDomainGuestsSid = 43,
			AccountComputersSid = 44,
			AccountControllersSid = 45,
			AccountCertAdminsSid = 46,
			AccountSchemaAdminsSid = 47,
			AccountEnterpriseAdminsSid = 48,
			AccountPolicyAdminsSid = 49,
			AccountRasAndIasServersSid = 50,
			NtlmAuthenticationSid = 51,
			DigestAuthenticationSid = 52,
			SChannelAuthenticationSid = 53,
			ThisOrganizationSid = 54,
			OtherOrganizationSid = 55,
			BuiltinIncomingForestTrustBuildersSid = 56,
			BuiltinPerformanceMonitoringUsersSid = 57,
			BuiltinPerformanceLoggingUsersSid = 58,
			BuiltinAuthorizationAccessSid = 59,
			WinBuiltinTerminalServerLicenseServersSid = 60,
			MaxDefined = 60,
		}
		[Flags]
		public enum System_Security_Principal_PolicyRights : int
		{
			POLICY_VIEW_LOCAL_INFORMATION = 1,
			POLICY_VIEW_AUDIT_INFORMATION = 2,
			POLICY_GET_PRIVATE_INFORMATION = 4,
			POLICY_TRUST_ADMIN = 8,
			POLICY_CREATE_ACCOUNT = 16,
			POLICY_CREATE_SECRET = 32,
			POLICY_CREATE_PRIVILEGE = 64,
			POLICY_SET_DEFAULT_QUOTA_LIMITS = 128,
			POLICY_SET_AUDIT_REQUIREMENTS = 256,
			POLICY_AUDIT_LOG_ADMIN = 512,
			POLICY_SERVER_ADMIN = 1024,
			POLICY_LOOKUP_NAMES = 2048,
			POLICY_NOTIFICATION = 4096,
		}
		[Flags]
		public enum System_Runtime_Versioning_ComponentGuaranteesOptions : int
		{
			None = 0,
			Exchange = 1,
			Stable = 2,
			SideBySide = 4,
		}
		[Flags]
		public enum System_Runtime_Versioning_ResourceScope : int
		{
			None = 0,
			Machine = 1,
			Process = 2,
			AppDomain = 4,
			Library = 8,
			Private = 16,
			Assembly = 32,
		}
		[Flags]
		public enum System_Runtime_Versioning_SxSRequirements : int
		{
			None = 0,
			AppDomainID = 1,
			ProcessID = 2,
			CLRInstanceID = 4,
			AssemblyName = 8,
			TypeName = 16,
		}
		public enum System_Runtime_Versioning_TargetFrameworkId : int
		{
			NotYetChecked = 0,
			Unrecognized = 1,
			Unspecified = 2,
			NetFramework = 3,
			Portable = 4,
			NetCore = 5,
			Silverlight = 6,
			Phone = 7,
		}
		public enum System_CompatibilityFlag : int
		{
			SwallowUnhandledExceptions = 0,
			NullReferenceExceptionOnAV = 1,
			EagerlyGenerateRandomAsymmKeys = 2,
			FullTrustListAssembliesInGac = 3,
			DateTimeParseIgnorePunctuation = 4,
			OnlyGACDomainNeutral = 5,
			DisableReplacementCustomCulture = 6,
		}
		public enum System_AppDomain_PROFILE_API_CHECK_FLAGS : uint
		{
			PROFILE_API_CHECK_FLAGS_NONE = 0,
			PROFILE_API_CHECK_FLAGS_ALWAYS = 1,
			PROFILE_API_CHECK_FLAGS_NEVER = 2,
		}
		[Flags]
		public enum System_Guid_GuidStyles : int
		{
			NumberFormat = 0,
			None = 0,
			AllowParenthesis = 1,
			AllowBraces = 2,
			AllowDashes = 4,
			AllowHexPrefix = 8,
			Any = 15,
			RequireParenthesis = 16,
			RequireBraces = 32,
			RequireDashes = 64,
			DigitFormat = 64,
			ParenthesisFormat = 80,
			BraceFormat = 96,
			RequireHexPrefix = 128,
			HexFormat = 160,
		}
		public enum System_Guid_GuidParseThrowStyle : int
		{
			None = 0,
			All = 1,
			AllButOverflow = 2,
		}
		public enum System_Guid_ParseFailureKind : int
		{
			None = 0,
			ArgumentNull = 1,
			Format = 2,
			FormatWithParameter = 3,
			NativeException = 4,
			FormatWithInnerException = 5,
		}
		[Flags]
		public enum System_RuntimeType_DispatchWrapperType : int
		{
			Unknown = 1,
			Dispatch = 2,
			Record = 4,
			Error = 8,
			Currency = 16,
			BStr = 32,
			SafeArray = 65536,
		}
		public enum System_TimeZoneInfo_TimeZoneInfoResult : int
		{
			Success = 0,
			TimeZoneNotFoundException = 1,
			InvalidTimeZoneException = 2,
			SecurityException = 3,
		}
		public enum System_TimeZoneInfo_StringSerializer_State : int
		{
			Escaped = 0,
			NotEscaped = 1,
			StartOfToken = 2,
			EndOfLine = 3,
		}
		public enum System_Globalization_TimeSpanParse_TimeSpanThrowStyle : int
		{
			None = 0,
			All = 1,
		}
		public enum System_Globalization_TimeSpanParse_ParseFailureKind : int
		{
			None = 0,
			ArgumentNull = 1,
			Format = 2,
			FormatWithParameter = 3,
			Overflow = 4,
		}
		[Flags]
		public enum System_Globalization_TimeSpanParse_TimeSpanStandardStyles : int
		{
			None = 0,
			Invariant = 1,
			Localized = 2,
			Any = 3,
			RequireFull = 4,
		}
		public enum Microsoft_Win32_RegistryKey_RegistryInternalCheck : int
		{
			CheckSubKeyWritePermission = 0,
			CheckSubKeyReadPermission = 1,
			CheckSubKeyCreatePermission = 2,
			CheckSubTreeReadPermission = 3,
			CheckSubTreeWritePermission = 4,
			CheckSubTreeReadWritePermission = 5,
			CheckValueWritePermission = 6,
			CheckValueCreatePermission = 7,
			CheckValueReadPermission = 8,
			CheckKeyReadPermission = 9,
			CheckSubTreePermission = 10,
			CheckOpenSubKeyWithWritablePermission = 11,
			CheckOpenSubKeyPermission = 12,
		}
		public enum System_Security_Policy_Evidence_EvidenceLockHolder_LockType : int
		{
			Reader = 0,
			Writer = 1,
		}
		[Flags]
		public enum System_Security_Policy_Evidence_EvidenceEnumerator_Category : int
		{
			Host = 1,
			Assembly = 2,
		}
		public enum System_Security_AccessControl_CommonAcl_ComparisonResult : int
		{
			LessThan = 0,
			EqualTo = 1,
			GreaterThan = 2,
		}
	}
}
