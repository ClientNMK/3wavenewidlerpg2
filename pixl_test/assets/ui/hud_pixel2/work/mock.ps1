$ErrorActionPreference='Stop'; Add-Type -AssemblyName System.Drawing
$root="C:\Users\HP\3wavestudio\3wavenewidlerpg2\pixl_test\assets\ui\hud_pixel2"
$A="C:\Users\HP\3wavestudio\3wavenewidlerpg2\3wavenewidlerpg2\Assets\Resource\Textures"
function Get-P($n){ New-Object System.Drawing.Bitmap "$root\parts\$n.png" }
$W=1080; $H=1000
$cv=New-Object System.Drawing.Bitmap -ArgumentList $W,$H
$g=[System.Drawing.Graphics]::FromImage($cv); $g.InterpolationMode='NearestNeighbor'; $g.PixelOffsetMode='Half'; $g.TextRenderingHint='AntiAliasGridFit'
$g.Clear([System.Drawing.ColorTranslator]::FromHtml('#6E8B4E')); $rng=New-Object System.Random 5
for($i=0;$i -lt 260;$i++){ $x=$rng.Next($W);$y=$rng.Next($H); $g.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255,92,118,64))),$x,$y,$rng.Next(9,42),$rng.Next(6,15)) }
$g.DrawImage((New-Object System.Drawing.Bitmap "C:\Users\HP\3wavestudio\3wavenewidlerpg2\ArtSource\girl_base\animations\idle\frames\frame_000.png"),330,420,384,384)
$sl=New-Object System.Drawing.Bitmap "C:\Users\HP\3wavestudio\3wavenewidlerpg2\ArtSource\monsters\mob_slime\pose\mob_slime_se.png"
$g.DrawImage($sl,650,330,288,288)
function PutS($img,[int]$x,[int]$y,[int]$s){ $g.DrawImage($img,(New-Object System.Drawing.Rectangle $x,$y,($img.Width*$s),($img.Height*$s)),0,0,$img.Width,$img.Height,[System.Drawing.GraphicsUnit]::Pixel) }
# horizontal 3-slice in source pixels, drawn at scale s
function NineH($img,[int]$x,[int]$y,[int]$w,[int]$cap,[int]$s){ $h=$img.Height
  $g.DrawImage($img,(New-Object System.Drawing.Rectangle $x,$y,($cap*$s),($h*$s)),0,0,$cap,$h,[System.Drawing.GraphicsUnit]::Pixel)
  $g.DrawImage($img,(New-Object System.Drawing.Rectangle ($x+$cap*$s),$y,($w-2*$cap*$s),($h*$s)),$cap,0,($img.Width-2*$cap),$h,[System.Drawing.GraphicsUnit]::Pixel)
  $g.DrawImage($img,(New-Object System.Drawing.Rectangle ($x+$w-$cap*$s),$y,($cap*$s),($h*$s)),($img.Width-$cap),0,$cap,$h,[System.Drawing.GraphicsUnit]::Pixel) }
$f=New-Object System.Drawing.Font("Malgun Gothic",26,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
$fs=New-Object System.Drawing.Font("Malgun Gothic",20,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
function Txt($t,$fnt,[float]$cx,[float]$cy){ $sz=$g.MeasureString($t,$fnt); $g.DrawString($t,$fnt,[System.Drawing.Brushes]::Black,$cx-$sz.Width/2+2,$cy-$sz.Height/2+2); $g.DrawString($t,$fnt,[System.Drawing.Brushes]::White,$cx-$sz.Width/2,$cy-$sz.Height/2) }
# portrait (frame has transparent center) + pixel heroine head
$gi=New-Object System.Drawing.Bitmap "C:\Users\HP\3wavestudio\3wavenewidlerpg2\ArtSource\girl_base\animations\idle\frames\frame_000.png"
$g.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml('#5A3826'))),26,26,100,100)
$g.DrawImage($gi,(New-Object System.Drawing.Rectangle 26,20,104,104),30,22,52,52,[System.Drawing.GraphicsUnit]::Pixel)
PutS (Get-P 'frame_portrait') 18 18 1
PutS (Get-P 'badge_level') 32 132 1; Txt "Lv.25" $fs 76 148
Txt "쉘걸" $f 190 40
# currency plates + Unity star coin + goods coin
$pl=Get-P 'plate_currency'
NineH $pl 520 28 230 26 1; PutS (New-Object System.Drawing.Bitmap "$A\UI\HUD\icon_gold.png") 486 20 1; Txt "4,276,035" $fs 650 54
NineH $pl 800 28 200 26 1; PutS (New-Object System.Drawing.Bitmap "$A\Goods\icon_goods_gold.png") 780 22 2; Txt "19,999" $fs 910 54
PutS (Get-P 'btn_plus') 1008 26 1
# stage bar + progress
$st=Get-P 'plate_stage'; NineH $st 170 150 740 20 1; Txt "숲  Stage 1-18                       0/20" $f 540 176
$tr=Get-P 'bar_trough'; NineH $tr 240 214 600 16 1
$fl=Get-P 'bar_fill_sample'; $g.DrawImage($fl,(New-Object System.Drawing.Rectangle 240,218,360,$fl.Height),0,0,200,$fl.Height,[System.Drawing.GraphicsUnit]::Pixel)
# side buttons
PutS (Get-P 'btn_mail') 960 120 1; PutS (Get-P 'btn_rank') 960 236 1
# bottom: slots row + main button + close
for($k=0;$k -lt 5;$k++){ PutS (Get-P 'slot') (170+$k*80) 830 1 }
PutS (Get-P 'btn_main') 600 836 1; Txt "보스 도전" $f 676 862
PutS (Get-P 'btn_close') 770 836 1; PutS (Get-P 'btn_sub') 840 842 1; Txt "AUTO" $fs 879 862
$g.Dispose(); $cv.Save("$root\review\hud_pixel2_v1_assembled.png"); "ok"
