$packageName= 'prstats-cli'
$toolsDir   = "$(Split-Path -Parent $MyInvocation.MyCommand.Definition)"
$url        = 'tools/prstats-cli-win-x86-v0.1.0.0.msi'
$url64      = 'tools/prstats-cli-win-x64-v0.1.0.0.msi'

$packageArgs = @{
  packageName   = $packageName
  fileType      = 'msi'
  url           = $url
  url64bit      = $url64
  silentArgs    = "/qn /norestart"
  validExitCodes= @(0, 3010, 1641)
  softwareName  = 'prstats-cli*'
  checksum      = '5AA26246FDF559F6CE0133578CC2424556E0B054B32CE14017540524AF30DA3C'
  checksumType  = 'sha256'
  checksum64    = '0BD9BEBFFBC9A61C78EF91EE051C5E5AB9A364772E95F64D73DA7E04668278D4'
  checksumType64= 'sha256'
}

Install-ChocolateyPackage @packageArgs