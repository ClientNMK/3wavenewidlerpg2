# 평평한 창 부품 직접 찍기 (UIBOWL_NOTES 원칙: 얇은 틀, 명도 3단계, 강조 노랑 하나) + 바텀 패널 조립 미리보기
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$dir  = Split-Path $PSScriptRoot -Parent
$art  = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource'
$game = 'C:\3wavestudio\3wavenewidlerpg2\3wavenewidlerpg2\Assets\Resource'
# 'warm': 크림 카드 + 노란 선택 (레츠고 레전드 톤), 'cute': warm + 2px 외곽선 + 창 모서리 껍데기 (Dino Mutant 구조)
# 'pop': 새 팔레트 (Dino Mutant 흰 카드·굵은 외곽선 + 레츠고 레전드 주황 모래 바탕·노랑·초록)
$theme = $args[0]
$pop = $theme -eq 'pop'
$warm = $theme -eq 'warm' -or $theme -eq 'cute'
$cute = $theme -eq 'cute' -or $pop
$ow = if ($cute) { 2 } else { 1 }
$suffix = if ($theme) { "_$theme" } else { '' }
$out  = "$dir\parts_flat$suffix"; New-Item -ItemType Directory -Force $out | Out-Null
function C($h) { [System.Drawing.ColorTranslator]::FromHtml($h) }
$K = C '#2A1A14'; $D1 = C '#402820'; $D2 = C '#583830'; $D3 = C '#7A5040'; $D4 = C '#A87850'
$Y0 = C '#B87828'; $Y1 = C '#EDB948'; $Y2 = C '#FFF1B8'; $R1 = C '#A03830'; $CR = C '#F8E0C8'; $CS = C '#D8C0A8'
# 부품별 색: 창 채움/테두리, 줄 채움/윗선/아랫선/홈, 탭 on/off, seg on/off, 글자(이름, 레벨, 수치, 탭 on, 탭 off)
if ($pop) {
  $K = C '#3A2A20'; $W0 = C '#FFFBF2'; $W1 = C '#F1E6D2'; $S0 = C '#F2C48A'; $S1 = C '#E3A15C'; $S2 = C '#C27D45'; $B0 = C '#6B4632'
  $Y0 = C '#E89A2C'; $Y1 = C '#FFD34E'; $Y2 = C '#FFF0A8'; $G1 = C '#8CCB4F'; $R1 = C '#E8533F'
  $D1 = $S2; $D2 = C '#D9CDB8'; $D3 = C '#D9CDB8'; $D4 = $W1; $CR = $W0; $CS = $W1
  $pf = $S1; $pr = $S0; $rf = $W0; $rt = $null; $rb = $W1; $rh = $W1
  $ton = @($Y1, $Y2, $Y0); $toff = @($S0, $null, $S2); $son = @($Y1, $Y2, $Y0); $soff = @($S2, $null, $null)
  $tx = @('#3A2A20', '#C27D45', '#5E9A35', '#3A2A20', '#6B4632')
} elseif ($warm) {
  $pf = $D2; $pr = $D3; $rf = $CR; $rt = $null; $rb = $CS; $rh = $D4
  $ton = @($Y1, $Y2, $Y0); $toff = @($D3, $D4, $null); $son = @($Y1, $Y2, $Y0); $soff = @($D1, $null, $null)
  $tx = @('#402820', '#7A5040', '#A03830', '#402820', '#D8C0A8')
} else {
  $pf = $K; $pr = $D2; $rf = $D2; $rt = $D3; $rb = $D1; $rh = $K
  $ton = @($D2, $Y1, $null); $toff = @($D1, $null, $null); $son = @($D3, $D4, $Y1); $soff = @($K, $null, $D1)
  $tx = @('#F8E0C8', '#A87850', '#EDB948', '#F8E0C8', '#A87850')
}

