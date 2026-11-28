Add-Type -AssemblyName System.Drawing
Add-Type -Path 'C:\3wavestudio\3wavenewidlerpg2\ArtSource\tools\GifMaker.cs' -ReferencedAssemblies System.Drawing
$S = 'C:\Users\DH\AppData\Local\Temp\claude\b6904a4aaccdc9f4\d8c1e724-0e40-49fd-bb75-ef0dbf42e93f\scratchpad\bossfx'
$A = 'C:\3wavestudio\3wavenewidlerpg2\pixl_test\assets\monster\monster_boss_horned\v4\animations'
$G = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource\girl_base\animations\idle\frames'
function Get-Frames($dir) { Get-ChildItem $dir -Filter *.png | Sort-Object Name | ForEach-Object { [System.Drawing.Bitmap]::FromFile($_.FullName) } }
function Get-Fx($p) {
  $m = (Get-Content "$S\$p.json" -Raw | ConvertFrom-Json).meta.pixelfx
  @{ sheet = [System.Drawing.Bitmap]::FromFile("$S\$p.png"); fw = $m.frameSize[0]; fh = $m.frameSize[1]; px = $m.pivot[0]; py = $m.pivot[1]; cols = $m.columns; cnt = $m.frames }
}
function Draw-Fx($g, $fx, $e, $x, $y) {
  if ($e -lt 0 -or $e -ge $fx.cnt) { return }
  $sx = ($e % $fx.cols) * $fx.fw; $sy = [math]::Floor($e / $fx.cols) * $fx.fh
  $g.DrawImage($fx.sheet, (New-Object System.Drawing.Rectangle(($x - $fx.px), ($y - $fx.py), $fx.fw, $fx.fh)), $sx, $sy, $fx.fw, $fx.fh, [System.Drawing.GraphicsUnit]::Pixel)
}
$idle = Get-Frames "$A\idle\frames"; $skill = Get-Frames "$A\skill\frames"
$girl = Get-Frames $G | ForEach-Object { $_.RotateFlip('RotateNoneFlipX'); $_ }
$slam = Get-Fx 'boss\ground-slam\ground-slam'
$sizes = 1..4 | ForEach-Object { Get-Fx "grow\s$_\boss\earth-spike\earth-spike" }
$SKT = 0.1; $IDT = 0.12; $LEAD = 0.5; $HITF = 6
$hit = $LEAD + $HITF * $SKT

$PW = 720; $PH = 420; $GY = 230; $BX = 200; $TIPX = 146
$OX = $BX + $TIPX; $GIRLX = $OX + 170
$STEP = 40; $GAP = 0.07; $DEPTH = 0.5
# every spike: position and start time, 8 directions out from the blade point until it leaves the screen
$spikes = @()
for ($i = 0; $i -lt 8; $i++) {
  $an = $i * [math]::PI / 4
  for ($k = 1; $k -lt 30; $k++) {
    $r = 18 + $k * $STEP
    $x = [int]($OX + [math]::Cos($an) * $r); $y = [int]($GY + [math]::Sin($an) * $r * $DEPTH)
    if ($x -lt -20 -or $x -gt $PW + 20 -or $y -lt 40 -or $y -gt $PH + 10) { break }
    $spikes += @{ x = $x; y = $y; t0 = $hit + $k * $GAP; fx = $sizes[[math]::Min(3, [math]::Floor(($k - 1) / 2))] }
  }
}
$spikes = $spikes | Sort-Object { $_.y }
$total = [int](($hit + 29 * 0 + ($spikes | ForEach-Object { $_.t0 } | Measure-Object -Maximum).Maximum - $hit + 2.0) * 30)
$outDir = "$S\composite_phase2"; New-Item -ItemType Directory -Force $outDir | Out-Null
Remove-Item "$outDir\*.png" -ErrorAction SilentlyContinue
$paths = @()
for ($f = 0; $f -lt $total; $f++) {
  $t = $f / 30.0
  $si = [math]::Floor(($t - $LEAD) / $SKT)
  if ($t -ge $LEAD -and $si -lt $skill.Count) { $boss = $skill[$si] } else { $boss = $idle[[int]([math]::Floor($t / $IDT)) % $idle.Count] }
  $bmp = New-Object System.Drawing.Bitmap($PW, $PH)
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.Clear([System.Drawing.ColorTranslator]::FromHtml('#6b6458')); $g.InterpolationMode = 'NearestNeighbor'; $g.PixelOffsetMode = 'Half'
  foreach ($q in $spikes) { if ($q.y -lt $GY) { Draw-Fx $g $q.fx ([math]::Floor(($t - $q.t0) * 30)) $q.x $q.y } }
  $g.DrawImage($boss, $BX, ($GY - 176), 192, 192)
  $g.DrawImage($girl[[int]([math]::Floor($t / 0.12)) % $girl.Count], ($GIRLX - 64), ($GY - 112), 128, 128)
  Draw-Fx $g $slam ([math]::Floor(($t - $hit) * 30)) $OX $GY
  foreach ($q in $spikes) { if ($q.y -ge $GY) { Draw-Fx $g $q.fx ([math]::Floor(($t - $q.t0) * 30)) $q.x $q.y } }
  $g.Dispose()
  $p = "$outDir\f_{0:D3}.png" -f $f; $bmp.Save($p); $bmp.Dispose(); $paths += $p
}
[GifMaker]::Make($paths, "$S\boss_shockwave_phase2_grow_x1.gif", 1, 3, $null, -1)
"done $total frames, $($spikes.Count) spikes"
