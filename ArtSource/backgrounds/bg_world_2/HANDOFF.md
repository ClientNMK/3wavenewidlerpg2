# HANDOFF — 배경: 가을 숲 (bg_world_2)

확정일: 2026-11-27. **이 폴더에는 확정본만 둡니다.**

## 한 줄 요약
가을 숲 속 빈터. 가운데는 넓은 흙·낙엽 바닥(싸우는 곳), 가장자리는 주황 나무·이끼 바위·그루터기·금빛 등불·청록 연못·나무 울타리. 위에서 내려다본 시점, 9:16 세로.

## 폴더 구성
```
ArtSource/backgrounds/bg_world_2/
├─ HANDOFF.md
├─ bg_world_2.png                     ← 게임용 1152×2048 (원본 3배 확대, 위아래 8px씩 자름)
├─ source/bg_world_2_384x688.png      ← PixelLab Pro 원본
└─ preview/bg_world_2_screen_preview.png ← 왼쪽 전체(노란 틀 = 기본 줌 화면), 오른쪽 폰 화면 + HUD 가림
```
이름: 2026-11-27 `bg_forest` → `bg_world_2`. 게임에 넣으면 지금 `Assets/Resource/Textures/Bg/bg_world_2.png`(모래 바닷가 2048×2048)를 대체한다. 짝 몬스터: `../../monsters/monster_stage_002`.

## 왜 세로인가
- 지금 bg_world_1~3은 2048×2048(20.48×20.48칸)인데, 카메라 기본 줌(orthographicSize 8)은 세로 16칸 × 가로 9칸만 보여 준다 → 그림의 약 2/3가 안 보임.
- 이 배경은 1152×2048(100 PPU 기준 11.52×20.48칸). 9칸보다 넓어 3:4 태블릿까지 거의 덮는다.
- 구도: 위쪽 약 15%(스테이지 바)·아래쪽 약 25%(보스 버튼·스킬·탭)는 HUD에 가려지므로 나무만 두고, 싸우는 바닥은 가운데.

## 엔진 연결 방법
- 텍스처 임포트는 bg_world와 같게(Point 필터, 압축 없음, PPU 100, Max Size 2048).
- Bg 오브젝트 SpriteRenderer 크기가 20.48×20.48 → 11.52×20.48로 바뀜. 줌 확대 시 카메라는 배경 경계로 막히므로(InGameManager 카메라 Clamp) 그대로 동작해야 함 — 확인 필요.

## 만든 방법
1. `create_image_pro` 340×340로 먼저 정사각 시안(v1, `pixl_test/assets/background/forest/bg_forest_pro_v1.png`).
2. 확정본: `create_image_pro` 384×688, 생성 25회.
   - style_image = bg_world_2를 512로 줄인 것, style_copy = outline·detail·shading (색은 복사 안 함)
   - reference_images = v1 ("same forest: trees, leaf colors, props, ground color and lighting")
   - 설명: "tall portrait high top-down game battle arena background, cozy autumn forest clearing. Large open empty warm dirt and fallen-leaf floor filling the middle of the image for characters to fight. Chunky round trees with orange and amber leaves, mossy rocks, tree stumps, mushrooms, small hanging gold lanterns along the left and right edges. A small teal pond near the upper right, wooden fences near the bottom corners. Top and bottom edges dense with tree canopy. Warm cocoa and honey tones, cute cozy idle rpg"
3. 3배 nearest 확대 → 1152×2064 → 위아래 8px 잘라 1152×2048.

## PixelLab ID
- 확정본 job: `15816a60-bab0-4a8d-b2e8-df29ea01586f` (asset `a3d12a1b-d215-5e85-b0b0-ca3072c6b4d2`)
- v1 정사각 job: `f157b715-75ba-4f60-b57e-232ee36e2f2a`

## 남은 점
- 바닥이 주황·갈색이라 크림 모래(bg_world_2)보다 캐릭터 대비가 약간 낮다. 필요하면 바닥만 로컬로 밝게(생성 0회).
- 월드 차트(World_Resource)에 어떤 월드로 쓸지는 기획에서 정함.
