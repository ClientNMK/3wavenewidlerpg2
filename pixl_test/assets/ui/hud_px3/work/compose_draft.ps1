# 가안: 왼쪽 = 전투 HUD(보스 도전 + 자동 토글), 오른쪽 = 캐릭터 정보창
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$dir  = Split-Path $PSScriptRoot -Parent
$art  = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource'
$game = 'C:\3wavestudio\3wavenewidlerpg2\3wavenewidlerpg2\Assets\Resource'
$LW = 240; $LH = 427; $SC = 3; $GAP = 8
$fonts = New-Object System.Drawing.Text.PrivateFontCollection
$fonts.AddFontFile("$game\Font\DNFBitBitv2.ttf")
$family = $fonts.Families[0]

function Get-Half($path) {
  $src = New-Object System.Drawing.Bitmap $path
  $res = New-Object System.Drawing.Bitmap ([int]($src.Width / 2)), ([int]($src.Height / 2))
  for ($y = 0; $y -lt $res.Height; $y++) { for ($x = 0; $x -lt $res.Width; $x++) {
    $v = @($src.GetPixel(2*$x, 2*$y).ToArgb(), $src.GetPixel(2*$x+1, 2*$y).ToArgb(), $src.GetPixel(2*$x, 2*$y+1).ToArgb(), $src.GetPixel(2*$x+1, 2*$y+1).ToArgb())
    $m = ($v | Group-Object | Sort-Object Count -Descending | Select-Object -First 1).Name
    $res.SetPixel($x, $y, [System.Drawing.Color]::FromArgb([int]$m))
  } }
  $src.Dispose(); return $res
}

# 상자 안에서 가장 큰 덩어리만 남기고 여백을 자른다
function Get-Piece($sheet, $bx, $by, $bw, $bh) {
  $bw = [math]::Min($bw, $sheet.Width - $bx); $bh = [math]::Min($bh, $sheet.Height - $by)
  $id = New-Object 'int[,]' $bw, $bh; $best = 0; $bestN = 0; $cur = 0
  for ($y0 = 0; $y0 -lt $bh; $y0++) { for ($x0 = 0; $x0 -lt $bw; $x0++) {
    if ($id[$x0, $y0] -ne 0 -or $sheet.GetPixel($bx + $x0, $by + $y0).A -eq 0) { continue }
    $cur++; $n = 0; $q = New-Object System.Collections.Generic.Queue[int]; $q.Enqueue($y0 * $bw + $x0); $id[$x0, $y0] = $cur
    while ($q.Count) {
      $v = $q.Dequeue(); $px = $v % $bw; $py = [int][math]::Floor($v / $bw); $n++
      for ($dy = -1; $dy -le 1; $dy++) { for ($dx = -1; $dx -le 1; $dx++) {
        $nx = $px + $dx; $ny = $py + $dy
        if ($nx -lt 0 -or $ny -lt 0 -or $nx -ge $bw -or $ny -ge $bh) { continue }
        if ($id[$nx, $ny] -ne 0 -or $sheet.GetPixel($bx + $nx, $by + $ny).A -eq 0) { continue }
        $id[$nx, $ny] = $cur; $q.Enqueue($ny * $bw + $nx)
      } }
    }
    if ($n -gt $bestN) { $bestN = $n; $best = $cur }
  } }
  $minX = $bw; $maxX = -1; $minY = $bh; $maxY = -1
  for ($y = 0; $y -lt $bh; $y++) { for ($x = 0; $x -lt $bw; $x++) { if ($id[$x, $y] -eq $best) {
    if ($x -lt $minX) { $minX = $x }; if ($x -gt $maxX) { $maxX = $x }; if ($y -lt $minY) { $minY = $y }; if ($y -gt $maxY) { $maxY = $y } } } }
  $piece = New-Object System.Drawing.Bitmap ($maxX - $minX + 1), ($maxY - $minY + 1)
  for ($y = $minY; $y -le $maxY; $y++) { for ($x = $minX; $x -le $maxX; $x++) { if ($id[$x, $y] -eq $best) { $piece.SetPixel($x - $minX, $y - $minY, $sheet.GetPixel($bx + $x, $by + $y)) } } }
  return $piece
}

