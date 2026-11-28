# HANDOFF — 정령 펫: 후드 (pet_hood)

확정일: 2026-11-26 (디자인 교체: 어두운 그림자 A·B → 크림 망토 C1). **이 폴더에는 확정본만 둡니다.**

## 한 줄 요약
메인 캐릭터(소라게 후드 소녀)를 따라다니는 **크림색 후드 망토 정령**. 망토 안 얼굴은 어두운 그림자, 동그란 금빛 눈 두 개, 목에 빨간 리본과 금방울(메인 캐릭터 리본과 세트). 단계별로 진화하며, 몬스터를 통째로 삼키는 **포식(devour)** 동작이 있다.
lv1 크기: 약 38×41px (메인 캐릭터 허리쯤).

## 폴더 구성
```
ArtSource/pets/pet_hood/
├─ HANDOFF.md
├─ hood_001/                           ← lv1 (HoodChart Resorce = hood_001)
│  ├─ pose/hood_001_se.png             ← 기본 자세 (idle frame_000), 128×128
│  └─ animations/                      ← 모두 128×128, 바닥선 y=112, 중심 x=64
│     ├─ idle/    frames 8장, 0.12초/프레임 반복 (둥실둥실, 망토·방울 살랑)
│     └─ devour/  frames 13장, 0.08초/프레임 1회 (포식)
├─ hood_001_preview_x3.gif             ← idle 2회 → 포식 → idle 연결 확인용
├─ hood_002/                           ← lv2 (Resorce = hood_002, 2026-11-26): 후드 끝에 작은 뿔 두 개, 몸 약 37×47px. 구성은 hood_001과 같음
│  ├─ pose/hood_002_se.png
│  └─ animations/idle (8장, 0.12초), devour (13장, 0.08초)
├─ hood_002_preview_x3.gif
├─ hood_001_skin_shell/                ← 코스튬: 조개 모자 (hood_001 몸 + 산호 줄무늬 소라 껍데기 모자, 2026-11-26). 구성은 hood_001과 같음
│  ├─ pose/hood_001_skin_shell_se.png
│  └─ animations/idle (8장, 0.12초), devour (13장, 0.08초)
├─ hood_001_skin_shell_preview_x3.gif
├─ costum_11001/               ← 코스튬: 겨울 (빨간 뜨개 후드 + 흰 털 테두리 + 방울, 망토 빨강, 2026-11-28). 메인 캐릭터 `costum_10001`와 한 벌. 구성은 hood_001과 같음
│  ├─ pose/costum_11001_se.png
│  └─ animations/idle (8장, 0.12초), devour (13장, 0.08초)
├─ costum_11001_preview_x3.gif
└─ effects/
   ├─ devour_burst/                    ← hood_001 포식 이펙트 확정본 (2026-11-26): 호박색 반짝 링
   │  ├─ collect-burst.png / .json     ← 칸 112×108, 7열, 42프레임, 30fps, pivot (56, 94)
   │  ├─ frames/, devour_burst_x1.gif, devour_burst_demo_x3.gif
   │  └─ source/collect-burst.js, options.json
   ├─ move_trail/                      ← hood_001 이동 이펙트 확정본 (2026-11-26): 망토 끝에서 흘러나오는 반짝이 꼬리 (반복)
   │  ├─ hood-trail.png / .json        ← 칸 96×56, 6열, 60프레임(2초 반복), 30fps, pivot (64, 48)
   │  ├─ frames/, move_trail_x1.gif, move_trail_demo_x3.gif
   │  └─ source/hood-trail.js, options.json
   └─ levelup/                         ← hood_001 레벨업 이펙트 확정본 (2026-11-27): 머리 위로 빛이 모였다가 터짐 (1회)
      ├─ hood-levelup.png / .json      ← 칸 112×128, 49프레임(1.6초), 30fps, pivot (56, 112)
      ├─ frames/, levelup_x1.gif, levelup_demo_x3.gif
      └─ source/hood-levelup.js, options.json
```
**리소스 이름 규칙**: `Chart/HoodChart_Dev.xlsx`의 `Resorce` 열과 같게 레벨 순서대로 `hood_001`(lv1), `hood_002`(lv2), `hood_003`… 폴더 이름과 pose 파일 이름(`hood_00N_se.png`)을 맞춘다.

각 동작 폴더에 `<동작>_strip.png`(가로 스트립), `<동작>_x1.gif`(원본, 투명), `<동작>_preview_x3.gif`(확인용)가 있다.

