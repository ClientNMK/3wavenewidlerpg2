Add-Type -AssemblyName System.Drawing
Add-Type -Path 'C:\3wavestudio\3wavenewidlerpg2\ArtSource\tools\GifMaker.cs' -ReferencedAssemblies System.Drawing
$S = 'C:\Users\DH\AppData\Local\Temp\claude\b6904a4aaccdc9f4\d8c1e724-0e40-49fd-bb75-ef0dbf42e93f\scratchpad\bossfx'
$A = 'C:\3wavestudio\3wavenewidlerpg2\pixl_test\assets\monster\monster_boss_horned\v4\animations'
$G = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource\girl_base\animations\idle\frames'
function Get-Frames($dir) { Get-ChildItem $dir -Filter *.png | Sort-Object Name | ForEach-Object { [System.Drawing.Bitmap]::FromFile($_.FullName) } }
$idle = Get-Frames "$A\idle\frames"; $skill = Get-Frames "$A\skill\frames"
$girl = Get-Frames $G | ForEach-Object { $_.RotateFlip('RotateNoneFlipX'); $_ }
$SKT = 0.1; $IDT = 0.12; $LEAD = 0.5; $HITF = 6
$hit = $LEAD + $HITF * $SKT

# effect pivot x is relative to the blade point; d = start delay from the hit
$fxList = @(
  @{ p='boss\ground-slam\ground-slam'; dx=0;   d=0 },
  @{ p='boss\earth-spike\earth-spike'; dx=150; d=-0.06 }
)
foreach ($fx in $fxList) {
  $m = (Get-Content "$S\$($fx.p).json" -Raw | ConvertFrom-Json).meta.pixelfx
  $fx.sheet = [System.Drawing.Bitmap]::FromFile("$S\$($fx.p).png")
  $fx.fw = $m.frameSize[0]; $fx.fh = $m.frameSize[1]; $fx.px = $m.pivot[0]; $fx.py = $m.pivot[1]; $fx.cols = $m.columns; $fx.cnt = $m.frames
}
$PW = 520; $PH = 240; $GY = 210; $BX = 30; $TIPX = 146
$GIRLX = $BX + $TIPX + 150
$total = [int](($hit + 2.2) * 30)
$outDir = "$S\composite_combo2"; New-Item -ItemType Directory -Force $outDir | Out-Null
Remove-Item "$outDir\*.png" -ErrorAction SilentlyContinue
$paths = @()
for ($f = 0; $f -lt $total; $f++) {
  $t = $f / 30.0
  $si = [math]::Floor(($t - $LEAD) / $SKT)
  if ($t -ge $LEAD -and $si -lt $skill.Count) { $boss = $skill[$si] } else { $boss = $idle[[int]([math]::Floor($t / $IDT)) % $idle.Count] }
  $bmp = New-Object System.Drawing.Bitmap($PW, $PH)
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.Clear([System.Drawing.ColorTranslator]::FromHtml('#6b6458')); $g.InterpolationMode = 'NearestNeighbor'; $g.PixelOffsetMode = 'Half'
  $g.DrawImage($boss, $BX, ($GY - 176), 192, 192)
  $g.DrawImage($girl[[int]([math]::Floor($t / 0.12)) % $girl.Count], ($GIRLX - 64), ($GY - 112), 128, 128)
  foreach ($fx in $fxList) {
    $e = [math]::Floor(($t - $hit - $fx.d) * 30)
    if ($e -ge 0 -and $e -lt $fx.cnt) {
      $sx = ($e % $fx.cols) * $fx.fw; $sy = [math]::Floor($e / $fx.cols) * $fx.fh
      $dst = New-Object System.Drawing.Rectangle(($BX + $TIPX + $fx.dx - $fx.px), ($GY - $fx.py), $fx.fw, $fx.fh)
      $g.DrawImage($fx.sheet, $dst, $sx, $sy, $fx.fw, $fx.fh, [System.Drawing.GraphicsUnit]::Pixel)
    }
  }
  $g.Dispose()
  $p = "$outDir\f_{0:D3}.png" -f $f; $bmp.Save($p); $bmp.Dispose(); $paths += $p
}
[GifMaker]::Make($paths, "$S\boss_shockwave_combo_v2_x2.gif", 2, 3, $null, -1)
"done $total"
