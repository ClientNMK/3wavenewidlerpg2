$ErrorActionPreference='Stop'; Add-Type -AssemblyName System.Drawing
$root="C:\Users\HP\3wavestudio\3wavenewidlerpg2\pixl_test\assets\ui\hud_gpt"
function Get-X($n){ New-Object System.Drawing.Bitmap "$root\parts_pixel\$n.png" }
# low-res canvas = game pixels (360 x 253), then x3
$W=360; $H=253; $S=3
$lo=New-Object System.Drawing.Bitmap -ArgumentList $W,$H
$g=[System.Drawing.Graphics]::FromImage($lo); $g.InterpolationMode='NearestNeighbor'; $g.PixelOffsetMode='Half'
$g.Clear([System.Drawing.ColorTranslator]::FromHtml('#6E8B4E')); $rng=New-Object System.Random 5
for($i=0;$i -lt 220;$i++){ $x=$rng.Next($W);$y=$rng.Next($H); $g.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255,92,118,64))),$x,$y,$rng.Next(3,14),$rng.Next(2,5)) }
$g.DrawImage((New-Object System.Drawing.Bitmap "C:\Users\HP\3wavestudio\3wavenewidlerpg2\ArtSource\girl_base\animations\idle\frames\frame_000.png"),100,110,128,128)
$sl=New-Object System.Drawing.Bitmap "C:\Users\HP\3wavestudio\3wavenewidlerpg2\ArtSource\monsters\mob_slime\pose\mob_slime_se.png"
$g.DrawImage($sl,216,86,96,96); $g.DrawImage($sl,252,160,64,64)
function PutX($img,[int]$x,[int]$y){ $g.DrawImage($img,(New-Object System.Drawing.Rectangle $x,$y,$img.Width,$img.Height),0,0,$img.Width,$img.Height,[System.Drawing.GraphicsUnit]::Pixel) }
# 9-slice in pixel space (nearest): fixed caps of 8px
function NineX($img,[int]$x,[int]$y,[int]$w,[int]$c){ $h=$img.Height
  $g.DrawImage($img,(New-Object System.Drawing.Rectangle $x,$y,$c,$h),0,0,$c,$h,[System.Drawing.GraphicsUnit]::Pixel)
  $g.DrawImage($img,(New-Object System.Drawing.Rectangle ($x+$c),$y,($w-2*$c),$h),$c,0,($img.Width-2*$c),$h,[System.Drawing.GraphicsUnit]::Pixel)
  $g.DrawImage($img,(New-Object System.Drawing.Rectangle ($x+$w-$c),$y,$c,$h),($img.Width-$c),0,$c,$h,[System.Drawing.GraphicsUnit]::Pixel) }
$fr=Get-X 'frame_portrait_small'; PutX $fr 7 7
# portrait: pixel heroine head crop inside frame
$gi=New-Object System.Drawing.Bitmap "C:\Users\HP\3wavestudio\3wavenewidlerpg2\ArtSource\girl_base\animations\idle\frames\frame_000.png"
$g.DrawImage($gi,(New-Object System.Drawing.Rectangle 10,10,32,32),36,26,40,40,[System.Drawing.GraphicsUnit]::Pixel)
$pill=Get-X 'hud_pill'
NineX $pill 156 11 88 8; PutX (Get-X 'icon_gold') 150 9
NineX $pill 256 11 78 8; PutX (Get-X 'icon_gem') 251 9
PutX (Get-X 'btn_plus_small') 336 10
$g.Dispose()
$hi=New-Object System.Drawing.Bitmap -ArgumentList ($W*$S),($H*$S); $g2=[System.Drawing.Graphics]::FromImage($hi); $g2.InterpolationMode='NearestNeighbor'; $g2.PixelOffsetMode='Half'; $g2.DrawImage($lo,0,0,$W*$S,$H*$S)
$g2.TextRenderingHint='AntiAliasGridFit'
$fs=New-Object System.Drawing.Font("Malgun Gothic",22,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
function Txt($t,[float]$cx,[float]$cy){ $sz=$g2.MeasureString($t,$fs); $g2.DrawString($t,$fs,[System.Drawing.Brushes]::Black,$cx-$sz.Width/2+2,$cy-$sz.Height/2+2); $g2.DrawString($t,$fs,[System.Drawing.Brushes]::White,$cx-$sz.Width/2,$cy-$sz.Height/2) }
Txt "4,276,035" (205*$S) (19*$S); Txt "19,999" (297*$S) (19*$S)
$g2.DrawString("쉘걸",$fs,[System.Drawing.Brushes]::White,52*$S,9*$S); $g2.DrawString("Lv.25",$fs,[System.Drawing.Brushes]::White,52*$S,22*$S)
$g2.Dispose(); $hi.Save("$root\review\hud_top_pixel_test.png")
# side by side compare (top 300px of each)
$a=New-Object System.Drawing.Bitmap "$root\review\hud_top_v2_assembled.png"
$cmp=New-Object System.Drawing.Bitmap -ArgumentList 1080,640; $g3=[System.Drawing.Graphics]::FromImage($cmp); $g3.Clear([System.Drawing.Color]::Black)
$g3.DrawImage($a,(New-Object System.Drawing.Rectangle 0,0,1080,310),(New-Object System.Drawing.Rectangle 0,0,1080,310),[System.Drawing.GraphicsUnit]::Pixel)
$g3.DrawImage($hi,(New-Object System.Drawing.Rectangle 0,320,1080,320),(New-Object System.Drawing.Rectangle 0,0,1080,320),[System.Drawing.GraphicsUnit]::Pixel)
$lf=New-Object System.Drawing.Font("Malgun Gothic",24,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
$g3.DrawString("A  지금 (Codex 원본 그대로)",$lf,[System.Drawing.Brushes]::Yellow,20,260); $g3.DrawString("B  같은 부품을 게임 도트 크기로 변환",$lf,[System.Drawing.Brushes]::Yellow,20,590)
$g3.Dispose(); $cmp.Save("$root\review\hud_top_compare_smooth_vs_pixel.png"); "ok"