**코스튬 이름 규칙**: `Chart/CostumeChart_Dev.xlsx`의 `Skin_Name`과 같게 (후드 코스튬은 Data_Id 11001~, 예: `costum_11001`). 그전에 만든 조개 모자 `hood_001_skin_shell`은 차트에 아직 없어 옛 이름 그대로.

포식 상태 표식(몬스터 머리 위 아이콘)은 상태이상 아이콘 체계로 확정: `ArtSource/ui/status_icons/status_devour.png` (16×16, 그쪽 HANDOFF 참고). 이전 이빨 입 표식(`devour_mark/`)은 색이 안 맞아 삭제함.

포식 타이밍(아래 표)은 hood_002·조개 모자 코스튬도 같다 (frame_005에서 몬스터 숨김, frame_012 = idle frame_000).

## 포식(devour) 엔진 연결
동작 흐름: 움츠림 → 높이 솟구치며 후드를 크게 벌림 → 쿵 내려앉으며 꿀꺽 → 우물우물 → 원래 자세.

| 구간 | 프레임 |
|---|---|
| 움츠림 | frame_000~001 |
| 솟구침·입 벌림 | frame_002~004 |
| 덮침(삼키는 순간) | **frame_005** |
| 눌림·우물우물 | frame_006~009 |
| 복귀 | frame_010~012 (frame_012 ≈ idle frame_000, 바로 idle로) |

1. devour 재생 시작과 동시에 대상 몬스터를 후드 앞쪽으로 끌어당긴다
2. **솟구침 구간(002~004)** 에 몬스터를 입 쪽(128 캔버스 기준 대략 (66, 88))으로 이동하면서 크기를 줄인다
3. **frame_005** 에서 몬스터를 숨긴다(삭제)
4. 끝나면 idle로 돌아간다. 왼쪽을 볼 때는 좌우 반전 (입 x도 128−66=62)

프레임 시간: idle 0.12초, 포식 0.08초 (이전 후드와 같음).

## 이펙트 엔진 연결 (hood_001, 2026-11-26 확정)
이펙트는 후드와 별도 스프라이트. 왼쪽을 볼 때는 후드와 함께 좌우 반전(x 오프셋도 반대로).

| 이펙트 | 파일 | 위치 (이펙트 pivot을 놓는 곳) | 그리는 순서 | 재생 |
|---|---|---|---|---|
| 포식 반짝 링 | `effects/devour_burst/collect-burst.png/.json` | 후드 pivot (64,112)에서 **오른쪽 6px, 아래 19px** = 128 캔버스 (70, 131) | 후드 **앞** | devour **frame_005(0.40초)** 에 30fps 1회 |
| 이동 꼬리 | `effects/move_trail/hood-trail.png/.json` | 후드 pivot에서 **왼쪽 4px** = (60, 112) | 후드 **뒤** | 후드가 움직이는 동안 30fps 반복, 멈추면 끔 (끌 때 0.3초쯤 페이드하면 자연스러움) |
| 레벨업 | `effects/levelup/hood-levelup.png/.json` | 후드 pivot (64,112)에 그대로 | 후드 **앞** | 레벨업 버튼을 누를 때 idle 그대로 30fps 1회 (포식 동작 대신). 터지는 순간 = 0.28초(frame_008) |

- 포식 링: pixelfx `pickups/collect-burst`를 색만 호박·금색으로 바꿈(코드 수정 없음). 옵션 `source/options.json`. 링이 몸통 크기보다 조금 크게 퍼지는 게 의도
- 이동 꼬리: 라이브러리에 맞는 게 없어 새로 만든 pixelfx `pets/hood-trail` (반짝이 알갱이가 망토 끝에서 뒤·아래로 흘러감, 연기 방울은 끔). 옵션 count 40, wisp 0, speed 34, hem 7, light 0.9, 팔레트 기본값(크림·호박). 오른쪽을 향할 때 꼬리가 왼쪽으로 흐르게 그려져 있음
- 예전 어두운 후드용 청록 연기 `effects/devour_poof`는 2026-11-26 ArtSource에서 삭제함. 게임 쪽 `Assets/Resource/Textures/Effect/devour_poof` + 프리팹 `Prefabs/Effect/devour_poof`(Hood.cs `PoofKey`)는 아직 그대로라서, 개발에서 `devour_burst`로 바꿔야 함
- 레벨업: pixelfx `ui/level-up`에서 빛기둥·링·글자를 빼고 파티클만 남긴 복사본 `pets/hood-levelup`. 머리 위 한 점으로 빛줄기가 모임 → 번쩍임 + 위로 솟는 짧은 빛줄기 + 사방으로 뻗는 선·불꽃 → 반짝이가 남았다 사라짐. 색은 포식 링과 같은 크림·호박 팔레트. 옵션 `head` −46 = 터지는 중심이 pivot에서 46px 위 (hood_001 머리 바로 위). hood_002 이상은 키가 달라 `head`만 바꿔 다시 렌더
- 다시 렌더: pixelfx 폴더 `C:\Users\HP\Documents\Claude\pixelfx`에서 `node tools/render.mjs <id> --sheet --opts '<options.json 내용>'`
- 데모: 각 폴더의 `*_demo_x3.gif` (이동 데모는 바닥 눈금을 흘려 움직이는 느낌을 냄)

