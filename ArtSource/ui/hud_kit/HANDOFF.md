# HANDOFF — UI: 상단 HUD 부품 키트 (hud_kit)

확정일: 2026-11-26. **이 폴더에는 확정본만 둡니다.**

## 한 줄 요약
세로 모바일 방치형 RPG 상단 HUD용 픽셀 UI 부품 27개. 짙은 남색 판 + 금테 스타일. **부품을 따로 주고 유니티에서 조합·늘리기·줄이기를 한다** (사용자 요청). 글자와 숫자는 이미지에 넣지 않고 엔진 폰트로 얹는다.

## 폴더 구성
```
ArtSource/ui/hud_kit/
├─ HANDOFF.md
├─ parts/                          ← 낱개 부품, 투명 배경, 여백 잘라냄 (27개)
├─ preview/
│  ├─ hud_assembled_x2.png         ← 부품 조립 예시 (2배, 글자는 맑은 고딕으로 얹은 예시)
│  ├─ parts_set1.png               ← 1차 부품 13개 목록
│  └─ parts_set2.png               ← 2차 부품 14개 목록
└─ source/
   ├─ hud_kit_set1_sheet.png       ← 1차 생성 원본 시트 512×512
   └─ hud_kit_set2_sheet.png       ← 2차 생성 원본 시트 512×512
```

## 부품 목록 (parts/)
| 파일 | 크기 | 용도 | 9-slice |
|---|---|---|---|
| plate_stage | 342×54 | 얇은 금테 판. 재화 바·스테이지 바 빽판 | 12px |
| plate_box | 202×74 | 테두리 어두운 판 | 12px |
| btn_wide | 238×71 | 큰 버튼 판 (보스 즉시 도전 등) | 12px |
| plate_pill_ornament | 302×64 | 가운데 금장식 판. **가로로 늘리면 장식도 늘어남** → 길이 고정해서 사용 | 좌우만, 가운데 금지 |
| frame_portrait | 119×119 | 원형 초상화 틀 | — |
| badge_level | 86×32 | 레벨 뱃지 | 10px |
| slot_buff | 67×67 | 빈 칸 틀 | 12px |
| btn_toggle | 116×50 | 자동 토글 (왼쪽 손잡이 포함) | — |
| icon_gold / icon_gem | 69×70 / 70×70 | 재화 아이콘 (판 왼쪽 끝에 겹쳐 배치) | — |
| btn_plus | 58×59 | + 버튼 | — |
| btn_mail / btn_rank | 94×94 | 우편함 / 랭킹 사이드 버튼 | — |
| buff_gold / buff_exp / buff_atk / buff_speed | 35×35 | 버프 아이콘. **자체 금테가 있어 slot_buff 안에 넣지 말고 단독 사용** | — |
| bar_trough | 215×21 | 진행도 바 빈 틀 | 좌우 8px |
| bar_fill | 207×9 | 진행도 채움 줄만. Image Type = Filled (Horizontal)로 bar_trough 위에 | — |
| bar_filled_sample | 215×16 | 채워진 모습 참고용 (게임에 쓰지 않아도 됨) | — |
| btn_chest | 65×66 | 보물상자 버튼 | — |
| icon_boss | 52×52 | 해골 보스 아이콘 | — |
| btn_settings / btn_shop | 51×50 | 설정 / 상점 버튼 | — |
| btn_close | 31×31 | 닫기 X | — |
| btn_arrow | 26×27 | 넘기기 (>>) | — |
| notify_dot | 22×22 | 빨간 알림 점 (버튼 오른쪽 위에 겹침) | — |

9-slice 값은 조립 예시를 만들 때 쓴 추정값이다. 유니티 Sprite Editor에서 눈으로 맞춰 조정할 것.

## 유니티 설정
- Texture Type: Sprite (2D and UI), **Filter Mode: Point (no filter)**, Compression: None
- 판·틀은 Image Type = Sliced (Sprite Editor에서 Border 지정)
- 픽셀이 고르게 보이려면 부품끼리 같은 배율로 키운다 (예: 전부 2배).

## 알려진 문제
- **2차 부품(버프 아이콘, 상자, 설정, 상점, 닫기, 화살표, 알림 점, 해골, 진행도 바)은 1차보다 작게 그려졌다.** PixelLab이 배치를 무시하고 절반 크기로 그렸음. 예: btn_chest 65px vs btn_mail 94px. 같은 화면에서 크기를 맞추려고 2차만 1.5배 하면 픽셀 크기가 달라 보인다. 필요하면 같은 크기로 다시 생성.

## 만드는 방법 (다시 만들거나 추가할 때)
- PixelLab `create_ui_asset`, 512×512 (25 generations), `pieces`로 부품 배치 지정
- 스타일 참고 이미지: 메인 캐릭터 idle frame_000 + 슬라임 pose + 골드 더미를 1배로 나란히 붙인 272×128 이미지 (`pixl_test/assets/ui/currency_bar/work/style_ref_x1.png`). **참고 게임 스크린샷은 스타일 참고로 쓰지 않는다** (그 게임 그림체를 따라가므로).
- 자르기: 시트에서 투명하지 않은 영역을 찾아 부품별로 자름 (`pixl_test/assets/ui/hud_kit/work/SplitParts.cs`)
- 진행도 채움(bar_fill)은 bar_filled_sample의 주황 픽셀만 뽑아 끝을 둥글게 이어 붙여 만듦

## PixelLab ID
| 이름 | ui_asset_id |
|---|---|
| 1차 시트 (판·틀·재화·사이드 버튼) | `5fc4812c-c44e-4ef6-8d7b-7c44f6279227` |
| 2차 시트 (버프·진행도·상자·보스·설정·상점) | `fa3e18bf-f93c-4cac-a8e4-cc6928be233b` |
| 탈락: 통짜 재화 바 v1 / v2 A 광택 / v2 B 나무 / v2 C 조개(실패) | `0d381f29-…` / `805f6ce8-…` / `56ca1fd6-…` / `4426e305-…` |

## 다음 할 일
- 2차 부품 크기 맞추기 (필요 시 재생성)
- 버튼 눌림 상태 (create_ui_states로 hover/pressed 생성 가능)
- 한글 픽셀 폰트 선정 (갈무리, 둥근모 계열)
