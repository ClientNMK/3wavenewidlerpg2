param([string]$Name = 'girl_gale_demo_x2', [int]$Scale = 2)
Add-Type -AssemblyName System.Drawing
Add-Type -Path 'C:\3wavestudio\3wavenewidlerpg2\ArtSource\tools\GifMaker.cs' -ReferencedAssemblies System.Drawing
$S = 'C:\Users\DH\AppData\Local\Temp\claude\b6904a4aaccdc9f4\d8c1e724-0e40-49fd-bb75-ef0dbf42e93f\scratchpad\girlfx'
$G = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource\girl_base'
$MR = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource\monsters'
function Get-Frames($dir, $flip) { Get-ChildItem $dir -Filter *.png | Sort-Object Name | ForEach-Object { $b = [System.Drawing.Bitmap]::FromFile($_.FullName); if ($flip) { $b.RotateFlip('RotateNoneFlipX') }; $b } }
function Get-Fx($base) {
  $m = (Get-Content "$base.json" -Raw | ConvertFrom-Json).meta.pixelfx
  $sheet = [System.Drawing.Bitmap]::FromFile("$base.png"); $fw = $m.frameSize[0]; $fh = $m.frameSize[1]; $fr = @()
  for ($i = 0; $i -lt $m.frames; $i++) {
    $b = New-Object System.Drawing.Bitmap($fw, $fh); $gg = [System.Drawing.Graphics]::FromImage($b)
    $gg.DrawImage($sheet, (New-Object System.Drawing.Rectangle(0, 0, $fw, $fh)), (($i % $m.columns) * $fw), ([math]::Floor($i / $m.columns) * $fh), $fw, $fh, [System.Drawing.GraphicsUnit]::Pixel); $gg.Dispose()
    $fr += $b
  }
  @{ fr = $fr; px = $m.pivot[0]; py = $m.pivot[1]; cnt = $m.frames }
}
function New-Sil($v) {
  $cm = New-Object System.Drawing.Imaging.ColorMatrix
  $cm.Matrix00 = 0; $cm.Matrix11 = 0; $cm.Matrix22 = 0; $cm.Matrix40 = $v; $cm.Matrix41 = $v; $cm.Matrix42 = $v
  $ia = New-Object System.Drawing.Imaging.ImageAttributes; $ia.SetColorMatrix($cm); $ia
}
function Draw-Img($g, $b, $x, $y, $ia) {
  $r = New-Object System.Drawing.Rectangle([int]$x, [int]$y, $b.Width, $b.Height)
  if ($ia) { $g.DrawImage($b, $r, 0, 0, $b.Width, $b.Height, [System.Drawing.GraphicsUnit]::Pixel, $ia) } else { $g.DrawImage($b, $r, 0, 0, $b.Width, $b.Height, [System.Drawing.GraphicsUnit]::Pixel) }
}
function Smooth($a, $b, $x) { $k = [math]::Max(0, [math]::Min(1, ($x - $a) / ($b - $a))); $k * $k * (3 - 2 * $k) }

$idle = Get-Frames "$G\animations\idle\frames" $false
$atk = Get-Frames "$G\animations\attack3\frames" $false
$kinds = @('monster_stage_002', 'monster_stage_002_blue', 'monster_stage_002_red', 'monster_stage_002_purple')
$mobs = @()
$x0 = @(215, 262, 305, 350); $off = @(-16, 12, -4, 20)
for ($i = 0; $i -lt 4; $i++) {
  $mobs += @{ idle = (Get-Frames "$MR\$($kinds[$i])\animations\idle\frames" $true); dead = (Get-Frames "$MR\$($kinds[$i])\animations\dead\frames" $true); x0 = $x0[$i]; off = $off[$i]; ph = $i * 0.17 }
}
$gale = Get-Fx "$S\melee\blade-gale\blade-gale"
$white = New-Sil 1