## 규칙 (메인 캐릭터와 동일)
- **오른쪽 반측면(south-east) 한 방향만**. 왼쪽은 엔진에서 좌우 반전
- 128×128 캔버스, 기본 자세 몸 아래 끝 111행(바닥선 y=112), 가로 중심 x=64, 엔진 pivot (64,112)
- 떠 있는 정령이므로 idle에서 아래 끝이 위아래로 몇 px 움직인다. 공용 바닥 그림자(`ArtSource/common/shadow/`)를 고정 위치에 깔면 떠 있는 느낌이 산다. 화면에서 더 띄우고 싶으면 엔진에서 y 오프셋으로 올린다(그림자는 그대로)

## 진화 단계 확장 규칙 (다음 레벨 만들 때)
- **모든 레벨이 같은 128 캔버스·같은 pivot (64,112)** 을 쓴다. 그래야 레벨을 바꿔도 위치가 튀지 않는다
- 몸 크기 목표: lv1 약 38px → lv2 약 48px → lv3 약 64px → lv4 약 84px (128 안에 포식 동작 여유 포함)
- **고정 요소** (모든 레벨 공통): 크림 후드 망토 실루엣, 어두운 그림자 얼굴, 동그란 금빛 눈, 빨간 리본+금방울
- **레벨마다 하나씩 추가**: 예) 망토 자락 무늬 → 어깨 망토 → 떠다니는 작은 손 → 왕관·후광 등
- **코스튬**(예: 조개 모자)은 같은 몸에서 머리 부분만 바꾼 버전으로 idle·포식을 통째로 따로 만든다 (겹치는 모자 레이어 방식은 프레임마다 머리 위치가 달라 들뜨므로 쓰지 않음)
- 동작 세트는 레벨마다 같게: idle(8장, 0.12초) + devour(13장, 0.08초), 포식 타이밍 표만 새로 적는다
- 만드는 방법: 이전 레벨 이미지를 style 이미지로 `create_image_pro_flash` (south-east) → `create_object_pro_flash`(source_image_id, 1방향, 무료) → `set_canvas_size` 96 (무료) → `animate_object` v3. idle 설명에 "eyes stay open, no blinking, does not turn"을 꼭 넣는다

