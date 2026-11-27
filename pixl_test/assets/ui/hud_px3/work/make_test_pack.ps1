# 인게임 테스트 묶음: 성장 창 시트에서 부품 8개를 잘라 늘리는 부품의 가운데를 균일하게 만든다
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$dir = Split-Path $PSScriptRoot -Parent
$pack = "$dir\test_pack_v1"; New-Item -ItemType Directory -Force "$pack\parts" | Out-Null

function Get-Piece($sheet, $bx, $by, $bw, $bh) {
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
  $piece = New-Object System.Drawing.Bitmap ($maxX - $minX + 1), ($maxY - $minY + 1)
  for ($y = $minY; $y -le $maxY; $y++) { for ($x = $minX; $x -le $maxX; $x++) { if ($id[$x, $y] -eq $best) { $piece.SetPixel($x - $minX, $y - $minY, $sheet.GetPixel($bx + $x, $by + $y)) } } }
  return $piece
}

# 가운데 구간을 줄마다 '가장 많은 색'으로 채워 모든 열(행)을 똑같게 만든다
function Set-UniformCols($img, $edgeL, $edgeR) {
  for ($y = 0; $y -lt $img.Height; $y++) {
    $cnt = @{}; for ($x = $edgeL; $x -lt $img.Width - $edgeR; $x++) { $k = $img.GetPixel($x, $y).ToArgb(); $cnt[$k] = 1 + [int]$cnt[$k] }
    $mode = [System.Drawing.Color]::FromArgb([int](($cnt.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 1).Key))
    for ($x = $edgeL; $x -lt $img.Width - $edgeR; $x++) { $img.SetPixel($x, $y, $mode) }
  }
}
function Set-UniformRows($img, $edgeT, $edgeB) {
  for ($x = 0; $x -lt $img.Width; $x++) {
    $cnt = @{}; for ($y = $edgeT; $y -lt $img.Height - $edgeB; $y++) { $k = $img.GetPixel($x, $y).ToArgb(); $cnt[$k] = 1 + [int]$cnt[$k] }
    $mode = [System.Drawing.Color]::FromArgb([int](($cnt.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 1).Key))
    for ($y = $edgeT; $y -lt $img.Height - $edgeB; $y++) { $img.SetPixel($x, $y, $mode) }
  }
}

$src = New-Object System.Drawing.Bitmap "$dir\source\ref_growth_window_sheet.png"
$sheet = New-Object System.Drawing.Bitmap $src; $src.Dispose()
# 이름, 시트 상자(x,y,w,h), 9-slice 테두리(L,R,T,B) / 0,0,0,0 = 늘리지 않음, 세로로도 늘리는지
$defs = @(
  @('panel_window', 0, 0, 173, 173, 24, 24, 24, 24, $true),
  @('tab_on', 341, 9, 105, 48, 14, 14, 0, 0, $false),
  @('tab_off', 236, 9, 104, 48, 14, 14, 0, 0, $false),
  @('row_stat', 5, 173, 339, 74, 18, 18, 14, 14, $true),
  @('btn_upgrade', 5, 255, 147, 74, 16, 16, 14, 16, $true),
  @('btn_close', 173, 13, 56, 58, 0, 0, 0, 0, $false),
  @('icon_stat_attack', 5, 423, 72, 72, 0, 0, 0, 0, $false),
  @('icon_stat_hp', 79, 423, 72, 72, 0, 0, 0, 0, $false))
foreach ($d in $defs) {
  $piece = Get-Piece $sheet $d[1] $d[2] $d[3] $d[4]
  if ($d[5] -gt 0) { Set-UniformCols $piece $d[5] $d[6] }
  if ($d[9]) { Set-UniformRows $piece $d[7] $d[8] }
  $semi = 0; for ($y = 0; $y -lt $piece.Height; $y++) { for ($x = 0; $x -lt $piece.Width; $x++) { $a = $piece.GetPixel($x, $y).A; if ($a -gt 0 -and $a -lt 255) { $semi++ } } }
  $piece.Save("$pack\parts\$($d[0]).png")
  '{0,-18} {1,3}x{2,-3}  border L{3} R{4} T{5} B{6}  semi-alpha px {7}' -f $d[0], $piece.Width, $piece.Height, $d[5], $d[6], $d[7], $d[8], $semi
  $piece.Dispose()
}
