# parts_w 부품(1배)으로 성장 창 조립 미리보기. 화면 240x427 논리 크기의 3배
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$dir  = Split-Path $PSScriptRoot -Parent
$art  = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource'
$game = 'C:\3wavestudio\3wavenewidlerpg2\3wavenewidlerpg2\Assets\Resource'
$LW = 240; $LH = 427; $SC = 3
$fonts = New-Object System.Drawing.Text.PrivateFontCollection
$fonts.AddFontFile("$game\Font\DNFBitBitv2.ttf"); $family = $fonts.Families[0]
function Load($path) { $tmp = New-Object System.Drawing.Bitmap $path; $copy = New-Object System.Drawing.Bitmap $tmp; $tmp.Dispose(); return $copy }
function Draw9($img, $dx, $dy, $dw, $dh, $bl, $br, $bt, $bb) {
  $sx = @(0, $bl, ($img.Width - $br), $img.Width); $sy = @(0, $bt, ($img.Height - $bb), $img.Height)
  $tx = @($dx, ($dx + $bl), ($dx + $dw - $br), ($dx + $dw)); $ty = @($dy, ($dy + $bt), ($dy + $dh - $bb), ($dy + $dh))
  for ($j = 0; $j -lt 3; $j++) { for ($i = 0; $i -lt 3; $i++) {
    $sw = $sx[$i+1] - $sx[$i]; $sv = $sy[$j+1] - $sy[$j]; $tw = $tx[$i+1] - $tx[$i]; $tv = $ty[$j+1] - $ty[$j]
    if ($sw -le 0 -or $sv -le 0 -or $tw -le 0 -or $tv -le 0) { continue }
    $g.DrawImage($img, (New-Object System.Drawing.Rectangle $tx[$i], $ty[$j], $tw, $tv), (New-Object System.Drawing.Rectangle $sx[$i], $sy[$j], $sw, $sv), [System.Drawing.GraphicsUnit]::Pixel)
  } }
}
function Put($img, $dx, $dy) { Draw9 $img $dx $dy $img.Width $img.Height 0 0 0 0 }
$P = @{}; foreach ($n in 'panel_window','btn_close','tab_on','tab_off','btn_upgrade','seg_on','seg_off','list_row') { $P[$n] = Load "$dir\parts_w\$n.png" }

$cv = New-Object System.Drawing.Bitmap $LW, $LH
$g = [System.Drawing.Graphics]::FromImage($cv)
$bg = Load "$game\Textures\Bg\bg_world_2.png"
$g.InterpolationMode = 'HighQualityBicubic'
$g.DrawImage($bg, (New-Object System.Drawing.Rectangle 0, 0, $LW, $LH), (New-Object System.Drawing.Rectangle 448, 0, 1152, 2048), [System.Drawing.GraphicsUnit]::Pixel)
$g.InterpolationMode = 'NearestNeighbor'; $g.PixelOffsetMode = 'Half'
$texts = @(); $CREAM = '#F8E0C8'; $GOLD = '#EDB948'; $DARK = '#402820'; $LINE = '#2A1A14'

$shadow = Load "$art\common\shadow\shadow.png"
foreach ($sp in @(@("$art\girl_base\animations\idle\frames\frame_000.png", 90, 120, $false), @("$art\monsters\mob_slime_pink\animations\idle\frames\frame_000.png", 160, 112, $true))) {
  Put $shadow ($sp[1] - 20) ($sp[2] - 5); $img = Load $sp[0]; if ($sp[3]) { $img.RotateFlip('RotateNoneFlipX') }; Put $img ($sp[1] - 64) ($sp[2] - 112); $img.Dispose()
}

# 창
Draw9 $P.panel_window 4 140 232 250 6 6 6 6
Put $P.btn_close 216 132
$tabNames = '강화', '레벨', '전직', '어빌리티'
foreach ($i in 0..3) {
  $tb = if ($i -eq 0) { $P.tab_on } else { $P.tab_off }
  Draw9 $tb (10 + 55 * $i) 148 54 17 5 5 0 0
  $texts += , @($tabNames[$i], (37 + 55 * $i), 152, 8, $(if ($i -eq 0) { $CREAM } else { '#A87850' }))
}
foreach ($i in 0..2) {
  $sg = if ($i -eq 0) { $P.seg_on } else { $P.seg_off }; $L = if ($i -eq 0) { 5 } else { 15 }; $R = if ($i -eq 0) { 15 } else { 5 }
  Draw9 $sg (10 + 74 * $i) 169 70 15 $L $R 0 0
  $texts += , @(('x1', 'x10', 'x100')[$i], (45 + 74 * $i), 172, 7, $CREAM)
}
$stats = @(@('공격력', 'Lv.13만', '562만 > 712만', '45양'), @('체력', 'Lv.11만', '57만 > 57만', '2067자'), @('체력 재생', 'Lv.12만', '1194자 > 1195자', '2774자'), @('방어력', 'Lv.14만', '14만 > 14만', '1961자'), @('관통', 'Lv.16만', '16만 > 16만', '55양'))
foreach ($i in 0..4) {
  $ry = 189 + 28 * $i
  Draw9 $P.list_row 10 $ry 220 25 25 5 0 0
  Draw9 $P.btn_upgrade 180 ($ry + 2) 46 21 5 5 0 0
  $texts += , @($stats[$i][0], 34, ($ry + 3), 7, $CREAM, 'l')
  $texts += , @($stats[$i][1], 34, ($ry + 14), 6, '#D8C0A8', 'l')
  $texts += , @($stats[$i][2], 90, ($ry + 14), 6, $GOLD, 'l')
  $texts += , @('강화', 203, ($ry + 4), 6, $DARK)
  $texts += , @($stats[$i][3], 203, ($ry + 13), 6, $DARK)
}
$g.Dispose()

$big = New-Object System.Drawing.Bitmap ($LW * $SC), ($LH * $SC)
$g2 = [System.Drawing.Graphics]::FromImage($big)
$g2.InterpolationMode = 'NearestNeighbor'; $g2.PixelOffsetMode = 'Half'
$g2.DrawImage($cv, 0, 0, $LW * $SC, $LH * $SC)
$g2.PixelOffsetMode = 'Default'; $g2.TextRenderingHint = 'SingleBitPerPixelGridFit'
$lb = New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml($LINE))
foreach ($t in $texts) {
  $f = New-Object System.Drawing.Font $family, ($t[3] * $SC), ([System.Drawing.FontStyle]::Regular), ([System.Drawing.GraphicsUnit]::Pixel)
  $x = $t[1] * $SC; if ($t[5] -ne 'l') { $x -= $g2.MeasureString($t[0], $f).Width / 2 }
  if ($t[4] -ne $DARK) { foreach ($o in @(@(-1,0),@(1,0),@(0,-1),@(0,1))) { $g2.DrawString($t[0], $f, $lb, ($x + $o[0] * 2), ($t[2] * $SC + $o[1] * 2)) } }
  $g2.DrawString($t[0], $f, (New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml($t[4]))), $x, ($t[2] * $SC)); $f.Dispose()
}
$big.Save("$dir\review\window_w_assembled_x3.png")
