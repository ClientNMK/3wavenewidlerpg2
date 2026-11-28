param([int]$Impact = 1, [string]$Name = 'girl_iai_demo_x2', [int]$Scale = 2)
Add-Type -AssemblyName System.Drawing
Add-Type -Path 'C:\3wavestudio\3wavenewidlerpg2\ArtSource\tools\GifMaker.cs' -ReferencedAssemblies System.Drawing
$S = 'C:\Users\DH\AppData\Local\Temp\claude\b6904a4aaccdc9f4\d8c1e724-0e40-49fd-bb75-ef0dbf42e93f\scratchpad\girlfx'
$G = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource\girl_base'
$M = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource\monsters\monster_stage_002\animations'
function Get-Frames($dir, $flip) { Get-ChildItem $dir -Filter *.png | Sort-Object Name | ForEach-Object { $b = [System.Drawing.Bitmap]::FromFile($_.FullName); if ($flip) { $b.RotateFlip('RotateNoneFlipX') }; $b } }
function Get-Fx($base, $flip) {
  $m = (Get-Content "$base.json" -Raw | ConvertFrom-Json).meta.pixelfx
  $sheet = [System.Drawing.Bitmap]::FromFile("$base.png"); $fw = $m.frameSize[0]; $fh = $m.frameSize[1]; $fr = @()
  for ($i = 0; $i -lt $m.frames; $i++) {
    $b = New-Object System.Drawing.Bitmap($fw, $fh); $gg = [System.Drawing.Graphics]::FromImage($b)
    $gg.DrawImage($sheet, (New-Object System.Drawing.Rectangle(0, 0, $fw, $fh)), (($i % $m.columns) * $fw), ([math]::Floor($i / $m.columns) * $fh), $fw, $fh, [System.Drawing.GraphicsUnit]::Pixel); $gg.Dispose()
    if ($flip) { $b.RotateFlip('RotateNoneFlipX') }
    $fr += $b
  }
  $px = $m.pivot[0]; if ($flip) { $px = $fw - $px }
  @{ fr = $fr; px = $px; py = $m.pivot[1]; cnt = $m.frames }
}
function New-Sil($v) {
  $cm = New-Object System.Drawing.Imaging.ColorMatrix
  $cm.Matrix00 = 0; $cm.Matrix11 = 0; $cm.Matrix22 = 0; $cm.Matrix40 = $v; $cm.Matrix41 = $v; $cm.Matrix42 = $v
  $ia = New-Object System.Drawing.Imaging.ImageAttributes; $ia.SetColorMatrix($cm); $ia
}
function Draw-Img($g, $b, $x, $y, $ia) {
  $r = New-Object System.Drawing.Rectangle($x, $y, $b.Width, $b.Height)
  if ($ia) { $g.DrawImage($b, $r, 0, 0, $b.Width, $b.Height, [System.Drawing.GraphicsUnit]::Pixel, $ia) } else { $g.DrawImage($b, $r, 0, 0, $b.Width, $b.Height, [System.Drawing.GraphicsUnit]::Pixel) }
}
function Draw-Fx($g, $fx, $e, $x, $y, $ia) { if ($e -ge 0 -and $e -lt $fx.cnt) { Draw-Img $g $fx.fr[$e] ($x - $fx.px) ($y - $fx.py) $ia } }

$idle = Get-Frames "$G\animations\idle\frames" $false
$hurt = Get-Frames "$G\animations\hurt\frames" $false
$van = Get-Frames "$G\animations\vanish\frames" $false
$back = Get-Frames "$G\animations\backstab\frames" $true
$mIdle = Get-Frames "$M\idle\frames" $true
$mDead = Get-Frames "$M\dead\frames" $true
$cut = Get-Fx "$S\melee\delayed-cut\delayed-cut" $true
$vb = Get-Fx "$G\effects\vanish_burst\collect-burst" $false
$white = New-Sil 1; $black = New-Sil 0

