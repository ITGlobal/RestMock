$SOLUTION_DIR = Resolve-Path "."
$ARTIFACTS = Join-Path $SOLUTION_DIR "artifacts"
$CONFIGURATION = "Release"
$VERSION = "0.0.0-dev"

# INIT
Write-Host ".NET SDK version: $(dotnet --version)"
if(-not (Test-Path $ARTIFACTS)) {
    New-Item $ARTIFACTS -ItemType Directory | Out-Null
}

# CLEAN
& dotnet clean -v q /nologo
if($LASTEXITCODE -ne 0) {
    Write-Host "'dotnet clean' failed with $LASTEXITCODE" -f red
    exit $LASTEXITCODE
}

Get-ChildItem $ARTIFACTS | Remove-Item -Recurse -Force

# VERSION
$version = $(git tag --list v*) | Select-Object -Last 1
if ([string]::IsNullOrWhiteSpace($version)) {
    Write-Host "No valid git version tag found, falling back to v0.0"
    $version = "v0.0"
}

$match = [regex]::Match($version, "^v(|\.)([0-9]+)\.([0-9]+)(|\.[0-9]+)$")
if ($match.Success) {
    $VER_MAJOR = [int]::Parse($match.Groups[2].Value)
    $VER_MINOR = [int]::Parse($match.Groups[3].Value)
}
else {
    Write-Host "Git version tag is malformed: `"$version`". Expected a `"v[0-9].[0-9]`" value" -f Red
    Wrire-Host "Will use fallback value v0.0"
    $VER_MAJOR = 0
    $VER_MINOR = 0
}

$VER_SUFFIX = ""
if ($env:APPVEYOR) {
    $VER_BUILD = [int]::Parse($env:APPVEYOR_BUILD_NUMBER)
    if (-not [string]::IsNullOrEmpty($env:APPVEYOR_PULL_REQUEST_NUMBER)) {
        Write-Host "It's a " -n
        Write-Host "PullRequest #$env:APPVEYOR_PULL_REQUEST_NUMBER" -n -f Green
        Write-Host " build"
        $VER_SUFFIX = "-pullrequest$env:APPVEYOR_PULL_REQUEST_NUMBER"
    }
    else {
        if ($env:APPVEYOR_REPO_BRANCH -ne "master") {
            Write-Host "It's a " -n
            Write-Host "branch $env:APPVEYOR_REPO_BRANCH" -n -f Green
            Write-Host " build"
            $safeBranchName = [regex]::Replace($env:APPVEYOR_REPO_BRANCH, "[^a-zA-Z0-9]", "")
            $VER_SUFFIX = "-$$safeBranchName"
        }
    }
}
else {
    Write-Host "Build is not running on AppVeyor, will use default build number 0"
    $VER_BUILD = 0
}

$VERSION = "$VER_MAJOR.$VER_MINOR.$VER_BUILD$VER_SUFFIX"
Write-Host "Version number: " -n
Write-Host "$VERSION" -f yellow

if ($env:APPVEYOR) { 
    appveyor UpdateBuild -Version "$VERSION"
}

# RESTORE
& dotnet restore -v q /nologo
if($LASTEXITCODE -ne 0) {
    Write-Host "'dotnet restore' failed with $LASTEXITCODE" -f red
    exit $LASTEXITCODE
}

# COMPILE
& dotnet build -v q -c $CONFIGURATION /nologo /p:Version=$VERSION
if($LASTEXITCODE -ne 0) {
    Write-Host "'dotnet restore' failed with $LASTEXITCODE" -f red
    exit $LASTEXITCODE
}

# PACKAGE
& dotnet pack /nologo -v q -c $CONFIGURATION /p:Version=$VERSION --include-symbols --include-source --output $ARTIFACTS ./src/RestMock/RestMock.csproj
if ($LASTEXITCODE -ne 0) {
    Write-Host "`"dotnet pack`" failed with $LASTEXITCODE"
    exit $LASTEXITCODE
}


# TEST
$testXml = Join-Path $ARTIFACTS "tests.xml"
if(Test-Path $testXml) {
    Remove-Item $testXml
}

pushd ./tests
& dotnet xunit -nologo -configuration $CONFIGURATION -nobuild -xml $testXml
if ($LASTEXITCODE -ne 0) {
    Write-Host "`"dotnet xunit`" failed with $LASTEXITCODE"
    popd
    exit $LASTEXITCODE
}
popd

Write-Host "Completed" -f Green
