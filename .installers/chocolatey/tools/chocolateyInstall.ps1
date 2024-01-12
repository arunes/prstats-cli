$packageName= 'prstats-cli'
$toolsDir   = "$(Split-Path -Parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/arunes/prstats-cli/releases/download/v0.1/prstats-cli-win-x86-v0.1.0.0.msi'
$url64      = 'https://github.com/arunes/prstats-cli/releases/download/v0.1/prstats-cli-win-x64-v0.1.0.0.msi'

$packageArgs = @{
  packageName   = $packageName
  fileType      = 'msi'
  url           = $url
  url64bit      = $url64
  silentArgs    = "/qn /norestart"
  validExitCodes= @(0, 3010, 1641)
  softwareName  = 'prstats-cli*'
  checksum      = '9A70F2998DCE08AA24613827F3395F08DE6928803584A06C9D209546CAA69397'
  checksumType  = 'sha256'
  checksum64    = '98CF6586B17FBCAD9A1367CC7ED3F4F72E93C185D77CAC3D38B2E4B454DFA002'
  checksumType64= 'sha256'
}

Install-ChocolateyPackage @packageArgs