$PW = 480; $PH = 270; $GY = 225; $GX = 80
$tA = 0.4; $tE = $tA + 0.21; $BURSTF = 36; $tK = $tE + $BURSTF / 30.0; $STOP = 3 / 30.0
$cuts = 0..9 | ForEach-Object { $tE + 0.25 + $_ * 0.08 }
$shake = @(@(7, -3), @(-6, 3), @(5, -2), @(-4, 2), @(2, -1), @(-1, 1))
$total = [int](($tK + $STOP + 1.3) * 30)
$outDir = "$S\composite_$Name"; New-Item -ItemType Directory -Force $outDir | Out-Null
Remove-Item "$outDir\*.png" -ErrorAction SilentlyContinue
$paths = @()
for ($f = 0; $f -lt $total; $f++) {
  $tr = $f / 30.0 + 0.0001
  $stopped = $tr -ge $tK -and $tr -lt $tK + $STOP
  if ($tr -lt $tK) { $t = $tr } else { $t = [math]::Max($tK, $tr - $STOP) }
  $k = [int][math]::Floor(($tr - $tK) * 30)
  $bg = '#6b6458'
  $bmp = New-Object System.Drawing.Bitmap($PW, $PH)
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.Clear([System.Drawing.ColorTranslator]::FromHtml($bg)); $g.InterpolationMode = 'NearestNeighbor'; $g.PixelOffsetMode = 'Half'
  $j = $k - 3
  if (-not $stopped -and $j -ge 0 -and $j -lt $shake.Count) { $g.TranslateTransform($shake[$j][0], $shake[$j][1]) }
  # the gale walks forward 110 px while it spins, then stands and bursts
  $fxT = $tr - $tE
  $galeX = $GX + 50 + 110 * (Smooth 0.1 1.0 $fxT)
  # a cut landed in the last frame?
  $hit = $false; foreach ($c in $cuts) { if ($tr -ge $c -and $tr -lt $c + 0.034) { $hit = $true } }
  foreach ($m in $mobs) {
    $pull = Smooth 0.25 0.95 $fxT
    $mx = $m.x0 + ($galeX + $m.off - $m.x0) * $pull
    $msil = $null
    if ($t -gt $tK + 0.02) {
      $d = $t - $tK; $dir = 1; if ($m.off -lt 0) { $dir = -1 }
      $mx += $dir * 40 * (1 - [math]::Pow(2, -10 * [math]::Min(1, $d / 0.4)))
      $mb = $m.dead[[math]::Min($m.dead.Count - 1, [int][math]::Floor($d / 0.1))]
      if ($d -lt 0.07) { $msil = $white }
    } else {
      $mb = $m.idle[[int]([math]::Floor(($t + $m.ph) / 0.12)) % $m.idle.Count]
      if ($hit -and $pull -gt 0.3) { $msil = $white; $mx += 2 }
      if ($stopped) { $msil = $white }
    }
    Draw-Img $g $mb ($mx - 64) ($GY - 112) $msil
  }
  if ($t -ge $tA -and $t -lt $tA + $atk.Count * 0.07) { $gb = $atk[[int][math]::Floor(($t - $tA) / 0.07)] } else { $gb = $idle[[int]([math]::Floor($t / 0.12)) % $idle.Count] }
  $gsil = $null
  Draw-Img $g $gb ($GX - 64) ($GY - 112) $gsil
  $e = [int][math]::Floor($fxT * 30 + 0.001)
  if ($e -ge 0 -and $e -lt $gale.cnt) { Draw-Img $g $gale.fr[$e] ($galeX - $gale.px) ($GY - $gale.py) $null }
  $g.Dispose()
  $p = "$outDir\f_{0:D3}.png" -f $f; $bmp.Save($p); $bmp.Dispose(); $paths += $p
}
[GifMaker]::Make($paths, "$S\$Name.gif", $Scale, 3, $null, -1)
"done $total frames, burst at $tK"
