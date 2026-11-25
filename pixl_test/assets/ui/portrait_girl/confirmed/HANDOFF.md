# HANDOFF — UI: 메인 캐릭터 초상화 (portrait_girl)

확정일: 2026-11-26. **이 폴더에는 확정본만 둡니다.**

## 한 줄 요약
상단 HUD 원형 초상화 틀(`../hud_kit/parts/frame_portrait.png`)에 넣는 메인 캐릭터(소라게 후드 소녀, 기본형) 얼굴. 72×72, 원 모양으로 잘림, 원 밖은 투명.

## 폴더 구성
```
ArtSource/ui/portrait_girl/
├─ HANDOFF.md
├─ portrait_girl.png                 ← 게임용. 72×72, 틀 구멍 모양대로 잘림, 투명 배경
├─ preview/portrait_in_frame_x3.png  ← 틀에 넣은 모습 (3배)
└─ source/portrait_girl_raw.png      ← 자르기 전 원본 72×72 (다시 자를 때 사용)
```

## 엔진 연결 방법
- `frame_portrait.png`(119×119)의 구멍은 **(23, 23)~(94, 94)**, 지름 72px, 중심 (58.5, 58.5).
- 초상화를 틀 왼쪽 위 기준 **(23, 23)** 에 놓는다. 틀 구멍의 남색 배경 위에 얹히는 구조라 틀보다 **위 레이어**에 두면 된다 (초상화가 이미 구멍 모양으로 잘려 있어서 금테를 가리지 않음).
- 틀을 늘리거나 줄이면 초상화도 같은 배율로. 필터는 Point(최근접), 압축 없음.

## 만든 방법
1. 기준 이미지: `ArtSource/girl_base/pose/girl_base_se_p2.png`에서 머리 부분 64×64를 잘라 style_image로 사용
2. `create_image_pro_flash`, 72×72, seed 5. 설명: "portrait bust of this same chibi girl for a round UI avatar: head and shoulders only, big head filling most of the image, face turned slightly to the right in three-quarter view with both golden eyes visible, gentle confident smile, wavy ash-cream short hair, big red ribbon with a gold bell at the collar, dark brown dress collar, centered, no hands, no dagger"
3. 틀 구멍 색과 같은 픽셀 위치만 남기고 잘라냄 (무료)

## PixelLab ID
| 이름 | ID | 비고 |
|---|---|---|
| 초상화 A | image `f57c3a86-9e49-5760-af0a-dfbfdfff9cbb` | **확정** |
| 초상화 B (탈락) | image `90925ed3-44c6-5bec-96d8-fbcc0e569a2f` | 얼굴 더 크고 볼 홍조, seed 17 |

## 알려진 점
- 기본형(후드 없음) 얼굴. 후드를 장착한 버전 초상화가 필요하면 같은 방법으로 hood_0 기준 이미지를 써서 만들면 됨
