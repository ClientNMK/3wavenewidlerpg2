# HANDOFF — 아이템: 펫 알 2 (pet_egg_2)

확정일: 2026-11-26. **이 폴더에는 확정본만 둡니다.** 짝: `../pet_egg_1/`

## 한 줄 요약
복숭앗빛 알에 소라 소용돌이 줄무늬, 위쪽 금 사이로 금빛이 새어 나옴, 해초 둥지 위에 놓임. 곧 부화할 것 같은 느낌. 위에서 보는 한 방향.

## 폴더 구성
```
ArtSource/items/pet_egg_2/
├─ HANDOFF.md
├─ pet_egg_2.png                   ← 48×48, 투명 배경 (둥지 포함 x4~43, y10~46)
├─ skins/                          ← 색 스킨 4종 (원본과 크기·위치 동일, 껍질 색만 다름)
│  ├─ pet_egg_2_skin1_mint.png
│  ├─ pet_egg_2_skin2_sky.png
│  ├─ pet_egg_2_skin3_lavender.png
│  └─ pet_egg_2_skin4_night.png    ← 진한 남보라 (밤하늘)
└─ preview/
   ├─ pet_egg_2_x3.png             ← 3배 확인용
   ├─ pet_eggs_scale_x3.png        ← 메인 캐릭터 옆 실제 크기 (알 1·2 함께)
   └─ pet_egg_skins_x3.png         ← 원본 + 스킨 4종 비교 (위 줄 알 1, 아래 줄 알 2)
```

## 색 스킨 (2026-11-26 확정)
- 복숭아 껍질·껍질 외곽선만 민트·하늘·라벤더·밤하늘로 바꿈. 금빛 틈·해초 둥지는 원본 그대로.
- pivot·크기 원본과 같음 → 같은 오브젝트에서 스프라이트만 교체하면 됨.
- 만든 방법: PixelLab 생성 없이 로컬 색조 변환 (`pixl_test/assets/item/pet_egg/work/tools/EggRecolor.cs`, 색조 350~25° 픽셀만, 색조 이동 +135/+190/+255/+235, 밤하늘은 밝기 −0.28).

## 엔진 연결 방법
- 바닥 기준 pivot **(24, 47)** → 메인 캐릭터와 같은 바닥선(y=112)에 선다.
- **idle(상하 흔들흔들)은 이미지 없이 엔진에서 처리** (사용자와 합의, 2026-11-26). 값은 `../pet_egg_1/HANDOFF.md`와 같음 (2px, 약 1.2초, sine, 정수 픽셀 반올림).
- **주의:** 이 알은 해초 둥지가 같은 그림에 붙어 있어서 통째로 움직이면 둥지도 같이 뜬다. 둥지는 두고 알만 흔들려면 알/둥지를 두 장으로 나눠야 함 (요청 시 무료로 분리 가능).
- 좌우 반전 불필요.

## 만든 방법
`create_object_pro_flash`, 48×48, 1방향, seed 29. style_image = 슬라임 기본 자세 48px 잘라낸 것(색은 복사 안 함). 설명: "a mysterious pet egg item for an idle RPG: one upright egg with a pale pink-peach shell covered in spiral seashell stripes, a small crack near the top with a faint golden glow peeking out, sitting in a little nest of seaweed, glossy highlight, thick dark outline, cute chunky pixel art"

## PixelLab ID
| 이름 | ID |
|---|---|
| 펫 알 2 오브젝트 | `c535a83a-4944-445a-ae2a-8888077c970e` |
