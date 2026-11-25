$ErrorActionPreference='Stop'; Add-Type -AssemblyName System.Drawing
$root="C:\Users\HP\3wavestudio\3wavenewidlerpg2\pixl_test\assets\ui\hud_kit"
function Get-Part($n){ New-Object System.Drawing.Bitmap "$root\parts\$n.png" }
$W=800; $H=270
$cv=New-Object System.Drawing.Bitmap -ArgumentList $W,$H
$g=[System.Drawing.Graphics]::FromImage($cv)
$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
$g.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::Half
$g.Clear([System.Drawing.Color]::FromArgb(255,62,84,56))

function Draw-Img($img,[int]$x,[int]$y){ $g.DrawImage($img,(New-Object System.Drawing.Rectangle $x,$y,$img.Width,$img.Height),0,0,$img.Width,$img.Height,[System.Drawing.GraphicsUnit]::Pixel) }
function Draw-Nine($img,[int]$x,[int]$y,[int]$w,[int]$h,[int]$i){
  $sw=$img.Width; $sh=$img.Height
  $sx=@(0,$i,($sw-$i)); $sws=@($i,($sw-2*$i),$i)
  $sy=@(0,$i,($sh-$i)); $shs=@($i,($sh-2*$i),$i)
  $dx=@($x,($x+$i),($x+$w-$i)); $dws=@($i,($w-2*$i),$i)
  $dy=@($y,($y+$i),($y+$h-$i)); $dhs=@($i,($h-2*$i),$i)
  for($r=0;$r -lt 3;$r++){ for($c=0;$c -lt 3;$c++){
    $g.DrawImage($img,(New-Object System.Drawing.Rectangle $dx[$c],$dy[$r],$dws[$c],$dhs[$r]),$sx[$c],$sy[$r],$sws[$c],$shs[$r],[System.Drawing.GraphicsUnit]::Pixel) } }
}
$plate=Get-Part 'plate_stage'
# portrait + level badge
Draw-Img (Get-Part 'frame_portrait') 12 12
Draw-Img (Get-Part 'badge_level') 29 122
# currency plates (9-slice stretched)
Draw-Nine $plate 168 22 230 46 12; Draw-Img (Get-Part 'icon_gold') 140 10
Draw-Nine $plate 448 22 240 46 12; Draw-Img (Get-Part 'icon_gem') 420 10; Draw-Img (Get-Part 'btn_plus') 664 16
# buff slots
for($k=0;$k -lt 4;$k++){ Draw-Img (Get-Part 'slot_buff') (150+$k*72) 88 }
Draw-Img (Get-Part 'btn_toggle') 560 98
# stage plate
Draw-Nine $plate 150 196 520 46 12
Draw-Img (Get-Part 'btn_mail') 698 70
Draw-Img (Get-Part 'btn_rank') 698 170
# v2 parts: buff icons in slots, alert dot, progress bar, boss icon
$buffs='buff_gold','buff_exp','buff_atk','buff_speed'
for($k=0;$k -lt 4;$k++){ Draw-Img (Get-Part $buffs[$k]) (150+$k*72+16) (88+16) }
Draw-Img (Get-Part 'notify_dot') 776 66
Draw-Img (Get-Part 'bar_trough') 300 160
$fill=Get-Part 'bar_fill'; $fw=[int]($fill.Width*0.6)
$g.DrawImage($fill,(New-Object System.Drawing.Rectangle 304,166,$fw,$fill.Height),0,0,$fw,$fill.Height,[System.Drawing.GraphicsUnit]::Pixel)
Draw-Img (Get-Part 'icon_boss') 92 196
$g.Dispose()
# scale x2 and add sample text
$s=2; $out=New-Object System.Drawing.Bitmap -ArgumentList ($W*$s),($H*$s)
$g2=[System.Drawing.Graphics]::FromImage($out); $g2.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor; $g2.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::Half
$g2.DrawImage($cv,0,0,$W*$s,$H*$s); $g2.TextRenderingHint=[System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
$f=New-Object System.Drawing.Font("Malgun Gothic",30,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
$fs=New-Object System.Drawing.Font("Malgun Gothic",22,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
function Txt($t,$fnt,[float]$cx,[float]$cy){ $sz=$g2.MeasureString($t,$fnt); $g2.DrawString($t,$fnt,[System.Drawing.Brushes]::Black,$cx-$sz.Width/2+2,$cy-$sz.Height/2+2); $g2.DrawString($t,$fnt,[System.Drawing.Brushes]::White,$cx-$sz.Width/2,$cy-$sz.Height/2) }
Txt "1.2억" $f (2*290) (2*45)
Txt "8,350" $f (2*560) (2*45)
Txt "Lv.25" $fs (2*72) (2*138)
Txt "자동" $fs (2*618) (2*123)
Txt "숲  Stage 1-7          0/20" $f (2*410) (2*219)
$g2.Dispose(); $out.Save("$root\review\hud_kit_v1_assembled_x2.png")
"ok"
