# B 시트(절반본)를 부품별 PNG로 분해: 덩어리 찾기 -> 동전/판, 아이콘/뱃지 분리 -> 외곽선 1px 통일 -> 9-slice 측정
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$dir = Split-Path $PSScriptRoot -Parent
$out = "$dir\parts_caseB"; New-Item -ItemType Directory -Force $out | Out-Null
$sheet = New-Object System.Drawing.Bitmap "$dir\source\hud_caseB_other_half.png"
$SW = $sheet.Width; $SH = $sheet.Height
$K = [System.Drawing.ColorTranslator]::FromHtml('#2A1A14')
$CREAM = [System.Drawing.ColorTranslator]::FromHtml('#F8E0C8')
$INSET = [System.Drawing.ColorTranslator]::FromHtml('#402820')
$CLEAR = [System.Drawing.Color]::FromArgb(0, 0, 0, 0)
function Hex($c) { '#{0:X2}{1:X2}{2:X2}' -f $c.R, $c.G, $c.B }

# 1) 붙어 있는 픽셀 덩어리(8방향)의 테두리 상자
$seen = New-Object 'bool[,]' $SW, $SH
$boxes = @()
for ($y0 = 0; $y0 -lt $SH; $y0++) { for ($x0 = 0; $x0 -lt $SW; $x0++) {
  if ($seen[$x0, $y0] -or $sheet.GetPixel($x0, $y0).A -eq 0) { continue }
  $q = New-Object System.Collections.Generic.Queue[int]; $q.Enqueue($y0 * $SW + $x0); $seen[$x0, $y0] = $true
  $minX = $x0; $maxX = $x0; $minY = $y0; $maxY = $y0; $n = 0
  while ($q.Count) {
    $v = $q.Dequeue(); $px = $v % $SW; $py = [int][math]::Floor($v / $SW); $n++
    if ($px -lt $minX) { $minX = $px }; if ($px -gt $maxX) { $maxX = $px }; if ($py -lt $minY) { $minY = $py }; if ($py -gt $maxY) { $maxY = $py }
    for ($dy = -1; $dy -le 1; $dy++) { for ($dx = -1; $dx -le 1; $dx++) {
      $nx = $px + $dx; $ny = $py + $dy
      if ($nx -lt 0 -or $ny -lt 0 -or $nx -ge $SW -or $ny -ge $SH) { continue }
      if ($seen[$nx, $ny] -or $sheet.GetPixel($nx, $ny).A -eq 0) { continue }
      $seen[$nx, $ny] = $true; $q.Enqueue($ny * $SW + $nx)
    } }
  }
  if ($n -ge 50) { $boxes += , @($minX, $minY, ($maxX - $minX + 1), ($maxY - $minY + 1)) }
} }

