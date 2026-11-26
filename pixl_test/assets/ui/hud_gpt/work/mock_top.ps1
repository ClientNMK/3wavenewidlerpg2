$ErrorActionPreference='Stop'; Add-Type -AssemblyName System.Drawing
$root="C:\Users\HP\3wavestudio\3wavenewidlerpg2\pixl_test\assets\ui\hud_gpt"
Add-Type -Path "$root\work\Nine.cs" -ReferencedAssemblies System.Drawing
function Get-P($n){ New-Object System.Drawing.Bitmap "$root\parts\$n.png" }
# phone-width canvas (1080 wide), top area + some field
$W=1080; $H=760
$cv=New-Object System.Drawing.Bitmap -ArgumentList $W,$H
$g=[System.Drawing.Graphics]::FromImage($cv); $g.SmoothingMode='AntiAlias'; $g.TextRenderingHint='AntiAliasGridFit'
$g.Clear([System.Drawing.ColorTranslator]::FromHtml('#6E8B4E')); $rng=New-Object System.Random 5
for($i=0;$i -lt 220;$i++){ $x=$rng.Next($W);$y=$rng.Next($H); $g.FillEllipse((New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(90,40,70,30))),$x,$y,$rng.Next(10,46),$rng.Next(6,20)) }
# field sprites
$g.InterpolationMode='NearestNeighbor'; $g.PixelOffsetMode='Half'
$g.DrawImage((New-Object System.Drawing.Bitmap "C:\Users\HP\3wavestudio\3wavenewidlerpg2\ArtSource\girl_base\animations\idle\frames\frame_000.png"),300,330,384,384)
$sl=New-Object System.Drawing.Bitmap "C:\Users\HP\3wavestudio\3wavenewidlerpg2\ArtSource\monsters\mob_slime\pose\mob_slime_se.png"
$g.DrawImage($sl,650,260,288,288); $g.DrawImage($sl,760,480,192,192)
$g.PixelOffsetMode='HighQuality'; $g.InterpolationMode='HighQualityBicubic'
function Put($img,[float]$x,[float]$y,[float]$w,[float]$h){ $g.InterpolationMode='HighQualityBicubic'; $g.DrawImage($img,$x,$y,$w,$h) }
# portrait: frame plate + face from concept sheet clipped to rounded rect
$fr=Get-P 'frame_portrait_small'; Put $fr 24 24 132 132
$cs=New-Object System.Drawing.Bitmap "C:\Users\HP\3wavestudio\3wavenewidlerpg2\ArtSource\girl_base\concept\shellgirl_concept_sheet.png"
$clip=New-Object System.Drawing.Drawing2D.GraphicsPath; $clip.AddRectangle((New-Object System.Drawing.RectangleF 34,34,112,112)); $g.SetClip($clip)
$g.DrawImage($cs,(New-Object System.Drawing.RectangleF 30,30,120,120),(New-Object System.Drawing.RectangleF 222,640,220,220),[System.Drawing.GraphicsUnit]::Pixel); $g.ResetClip()
# name + level text (engine font stand-in)
$f=New-Object System.Drawing.Font("Malgun Gothic",30,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
$fs=New-Object System.Drawing.Font("Malgun Gothic",22,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
function Txt($t,$fnt,[float]$x,[float]$y,[bool]$center){ $sz=$g.MeasureString($t,$fnt); if($center){$x=$x-$sz.Width/2; $y=$y-$sz.Height/2}; $g.DrawString($t,$fnt,[System.Drawing.Brushes]::Black,$x+2,$y+2); $g.DrawString($t,$fnt,[System.Drawing.Brushes]::White,$x,$y) }
Txt "쉘걸" $f 168 30 $false
Txt "Lv.25" $fs 168 72 $false
# currency pills (9-slice) with icons on left end
$pill=Get-P 'hud_pill'; $s=0.34; $ph=$pill.Height*$s
[Nine]::Draw($g,$pill,(New-Object System.Drawing.RectangleF 470,34,260,$ph),95,0,95,0,$s)
Put (Get-P 'icon_gold') 452 22 78 80
Txt "4,276,035" $fs 640 (34+$ph/2) $true
[Nine]::Draw($g,$pill,(New-Object System.Drawing.RectangleF 770,34,230,$ph),95,0,95,0,$s)
$gem=Get-P 'icon_gem'; Put $gem 758 20 (84*$gem.Width/$gem.Height) 84
Txt "19,999" $fs 890 (34+$ph/2) $true
Put (Get-P 'btn_plus_small') 1006 36 54 55
$g.Dispose(); $cv.Save("$root\review\hud_top_v2_assembled.png"); "ok"