function Draw9($gfx, $img, $dx, $dy, $dw, $dh, $bl, $br, $bt, $bb) {
  $sx = @(0, $bl, ($img.Width - $br), $img.Width); $sy = @(0, $bt, ($img.Height - $bb), $img.Height)
  $tx = @($dx, ($dx + $bl), ($dx + $dw - $br), ($dx + $dw)); $ty = @($dy, ($dy + $bt), ($dy + $dh - $bb), ($dy + $dh))
  for ($j = 0; $j -lt 3; $j++) { for ($i = 0; $i -lt 3; $i++) {
    $sw = $sx[$i+1] - $sx[$i]; $sv = $sy[$j+1] - $sy[$j]; $tw = $tx[$i+1] - $tx[$i]; $tv = $ty[$j+1] - $ty[$j]
    if ($sw -le 0 -or $sv -le 0 -or $tw -le 0 -or $tv -le 0) { continue }
    $gfx.DrawImage($img, (New-Object System.Drawing.Rectangle $tx[$i], $ty[$j], $tw, $tv), (New-Object System.Drawing.Rectangle $sx[$i], $sy[$j], $sw, $sv), [System.Drawing.GraphicsUnit]::Pixel)
  } }
}
function Put($gfx, $img, $dx, $dy) { $gfx.DrawImage($img, (New-Object System.Drawing.Rectangle $dx, $dy, $img.Width, $img.Height), (New-Object System.Drawing.Rectangle 0, 0, $img.Width, $img.Height), [System.Drawing.GraphicsUnit]::Pixel) }
function Load($path) { $tmp = New-Object System.Drawing.Bitmap $path; $copy = New-Object System.Drawing.Bitmap $tmp; $tmp.Dispose(); return $copy }

# ---- 부품 준비 ----
$s1 = Get-Half "$dir\source\draft_boss_block_sheet.png"; $s1.Save("$dir\source\draft_boss_block_half.png")
$s2 = Get-Half "$dir\source\draft_char_window_sheet.png"; $s2.Save("$dir\source\draft_char_window_half.png")
$sign   = Get-Piece $s1 2 2 124 22;  $track = Get-Piece $s1 2 25 81 15;  $fill = Get-Piece $s1 83 26 42 13
$bossBtn = Get-Piece $s1 2 42 68 29; $skull = Get-Piece $s1 73 42 29 29
$tgOn   = Get-Piece $s1 2 77 56 27;  $tgOff = Get-Piece $s1 62 77 56 27
$panel  = Get-Piece $s2 0 0 81 94;   $title = Get-Piece $s2 81 2 46 20;  $close = Get-Piece $s2 81 21 21 21
$level  = Get-Piece $s2 101 24 27 16; $row  = Get-Piece $s2 81 44 46 19;  $pframe = Get-Piece $s2 80 62 34 34
$winBtn = Get-Piece $s2 2 105 59 23
$pc = "$dir\parts_caseB"
$plate = Load "$pc\plate_currency.png"; $coin = Load "$pc\icon_gold.png"; $slot = Load "$pc\slot.png"; $portrait = Load "$pc\frame_portrait.png"
$girl = Load "$art\girl_base\animations\idle\frames\frame_000.png"
$shadow = Load "$art\common\shadow\shadow.png"; $fire = Load "$art\ui\status_icons\status_burn.png"
$bg = Load "$game\Textures\Bg\bg_world_2.png"

$fullW = $LW * 2 + $GAP
$base = New-Object System.Drawing.Bitmap $fullW, $LH
$win  = New-Object System.Drawing.Bitmap $fullW, $LH
$gb = [System.Drawing.Graphics]::FromImage($base); $gw = [System.Drawing.Graphics]::FromImage($win)
$gb.Clear([System.Drawing.ColorTranslator]::FromHtml('#15131A'))
$textA = @(); $textB = @()
$CREAMTXT = '#F8E0C8'; $DARKTXT = '#402820'; $LINE = '#2A1A14'

