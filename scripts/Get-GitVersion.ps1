# Extract version from git tag

if ((git describe --tags --exact-match 2> $null) -match "v?(\d+\.\d+\.\d+)")
{
    $matches[1]
}
else
{
    # Get version information from the last tag in history

    $version = if ((git describe --tags 2> $null) -match "v?(\d+\.\d+\.\d+)-(\d+)")
    {
        $commitCount = $matches[2]
        $matches[1]
    }
    else
    {
        # No tag exists
        $commitCount = (git rev-list --all --count)
        '0.0.0'
    }

    $pre = $env:APPVEYOR_REPO_BRANCH

    if ($pre -eq $null)
    {
        # Get branch information from current commit
        $pre = git rev-parse --abbrev-ref HEAD
    }

    $pre = $pre  -replace '^feature/','' -replace '/',''

    "$version-$pre.$commitCount"
}
