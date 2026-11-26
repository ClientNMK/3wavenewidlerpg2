# HANDOFF — 아이템: 펫 알 1 (pet_egg_1)

확정일: 2026-11-26. **이 폴더에는 확정본만 둡니다.** 짝: `../pet_egg_2/`

## 한 줄 요약
크림색 알에 청록 반점과 소라 무늬, 위에 해초 한 줄기. 슬라임과 같은 색감·외곽선. 위에서 보는 한 방향.

## 폴더 구성
```
ArtSource/items/pet_egg_1/
├─ HANDOFF.md
├─ pet_egg_1.png                   ← 48×48, 투명 배경 (알 본체 x9~36, y2~46)
├─ skins/                          ← 색 스킨 4종 (원본과 크기·위치 동일, 반점 색만 다름)
│  ├─ pet_egg_1_skin1_pink.png
│  ├─ pet_egg_1_skin2_purple.png
│  ├─ pet_egg_1_skin3_orange.png
│  └─ pet_egg_1_skin4_blue.png
└─ preview/
   ├─ pet_egg_1_x3.png             ← 3배 확인용
   ├─ pet_eggs_scale_x3.png        ← 메인 캐릭터 옆 실제 크기 (알 1·2 함께)
   └─ pet_egg_skins_x3.png         ← 원본 + 스킨 4종 비교 (위 줄 알 1, 아래 줄 알 2)
```

## 색 스킨 (2026-11-26 확정)
- 청록 반점만 분홍·보라·주황·파랑으로 바꿈. 크림 껍질·해초·외곽선은 원본 그대로.
- pivot·크기 원본과 같음 → 같은 오브젝트에서 스프라이트만 교체하면 됨.
- 만든 방법: PixelLab 생성 없이 로컬 색조 변환 (`pixl_test/assets/item/pet_egg/work/tools/EggRecolor.cs`, 색조 125~180°·밝기 0.36~0.85 픽셀만, 채도 ×2.2+0.05, 색조 이동 +170/+115/−135/+55).

## 엔진 연결 방법
- 바닥 기준 pivot **(24, 47)** → 메인 캐릭터와 같은 바닥선(y=112)에 선다.
- **idle(상하 흔들흔들)은 이미지 없이 엔진에서 처리** (사용자와 합의, 2026-11-26):
  - 위아래로 **2px**, 한 번 오르내리는 데 **약 1.2초**, sine 곡선
  - 픽셀이 뭉개지지 않도록 위치를 **정수 픽셀로 반올림**(Pixel Perfect Camera 또는 Mathf.Round)
  - 원하면 바닥에 작은 그림자를 두고 알이 올라갈 때 그림자를 살짝 줄이면 떠 있는 느낌이 산다
- 좌우 반전 불필요.

## 만든 방법
`create_object_pro_flash`, 48×48, 1방향, seed 3. style_image = 슬라임 기본 자세 48px 잘라낸 것(색은 복사 안 함). 설명: "a cute pet egg item for an idle RPG: a single round cream-white egg standing upright, decorated with soft turquoise spots and a small curly seashell pattern, a tiny seaweed sprig stuck on top, glossy highlight, thick dark outline, chunky pixel art"

## PixelLab ID
| 이름 | ID |
|---|---|
| 펫 알 1 오브젝트 | `aa8f5b95-eea6-4b31-9884-99c7d5b9c737` |

흔들기·금 가는 단계·부화 동작이 필요하면 이 오브젝트에 `animate_object` v3로 추가 (48×48 8프레임 = 생성 1회).
