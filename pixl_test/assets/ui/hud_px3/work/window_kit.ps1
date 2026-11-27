# 창 부품 시트(PixelLab 2배) → 1배 낱개. list_row(parts_w)와 같은 굵기. 늘리는 구간은 한 열(행)로 통일
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$dir = Split-Path $PSScriptRoot -Parent
$out = "$dir\parts_w"
$src = New-Object System.Drawing.Bitmap "$dir\source\window_kit_full.png"

function Get-Half($bx, $by, $bw, $bh) {
  $p = New-Object System.Drawing.Bitmap ([int]($bw / 2)), ([int]($bh / 2))
  for ($y = 0; $y -lt $p.Height; $y++) { for ($x = 0; $x -lt $p.Width; $x++) {
    $v = @(); foreach ($o in @(@(0,0),@(1,0),@(0,1),@(1,1))) { $v += $src.GetPixel($bx + 2*$x + $o[0], $by + 2*$y + $o[1]).ToArgb() }
    $p.SetPixel($x, $y, [System.Drawing.Color]::FromArgb([int](($v | Group-Object | Sort-Object Count -Descending | Select-Object -First 1).Name)))
  } }
  # 투명 여백 잘라냄
  $minX = $p.Width; $maxX = -1; $minY = $p.Height; $maxY = -1
  for ($y = 0; $y -lt $p.Height; $y++) { for ($x = 0; $x -lt $p.Width; $x++) { if ($p.GetPixel($x, $y).A -ne 0) {
    if ($x -lt $minX) { $minX = $x }; if ($x -gt $maxX) { $maxX = $x }; if ($y -lt $minY) { $minY = $y }; if ($y -gt $maxY) { $maxY = $y } } } }
  return $p.Clone((New-Object System.Drawing.Rectangle $minX, $minY, ($maxX - $minX + 1), ($maxY - $minY + 1)), $p.PixelFormat)
}
function Set-Flat($p, $a, $b, $byCol) {
  $len = if ($byCol) { $p.Height } else { $p.Width }
  $cnt = @{}
  for ($i = $a; $i -le $b; $i++) { $k = (0..($len - 1) | ForEach-Object { if ($byCol) { $p.GetPixel($i, $_).ToArgb() } else { $p.GetPixel($_, $i).ToArgb() } }) -join ','; $cnt[$k] += 1 }
  $best = (($cnt.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 1).Key -split ',')
  for ($i = $a; $i -le $b; $i++) { for ($j = 0; $j -lt $len; $j++) { $c = [System.Drawing.Color]::FromArgb([int]$best[$j]); if ($byCol) { $p.SetPixel($i, $j, $c) } else { $p.SetPixel($j, $i, $c) } } }
}
# 이름, 시트 상자(2배 좌표), 9-slice(L R T B)
$list = @(
  @('panel_window', 0, 0, 112, 112, @(6, 6, 6, 6)),
  @('btn_close', 110, 11, 42, 42, $null),
  @('tab_on', 155, 7, 90, 42, @(5, 5, 0, 0)),
  @('tab_off', 155, 51, 90, 42, @(5, 5, 0, 0)),
  @('btn_upgrade', 3, 119, 98, 50, @(5, 5, 0, 0)),
  @('seg_on', 103, 119, 90, 38, @(5, 15, 0, 0)),
  @('seg_off', 103, 159, 90, 38, @(15, 5, 0, 0)),
  @('slot_icon', 195, 119, 50, 50, @(4, 4, 4, 4))
)
foreach ($e in $list) {
  $p = Get-Half $e[1] $e[2] $e[3] $e[4]; $s = $e[5]
  if ($s) {
    Set-Flat $p $s[0] ($p.Width - 1 - $s[1]) $true
    if ($s[2] -gt 0) { Set-Flat $p $s[2] ($p.Height - 1 - $s[3]) $false }
  }
  $p.Save("$out\$($e[0]).png")
  $nine = if ($s) { 'L {0} R {1} T {2} B {3}' -f $s[0], $s[1], $s[2], $s[3] } else { '-' }
  '{0,-14} {1,3}x{2,-3}  {3}' -f $e[0], $p.Width, $p.Height, $nine
}
