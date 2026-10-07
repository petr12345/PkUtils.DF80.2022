# PkNetUtils JSON serialization refactor

The purpose of this refactoring is to remove the dependency on obsolete BinaryFormatter-based object serialization and replace it with explicit JSON contracts. The changes cover persistent application settings, shared-memory object transfer, deep cloning, affected sample applications, and serialized WinForms resources. Stable type discriminators and versioned payloads make supported data types and protocol compatibility explicit, while preserving the existing application-facing APIs where practical.

The refactoring also updates XML documentation and regression tests, removes unsafe formatter settings and CLS-compliance declarations, and retains the original project structure and target frameworks. It deliberately leaves unrelated source files and the native shared-memory mapping and synchronization implementation unchanged.

## Changes

- **ApplicationStorage<T>** now writes JSON dictionaries to `.json` isolated-storage files. Normal load, safe load, settings-name suffixes, `LastSafeLoadResult`, and `LastLoadFoundAnyStorage` remain available. Serialization completes before opening the output file, so a rejected JSON contract does not truncate an existing valid store. Isolated-storage handles are disposed.
- **ApplicationObjStorage** uses the CCA-style explicit registry and `$type`/`$value` converter. Defaults are Int32, Point, Size and Rectangle. Register custom concrete JSON-compatible types before constructing a loading store or saving values. Registration is process-wide and thread-safe. Arbitrary CLR type names in JSON are never resolved. Open types and System.Object registrations are rejected.
- **NativeMemory.Segment** keeps the object constructors, `SetData`, and `GetData`, and transfers versioned UTF-8 JSON. It uses its own registry, the shared registered-object converter, and a protocol envelope: `$format = "PK.PkUtils.SharedMemory.Json"`, `$version = 1`, `$data = { "$type": ..., "$value": ... }`. Register the same discriminator for compatible payload types in both processes. Serializer/protocol failures are wrapped in SharedMemoryException with the original cause. Protected stream encode/decode helpers permit protocol testing independently of Win32 transport. BaseSegment and its native mapping/mutex implementation are byte-for-byte unchanged.
- **Cloning** has a new CloneHelperJson. Existing CloneHelperBinary and MakeCloneableBinary names are retained as source-compatible JSON APIs, with updated XML comments. Mutable graph cycles and repeated references use ReferenceHandler.Preserve. The source object supplies the root runtime type; nested polymorphism needs JSON attributes or explicit serializer options. BSID and immutable cloning-test types have explicit JSON constructors. Private-field-only contracts and formatter callbacks are not copied automatically.
- **Clipboard images** use a dedicated JSON converter with format GUIDs and PNG bytes. Decoded images are copied before the decoding stream is disposed. The sample no longer implements formatter callbacks.
- **Tagging editor** replaces the binary file option with a versioned JSON DTO containing logical text and field positions. Its FieldTypeId converter uses an explicit stable allow-list. File-dialog order and enum numeric values are preserved. LogInfo equality compares field identifier and position rather than serialized bytes. The existing XML option is retained.
- **WinForms resources** no longer contain binary-serialized ImageListStreamer or empty HashSet objects. Existing bitmap pixels/masks were decoded into plain PNG byte resources, preserving image order, keys, size, and transparency. Empty selections are initialized in code. No generated/replacement artwork was used.
- **Project settings** no longer enable unsafe formatter serialization, suppress resource formatter warnings, or reference the System.Runtime.Serialization.Formatters compatibility package.
- **CLS declarations** were removed throughout the supplied PkNetUtils sources as requested. The sole remaining declaration was `[TestFixture, CLSCompliant(false)]`, which is now `[TestFixture]`. Final builds use no CLS-warning suppression or downgrade.
- **XML comments and tests** are updated for JSON behavior and compatibility. JsonMigrationTests covers registered object persistence, geometry, null, Unicode, unsupported types, failed-save preservation, registry conflicts, cyclic cloning, protocol validation, insufficient native capacity, and unregistered native payloads. Existing PersonData fixtures preserve explicitly modified array data rather than depending on constructor-generated defaults.

## Compatibility

- Old `.dtt` files are untouched and are not automatically loaded or migrated. No BinaryFormatter fallback or legacy formatter implementation is included. Form layout stored only in an old binary file will start with defaults until saved in JSON.
- Old shared-memory participants and the new JSON participants are not wire-compatible; upgrade both processes together. Shared-memory payload types need registration in each process.
- Old `.csb` / `.lsb` tagging files are not loaded by the new JSON option. Existing XML and plain-text options remain available.
- CloneHelperBinary.ToByteArray now produces UTF-8 JSON, not legacy formatter bytes. External callers that relied on private fields, ISerializable callbacks, arbitrary nested runtime types, or formatter byte identity need explicit JSON contracts/converters or dedicated copy methods.
- Serializable attributes, obsolete exception-serialization members, XML/DataContract serializers, BinaryReader/BinaryWriter and raw struct/byte transport are not equivalent to invoking BinaryFormatter. Such unrelated compatibility declarations and transport code were preserved when no change was necessary.

