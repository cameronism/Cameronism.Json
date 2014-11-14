[CmdletBinding()]
Param(
	[Parameter(Mandatory=$True,Position=1)]
	[string]$version
)

pushd "$PSScriptRoot\Cameronism.Json"

. "C:\Users\cjordan\Downloads\NuGet.exe" pack Cameronism.Json.csproj -Build -Prop Configuration=Release -Version $version
. "C:\Users\cjordan\Downloads\NuGet.exe" push "Cameronism.Json.$version.nupkg"

popd