foreach ($screen in 0, 1) {
  $ox = $screen * ($LW + $GAP)
  $gb.SetClip((New-Object System.Drawing.Rectangle $ox, 0, $LW, $LH))
  $gb.InterpolationMode = 'HighQualityBicubic'; $gb.PixelOffsetMode = 'Default'
  $gb.DrawImage($bg, (New-Object System.Drawing.Rectangle $ox, 0, $LW, $LH), (New-Object System.Drawing.Rectangle 448, 0, 1152, 2048), [System.Drawing.GraphicsUnit]::Pixel)
  $gb.InterpolationMode = 'NearestNeighbor'; $gb.PixelOffsetMode = 'Half'

  # 전투 장면
  foreach ($p in @(@(84, 252), @(120, 260), @(184, 242), @(196, 290), @(60, 316))) { Put $gb $shadow ($ox + $p[0] - 20) ($p[1] - 5) }
  $sprites = @(
    @("$art\pets\pet_hood\hood_001\animations\idle\frames\frame_000.png", 84, 252, $false),
    @("$art\girl_base\animations\idle\frames\frame_000.png", 116, 260, $false),
    @("$art\monsters\mob_slime_pink\animations\idle\frames\frame_000.png", 184, 242, $true),
    @("$art\monsters\mob_slime_blue\animations\idle\frames\frame_000.png", 196, 290, $true),
    @("$art\monsters\mob_slime\animations\idle\frames\frame_000.png", 60, 316, $false))
  foreach ($sp in $sprites) { $img = Load $sp[0]; if ($sp[3]) { $img.RotateFlip('RotateNoneFlipX') }; Put $gb $img ($ox + $sp[1] - 64) ($sp[2] - 112); $img.Dispose() }

  # 상단: 초상화, 재화
  Put $gb $portrait ($ox + 4) 4
  for ($y = -11; $y -le 11; $y++) { for ($x = -11; $x -le 11; $x++) { if ($x*$x + $y*$y -le 125) { $c = $girl.GetPixel(62 + $x, 70 + $y); if ($c.A -gt 128) { $base.SetPixel($ox + 22 + $x, 22 + $y, $c) } } } }
  Put $gb $plate ($ox + 164) 5; Put $gb $coin ($ox + 167) 8
  $textA += , @('소라', ($ox + 44), 6, 9, 'l', $CREAMTXT, $LINE); $textA += , @('Lv.12', ($ox + 44), 17, 7, 'l', $CREAMTXT, $LINE)
  $textA += , @('12,480', ($ox + 231), 9, 8, 'r', $CREAMTXT, $LINE)

  # 스테이지 + 진행바 + 보스 도전 + 자동 토글
  Put $gb $sign ($ox + [int](($LW - $sign.Width) / 2)) 30
  $textA += , @('Stage 1-7', ($ox + 120), 34, 9, 'c', $CREAMTXT, $LINE)
  Draw9 $gb $track ($ox + 66) 53 104 $track.Height 6 6 0 0
  Draw9 $gb $fill ($ox + 69) (53 + [int](($track.Height - $fill.Height) / 2)) 60 $fill.Height 4 4 0 0
  $textA += , @('6/10', ($ox + 174), 55, 7, 'l', $CREAMTXT, $LINE)
  Put $gb $skull ($ox + 38) 72
  Put $gb $bossBtn ($ox + 68) 72
  $textA += , @('보스 도전', ($ox + 68 + [int]($bossBtn.Width / 2)), 79, 9, 'c', $DARKTXT, $null)
  if ($screen -eq 0) { Put $gb $tgOn ($ox + 140) 73; $textA += , @('자동', ($ox + 158), 80, 8, 'c', $CREAMTXT, $LINE) }
  else { Put $gb $tgOff ($ox + 140) 73; $textA += , @('자동', ($ox + 177), 80, 8, 'c', '#7A5040', $null) }

  # 하단: 슬롯, 탭 띠
  foreach ($i in 0..4) { Put $gb $slot ($ox + 46 + 30 * $i) 368 }
  Put $gb $fire ($ox + 52) 374
  $gb.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(225, 64, 40, 32))), $ox, 402, $LW, 25)
  $gb.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml($LINE))), $ox, 401, $LW, 1)
  $tabs = '성장', '후드', '스킬', '둥지', '상점'
  foreach ($i in 0..4) { $textA += , @($tabs[$i], ($ox + 24 + 48 * $i), 409, 9, 'c', $CREAMTXT, $LINE) }
  $gb.ResetClip()
}

