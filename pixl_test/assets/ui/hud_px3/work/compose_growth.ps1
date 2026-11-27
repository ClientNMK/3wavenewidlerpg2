# 가안: 레퍼런스 구조의 성장(강화) 창. 시트 부품을 줄이지 않고 그대로 쓰고 화면은 360x640 논리 크기의 2배
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$dir  = Split-Path $PSScriptRoot -Parent
$art  = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource'
$game = 'C:\3wavestudio\3wavenewidlerpg2\3wavenewidlerpg2\Assets\Resource'
$LW = 360; $LH = 640; $SC = 2
$fonts = New-Object System.Drawing.Text.PrivateFontCollection
$fonts.AddFontFile("$game\Font\DNFBitBitv2.ttf")
$family = $fonts.Families[0]

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
function Draw9($gfx, $img, $dx, $dy, $dw, $dh, $bl, $br, $bt, $bb) {
  $sx = @(0, $bl, ($img.Width - $br), $img.Width); $sy = @(0, $bt, ($img.Height - $bb), $img.Height)
  $tx = @($dx, ($dx + $bl), ($dx + $dw - $br), ($dx + $dw)); $ty = @($dy, ($dy + $bt), ($dy + $dh - $bb), ($dy + $dh))
  for ($j = 0; $j -lt 3; $j++) { for ($i = 0; $i -lt 3; $i++) {
    $sw = $sx[$i+1] - $sx[$i]; $sv = $sy[$j+1] - $sy[$j]; $tw = $tx[$i+1] - $tx[$i]; $tv = $ty[$j+1] - $ty[$j]
    if ($sw -le 0 -or $sv -le 0 -or $tw -le 0 -or $tv -le 0) { continue }
    $gfx.DrawImage($img, (New-Object System.Drawing.Rectangle $tx[$i], $ty[$j], $tw, $tv), (New-Object System.Drawing.Rectangle $sx[$i], $sy[$j], $sw, $sv), [System.Drawing.GraphicsUnit]::Pixel)
  } }
}
function Put($gfx, $img, $dx, $dy) { $gfx.DrawImage($img, (New-Object System.Drawing.Rectangle $dx, $dy, $img.Width, $img.Height), (New-Object System.Drawing.Rectangle 0, 0, $img.Width, $img.Height), [System.Drawing.GraphicsUnit]::Pixel) }
function Load($path) { $tmp = New-Object System.Drawing.Bitmap $path; $copy = New-Object System.Drawing.Bitmap $tmp; $tmp.Dispose(); return $copy }

$sheet = Load "$dir\source\ref_growth_window_sheet.png"
$panel = Get-Piece $sheet 0 0 173 173;    $close = Get-Piece $sheet 173 13 56 58
$tabOff = Get-Piece $sheet 236 9 104 48;  $tabOn = Get-Piece $sheet 341 9 105 48
$segOn = Get-Piece $sheet 171 85 134 46;  $segOff = Get-Piece $sheet 307 85 134 46
$segOn.RotateFlip('RotateNoneFlipX')
$row = Get-Piece $sheet 5 173 339 74;     $upBtn = Get-Piece $sheet 5 255 147 74
$nav = Get-Piece $sheet 4 336 441 81;     $title = Get-Piece $sheet 5 510 304 63
# 'listrow': 줄 판을 W5 list_row(1배)를 3배로 키운 것으로 교체
$useListRow = $args[0] -eq 'listrow'
if ($useListRow) {
  $lr = Load "$dir\parts_w\list_row.png"
  $row = New-Object System.Drawing.Bitmap ($lr.Width * 3), ($lr.Height * 3)
  $gr = [System.Drawing.Graphics]::FromImage($row); $gr.InterpolationMode = 'NearestNeighbor'; $gr.PixelOffsetMode = 'Half'
  $gr.DrawImage($lr, 0, 0, $row.Width, $row.Height); $gr.Dispose()
}
$icons = @(); foreach ($i in 0..3) { $icons += , (Get-Piece $sheet (5 + 74 * $i) 423 72 72) }

$cv = New-Object System.Drawing.Bitmap $LW, $LH
$g = [System.Drawing.Graphics]::FromImage($cv)
$bg = Load "$game\Textures\Bg\bg_world_2.png"
$g.InterpolationMode = 'HighQualityBicubic'
$g.DrawImage($bg, (New-Object System.Drawing.Rectangle 0, 0, $LW, $LH), (New-Object System.Drawing.Rectangle 448, 0, 1152, 2048), [System.Drawing.GraphicsUnit]::Pixel)
$g.InterpolationMode = 'NearestNeighbor'; $g.PixelOffsetMode = 'Half'
$texts = @(); $CREAM = '#F8E0C8'; $GOLD = '#EDB948'; $TEAL = '#6CC6BE'; $DARK = '#402820'; $LINE = '#2A1A14'

# 위쪽에 보이는 전투 장면 (캐릭터는 도트 1배 그대로)
$shadow = Load "$art\common\shadow\shadow.png"
foreach ($sp in @(@("$art\girl_base\animations\idle\frames\frame_000.png", 150, 142, $false), @("$art\monsters\mob_slime_pink\animations\idle\frames\frame_000.png", 250, 132, $true))) {
  Put $g $shadow ($sp[1] - 20) ($sp[2] - 5); $img = Load $sp[0]; if ($sp[3]) { $img.RotateFlip('RotateNoneFlipX') }; Put $g $img ($sp[1] - 64) ($sp[2] - 112); $img.Dispose()
}
Draw9 $g $title 70 6 220 40 60 60 14 14
$texts += , @('Stage 1-7', 180, 17, 13, 'c', $CREAM, $LINE)

