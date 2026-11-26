# HANDOFF — UI: 상태이상 아이콘 (status_icons)

확정일: 2026-11-26. **이 폴더에는 확정본만 둡니다.**

## 한 줄 요약
몬스터(또는 캐릭터) 머리 위에 뜨는 상태이상 표식. **테두리 없는 색 기호** 방식(B안): 효과마다 다른 색의 단순한 기호 + 진한 코코아 외곽선 + 밝은 포인트 한두 칸. 우리 게임 팔레트(크림·코코아·산호·청록·금색)에서 뽑은 색.

## 폴더 구성
```
ArtSource/ui/status_icons/
├─ HANDOFF.md
├─ status_devour.png   ← 포식 (후드 얼굴: 자두색 후드 + 금빛 눈 + 이빨 미소)
├─ status_poison.png   ← 독 (초록 해골)
├─ status_burn.png     ← 화상 (산호 불꽃, 안쪽 노랑)
├─ status_freeze.png   ← 빙결 (청록 얼음 결정, 흰 반짝임)
├─ status_stun.png     ← 기절 (금색 별)
├─ status_bleed.png    ← 출혈 (빨간 핏방울, 분홍 하이라이트)
├─ status_icons_sheet.png    ← 위 6개 가로 시트 (16px 칸, 순서: devour, poison, burn, freeze, stun, bleed)
└─ status_icons_preview.png  ← 확대본 + 슬라임 머리 위 1·3·4개 겹친 예시
```
모두 **16×16**, 투명 배경, 정지 이미지(애니메이션 없음).

## 엔진 연결
1. 머리 위 가로 중앙, 몬스터 머리 꼭대기에서 2~3px 위에 띄운다
2. 여러 개면 가로로 나란히, 아이콘 간격 13px(외곽선이 1px씩 겹치게) 정도가 보기 좋음. 가운데 정렬
3. 좌우 반전하지 않는다
4. 포식 표식은 후드가 대상으로 정하면 띄우고, 후드 devour frame_005(몬스터를 숨기는 순간)에 같이 지운다
5. 깜빡임·둥실거림이 필요하면 엔진에서 처리 (스케일 1.0↔1.1 또는 y ±1px)

## 색 (기호 / 포인트 / 외곽선 공통 #3A1E1A)
| 효과 | 기호색 | 포인트 |
|---|---|---|
| 포식 | #8A5A70 | #F5B547 (눈) |
| 독 | #8DB255 | #3A1E1A (눈·코·이) |
| 화상 | #EE7A5C | #FFD36B |
| 빙결 | #6CC6BE | #E8FFFB |
| 기절 | #EDB948 | #FFF1B8 |
| 출혈 | #CF4A40 | #FFB3A6 |

## 새 아이콘 추가하는 방법
PixelLab 없이 직접 그린다 (무료). pixl_test `assets\pet\pet_hood\work\tools\StatusIcon.cs`의 `Render(map, 저장경로, 16, false, 기호색, 기호색, 포인트색, "#3A1E1A")`에 12×12 이하 ASCII 지도(`#` 기호, `x` 외곽선색, `a` 포인트, `.` 빈칸)를 넣으면 외곽선까지 자동으로 그려진다. 기존 지도는 pixl_test `assets\ui\status_icons\work\make_icons_v2.ps1`에 있음.
기호는 12×12 안에 꽉 차게, 굵게(가는 1px 선은 외곽선 때문에 복잡해 보임).

## 알려진 점
- 동그란 배지형(A안)도 만들어 봤으나 B안으로 확정. A안 파일은 pixl_test `assets\ui\status_icons\work\st_*.png`