# 덩어리 중심이 가장 가까운 이름을 붙인다 (절반 시트 좌표)
$names = @{ plate_currency = @(41, 11); plate_stage = @(63, 39); slot = @(17, 67); slot_b = @(47, 67); badge_mail = @(77, 67); badge_rank = @(104, 67); btn_main = @(30, 100); frame_portrait = @(99, 104) }
function Get-Part($box) {
  $p = New-Object System.Drawing.Bitmap ($box[2] + 2), ($box[3] + 2)
  for ($y = 0; $y -lt $box[3]; $y++) { for ($x = 0; $x -lt $box[2]; $x++) { $p.SetPixel($x + 1, $y + 1, $sheet.GetPixel($box[0] + $x, $box[1] + $y)) } }
  return $p
}
# 가장자리의 기존 어두운 선을 걷어내고 바깥에 1px 외곽선을 새로 두른다
function Set-Outline($p) {
  $w = $p.Width; $ht = $p.Height
  function Open($x, $y) { ($x -lt 0 -or $y -lt 0 -or $x -ge $w -or $y -ge $ht -or $p.GetPixel($x, $y).A -eq 0) }
  $strip = @()
  for ($y = 0; $y -lt $ht; $y++) { for ($x = 0; $x -lt $w; $x++) {
    $c = $p.GetPixel($x, $y)
    if ($c.A -ne 0 -and (Hex $c) -eq (Hex $K) -and ((Open ($x-1) $y) -or (Open ($x+1) $y) -or (Open $x ($y-1)) -or (Open $x ($y+1)))) { $strip += , @($x, $y) }
  } }
  foreach ($s in $strip) { $p.SetPixel($s[0], $s[1], $CLEAR) }
  $add = @()
  for ($y = 0; $y -lt $ht; $y++) { for ($x = 0; $x -lt $w; $x++) {
    if ($p.GetPixel($x, $y).A -ne 0) { continue }
    if (-not ((Open ($x-1) $y) -and (Open ($x+1) $y) -and (Open $x ($y-1)) -and (Open $x ($y+1)))) { $add += , @($x, $y) }
  } }
  foreach ($s in $add) { $p.SetPixel($s[0], $s[1], $K) }
}
function Save-Trim($p, $name) {
  $minX = $p.Width; $maxX = -1; $minY = $p.Height; $maxY = -1
  for ($y = 0; $y -lt $p.Height; $y++) { for ($x = 0; $x -lt $p.Width; $x++) { if ($p.GetPixel($x, $y).A -ne 0) {
    if ($x -lt $minX) { $minX = $x }; if ($x -gt $maxX) { $maxX = $x }; if ($y -lt $minY) { $minY = $y }; if ($y -gt $maxY) { $maxY = $y } } } }
  $t = $p.Clone((New-Object System.Drawing.Rectangle $minX, $minY, ($maxX - $minX + 1), ($maxY - $minY + 1)), $p.PixelFormat)
  $t.Save("$out\$name.png")
  # 9-slice: 가운데를 지나는 '똑같은 열/행'이 이어지는 구간
  function Same-Run($count, $other, $byCol) {
    $mid = [int]($count / 2); $a = $mid; $b = $mid
    function Eq($i, $j) { for ($k = 0; $k -lt $other; $k++) { if ($byCol) { if ($t.GetPixel($i, $k).ToArgb() -ne $t.GetPixel($j, $k).ToArgb()) { return $false } } else { if ($t.GetPixel($k, $i).ToArgb() -ne $t.GetPixel($k, $j).ToArgb()) { return $false } } }; return $true }
    while ($a -gt 0 -and (Eq ($a - 1) $mid)) { $a-- }
    while ($b -lt $count - 1 -and (Eq ($b + 1) $mid)) { $b++ }
    return @($a, ($count - 1 - $b), ($b - $a + 1))
  }
  $cx = Same-Run $t.Width $t.Height $true; $cy = Same-Run $t.Height $t.Width $false
  '{0,-16} {1,3}x{2,-3}  L {3,2}  R {4,2}  (same cols {5,3})   T {6,2}  B {7,2}  (same rows {8,2})' -f $name, $t.Width, $t.Height, $cx[0], $cx[1], $cx[2], $cy[0], $cy[1], $cy[2]
  $t.Dispose()
}

foreach ($box in $boxes) {
  $cxy = @(($box[0] + $box[2] / 2), ($box[1] + $box[3] / 2))
  $name = ($names.GetEnumerator() | Sort-Object { [math]::Pow($_.Value[0] - $cxy[0], 2) + [math]::Pow($_.Value[1] - $cxy[1], 2) } | Select-Object -First 1).Key
  if ($name -eq 'slot_b') { continue }
  $p = Get-Part $box

  if ($name -eq 'plate_currency') {
    # 금색 동전만 떼어 icon_gold로, 빈 자리는 안쪽 색으로 메움
    $coin = New-Object System.Drawing.Bitmap $p.Width, $p.Height
    for ($y = 0; $y -lt $p.Height; $y++) { for ($x = 0; $x -lt 15; $x++) {
      $c = $p.GetPixel($x, $y)
      if ($c.A -ne 0 -and @('#EDB948', '#FFF1B8', '#B87828') -contains (Hex $c)) { $coin.SetPixel($x, $y, $c); $p.SetPixel($x, $y, $INSET) }
    } }
    Set-Outline $coin; Save-Trim $coin 'icon_gold'
  }
  if ($name -like 'badge_*') {
    # 안쪽 그림만 떼어 icon_*로, 뱃지는 빈 판(btn_round)으로
    $icon = New-Object System.Drawing.Bitmap $p.Width, $p.Height
    $mx = ($p.Width - 1) / 2; $my = ($p.Height - 1) / 2
    for ($y = 0; $y -lt $p.Height; $y++) { for ($x = 0; $x -lt $p.Width; $x++) {
      $c = $p.GetPixel($x, $y)
      if ($c.A -eq 0 -or ([math]::Pow($x - $mx, 2) + [math]::Pow($y - $my, 2)) -gt 72) { continue }
      if (@('#F8E0C8', '#D8C0A8') -notcontains (Hex $c)) { $icon.SetPixel($x, $y, $c); $p.SetPixel($x, $y, $CREAM) }
    } }
    Save-Trim $icon ($name -replace 'badge_', 'icon_')
    if ($name -eq 'badge_rank') { continue }
    $name = 'btn_round'
  }
  Set-Outline $p; Save-Trim $p $name
}