# 창
Draw9 $g $panel 4 150 352 452 24 24 24 24
Put $g $close 302 118
$tabNames = '강화', '레벨', '전직', '어빌리티'
foreach ($i in 0..3) {
  $tb = if ($i -eq 0) { $tabOn } else { $tabOff }
  Draw9 $g $tb (16 + 82 * $i) 162 80 38 14 14 12 10
  $texts += , @($tabNames[$i], (56 + 82 * $i), 173, 12, 'c', $(if ($i -eq 0) { $CREAM } else { '#A87850' }), $LINE)
}
$segNames = 'x1', 'x10', 'x100'
foreach ($i in 0..2) {
  $sg = if ($i -eq 0) { $segOn } else { $segOff }
  Draw9 $g $sg (16 + 110 * $i) 206 106 $sg.Height 52 16 0 0
  $texts += , @($segNames[$i], (16 + 110 * $i + 64), 219, 12, 'c', $(if ($i -eq 0) { $DARK } else { $CREAM }), $(if ($i -eq 0) { $null } else { $LINE }))
}
$stats = @(@('공격력', 'Lv.13만', '562만8723', '712만3496', '45양8007자'), @('체력', 'Lv.11만4400', '57만2000', '57만2005', '2067자7642해'), @('체력 재생', 'Lv.12만800', '1194자6847해', '1195자2268해', '2774자3543해'), @('방어력', 'Lv.14만1100', '14만1101', '14만1102', '1961자747해'))
foreach ($i in 0..3) {
  $ry = 258 + 84 * $i
  if ($useListRow) {
    Draw9 $g $row 14 $ry 332 $row.Height 75 18 0 0
    $g.DrawImage($icons[$i], 22, ($ry + 8), 58, 58)
  } else {
    Draw9 $g $row 14 $ry 332 80 18 18 14 14
    Put $g $icons[$i] 20 ($ry + 5)
  }
  $texts += , @($stats[$i][0], 98, ($ry + 8), 13, 'l', $CREAM, $LINE)
  $texts += , @($stats[$i][1], 98, ($ry + 28), 10, 'l', $CREAM, $LINE)
  $texts += , @(($stats[$i][2] + '  >'), 98, ($ry + 54), 10, 'l', '#D8C0A8', $LINE)
  $texts += , @($stats[$i][3], 98, ($ry + 54), 10, 'after', $GOLD, $LINE, ($stats[$i][2] + '  > '))
  $texts += , @('MAX 30만', 290, ($ry + 5), 8, 'c', $TEAL, $LINE)
  Draw9 $g $upBtn 238 ($ry + 18) 104 56 16 16 14 16
  $texts += , @('강화', 290, ($ry + 24), 12, 'c', $DARK, $null)
  $texts += , @($stats[$i][4], 290, ($ry + 46), 9, 'c', $DARK, $null)
}
Draw9 $g $nav 0 600 360 40 20 20 12 12
$navNames = '성장', '인벤토리', '스킬', '미션', '상점'
foreach ($i in 0..4) { $texts += , @($navNames[$i], (36 + 72 * $i), 612, 11, 'c', $(if ($i -eq 0) { $GOLD } else { $CREAM }), $LINE) }
$g.Dispose()

$big = New-Object System.Drawing.Bitmap ($LW * $SC), ($LH * $SC)
$g2 = [System.Drawing.Graphics]::FromImage($big)
$g2.InterpolationMode = 'NearestNeighbor'; $g2.PixelOffsetMode = 'Half'
$g2.DrawImage($cv, 0, 0, $LW * $SC, $LH * $SC)
$g2.PixelOffsetMode = 'Default'; $g2.TextRenderingHint = 'SingleBitPerPixelGridFit'
foreach ($item in $texts) {
  $f = New-Object System.Drawing.Font $family, ($item[3] * $SC), ([System.Drawing.FontStyle]::Regular), ([System.Drawing.GraphicsUnit]::Pixel)
  $x = $item[1] * $SC
  if ($item[4] -eq 'c') { $x -= $g2.MeasureString($item[0], $f).Width / 2 } elseif ($item[4] -eq 'r') { $x -= $g2.MeasureString($item[0], $f).Width }
  elseif ($item[4] -eq 'after') { $x += $g2.MeasureString($item[7], $f).Width - 6 * $SC }
  if ($item[6]) {
    $lb = New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml($item[6]))
    foreach ($o in @(@(-1,0),@(1,0),@(0,-1),@(0,1),@(-1,-1),@(1,1),@(-1,1),@(1,-1))) { $g2.DrawString($item[0], $f, $lb, ($x + $o[0] * $SC), ($item[2] * $SC + $o[1] * $SC)) }
  }
  $g2.DrawString($item[0], $f, (New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml($item[5]))), $x, ($item[2] * $SC)); $f.Dispose()
}
$g2.Dispose()
$outName = if ($useListRow) { 'draft_growth_window_listrow_x2' } else { 'draft_growth_window_x2' }
$big.Save("$dir\review\$outName.png")
"saved: $dir\review\$outName.png"
