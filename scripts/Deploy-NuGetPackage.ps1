param
(
    [String]
    $Version
)

if (Test-Path('.\src\ProxyMe\ProxyMe.nuspec'))
{
    nuget pack .\src\ProxyMe\ProxyMe.nuspec -OutputDirectory .\ -BasePath .\src\ProxyMe -Version $Version -Properties Configuration=Release
}
else
{
    Write-Error 'Missing nuspec file'
}