# ---- 캐릭터 정보창 (오른쪽 화면) ----
$ox = $LW + $GAP
$gw.InterpolationMode = 'NearestNeighbor'; $gw.PixelOffsetMode = 'Half'
Draw9 $gw $panel ($ox + 18) 92 204 252 12 12 12 12
Draw9 $gw $title ($ox + 60) 84 120 $title.Height 12 12 0 0
$textB += , @('캐릭터 정보', ($ox + 120), 88, 9, 'c', $CREAMTXT, $LINE)
Put $gw $close ($ox + 208) 84
Put $gw $pframe ($ox + 30) 112
for ($y = 0; $y -lt 22; $y++) { for ($x = 0; $x -lt 22; $x++) { $c = $girl.GetPixel(51 + $x, 59 + $y); if ($c.A -gt 128) { $win.SetPixel($ox + 36 + $x, 118 + $y, $c) } } }
$textB += , @('소라', ($ox + 70), 112, 10, 'l', $DARKTXT, $null)
Put $gw $level ($ox + 70) 127
$textB += , @('Lv.12', ($ox + 70 + [int]($level.Width / 2)), 130, 7, 'c', $CREAMTXT, $LINE)
Draw9 $gw $track ($ox + 100) 128 110 $track.Height 6 6 0 0
Draw9 $gw $fill ($ox + 103) (128 + [int](($track.Height - $fill.Height) / 2)) 66 $fill.Height 4 4 0 0
$stats = @(@('공격력', '1,280'), @('체력', '9,600'), @('방어력', '340'), @('치명타', '12%'), @('포식 확률', '8%'))
foreach ($i in 0..4) {
  $ry = 154 + 21 * $i
  # 시트의 줄 판은 창 바탕과 같은 크림색이라 안 보여서, 한 단계 어두운 크림으로 직접 그림
  $gw.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml($LINE))), ($ox + 30), $ry, 180, 17)
  $gw.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml('#D8C0A8'))), ($ox + 31), ($ry + 1), 178, 15)
  $gw.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml('#583830'))), ($ox + 33), ($ry + 3), 11, 11)
  $textB += , @($stats[$i][0], ($ox + 49), ($ry + 4), 8, 'l', $DARKTXT, $null)
  $textB += , @($stats[$i][1], ($ox + 204), ($ry + 4), 8, 'r', $DARKTXT, $null)
}
Draw9 $gw $winBtn ($ox + 75) 306 90 $winBtn.Height 14 14 0 0
$textB += , @('성장하기', ($ox + 120), 311, 9, 'c', $DARKTXT, $null)
$gb.Dispose(); $gw.Dispose()

# ---- 3배로 키우고 글자 얹기 ----
$big = New-Object System.Drawing.Bitmap ($fullW * $SC), ($LH * $SC)
$g2 = [System.Drawing.Graphics]::FromImage($big)
$g2.TextRenderingHint = 'SingleBitPerPixelGridFit'
function Say($item) {
  $f = New-Object System.Drawing.Font $family, ($item[3] * $SC), ([System.Drawing.FontStyle]::Regular), ([System.Drawing.GraphicsUnit]::Pixel)
  $tw = $g2.MeasureString($item[0], $f).Width
  $x = $item[1] * $SC; if ($item[4] -eq 'c') { $x -= $tw / 2 } elseif ($item[4] -eq 'r') { $x -= $tw }
  if ($item[6]) {
    $lb = New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml($item[6]))
    foreach ($o in @(@(-1,0),@(1,0),@(0,-1),@(0,1),@(-1,-1),@(1,1),@(-1,1),@(1,-1))) { $g2.DrawString($item[0], $f, $lb, ($x + $o[0] * $SC), ($item[2] * $SC + $o[1] * $SC)) }
  }
  $g2.DrawString($item[0], $f, (New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml($item[5]))), $x, ($item[2] * $SC)); $f.Dispose()
}
$g2.InterpolationMode = 'NearestNeighbor'; $g2.PixelOffsetMode = 'Half'
$g2.DrawImage($base, 0, 0, $fullW * $SC, $LH * $SC)
$g2.PixelOffsetMode = 'Default'
foreach ($item in $textA) { Say $item }
$g2.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(130, 20, 12, 10))), ($ox * $SC), 0, ($LW * $SC), ($LH * $SC))
$g2.PixelOffsetMode = 'Half'
$g2.DrawImage($win, 0, 0, $fullW * $SC, $LH * $SC)
$g2.PixelOffsetMode = 'Default'
foreach ($item in $textB) { Say $item }
$g2.Dispose()
$big.Save("$dir\review\draft_boss_and_charinfo_x3.png")
"saved: $dir\review\draft_boss_and_charinfo_x3.png"