function New-Part($w, $h) { New-Object System.Drawing.Bitmap $w, $h }
function Fill($b, $x, $y, $w, $h, $c) { for ($j = $y; $j -lt $y + $h; $j++) { for ($i = $x; $i -lt $x + $w; $i++) { $b.SetPixel($i, $j, $c) } } }
# 모서리 깎기 (외곽선 2px이면 모서리마다 3칸)
function Cut($b) {
  $pts = if ($ow -eq 2) { @(@(0,0), @(1,0), @(0,1)) } else { @(@(0,0)) }
  foreach ($p in $pts) { foreach ($q in @(@($p[0], $p[1]), @(($b.Width-1-$p[0]), $p[1]), @($p[0], ($b.Height-1-$p[1])), @(($b.Width-1-$p[0]), ($b.Height-1-$p[1])))) { $b.SetPixel($q[0], $q[1], [System.Drawing.Color]::Transparent) } }
}
# 판: 바깥 외곽선($ow px), 안쪽 채움, 위 1px 밝은 선, 아래 1px 어두운 선, 모서리 깎음
function Plate($w, $h, $fill, $top, $bot) {
  $b = New-Part $w $h; Fill $b 0 0 $w $h $K; Fill $b $ow $ow ($w-2*$ow) ($h-2*$ow) $fill
  if ($top) { Fill $b $ow $ow ($w-2*$ow) 1 $top }; if ($bot) { Fill $b $ow ($h-1-$ow) ($w-2*$ow) 1 $bot }
  Cut $b; return $b
}
$P = [ordered]@{}
# 창 바탕: 가장 어두움, 안쪽 1px 테두리선
if ($cute) {
  # 40x40 판(바깥 5px 여백) + 네 모서리에 11px 크림 껍데기(산호 반점)
  $b = New-Part 40 40; $in = Plate 30 30 $pf $null $null; Fill $in 2 2 26 1 $pr; Fill $in 2 27 26 1 $pr; Fill $in 2 2 1 26 $pr; Fill $in 27 2 1 26 $pr
  $g0 = [System.Drawing.Graphics]::FromImage($b); $g0.DrawImage($in, 5, 5, 30, 30); $g0.Dispose()
  $shell = '...KKKKK...', '..KCCCCCK..', '.KCCRCCCSK.', 'KCCCCCRCCSK', 'KCRCCCCCCSK', 'KCCCCRCCCSK', 'KCCCCCCCSSK', '.KSCCCCCSK.', '..KSSSSSK..', '...KKKKK...', '...........'
  $map = @{ K = $K; C = $CR; S = $CS; R = (C '#EE7A5C') }
  foreach ($o in @(@(0,0), @(29,0), @(0,29), @(29,29))) { for ($y = 0; $y -lt 11; $y++) { for ($x = 0; $x -lt 11; $x++) { $ch = [string]$shell[$y][$x]; if ($ch -ne '.') { $b.SetPixel($o[0] + $x, $o[1] + $y, $map[$ch]) } } } }
  $P.panel_window = @($b, '13 13 13 13')
} else {
  $b = Plate 16 16 $pf $null $null; Fill $b 1 1 14 1 $pr; Fill $b 1 14 14 1 $pr; Fill $b 1 1 1 14 $pr; Fill $b 14 1 1 14 $pr; $P.panel_window = @($b, '3 3 3 3')
}
# 목록 줄: 한 단계 밝음, 왼쪽 아이콘 홈
$hole = 36 - 2 * ($ow + 1)
$b = Plate 64 36 $rf $rt $rb; Fill $b ($ow + 1) ($ow + 1) $hole $hole $rh; $P.list_row = @($b, '35 3 0 0')
# 강화 버튼: 유일한 강조색
$b = Plate 40 26 $Y1 $Y2 $Y0; Fill $b $ow (24 - $ow) (40 - 2 * $ow) 1 $Y0; $P.btn_upgrade = @($b, '3 3 0 0')
$b = Plate 40 26 $D3 $D4 $D2; Fill $b $ow (24 - $ow) (40 - 2 * $ow) 1 $D2; $P.btn_upgrade_off = @($b, '3 3 0 0')
# 탭: 선택은 노란 윗줄만
$b = Plate 32 16 $ton[0] $ton[1] $ton[2]; $P.tab_on = @($b, '3 3 0 0')
$b = Plate 32 16 $toff[0] $toff[1] $toff[2]; $P.tab_off = @($b, '3 3 0 0')
# x1/x10/x100
$b = Plate 32 14 $son[0] $son[1] $son[2]; $P.seg_on = @($b, '3 3 0 0')
$b = Plate 32 14 $soff[0] $soff[1] $soff[2]; $P.seg_off = @($b, '3 3 0 0')
# 아이콘 칸, 닫기
$b = Plate 20 20 $K $D1 $null; $P.slot_icon = @($b, '3 3 3 3')
$b = Plate 13 13 $R1 $null $null; foreach ($i in 0..4) { $b.SetPixel(4+$i, 4+$i, $CR); $b.SetPixel(8-$i, 4+$i, $CR) }; $P.btn_close = @($b, '-')
foreach ($n in $P.Keys) { $P[$n][0].Save("$out\$n.png"); '{0,-16} {1,2}x{2,-2}  9-slice {3}' -f $n, $P[$n][0].Width, $P[$n][0].Height, $P[$n][1] }

