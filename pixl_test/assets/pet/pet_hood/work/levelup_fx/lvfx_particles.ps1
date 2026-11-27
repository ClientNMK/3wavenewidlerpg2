Add-Type -AssemblyName System.Drawing
Add-Type -Path 'C:\3wavestudio\3wavenewidlerpg2\ArtSource\tools\GifMaker.cs' -ReferencedAssemblies System.Drawing
$S = 'C:\Users\DH\AppData\Local\Temp\claude\b6904a4aaccdc9f4\d8c1e724-0e40-49fd-bb75-ef0dbf42e93f\scratchpad\lvfx'
$hoodDir = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource\pets\pet_hood\hood_001\animations\idle\frames'
$hood = Get-ChildItem $hoodDir -Filter *.png | Sort-Object Name | ForEach-Object { [System.Drawing.Bitmap]::FromFile($_.FullName) }

$fxList = @(
  @{ n='A particles';     p='pets\hood-levelup\hood-levelup' },
  @{ n='B particles 60%'; p='soft\pets\hood-levelup\hood-levelup' },
  @{ n='ref level-up';    p='ui\level-up\level-up' }
)
foreach ($fx in $fxList) {
  $j = Get-Content "$S\$($fx.p).json" -Raw | ConvertFrom-Json
  $m = $j.meta.pixelfx
  $fx.sheet = [System.Drawing.Bitmap]::FromFile("$S\$($fx.p).png")
  $fx.fw = $m.frameSize[0]; $fx.fh = $m.frameSize[1]; $fx.px = $m.pivot[0]; $fx.py = $m.pivot[1]
  $fx.cols = $m.columns; $fx.cnt = $m.frames
}

$PW = 240; $PH = 240; $pivX = 120; $pivY = 210
$lead = 15; $tail = 10
$total = $lead + ($fxList | ForEach-Object { $_.cnt } | Measure-Object -Maximum).Maximum + $tail
$outDir = "$S\composite"; New-Item -ItemType Directory -Force $outDir | Out-Null
$font = New-Object System.Drawing.Font('Consolas', 9)
$paths = @()
for ($f = 0; $f -lt $total; $f++) {
  $bmp = New-Object System.Drawing.Bitmap(($PW * 2), ($PH * 2))
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.Clear([System.Drawing.ColorTranslator]::FromHtml('#3a3a44')); $g.InterpolationMode = 'NearestNeighbor'; $g.PixelOffsetMode = 'Half'
  $hi = [int]([math]::Floor($f / 30.0 / 0.12)) % $hood.Count
  for ($k = 0; $k -lt $fxList.Count; $k++) {
    $fx = $fxList[$k]; $ox = ($k % 2) * $PW; $oy = [math]::Floor($k / 2) * $PH
    $g.SetClip((New-Object System.Drawing.Rectangle($ox, $oy, $PW, $PH)))
    $g.DrawImage($hood[$hi],($ox + $pivX - 64), ($oy + $pivY - 112), 128, 128)
    $e = $f - $lead
    if ($e -ge 0 -and $e -lt $fx.cnt) {
      $sx = ($e % $fx.cols) * $fx.fw; $sy = [math]::Floor($e / $fx.cols) * $fx.fh
      $dst = New-Object System.Drawing.Rectangle(($ox + $pivX - $fx.px), ($oy + $pivY - $fx.py), $fx.fw, $fx.fh)
      $g.DrawImage($fx.sheet, $dst, $sx, $sy, $fx.fw, $fx.fh, [System.Drawing.GraphicsUnit]::Pixel)
    }
    $g.DrawString($fx.n, $font, [System.Drawing.Brushes]::Wheat, ($ox + 4), ($oy + 4))
  }
  $g.Dispose()
  $p = "$outDir\f_{0:D3}.png" -f $f; $bmp.Save($p); $bmp.Dispose(); $paths += $p
}
[GifMaker]::Make($paths, "$S\hood_levelup_particles_x2.gif", 2, 3, '#3a3a44', -1)
"done $total"