## Validation

Using .NET SDK 10.0.100 on Linux:

- Both PkUtils library target variants build: net8.0-windows and net10.0-windows.
- The following nine project groups build successfully in **both** DF80.2022 and DF10.2026 variants (18 successful build commands): PkUtils.NUnitTests, PkUtils.UnitTests, TestCloning, TestSharedMemClient, TestSharedMemServer, TestTgSchema (including SubstEditLib), TestMultiSelectTreeview, TestMultiSelectOwnerDraw, and PK.Commands. Windows-targeting reference packs were enabled; builds were serialized with BuildInParallel=false.
- Final builds preserve the projects' TreatWarningsAsErrors configuration. The existing missing AllRules.ruleset produces an MSBuild warning; no source changes were made solely for that warning.
- Runtime validation against the built .NET 10 assemblies passed for cyclic/aliased Unicode cloning, immutable BSID, derived root types, object-storage runtime types, registration conflicts and rejection of System.Object. Twelve managed JsonMigrationTests cases and two existing immutable/derived NUnit cloning tests passed by direct invocation from a temporary console harness. These were not a complete test-runner execution of the full suites.
- The actual Segment JSON encode/decode implementation passed round trips and rejected malformed JSON, unknown protocol/version and unknown discriminators independently of Win32 transport.
- Tagging JSON field discriminators, rejection of unknown field types, DTO read/write and semantic equality passed runtime checks against the built sample assemblies.
- All 80 project XML files parse successfully. Every resx data entry was inspected: zero binary-serialized object entries remain. No active BinaryFormatter references, compatibility package references, unsafe formatter switches, or CLSCompliant declarations remain in source/project files.

**Windows execution still required:** full NUnit/MSTest suites, native mapping/attachment/mutex and cross-process tests, WinForms UI/clipboard behavior and designer rendering. These were compiled but cannot be fully executed on this Linux host. On Windows, build the desired project/solution normally, then run for example:

```powershell
dotnet test PkUtils.NUnitTests\PkUtils_NUnitTests.DF10.2026.csproj -c Release --filter "FullyQualifiedName~JsonMigrationTests|FullyQualifiedName~SegmentTests|FullyQualifiedName~MakeCloneableBinaryTest|FullyQualifiedName~BSIDTests"
```

Start the shared-memory server sample and then its client sample to validate the coordinated JSON cross-process contract. Run the cloning and tagging-editor samples to inspect image/clipboard and UI behavior.

## Preservation

All 1140 original files are present. 78 original files were modified; 1062 original files match the uploaded archive byte for byte. 8 source files were added, plus these notes and ChangedFiles.csv. The manifest lists changed/new source and project paths with SHA-256 hashes. New build bin/obj output is excluded from the deliverable. Existing unrelated documentation/assets are retained unchanged and may therefore still discuss their original implementations.


## Architecture documentation

Open [JsonSerializationRefactor_Architecture.html](JsonSerializationRefactor_Architecture.html) for an offline architecture guide with embedded UML diagrams, JSON payload examples, registration rules, and compatibility notes. The guide also documents image/clipboard and resource adapters.

| Diagram | PNG | Editable SVG |
|---|---|---|
| Application storage | [PNG](ApplicationStorage_UML.png) | [SVG](ApplicationStorage_UML.svg) |
| Shared JSON registry and converters | [PNG](JsonSerialization_UML.png) | [SVG](JsonSerialization_UML.svg) |
| Shared-memory Segment | [PNG](NativeMemorySegment_UML.png) | [SVG](NativeMemorySegment_UML.svg) |
| Cloning and compatibility APIs | [PNG](Cloning_UML.png) | [SVG](Cloning_UML.svg) |
| Tagging schema contracts | [PNG](TaggingSchema_UML.png) | [SVG](TaggingSchema_UML.svg) |

The diagrams describe the implemented architecture, not a future proposal. Selected members are shown with their actual visibility. Solid hollow-triangle arrows denote inheritance; dashed hollow-triangle arrows denote interface realization; solid open arrows denote navigable references; dashed open arrows denote use dependencies. Shared registries are references, not exclusive-lifetime compositions.