## PixelLab ID
| 이름 | ID | 비고 |
|---|---|---|
| lv1 이미지 | `03c35c86-d291-57d4-8d41-c62664514ef6` | create_image_pro_flash 48×48, seed 31, style = 큰 C 디자인(`5c123567…`) 48px 잘라낸 것 |
| lv1 오브젝트 | `a254e8c4-26ec-4b63-bb01-00695b47e5da` | 48→96×96 (set_canvas_size), 1방향 |
| └ idle | group `4df2954b-c343-4bac-a157-75555c177885` ("idle2") | 96 캔버스, 8프레임(+끝 1), 0~7 사용 |
| └ idle (탈락) | group `7d6a56be-6980-46df-85c8-1eea6b814a20` | 48 캔버스, 눈을 오래 감고 있어서 탈락 |
| └ devour | group `a258102c-8b05-4e69-8a4c-21583ce78e5a` | 96 캔버스, 12프레임(+시작 1). 수정: 빨간 혀(frame 2~9)와 주황 부스러기를 지움, 얼굴 안쪽은 얼굴색 #3A241F로 메움, 작은 조각 제거 |
| 큰 C 디자인 (참고) | `5c123567-fb06-5aa9-8860-2e4a80a77d80` | 64×64, style = 메인 캐릭터 64px 잘라낸 것 |
| 조개 모자 D (큰 콘셉트) | `d343a07b-3664-5ceb-a79f-2971a2f2b9d1` | 64×64, 코스튬 아이디어 원본 |
| lv2 이미지 | `b8c6b911-b4a6-5b2d-b235-298b33c70d71` | create_image_pro_flash 64×64, seed 52, style = lv1 이미지. 떠다니는 손 두 개는 직접 지움(가장 큰 덩어리만 남김) |
| lv2 오브젝트 | `b2f74b96-be5e-421d-8980-79ddf3986870` | 손 지운 그림을 first_frame으로 생성, 64→96×96 |
| └ idle | group `66591daa-c17c-4db4-b8ff-c48e7d551432` ("idle2") | 8프레임(+끝 1), 0~7 사용. 첫 idle `c01a3219…`은 고개를 돌려 눈이 사라져서 탈락 |
| └ devour | group `84576f7f-0103-41ee-a8d3-2e03bbdf5516` | 12프레임(+시작 1). 수정: 분홍 혀·부스러기·혀 외곽선 지움, frame_012를 idle frame_000으로 교체. `devour2` `7914514d…`는 혀가 더 커서 탈락 |
| 조개 모자 코스튬 이미지 | `230375fc-223b-5fb2-bebd-2b006c8038f5` | edit_image_pro_flash로 lv1 이미지의 후드만 소라 껍데기로 교체 |
| 조개 모자 오브젝트 | `45702588-79e3-4d0f-bcea-42f9cd9159b9` | 48→96×96 |
| └ idle | group `6ca9e8dd-240a-4f5a-bf5a-2ff9a8db8fde` | 8프레임(+끝 1), 0~7 사용 (7번에서 살짝 눈 깜빡임) |
| └ devour | group `59524a09-9f78-4898-ab55-8f1bd3daa64c` ("devour2") | 수정: 빨간 혀만 지움(소라 줄무늬 산호색은 건드리지 않게 색 범위 따로: h≤12, s≥0.6, l 0.48~0.62), frame_012를 idle frame_000으로 교체. 첫 devour `992d6bcb…`은 갈색 덩어리가 튀어서 탈락 |
| 겨울 코스튬 디자인 | edit_image job `8df99d8c-f3c9-4e93-9c02-a3e4d73c7796` index 0 | hood_001 기본 자세의 후드만 빨간 뜨개 후드로 바꾼 정지 그림 |
| 겨울 코스튬 동작 | transfer_outfit job `aa8efd33-…`(idle 0~6), `d83cabaa-…`(idle 7 + devour 0~5), `0e24205b-…`(devour 6~12) | 빨갛게 색만 바꾼 hood_001 idle·devour 21장을 (24,40)에서 80×80으로 잘라 옷 갈아입힘 → 같은 자리로 되돌림 → 크림 망토를 빨강(색상 355°)으로 바꿈. 포식 타이밍은 hood_001과 같음 |

겨울 코스튬 알려진 점: idle 5번에서 눈을 감음, devour 2·3번 입 안이 빨강(혀 자국), devour 6번 입 옆 검은 조각 (사용자 확인 후 그대로 확정). 작업 파일: pixl_test `assets\character\costume_winter\hood_winter\`

정렬: 96×96 원본을 128×128에 hood_001·hood_002는 (dx=17, dy=42), 조개 모자는 (dx=16, dy=42)로 배치 (몸 아래 끝 69행 → 111행).

탈락 기록 (어두운 그림자 디자인 A·B, 2026-11-26): 배경(따뜻한 해변)·캐릭터 색과 안 맞아 교체. A 이미지 `bec9a9f5…`/오브젝트 `b7fa4dff…`, B 이미지 `0f71a892…`/오브젝트 `d98c662a…`. 파일은 pixl_test `assets\pet\pet_hood\work\rejected_v1\`에 보관.

## 알려진 점
- devour는 덮칠 때 아래 끝이 바닥선보다 내려간다 (눌리는 느낌으로 의도): hood_001 1~2px, 조개 모자 최대 7px, hood_002 최대 8px
- 혀를 지운 자리에 포식 중간 2~3프레임 작은 어두운 자국이 남음 (입처럼 보이는 정도)
- 혀를 지운 자리라 솟구칠 때 입은 빨갛지 않고 어두운 얼굴로 보인다
- C1은 밝은 크림색이라, 물어뜯을 때 번쩍임(엔진 효과)은 주로 어두운 얼굴 부분만 청록으로 빛난다
- 수정·정렬 도구 (pixl_test `assets\pet\pet_hood\work\tools\`): `TongueFix.cs`(혀·부스러기 지우기), `KeepMain.cs`(작은 조각 제거), `Align.cs`(bbox·배치·스트립)
