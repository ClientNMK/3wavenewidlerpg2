# PixelLab HUD 시트 2종(A: 우리 팀 게임일 때 / B: 다른 회사 게임일 때)을 전투 화면 예시로 조립
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$dir  = Split-Path $PSScriptRoot -Parent
$art  = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource'
$game = 'C:\3wavestudio\3wavenewidlerpg2\3wavenewidlerpg2\Assets\Resource'
$LW = 270; $LH = 480; $SC = 3

$fonts = New-Object System.Drawing.Text.PrivateFontCollection
$fonts.AddFontFile("$game\Font\DNFBitBitv2.ttf")
$family = $fonts.Families[0]

# 2x2 칸을 다수결로 1칸으로 줄여 진짜 1px 도트로 만든다
function Get-Half($path) {
  $src = New-Object System.Drawing.Bitmap $path
  $out = New-Object System.Drawing.Bitmap ([int]($src.Width / 2)), ([int]($src.Height / 2))
  for ($y = 0; $y -lt $out.Height; $y++) { for ($x = 0; $x -lt $out.Width; $x++) {
    $v = @($src.GetPixel(2*$x, 2*$y).ToArgb(), $src.GetPixel(2*$x+1, 2*$y).ToArgb(), $src.GetPixel(2*$x, 2*$y+1).ToArgb(), $src.GetPixel(2*$x+1, 2*$y+1).ToArgb())
    $m = ($v | Group-Object | Sort-Object Count -Descending | Select-Object -First 1).Name
    $out.SetPixel($x, $y, [System.Drawing.Color]::FromArgb([int]$m))
  } }
  $src.Dispose(); return $out
}

