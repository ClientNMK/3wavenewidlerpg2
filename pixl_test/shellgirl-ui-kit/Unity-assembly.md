# 소라게 후드 소녀 · 전투 / 정령의 둥지 UI

preview.png의 왼쪽은 자동 전투 HUD, 오른쪽은 정령의 둥지입니다. parts 폴더의 21개 투명 PNG를 조합한 시안입니다. 배경과 전투 캐릭터는 기존 프로젝트 아트를 사용했습니다. 글자와 예시 수치는 미리보기에만 있으며, 실제 화면에서는 TextMeshProUGUI로 얹습니다.

## Unity 임포트

- parts만 Assets에 가져옵니다. Texture Type은 Sprite (2D and UI), Sprite Mode는 Single, Alpha Is Transparency는 켜고, Filter Mode는 Point, Compression은 None으로 설정합니다. Mip Maps는 끕니다.
- 기존 InGameScene의 Canvas Scaler 기준은 720×1280, Match Width입니다. 미리보기의 각 화면은 360×640이므로 화면 안의 배치와 글자 크기는 약 2배로 시작합니다. 터치 영역도 함께 확대합니다.
- 새 판의 1px을 기준 화면의 2px로 보이게 하려면 Canvas Reference PPU가 100일 때 새 9-slice 스프라이트의 PPU를 50으로 시작합니다. 기존 초상화·재화 아이콘은 아래 조립 구조의 자식 Image에서 크기를 맞춥니다.
- HUD는 Safe Area를 기준으로 상단·하단에 앵커합니다. 전투 배경만 전체 화면에 채웁니다. 폭에 맞춰 스케일할 때 9:16보다 짧은 화면에서도 상·하단이 겹치지 않는지 확인합니다.

## PNG 조합표

| 부품 | 쓰는 곳 | Image 설정 |
| --- | --- | --- |
| panel_navy | 재화, 스테이지, 둥지 카드 백판 | Sliced, Border L/R 6 · T/B 6 |
| panel_inset | 전투 상태 줄, 자동 전투, 부화/펫 슬롯 | Sliced, Border L/R 6 · T/B 6 |
| button_primary / pressed / disabled | 보스 도전, 보유 알 보기 버튼의 상태 | Sliced, Border L/R 6 · T/B 6 |
| tab_selected | 하단에서 선택된 탭의 배경 | Sliced, Border L/R 6 · T/B 6 |
| bar_track | 진행도 홈 | Sliced, Border L/R 5 · T/B 3 |
| bar_fill_mint | 진행도 채움 | Filled · Horizontal · Left |
| frame_portrait + portrait_girl + badge_level | 초상화와 레벨 백판 | Simple, 비율 유지 |
| icon_gold / gem / boss | 재화와 보스 버튼 | Simple, 비율 유지 |
| icon_battle / growth / gear / nest / lock | 하단 탭과 해금 안내 | Simple, 비율 유지 |
| icon_egg / pet_spirit | 알과 후드 정령 | Simple, 비율 유지 |

보스 버튼은 같은 RectTransform에 백판 Image를 두고 아이콘과 TMP를 자식으로 둡니다. 버튼의 상태 백판은 Sprite Swap으로 교체할 수 있습니다. 장식용 자식 Image와 TMP의 Raycast Target은 끕니다.

초상화는 119×119 프레임 안에서 72×72 초상화를 원본 기준 위·왼쪽 23px에 배치합니다. 함께 축소·확대하고 레벨 백판을 프레임 하단에 얹습니다.

진행바는 트랙 위에 채움 Image를 두고 진행률을 fillAmount로 연결합니다. 트랙이 2배 배율이면 채움 영역을 좌우 4px·상하 4px 들여 배치합니다.

## 화면 흐름

- 전투 화면은 재화 → 스테이지와 웨이브 → 캐릭터와 후드 정령 → 부화 상태와 보스 도전 → 하단 탭 순서입니다. 부화 상태 줄을 누르면 둥지로 이동합니다. 부화 준비 완료 시 라벨을 '부화 가능'으로 교체할 수 있습니다.
- 둥지는 알 부화와 장착 펫으로 나눴습니다. 후드 전직에 따른 부화·장착 슬롯 확장을 잠금 안내로 보여줍니다. 신규 알 해금은 향후 '보유 알 보기' 목록에서 잠금 항목으로 안내할 수 있습니다.
- Jetpost 결정의 적용 범위는 둥지 도입, 전직에 따른 둥지 확장, 동시 부화·장착 슬롯 증가, 신규 펫 알 해금입니다. 화면의 슬롯 배치와 65% 진행도는 예시이며, 포식·환원·부화 효율 규칙을 전제하지 않습니다.

신규 UI 판과 탭 아이콘은 시안용입니다. 초상화·재화 아이콘·보스 아이콘·알·후드 정령은 ArtSource의 기존 자산입니다.
