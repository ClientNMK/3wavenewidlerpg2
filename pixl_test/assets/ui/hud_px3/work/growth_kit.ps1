# 성장(강화) 창 시트 → 낱개 부품. 늘리는 부품은 가운데를 한 열(행)로 통일하고 9-slice를 잰다
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$dir = Split-Path $PSScriptRoot -Parent
$out = "$dir\parts_growth"; New-Item -ItemType Directory -Force $out | Out-Null
$sheet = New-Object System.Drawing.Bitmap "$dir\source\ref_growth_window_sheet.png"

# 상자 안에서 가장 큰 픽셀 덩어리만 잘라냄
function Get-Piece($bx, $by, $bw, $bh) {
  $bw = [math]::Min($bw, $sheet.Width - $bx); $bh = [math]::Min($bh, $sheet.Height - $by)
  $id = New-Object 'int[,]' $bw, $bh; $best = 0; $bestN = 0; $cur = 0
  for ($y0 = 0; $y0 -lt $bh; $y0++) { for ($x0 = 0; $x0 -lt $bw; $x0++) {
    if ($id[$x0, $y0] -ne 0 -or $sheet.GetPixel($bx + $x0, $by + $y0).A -eq 0) { continue }
    $cur++; $n = 0; $q = New-Object System.Collections.Generic.Queue[int]; $q.Enqueue($y0 * $bw + $x0); $id[$x0, $y0] = $cur
    while ($q.Count) {
      $v = $q.Dequeue(); $px = $v % $bw; $py = [int][math]::Floor($v / $bw); $n++
      for ($dy = -1; $dy -le 1; $dy++) { for ($dx = -1; $dx -le 1; $dx++) {
        $nx = $px + $dx; $ny = $py + $dy
        if ($nx -lt 0 -or $ny -lt 0 -or $nx -ge $bw -or $ny -ge $bh) { continue }
        if ($id[$nx, $ny] -ne 0 -or $sheet.GetPixel($bx + $nx, $by + $ny).A -eq 0) { continue }
        $id[$nx, $ny] = $cur; $q.Enqueue($ny * $bw + $nx)
      } }
    }
    if ($n -gt $bestN) { $bestN = $n; $best = $cur }
  } }
  $minX = $bw; $maxX = -1; $minY = $bh; $maxY = -1
  for ($y = 0; $y -lt $bh; $y++) { for ($x = 0; $x -lt $bw; $x++) { if ($id[$x, $y] -eq $best) {
    if ($x -lt $minX) { $minX = $x }; if ($x -gt $maxX) { $maxX = $x }; if ($y -lt $minY) { $minY = $y }; if ($y -gt $maxY) { $maxY = $y } } } }
  $p = New-Object System.Drawing.Bitmap ($maxX - $minX + 1), ($maxY - $minY + 1)
  for ($y = $minY; $y -le $maxY; $y++) { for ($x = $minX; $x -le $maxX; $x++) { if ($id[$x, $y] -eq $best) { $p.SetPixel($x - $minX, $y - $minY, $sheet.GetPixel($bx + $x, $by + $y)) } } }
  return $p
}
# 가운데 구간을 가장 많이 나오는 열(행)로 덮음
function Set-Flat($p, $a, $b, $byCol) {
  $len = if ($byCol) { $p.Height } else { $p.Width }
  $cnt = @{}
  for ($i = $a; $i -le $b; $i++) { $k = (0..($len - 1) | ForEach-Object { if ($byCol) { $p.GetPixel($i, $_).ToArgb() } else { $p.GetPixel($_, $i).ToArgb() } }) -join ','; $cnt[$k] += 1 }
  $best = (($cnt.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 1).Key -split ',')
  for ($i = $a; $i -le $b; $i++) { for ($j = 0; $j -lt $len; $j++) { $c = [System.Drawing.Color]::FromArgb([int]$best[$j]); if ($byCol) { $p.SetPixel($i, $j, $c) } else { $p.SetPixel($j, $i, $c) } } }
}
# 이름, 상자, 9-slice(L R T B, 늘리지 않으면 $null), 좌우 반전
$list = @(
  @('panel_window', 0, 0, 173, 173, @(24, 24, 24, 24)),
  @('btn_close', 173, 13, 56, 58, $null),
  @('tab_off', 236, 9, 104, 48, @(14, 14, 0, 0)),
  @('tab_on', 341, 9, 105, 48, @(14, 14, 0, 0)),
  @('seg_on', 171, 85, 134, 46, @(52, 16, 0, 0), $true),
  @('seg_off', 307, 85, 134, 46, @(52, 16, 0, 0)),
  @('list_row', 5, 173, 339, 74, @(18, 18, 0, 0)),
  @('slot_icon', 345, 170, 80, 80, @(10, 10, 10, 10)),
  @('btn_upgrade', 5, 255, 147, 74, @(16, 16, 0, 0)),
  @('icon_coin', 160, 260, 50, 50, $null),
  @('plate_dark', 4, 336, 441, 81, @(20, 20, 0, 0)),
  @('icon_atk', 5, 423, 72, 72, $null), @('icon_hp', 79, 423, 72, 72, $null),
  @('icon_def', 153, 423, 72, 72, $null), @('icon_armor', 227, 423, 72, 72, $null),
  @('plate_title', 5, 510, 304, 63, @(60, 60, 0, 0))
)
foreach ($e in $list) {
  $p = Get-Piece $e[1] $e[2] $e[3] $e[4]
  if ($e[6]) { $p.RotateFlip('RotateNoneFlipX') }
  $s = $e[5]
  if ($s) {
    Set-Flat $p $s[0] ($p.Width - 1 - $s[1]) $true
    if ($s[2] -gt 0) { Set-Flat $p $s[2] ($p.Height - 1 - $s[3]) $false }
  }
  $p.Save("$out\$($e[0]).png")
  $nine = if ($s) { 'L {0} R {1} T {2} B {3}' -f $s[0], $s[1], $s[2], $s[3] } else { '-' }
  '{0,-14} {1,3}x{2,-3}  {3}' -f $e[0], $p.Width, $p.Height, $nine
}