function New-Case($name, $sheetPath, $textHex, $lineHex, $barHex) {
  $half = Get-Half $sheetPath
  $half.Save("$dir\source\$name`_half.png")
  $cv = New-Object System.Drawing.Bitmap $LW, $LH
  $g = [System.Drawing.Graphics]::FromImage($cv)

  # 배경: 게임 배경 가운데를 9:16으로 잘라 넣음
  $bg = New-Object System.Drawing.Bitmap "$game\Textures\Bg\bg_world_2.png"
  $g.InterpolationMode = 'HighQualityBicubic'
  $g.DrawImage($bg, (New-Object System.Drawing.Rectangle 0, 0, $LW, $LH), (New-Object System.Drawing.Rectangle 448, 0, 1152, 2048), [System.Drawing.GraphicsUnit]::Pixel)
  $bg.Dispose()
  $g.InterpolationMode = 'NearestNeighbor'; $g.PixelOffsetMode = 'Half'

  function Put($img, $sx, $sy, $sw, $sh, $dx, $dy) {
    $g.DrawImage($img, (New-Object System.Drawing.Rectangle $dx, $dy, $sw, $sh), (New-Object System.Drawing.Rectangle $sx, $sy, $sw, $sh), [System.Drawing.GraphicsUnit]::Pixel)
  }
  function PutSprite($path, $pivotX, $pivotY, $flip) {
    $s = New-Object System.Drawing.Bitmap $path
    if ($flip) { $s.RotateFlip('RotateNoneFlipX') }
    Put $s 0 0 128 128 ($pivotX - 64) ($pivotY - 112); $s.Dispose()
  }
  $shadow = New-Object System.Drawing.Bitmap "$art\common\shadow\shadow.png"

  # 전투 장면 (캐릭터 1px = UI 1px)
  foreach ($p in @(@(92, 262), @(132, 270), @(200, 250), @(214, 300), @(70, 330))) { Put $shadow 0 0 40 10 ($p[0] - 20) ($p[1] - 5) }
  PutSprite "$art\pets\pet_hood\hood_001\animations\idle\frames\frame_000.png" 92 262 $false
  PutSprite "$art\girl_base\animations\idle\frames\frame_000.png" 128 270 $false
  PutSprite "$art\monsters\mob_slime_pink\animations\idle\frames\frame_000.png" 200 250 $true
  PutSprite "$art\monsters\mob_slime_blue\animations\idle\frames\frame_000.png" 214 300 $true
  PutSprite "$art\monsters\mob_slime\animations\idle\frames\frame_000.png" 70 330 $false

  # 하단 탭 바탕 (시트에 없는 부분이라 단색 띠)
  $bar = [System.Drawing.ColorTranslator]::FromHtml($barHex)
  $g.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(215, $bar))), 0, 444, $LW, 36)
  $g.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml($lineHex))), 0, 443, $LW, 1)

  # HUD 부품 (절반 시트 좌표)
  Put $half 80 85 40 40 4 4                       # 초상화 틀
  $girl = New-Object System.Drawing.Bitmap "$art\girl_base\animations\idle\frames\frame_000.png"
  for ($y = -13; $y -le 13; $y++) { for ($x = -13; $x -le 13; $x++) {
    if ($x * $x + $y * $y -gt 170) { continue }
    $c = $girl.GetPixel(62 + $x, 70 + $y)
    if ($c.A -gt 128) { $cv.SetPixel(24 + $x, 24 + $y, $c) }
  } }
  $girl.Dispose()
  Put $half 5 2 75 20 190 6                       # 재화 판 + 동전
  Put $half 3 28 122 20 74 34                     # 스테이지 판
  Put $half 64 55 27 26 238 64                    # 우편
  Put $half 92 55 26 26 238 96                    # 랭킹
  Put $half 3 88 54 25 108 364                    # 보스 도전 버튼
  foreach ($i in 0..4) { Put $half (3 + 30 * ($i % 2)) 53 30 30 (20 + 32 * $i) 402 }
  $fire = New-Object System.Drawing.Bitmap "$art\ui\status_icons\status_burn.png"; Put $fire 0 0 16 16 27 409; $fire.Dispose()
  Put $half 3 88 54 25 196 405                    # AUTO 버튼
  $g.Dispose()

  # 3배로 키운 뒤 글자를 얹는다
  $big = New-Object System.Drawing.Bitmap ($LW * $SC), ($LH * $SC)
  $g2 = [System.Drawing.Graphics]::FromImage($big)
  $g2.InterpolationMode = 'NearestNeighbor'; $g2.PixelOffsetMode = 'Half'
  $g2.DrawImage($cv, 0, 0, $LW * $SC, $LH * $SC)
  $g2.PixelOffsetMode = 'Default'
  $g2.TextRenderingHint = 'SingleBitPerPixelGridFit'
  $fill = New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml($textHex))
  $line = New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml($lineHex))
  function Say($text, $cx, $y, $px, $align) {
    $f = New-Object System.Drawing.Font $family, ($px * $SC), ([System.Drawing.FontStyle]::Regular), ([System.Drawing.GraphicsUnit]::Pixel)
    $w = $g2.MeasureString($text, $f).Width
    $x = $cx * $SC; if ($align -eq 'c') { $x -= $w / 2 } elseif ($align -eq 'r') { $x -= $w }
    foreach ($o in @(@(-1,0),@(1,0),@(0,-1),@(0,1),@(-1,-1),@(1,1),@(-1,1),@(1,-1))) { $g2.DrawString($text, $f, $line, ($x + $o[0] * $SC), ($y * $SC + $o[1] * $SC)) }
    $g2.DrawString($text, $f, $fill, $x, ($y * $SC)); $f.Dispose()
  }
  Say '소라' 48 6 9 'l'
  Say 'Lv.12' 48 17 7 'l'
  Say '12,480' 262 10 8 'r'
  Say 'Stage 1-7' 135 38 9 'c'
  Say '보스 도전' 135 370 9 'c'
  Say 'AUTO' 223 411 8 'c'
  $tabs = '성장', '후드', '스킬', '둥지', '상점'
  foreach ($i in 0..4) { Say $tabs[$i] (27 + 54 * $i) 456 9 'c' }
  $g2.Dispose()
  $big.Save("$dir\review\$name`_x3.png")
  $big.Dispose(); $cv.Dispose(); $half.Dispose()
  "saved: $dir\review\$name`_x3.png"
}

New-Case 'hud_caseA_ownteam' "$dir\source\hud_caseA_ownteam_sheet.png" '#FFFFFF' '#1B1D26' '#1B1D26'
New-Case 'hud_caseB_other'   "$dir\source\hud_caseB_other_sheet.png"   '#F8E0C8' '#2A1A14' '#402820'