# ---- 조립 미리보기: 360x640 논리 크기, 2배 ----
$LW = 360; $LH = 640; $SC = 2
function Load($path) { $t = New-Object System.Drawing.Bitmap $path; $c = New-Object System.Drawing.Bitmap $t; $t.Dispose(); return $c }
function Draw9($img, $s, $dx, $dy, $dw, $dh) {
  $v = $s -split ' ' | ForEach-Object { [int]$_ }; $bl = $v[0]; $br = $v[1]; $bt = $v[2]; $bb = $v[3]
  $sx = @(0, $bl, ($img.Width - $br), $img.Width); $sy = @(0, $bt, ($img.Height - $bb), $img.Height)
  $tx = @($dx, ($dx + $bl), ($dx + $dw - $br), ($dx + $dw)); $ty = @($dy, ($dy + $bt), ($dy + $dh - $bb), ($dy + $dh))
  for ($j = 0; $j -lt 3; $j++) { for ($i = 0; $i -lt 3; $i++) {
    $sw = $sx[$i+1] - $sx[$i]; $sv = $sy[$j+1] - $sy[$j]; $tw = $tx[$i+1] - $tx[$i]; $tv = $ty[$j+1] - $ty[$j]
    if ($sw -le 0 -or $sv -le 0 -or $tw -le 0 -or $tv -le 0) { continue }
    $g.DrawImage($img, (New-Object System.Drawing.Rectangle $tx[$i], $ty[$j], $tw, $tv), (New-Object System.Drawing.Rectangle $sx[$i], $sy[$j], $sw, $sv), [System.Drawing.GraphicsUnit]::Pixel)
  } }
}
function Put($img, $x, $y) { $g.DrawImage($img, $x, $y, $img.Width, $img.Height) }
$cv = New-Object System.Drawing.Bitmap $LW, $LH
$g = [System.Drawing.Graphics]::FromImage($cv)
$bg = Load "$game\Textures\Bg\bg_world_2.png"
$g.InterpolationMode = 'HighQualityBicubic'
$g.DrawImage($bg, (New-Object System.Drawing.Rectangle 0, 0, $LW, $LH), (New-Object System.Drawing.Rectangle 448, 0, 1152, 2048), [System.Drawing.GraphicsUnit]::Pixel)
$g.InterpolationMode = 'NearestNeighbor'; $g.PixelOffsetMode = 'Half'
$shadow = Load "$art\common\shadow\shadow.png"
foreach ($sp in @(@("$art\girl_base\animations\idle\frames\frame_000.png", 150, 250, $false), @("$art\monsters\mob_slime_pink\animations\idle\frames\frame_000.png", 240, 240, $true))) {
  Put $shadow ($sp[1] - 20) ($sp[2] - 5); $img = Load $sp[0]; if ($sp[3]) { $img.RotateFlip('RotateNoneFlipX') }; Put $img ($sp[1] - 64) ($sp[2] - 112); $img.Dispose()
}
$icons = @(); foreach ($n in 'icon_atk','icon_hp','icon_def','icon_armor') { $icons += , (Load "$dir\parts_growth\$n.png") }
$texts = @()
if ($cute) { Draw9 $P.panel_window[0] $P.panel_window[1] (-1) 294 362 352 } else { Draw9 $P.panel_window[0] $P.panel_window[1] 0 300 360 340 }
Put $P.btn_close[0] 330 284
$tabNames = '강화', '레벨', '전직', '어빌리티'
foreach ($i in 0..3) { $t = if ($i -eq 0) { 'tab_on' } else { 'tab_off' }; Draw9 $P[$t][0] $P[$t][1] (6 + 87 * $i) 306 85 16; $texts += , @($tabNames[$i], (48 + 87 * $i), 309, 8, $(if ($i -eq 0) { $tx[3] } else { $tx[4] }), 'c') }
foreach ($i in 0..2) { $t = if ($i -eq 0) { 'seg_on' } else { 'seg_off' }; Draw9 $P[$t][0] $P[$t][1] (6 + 117 * $i) 326 114 14; $texts += , @(('x1', 'x10', 'x100')[$i], (63 + 117 * $i), 328, 8, $(if ($i -eq 0) { $tx[3] } else { $tx[4] }), 'c') }
$stats = @(@('공격력', 'Lv.13만', '562만 > 712만', '45양'), @('체력', 'Lv.11만', '57만 > 57만', '2067자'), @('체력 재생', 'Lv.12만', '1194자 > 1195자', '2774자'), @('방어력', 'Lv.14만', '14만 > 14만', '1961자'), @('관통', 'Lv.16만', '16만 > 16만', '55양'), @('치명타', 'Lv.9만', '120% > 121%', '3양'))
foreach ($i in 0..5) {
  $ry = 346 + 40 * $i
  Draw9 $P.list_row[0] $P.list_row[1] 6 $ry 348 36
  $g.DrawImage($icons[$i % 4], (7 + $ow), ($ry + $ow + 1), $hole, $hole)
  $bt = if ($i -eq 5) { 'btn_upgrade_off' } else { 'btn_upgrade' }
  Draw9 $P[$bt][0] $P[$bt][1] 278 ($ry + 5) 72 26
  $texts += , @($stats[$i][0], 50, ($ry + 4), 9, $tx[0], 'l')
  $texts += , @($stats[$i][1], 50, ($ry + 20), 7, $tx[1], 'l')
  $texts += , @($stats[$i][2], 140, ($ry + 20), 7, $tx[2], 'l')
  $texts += , @('강화', 314, ($ry + 7), 7, '#402820', 'c')
  $texts += , @($stats[$i][3], 314, ($ry + 18), 6, '#402820', 'c')
}
$g.Dispose()
$fonts = New-Object System.Drawing.Text.PrivateFontCollection; $fonts.AddFontFile("$game\Font\DNFBitBitv2.ttf"); $family = $fonts.Families[0]
$big = New-Object System.Drawing.Bitmap ($LW * $SC), ($LH * $SC)
$g2 = [System.Drawing.Graphics]::FromImage($big); $g2.InterpolationMode = 'NearestNeighbor'; $g2.PixelOffsetMode = 'Half'
$g2.DrawImage($cv, 0, 0, $LW * $SC, $LH * $SC)
$g2.PixelOffsetMode = 'Default'; $g2.TextRenderingHint = 'SingleBitPerPixelGridFit'
foreach ($t in $texts) {
  $f = New-Object System.Drawing.Font $family, ($t[3] * $SC), ([System.Drawing.FontStyle]::Regular), ([System.Drawing.GraphicsUnit]::Pixel)
  $x = $t[1] * $SC; if ($t[5] -eq 'c') { $x -= $g2.MeasureString($t[0], $f).Width / 2 }
  $g2.DrawString($t[0], $f, (New-Object System.Drawing.SolidBrush (C $t[4])), $x, ($t[2] * $SC)); $f.Dispose()
}
$big.Save("$dir\review\flat_kit$suffix`_assembled_x2.png")
