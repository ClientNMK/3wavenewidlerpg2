# hud_pixel2 — 지금 게임 골드 느낌의 도트 HUD 키트 (2026-11-26)

기준: 유니티에 들어간 골드 `Assets/Resource/Textures/UI/HUD/icon_gold.png`(별 동전), `Goods/icon_goods_gold.png`(재화 동전)
방식: PixelLab create_ui_asset 512×512 (25 generations), 스타일 참고 = 위 두 동전 + 주인공 + 슬라임, 팔레트 21색 고정 (따뜻한 갈색·크림·금·산호·이끼)
PixelLab UI ID: `953230fa-d3ae-41dd-84bd-ba560c4ef249`

| 부품 | 크기 | 용도 | 9-slice (가로) |
|---|---|---|---|
| plate_currency | 301×53 | 재화 판 | 좌우 26 |
| plate_stage | 332×51 | 스테이지 바 | 좌우 20 |
| panel | 330×127 | 창 패널 (금 모서리) | 사방 24 |
| frame_portrait | 117×117 | 초상화 틀 (가운데 투명) | — |
| badge_level | 89×31 | 레벨 뱃지 | 좌우 14 |
| btn_plus | 57×57 | + 버튼 | — |
| slot | 65×66 | 스킬·아이템 칸 | 사방 14 |
| btn_main | 152×55 | 주황 주 버튼 | 좌우 20 |
| btn_sub | 78×41 | 갈색 보조 버튼 | 좌우 14 |
| btn_close | 53×54 | 빨간 닫기 | — |
| bar_trough | 330×31 | 진행도 빈 바 | 좌우 16 |
| bar_fill_sample | 331×23 | 채워진 모습 (채움 줄만 따로 떼려면 추가 작업 필요) | — |
| btn_mail / btn_rank | 106×105 / 106×106 | 사이드 버튼 | — |

유니티: Filter Point, Compression None. 판은 불투명이라 전투 중 반투명이 필요하면 Image 색의 알파로 조절.
조립 예시: `review/hud_pixel2_v1_assembled.png` (UI 1배, 캐릭터 3배)
