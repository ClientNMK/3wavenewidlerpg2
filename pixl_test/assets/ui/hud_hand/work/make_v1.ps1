# 손으로 찍은 도트 UI 샘플 v1: 재화 판, 주 버튼, 슬롯 (+ 미리보기용 동전·숫자)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
Add-Type -Path "$PSScriptRoot\Px.cs" -ReferencedAssemblies System.Drawing
$root  = Split-Path $PSScriptRoot -Parent
$art   = 'C:\3wavestudio\3wavenewidlerpg2\ArtSource'
$parts = "$root\parts"; $review = "$root\review"
New-Item -ItemType Directory -Force $parts, $review | Out-Null

# 팔레트 (캐릭터·상태 아이콘에서 가져온 색)
$K='#2A1A14'; $D1='#402820'; $D2='#583830'; $D3='#7A5040'
$C1='#D8C0A8'; $C2='#F8E0C8'
$G1='#B87828'; $G2='#EDB948'; $G3='#FFF1B8'
$R1='#7A2824'; $R2='#A03830'; $R3='#EE7A5C'
$pal = "K=$K;d=$D1;D=$D2;e=$D3;c=$C1;C=$C2;g=$G1;G=$G2;L=$G3;r=$R1;R=$R2;o=$R3"

# ---- 1. 재화 판 72x18: 원피스 색 판 + 크림 프릴 단 ----
$plate = New-Object Px 72, 18
$b = $plate.RRect(0, 0, 72, 14, @(2, 1)); $plate.Fill($b, $K)
$i = $plate.Erode($b);                    $plate.Fill($i, $D1)
$plate.Fill($plate.Edge($i, -1), $D2)
for ($x = 3; $x -le 63; $x += 6) {
  $plate.Art($x, 14, @('CCCCCK', 'cccccK', 'KcccK', ' KKK'), $pal)
}
$plate.Dot(2, 14, $K); $plate.Dot(2, 15, $K)
$plate.Save("$parts\plate_currency.png", 1)

# ---- 2. 주 버튼 80x21: 리본 빨강 + 금단추, 아래 3px는 눌리는 두께 ----
$btn = New-Object Px 80, 21
$o = $btn.RRect(0, 0, 80, 21, @(2, 1)); $btn.Fill($o, $K)
$btn.Fill($btn.Erode($o), $R1)
$f = $btn.Erode($btn.RRect(0, 0, 80, 18, @(2, 1))); $btn.Fill($f, $R2)
$btn.Fill($btn.Edge($f, -1), $R3)
$stud = @(' KKK ', 'KLGGK', 'KGGgK', 'KGggK', ' KKK ')
$btn.Art(5, 6, $stud, $pal); $btn.Art(70, 6, $stud, $pal)
$btn.Save("$parts\btn_main.png", 1)

# ---- 3. 슬롯 26x26: 알껍데기 테 + 안쪽 위에 이빨 ----
$slot = New-Object Px 26, 26
$o = $slot.RRect(0, 0, 26, 26, @(2, 1)); $slot.Fill($o, $K)
$rim = $slot.Erode($o); $slot.Fill($rim, $C2)
for ($y = 0; $y -lt 26; $y++) { for ($x = 0; $x -lt 26; $x++) { if ($rim[$x, $y] -and ($x + $y) -gt 26) { $slot.Dot($x, $y, $C1) } } }
$w = $slot.RRect(3, 3, 20, 20, @(1)); $slot.Fill($w, $K)
$wi = $slot.Erode($w); $slot.Fill($wi, $D1)
$slot.Fill($slot.Edge($wi, 1), $D2)
foreach ($x in 5, 9, 13, 17) { $slot.Art($x, 4, @('CCC', ' c '), $pal) }
$slot.Rect(1, 18, 2, 2, $R3)
$slot.Save("$parts\slot.png", 1)

# ---- 미리보기용: 동전 15x15, 3x5 숫자 ----
$coin = New-Object Px 15, 15
for ($y = 0; $y -lt 15; $y++) { for ($x = 0; $x -lt 15; $x++) {
  $d = [math]::Sqrt(($x - 7) * ($x - 7) + ($y - 7) * ($y - 7))
  if ($d -le 7.3) { $coin.Dot($x, $y, $K) }
  if ($d -le 6.2) {
    $c = $G2
    if ($d -gt 4.4 -and ($x + $y) -lt 12) { $c = $G3 }
    if ($d -gt 4.4 -and ($x + $y) -gt 16) { $c = $G1 }
    $coin.Dot($x, $y, $c)
  }
} }
$coin.Art(5, 5, @('  g  ', ' gLg ', 'gLgLg', ' gLg ', '  g  '), $pal)
$coin.Save("$parts\icon_gold.png", 1)

$font = @{
 '0'=@('CCC','C C','C C','C C','CCC'); '1'=@(' C ','CC ',' C ',' C ','CCC'); '2'=@('CCC','  C','CCC','C  ','CCC')
 '4'=@('C C','C C','CCC','  C','  C'); '8'=@('CCC','C C','CCC','C C','CCC'); ','=@('   ','   ','   ',' C ','C  ')
}
function Draw-Num($px, $text, $xRight, $y) {
  $x = $xRight - ($text.Length * 4 - 1)
  foreach ($ch in $text.ToCharArray()) { $px.Art($x, $y, $font["$ch"], $pal); $x += 4 }
}

# ---- 미리보기: 밝은 모래 / 어두운 바닥, 캐릭터와 같은 배율(3배) ----
$pv = New-Object Px 400, 132
$pv.Rect(0, 0, 200, 132, '#CDB58A'); $pv.Rect(200, 0, 200, 132, '#2B2630')
foreach ($ox in 0, 200) {
  $pv.PasteFile("$art\common\shadow\shadow.png", $ox + 28, 109, $false)
  $pv.PasteFile("$art\girl_base\animations\idle\frames\frame_000.png", $ox - 16, 2, $false)
  $pv.Paste($plate, $ox + 112, 10); $pv.Paste($coin, $ox + 107, 9); Draw-Num $pv '12,480' ($ox + 178) 14
  $pv.Paste($btn, $ox + 108, 40)
  $pv.Paste($slot, $ox + 108, 74); $pv.Paste($slot, $ox + 136, 74); $pv.Paste($slot, $ox + 164, 74)
  $pv.PasteFile("$art\ui\status_icons\status_burn.png", $ox + 113, 80, $false)
}
$pv.Save("$review\hud_hand_v1_x3.png", 3)
"saved: $review\hud_hand_v1_x3.png"
