# W5 list_row: PixelLab 결과(2배) → 1배로 줄이고, 가운데 늘리는 구간을 한 열로 통일
Add-Type -AssemblyName System.Drawing
$dir = Split-Path $PSScriptRoot
$src = New-Object System.Drawing.Bitmap "$dir\source\list_row_W5_full.png"
$box = @(25, 101, 206, 54)
$p = New-Object System.Drawing.Bitmap ($box[2] / 2), ($box[3] / 2)
for ($y = 0; $y -lt $p.Height; $y++) { for ($x = 0; $x -lt $p.Width; $x++) {
  $v = @(); foreach ($o in @(@(0,0),@(1,0),@(0,1),@(1,1))) { $v += $src.GetPixel($box[0] + 2*$x + $o[0], $box[1] + 2*$y + $o[1]).ToArgb() }
  $m = ($v | Group-Object | Sort-Object Count -Descending | Select-Object -First 1).Name
  $p.SetPixel($x, $y, [System.Drawing.Color]::FromArgb([int]$m))
} }
$src.Dispose()
if ($args[0] -eq 'dump') {
  $map = @{}; $keys = '.abcdefghijklmnopqrstuvwxyz'
  for ($y = 0; $y -lt $p.Height; $y++) { $line = ''; for ($x = 0; $x -lt $p.Width; $x++) {
    $c = $p.GetPixel($x, $y); $h = if ($c.A -eq 0) { 'none' } else { '#{0:X2}{1:X2}{2:X2}' -f $c.R, $c.G, $c.B }
    if (-not $map.ContainsKey($h)) { $map[$h] = $keys[$map.Count] }; $line += $map[$h] } ; $line }
  $map.GetEnumerator() | ForEach-Object { "$($_.Value) $($_.Key)" }; exit
}
# 늘리는 구간(x 26~96): 가장 많이 나오는 열 하나로 통일 (나뭇결·얼룩 제거)
$cols = @{}
for ($x = 26; $x -le 96; $x++) { $k = (0..($p.Height - 1) | ForEach-Object { $p.GetPixel($x, $_).ToArgb() }) -join ','; $cols[$k] += 1 }
$best = (($cols.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 1).Key -split ',')
for ($x = 26; $x -le 96; $x++) { for ($y = 0; $y -lt $p.Height; $y++) { $p.SetPixel($x, $y, [System.Drawing.Color]::FromArgb([int]$best[$y])) } }
$t = $p.Clone((New-Object System.Drawing.Rectangle 1, 1, ($p.Width - 2), ($p.Height - 2)), $p.PixelFormat)
New-Item -ItemType Directory -Force "$dir\parts_w" | Out-Null
$t.Save("$dir\parts_w\list_row.png")
$z = New-Object System.Drawing.Bitmap ($t.Width * 6), ($t.Height * 6)
$g = [System.Drawing.Graphics]::FromImage($z); $g.InterpolationMode = 'NearestNeighbor'; $g.PixelOffsetMode = 'Half'
$g.DrawImage($t, 0, 0, $z.Width, $z.Height); $z.Save("$dir\review\list_row_x6.png")
"size $($t.Width)x$($t.Height)  stretch cols x=25..95"
