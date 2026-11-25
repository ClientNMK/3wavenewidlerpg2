# HANDOFF — 몬스터: 해초 슬라임 (mob_slime)

확정일: 2026-11-25. **이 폴더에는 확정본만 둡니다.**

## 한 줄 요약
반투명 청록 젤리 슬라임, 점 눈 두 개와 작은 입, 머리 위에 해초. 메인 캐릭터(소라게 후드 소녀)와 같은 화풍, 키는 메인 캐릭터의 절반쯤. 원거리 공격(젤리 구체 발사).

## 폴더 구성
```
ArtSource/monsters/mob_slime/
├─ HANDOFF.md
├─ pose/mob_slime_se.png             ← 기본 자세, 오른쪽 반측면, 96×96
├─ animations/                       ← 모두 128×128, 바닥선 y=112, 중심 x=64
│  ├─ idle/    frames 9장, 0.12초/프레임 (말랑하게 출렁임)
│  ├─ move/    frames 9장, 0.08초/프레임 (제자리 통통 튀기, 엔진에서 이동)
│  ├─ attack/  frames 9장, 0.08초/프레임 (움츠렸다가 젤리 뱉기)
│  └─ dead/    frames 9장, 0.10초/프레임 1회 (눌렸다 부풀고 젤리 방울로 터짐 → 해초만 남음, 2026-11-26)
├─ projectile/
│  ├─ fly/     frames 4장, 32×32, 0.08초/프레임 반복 (날아가는 젤리 구체)
│  └─ hit/     frames 7장, 48×48, 0.08초/프레임 1회 (명중 시 터짐, 마지막은 거의 빈 프레임)
└─ attack_projectile_demo_x2.gif     ← 뱉기 → 비행 → 명중 연결 예시 (320×128, 2배)
```
각 동작 폴더에 `<동작>_strip.png`(가로 스트립), `<동작>_x1.gif`(원본, 투명), `<동작>_preview_x3.gif`(확인용)가 있다.

## 엔진 연결 방법 (원거리 공격)
1. attack 재생, **frame_007(8번째)** 에서 구체 생성
2. 생성 위치: 슬라임 128 캔버스 기준 입 부근 **(90, 95)** → 구체 fly 프레임의 중심은 32×32 기준 (20, 16)
3. 구체를 목표 쪽으로 이동하며 fly 4프레임 반복
4. 명중하면 그 위치에 hit 재생 (hit 중심은 48×48 기준 (28, 24))
5. 왼쪽을 볼 때는 슬라임·구체·명중 모두 좌우 반전, 생성 x도 반전 (128−90=38)

## 규칙 (메인 캐릭터와 동일)
- **오른쪽 반측면(south-east) 한 방향만**. 왼쪽은 엔진에서 좌우 반전
- 128×128 캔버스, 바닥 111행(바닥선 y=112), 가로 중심 x=64, 엔진 pivot (64,112)

## PixelLab ID
| 이름 | ID | 비고 |
|---|---|---|
| 슬라임 오브젝트 | `f7d57e06-0094-4369-b423-1b5a41689f31` | create_object_pro_flash 96×96, 8방향, style_image = 메인 캐릭터 south-east |
| idle | group `9b2c3b76-e25f-4e5b-be16-e50e8d903dda` | animate_object v3, south-east 8프레임(+시작 1) |
| move | group `3dd68eb0-f654-49b1-9015-48ed01748ae8` | 〃 |
| attack | group `2e653b72-5c42-44d8-ac81-8af56a61caa7` | 〃, 마지막 프레임 입 옆 젤리 꼬리 지움 (x86~96, y90~99) |
| dead | group `02de044a-df6a-4542-8327-1611429d5349` | animate_object v3, south-east 8프레임(+시작 1), "squashes flat, swells up round, then bursts into small jelly droplets… leaving only the seaweed" → 정렬만 → **확정** |
| └ dead (탈락) | group `3ba5f840-b163-40df-a5c0-9804a4e7addf` | 녹아서 웅덩이로 퍼지는 버전 (웅덩이가 사라지지 않음) |
| 구체 오브젝트 | `9687ced4-5f8b-43c3-8243-6c5d7297c6d2` | 32×32, style_image = 슬라임 몸 32px 잘라낸 것 |
| └ fly | group `8435545b-b682-40da-ac65-973b3cca0771` | 4프레임(+시작 1), 확정본은 0~3 사용 |
| └ hit | group `a7fa737a-fbdd-4398-89fb-e6f755267f9a` | 시작 프레임을 48×48로 키워서 생성, 6프레임(+시작 1) |

정렬: 슬라임 96×96 원본을 128×128에 (dx=14, dy=32)로 배치 (발 바닥 79행 → 111행).

## 색 바꾼 버전
`../mob_slime_blue`, `../mob_slime_pink`, `../mob_slime_gold` — 이 폴더의 모든 프레임에서 몸 색만 HSL 색상 회전 (파랑 +75°, 분홍 +200°, 금색 −95°, 채도 ×2.2~2.6). 해초·광택·입은 그대로. 새 색이 필요하면 같은 방식으로 만들면 됨 (PixelLab 생성 불필요).

## 알려진 점
- idle에서 위로 꽤 길게 늘어남 (젤리 느낌으로 통과)
- 공격은 박치기가 아니라 뱉기 동작
- dead: 1회 재생 후 마지막 프레임(바닥의 해초)에서 멈추거나 엔진에서 페이드아웃. 방울·해초가 바닥선보다 1~3px 아래까지 퍼짐 (탑뷰라 의도된 정도)
- dead는 파랑·분홍·금색에도 같은 색상 회전으로 추가함 (2026-11-26). 마지막 두 프레임은 원본 그대로, 금색은 밝은 몸 픽셀 색상 고정. 색 변환 도구는 pixl_test의 `assets\monster\mob_slime\work\tools\SlimeRecolor.cs`
