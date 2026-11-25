# HANDOFF — 아이템: 골드 동전 (item_gold_coin)

확정일: 2026-11-26. **이 폴더에는 확정본만 둡니다.**

## 한 줄 요약
몬스터가 떨어뜨리는 골드 획득 아이템. 세로로 선 작은 금화 1개, 가운데 마름모 문양. 가로(세로축 기준)로 도는 반복 애니메이션.

## 폴더 구성
```
ArtSource/items/item_gold_coin/
├─ HANDOFF.md
├─ pose/item_gold_coin.png            ← 정지 이미지, 32×32 (동전 실제 크기 약 15×16)
├─ animations/spin/                   ← 32×32, 8프레임, 0.08초/프레임 반복
│  ├─ frames/frame_000~007.png        ← 정면 → 얇아짐 → 옆면 → 다시 정면
│  ├─ spin_strip.png                  ← 가로 스트립 (칸 32px, 8칸)
│  ├─ spin_x1.gif                     ← 원본 크기, 투명
│  └─ spin_preview_x6.gif             ← 확인용 6배, 베이지 배경
└─ coin_spin_scene_x3.gif             ← 메인 캐릭터 옆에 놓은 실제 크기 확인용
```

## 엔진 연결 방법
- 동전 중심은 32×32 기준 **(15, 16)**, 동전 바닥은 약 24행. 바닥에 놓을 때는 pivot을 **(15, 24)** 로 두면 메인 캐릭터와 같은 바닥선(y=112)에 선다.
- spin은 반복 재생. 떨어질 때 통 튀는 움직임은 엔진에서 위치로 처리 (이미지 추가 없음).
- 위에서 내려다보는 각도라 한 방향만 있음. 좌우 반전 불필요.

## 만든 방법
1. 디자인: `create_object_pro_flash`, 32×32, 1방향, view = side. style_image = 같은 세션에서 만든 동전 더미(탈락) 32px 잘라낸 것. 설명: "one single small gold coin standing upright on its edge, seen from the front so the round face of the coin is fully visible…" (seed 7)
2. 회전: `animate_object` v3, 8프레임, keep_first_frame = false. 설명: "the gold coin spins one full turn around its vertical axis in place… Seamless loop, stays the same height and position"
3. 후처리: 프레임마다 위아래 1~2px 흔들려서 동전 중심을 (15, 16)에 맞춰 고정 (dy −1~+1)

## PixelLab ID
| 이름 | ID | 비고 |
|---|---|---|
| 동전 오브젝트 | `7a20d25d-c16a-4c00-9543-80f480d1c98d` | 마름모 문양 → **확정** |
| └ spin | group `3897a05d-7482-486e-b6ae-d2ddc5db1e35` | **확정** (중심 고정 후처리) |
| └ spin (탈락) | group `bf764473-cc77-4a7b-b833-e886fdc32958` | 뒤쪽 프레임에서 넓어졌다 납작해져 출렁임 |
| 별 문양 동전 (탈락) | `0578cb91-7a3d-4c1d-bbbc-9e919791513b` | |
| 동전 더미 (탈락) | `b82599b4-7c46-466e-874b-405f294a7965` | 48×48 |
| 동전 주머니 (탈락) | `2b997575-5663-4330-b60e-ce3e966f2479` | 48×48 |

## 알려진 점
- 엄밀히는 반 바퀴 돌고 다시 앞면이 나오는 방식. 앞뒷면 문양이 같아서 반복하면 계속 도는 것처럼 보임
