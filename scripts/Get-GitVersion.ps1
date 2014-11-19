param
(
    $BuildNumber
)

# Extract version from git tag

if ((git describe --tags --exact-match 2> $null) -match "v(\d+)\.(\d+)\.(\d+)(?:-([^-]+))?(.*)")
{
    $major = $matches[1]
    $minor = $matches[2]
    $patch = $matches[3]
    $pre = $matches[4]
    $tag = $matches[0]
}
else
{
    $pre = $null
    $tag = $null

    # Get version information from the last tag in history

    if ((git describe --tags 2> $null) -match "v(\d+)\.(\d+)\.(\d+)")
    {
        $major = $matches[1]
        $minor = $matches[2]
        $patch = $matches[3]
    }
    else
    {
        # No tag exists
        $major = 0
        $minor = 0
        $patch = 0
    }
}


$infoVersion = "$major.$minor.$patch"
$fileVersion = "$infoVersion.$BuildNumber"

if ($pre)
{
    $infoVersion = "$infoVersion-$pre"
}

if ($tag)
{
    $buildVersion = "$tag $BuildNumber"
}
else
{
    $buildVersion = $fileVersion
}


[PSCustomObject]@{
    Major = $major;
    Minor = $minor;
    Patch = $patch;
    Tag = $tag;
    Info = $infoVersion;
    File = $fileVersion;
    Assembly = $fileVersion;
    Build = $buildVersion;
}
