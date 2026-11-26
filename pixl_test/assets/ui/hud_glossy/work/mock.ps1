$ErrorActionPreference='Stop'; Add-Type -AssemblyName System.Drawing
$root="C:\Users\HP\3wavestudio\3wavenewidlerpg2\pixl_test\assets\ui\hud_glossy"
$pix="C:\Users\HP\3wavestudio\3wavenewidlerpg2\pixl_test\assets\ui\hud_kit\parts"
function Get-P($n){ New-Object System.Drawing.Bitmap "$root\parts\$n.png" }
function Get-X($n){ New-Object System.Drawing.Bitmap "$pix\$n.png" }
$W=1480; $H=560
$cv=New-Object System.Drawing.Bitmap -ArgumentList $W,$H
$g=[System.Drawing.Graphics]::FromImage($cv)
$g.SmoothingMode='AntiAlias'; $g.TextRenderingHint='AntiAliasGridFit'
$bg=New-Object System.Drawing.Drawing2D.LinearGradientBrush((New-Object System.Drawing.Rectangle 0,0,$W,$H),[System.Drawing.Color]::FromArgb(255,34,52,44),[System.Drawing.Color]::FromArgb(255,18,26,30),90)
$g.FillRectangle($bg,0,0,$W,$H)
function Put($img,[int]$x,[int]$y){ $g.InterpolationMode='HighQualityBicubic'; $g.DrawImage($img,$x,$y,$img.Width,$img.Height) }
function PutPix($img,[int]$x,[int]$y,[int]$s){ $g.InterpolationMode='NearestNeighbor'; $g.PixelOffsetMode='Half'; $g.DrawImage($img,$x,$y,$img.Width*$s,$img.Height*$s); $g.PixelOffsetMode='HighQuality' }
$f=New-Object System.Drawing.Font("Malgun Gothic",30,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
$fs=New-Object System.Drawing.Font("Malgun Gothic",22,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
function Txt($t,$fnt,[float]$cx,[float]$cy){ $sz=$g.MeasureString($t,$fnt); $g.DrawString($t,$fnt,[System.Drawing.Brushes]::Black,$cx-$sz.Width/2+2,$cy-$sz.Height/2+2); $g.DrawString($t,$fnt,[System.Drawing.Brushes]::White,$cx-$sz.Width/2,$cy-$sz.Height/2) }

# portrait + level
Put (Get-P 'frame_portrait') 10 6
Put (Get-P 'badge_level') 45 210
Txt "Lv.25" $fs 130 243
# currency bars with sockets
Put (Get-P 'bar_currency') 270 30;  Put (Get-P 'socket_icon') 230 12
PutPix (New-Object System.Drawing.Bitmap "C:\Users\HP\3wavestudio\3wavenewidlerpg2\pixl_test\assets\item\item_gold\work\gold_pile_a.png") 259 30 2
Txt "1.2억" $f 580 76
Put (Get-P 'bar_currency') 860 30; Put (Get-P 'socket_icon') 820 12
PutPix (Get-X 'icon_gem') 851 43 1
Txt "8,350" $f 1170 76
Put (Get-P 'btn_plus') 1330 31
# buff slots with icons
$buffs='buff_gold','buff_exp','buff_atk','buff_speed'
for($k=0;$k -lt 4;$k++){ $x=250+$k*100; Put (Get-P 'slot_buff') $x 140; PutPix (Get-X $buffs[$k]) ($x+13) 153 2 }
Put (Get-P 'fx_sparkle') 640 130
# progress bar
Put (Get-P 'bar_trough') 680 160
$fill=Get-P 'bar_fill'; $fw=[int]($fill.Width*0.62); $g.InterpolationMode='HighQualityBicubic'
$g.DrawImage($fill,(New-Object System.Drawing.Rectangle 698,176,$fw,$fill.Height),0,0,$fw,$fill.Height,[System.Drawing.GraphicsUnit]::Pixel)
Txt "62%" $fs 900 186
# stage bar + side buttons
Put (Get-P 'bar_stage') 230 250
Txt "숲  Stage 54-17                0/20" $f 680 297
Put (Get-P 'btn_side') 1300 150; PutPix (Get-X 'btn_chest') 1342 192 1
Put (Get-P 'btn_side') 1300 300; PutPix (Get-X 'icon_boss') 1349 349 1
# ornament panel sample
Put (Get-P 'panel_ornament') 330 330
Txt "보스 즉시 도전" $f 650 440
$g.Dispose(); $cv.Save("$root\review\hud_glossy_v1_assembled.png"); "ok"
