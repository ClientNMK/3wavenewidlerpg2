param([string]$OutDir)
Add-Type -AssemblyName System.Drawing
New-Item -ItemType Directory -Force $OutDir | Out-Null

$pal = @{
 'K'='#2a1b1a'; 'S'='#f7e4c4'; 'O'='#e6714e'; 'D'='#a8402f'; 'H'='#efd29a'; 'F'='#f9dcc4';
 'E'='#c27a24'; 'P'='#f2a08c'; 'B'='#4b2b25'; 'R'='#d03a32'; 'X'='#8c1f22'; 'L'='#7d4b3c';
 'N'='#6e3f2c'; 'W'='#ffffff'; 'M'='#5a1720'; 'Y'='#f2a516'
}

$top = @(
'......KK........KK......',
'.....KOOK......KOOK.....',
'......KK........KK......',
'.......K..KKKK..K.......',
'.......KKKSSSSKKK.......'
)
$body = @(
'....KBBBBKRRRRKBBBBK....',
'....KBBBBBRBBRBBBBBK....',
'...KFKBBBBBBBBBBBBKFK...',
'....KKBBBBBBBBBBBBKK....',
'....KBBBBBBBBBBBBBBK....',
'...KBBBBBBBBBBBBBBBBK...',
'...KLLLLLLLLLLLLLLLLK...',
'....KKKKKFFKKFFKKKKK....',
'........KNNKKNNK........',
'.......KNNNKKNNNK.......',
'.......KKKK..KKKK.......'
)
$base = $top + @(
'......KSSOOSSOOSSK......',
'.....KOOSSOOOOSSOOK.....',
'....KSSOOSKKKKSOOSSK....',
'....KOOSSKYKKYKSSOOK....',
'...KSSOOSSKKKKSSOOSSK...',
'...KOOSSOOSSSSOOSSOOK...',
'..KOOKHHHHHHHHHHHHKOOK..',
'..KSSKHHHHKHHKHHHHKSSK..',
'..KOOKHFFHFFFFHFFHKOOK..',
'..KSSKHWKFFFFFFKWHKSSK..',
'..KOOKHKEFFFFFFEKHKOOK..',
'..KSSKHPFFFKKFFFPHKSSK..',
'..KOOKHFFFFFFFFFFHKOOK..',
'...KOKHHFFFFFFFFHHKOK...',
'....KKBBKRRKKRRKBBKK....',
'....KBBBKRRXXRRKBBBK....'
) + $body
$trans = $top + @(
'......KOOKKKKKKOOK......',
'.....KOKWWYWYYWWKOK.....',
'....KOOKWYYKKYYWKOOK....',
'....KOOKWWYKKYWWKOOK....',
'...KOOOOKKKKKKKKOOOOK...',
'.KKDOOOOOOOOOOOOOOOODKK.',
'KDDWWWWWWWWWWWWWWWWWWDDK',
'KDMMWMHWHHWHHWHHWHMWMMDK',
'KDWMMKHFKHFFFFHKFHKMMWDK',
'KDWWMKHWKFFFFFFKWHKMWWDK',
'KDWMMKHKEFFFFFFEKHKMMWDK',
'KDWWMKHPFFFKKFFFPHKMWWDK',
'KDWMMKHFFFFFFFFFFHKMMWDK',
'.KDWMKHHFFFFFFFFHHKMWDK.',
'.KDDKKBBKRRKKRRKBBKKDDK.',
'..KKKBBBKRRXXRRKBBBKKK..'
) + $body

function New-Sprite($rows, $name) {
  if ($rows.Count -ne 32) { throw "$name rows=$($rows.Count)" }
  $bmp = New-Object System.Drawing.Bitmap 24, 32
  for ($y=0; $y -lt 32; $y++) {
    if ($rows[$y].Length -ne 24) { throw "$name row $y len=$($rows[$y].Length)" }
    for ($x=0; $x -lt 24; $x++) {
      $c = [string]$rows[$y][$x]
      if ($c -ne '.') { $bmp.SetPixel($x, $y, [System.Drawing.ColorTranslator]::FromHtml($pal[$c])) }
    }
  }
  $bmp.Save((Join-Path $OutDir "$name.png")); return $bmp
}
function Save-Scaled($bmp, $s, $path, $bg) {
  $o = New-Object System.Drawing.Bitmap ($bmp.Width*$s), ($bmp.Height*$s)
  $g = [System.Drawing.Graphics]::FromImage($o); if ($bg) { $g.Clear($bg) }
  $g.InterpolationMode='NearestNeighbor'; $g.PixelOffsetMode='Half'
  $g.DrawImage($bmp, 0, 0, $o.Width, $o.Height); $o.Save($path); $g.Dispose(); $o.Dispose()
}
$a = New-Sprite $base 'shellgirl_base_24x32'
$b = New-Sprite $trans 'shellgirl_transform_24x32'
Save-Scaled $a 3 (Join-Path $OutDir 'shellgirl_base_x3.png') $null
Save-Scaled $b 3 (Join-Path $OutDir 'shellgirl_transform_x3.png') $null
# preview: side by side on beige, 1x gap
$pair = New-Object System.Drawing.Bitmap 56, 36
$g = [System.Drawing.Graphics]::FromImage($pair); $g.Clear([System.Drawing.ColorTranslator]::FromHtml('#ebe4d6'))
$g.DrawImage($a, 2, 2, 24, 32); $g.DrawImage($b, 30, 2, 24, 32); $g.Dispose()
Save-Scaled $pair 3 (Join-Path $OutDir 'preview_x3.png') $null
Save-Scaled $pair 10 (Join-Path $OutDir 'preview_x10.png') $null
Get-ChildItem $OutDir | Select-Object Name, Length