$PW = 380; $PH = 200; $GY = 165; $GX0 = 120; $MX = 205; $GX1 = $MX + 42
$tV = 0.5; $tB = $tV + 0.84 + 0.2; $tS = $tB + 0.28; $BURSTF = 6; $tK = $tS + $BURSTF / 30.0
$STOP = 5 / 30.0; if (-not $Impact) { $STOP = 0 }
$shake = @(@(7, -3), @(-6, 3), @(5, -2), @(-4, 2), @(2, -1), @(-1, 1))
$total = [int](($tK + $STOP + 1.4) * 30)
$outDir = "$S\composite_$Name"; New-Item -ItemType Directory -Force $outDir | Out-Null
Remove-Item "$outDir\*.png" -ErrorAction SilentlyContinue
$paths = @()
for ($f = 0; $f -lt $total; $f++) {
  $tr = $f / 30.0 + 0.0001
  $stopped = $Impact -and $tr -ge $tK -and $tr -lt $tK + $STOP
  if ($tr -lt $tK) { $t = $tr } else { $t = [math]::Max($tK, $tr - $STOP) }
  $k = [int][math]::Floor(($tr - $tK) * 30)
  $bg = '#6b6458'; $sil = $null; $msil = $null
  if ($stopped) { if ($k -eq 0 -or $k -eq 3) { $bg = '#fffaf4'; $sil = $black; $msil = $black } else { $bg = '#0b0910'; $sil = $white; $msil = $white } }
  elseif ($tr -ge $tK -and $k -lt [int][math]::Round($STOP * 30) + 2) { $msil = $white }
  $bmp = New-Object System.Drawing.Bitmap($PW, $PH)
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.Clear([System.Drawing.ColorTranslator]::FromHtml($bg)); $g.InterpolationMode = 'NearestNeighbor'; $g.PixelOffsetMode = 'Half'
  $j = $k - [int][math]::Round($STOP * 30)
  if (-not $stopped -and $j -ge 0 -and $j -lt $shake.Count) { $g.TranslateTransform($shake[$j][0], $shake[$j][1]) }
  # monster: idle, frozen while the line hangs, then dies
  if ($t -lt $tS) { $mb = $mIdle[[int]([math]::Floor($t / 0.12)) % $mIdle.Count] }
  elseif ($t -le $tK + 0.06) { $mb = $mIdle[[int]([math]::Floor($tS / 0.12)) % $mIdle.Count] }
  else { $mb = $mDead[[math]::Min($mDead.Count - 1, [int][math]::Floor(($t - $tK - 0.06) / 0.1))] }
  Draw-Img $g $mb ($MX - 64) ($GY - 112) $msil
  # girl
  $gb = $null; $gx = $GX0
  if ($t -lt $tV - 0.24) { $gb = $idle[[int]([math]::Floor($t / 0.12)) % $idle.Count] }
  elseif ($t -lt $tV) { $gb = $hurt[[math]::Min(5, 3 + [int][math]::Floor(($t - $tV + 0.24) / 0.08))] }
  elseif ($t -lt $tV + 0.84) { $gb = $van[[int][math]::Floor(($t - $tV) / 0.06)] }
  elseif ($t -ge $tB) {
    $gx = $GX1; $bi = [int][math]::Floor(($t - $tB) / 0.07)
    if ($bi -lt $back.Count) { $gb = $back[$bi] } else { $gb = $back[$back.Count - 1] }
  }
  if ($gb) { Draw-Img $g $gb ($gx - 64) ($GY - 112) $sil }
  Draw-Fx $g $vb ([int][math]::Floor(($t - $tV - 0.30) * 30)) $GX0 ($GY + 2)
  $e = [int][math]::Floor(($tr - $tS) * 30 + 0.001)
  $fia = $null; if ($stopped -and ($k -eq 0 -or $k -eq 3)) { $fia = $black }; Draw-Fx $g $cut $e $GX1 $GY $fia
  $g.Dispose()
  $p = "$outDir\f_{0:D3}.png" -f $f; $bmp.Save($p); $bmp.Dispose(); $paths += $p
}
[GifMaker]::Make($paths, "$S\$Name.gif", $Scale, 3, $null, -1)
"done $total frames, burst at $tK"
