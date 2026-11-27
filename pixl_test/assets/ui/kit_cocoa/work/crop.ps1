# sheet.png(투명 배경, 3배)를 manifest.json대로 잘라 ../parts 에 저장하고 투명도를 검사한다
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$out = Join-Path (Split-Path $PSScriptRoot -Parent) 'parts'; New-Item -ItemType Directory -Force $out | Out-Null
$m = Get-Content "$PSScriptRoot\manifest.json" -Raw -Encoding UTF8 | ConvertFrom-Json
$sheet = New-Object System.Drawing.Bitmap "$PSScriptRoot\sheet.png"
"sheet $($sheet.Width)x$($sheet.Height)  corner alpha $($sheet.GetPixel(0, 0).A)"
foreach ($p in $m.parts) {
  $part = $sheet.Clone((New-Object System.Drawing.Rectangle $p.x, $p.y, $p.w, $p.h), [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $part.Save("$out\$($p.name).png")
  $c = $part.GetPixel([int]($p.w / 2), [int]($p.h / 2)); $k = $part.GetPixel(0, 0)
  '{0,-16} {1,3}x{2,-3} center a={3,3} rgb={4:X2}{5:X2}{6:X2}  corner a={7}' -f $p.name, $p.w, $p.h, $c.A, $c.R, $c.G, $c.B, $k.A
  $part.Dispose()
}
$sheet.Dispose()
