# parts_table.md를 넣어 ../NOTES.md(개발 전달용)를 만든다
import os
HERE = os.path.dirname(os.path.abspath(__file__))
table = open(os.path.join(HERE, "parts_table.md"), encoding="utf-8").read()
notes = f"""# kit_cocoa — 코코아 + 금선 UI 키트

작성: 2026-11-27. 상태: **테스트용 (확정본 아님)**. 기준 문서: `Plan/UI_Style_Guide.md`.
부품은 `parts/` 62개. 모두 투명 배경 PNG, 글자 없음, **3배 크기**(화면 1080 폭 기준, 360 폭 기준의 3배).
조립 예시: `preview/assembled_from_parts_x2.png` (이 PNG들만으로 조립한 화면).

## 임포트 설정 (전부 같음)
- Texture Type: Sprite (2D and UI), Mesh Type: Full Rect
- Filter Mode: **Bilinear** (도트 아님), Compression: None, sRGB 켬
- 반투명은 이미지에 들어 있음. Image 색은 흰색 그대로 둔다 (아래 '색 입히는 부품' 제외).
- 9-slice가 있는 부품은 Sprite Editor Border에 아래 값을 넣고 Image Type = Sliced.
- 1080 폭이 아닌 해상도에서는 Canvas Scaler가 줄이거나 키운다. 따로 맞출 필요 없음.

## 부품
{table}
## 색 입히는 부품 (흰색으로 뽑음)
- `bar_fill`: 경험치 `#9BD65A`, 스테이지 진행 `#EE7A5C`. Image Type = Filled 또는 Sliced로 길이 조절.
- `badge_fill`: 능력치 색 (공격 `#FF8A5C`, 체력 `#FF6F7D`, 재생 `#8CCB4F`, 방어 `#6FB6F2`, 관통 `#B58CF0`, 치명 `#FFD34E`). 그 위에 `badge_ring`, 그 위에 `icon_*`.

## 조립 메모
- 구분선(탭 아래): 높이 1px 금색(`#E6C27A`) Image 한 줄 + 가운데에 `win_rule_center` + 양끝에 `win_rule_end`(오른쪽은 좌우 반전). 선이 장식의 세로 가운데를 지나게 맞춘다.
- 닫는 선(목록 마지막 줄 아래): 같은 1px 금색 줄 + 가운데에 `win_rule_close` + 양끝에 `win_rule_end`. 선은 `win_rule_close`의 위에서 11(논리) = 33px 지점을 지난다.
- 간단한 구분이 필요한 곳은 1px 금색 줄 + `win_rule_gem`.
- 글자: DNFBitBitv2. 기본 `#FFF3DC`, 보조 `#C2A888`, 금색 판(버튼·선택된 탭) 위는 `#2A1A14`.
- 창(`win_panel`)은 화면 아래에 붙는 판이라 아래 모서리는 둥글지 않다.

## 장식 부품 4개 (금선 한 색, 코드로 그린 1차본)
- 장식 4개는 금선 둘레에 어두운 테두리가 있고, 그 자리로 사방 3(논리) = 9px 여백이 더 있다. 위치를 잡을 때 이 여백만큼 바깥으로 나가게 둔다.
- `frame_stage`: 스테이지 바. 가운데는 직선뿐이라 가로로 늘어난다. 양끝 39(논리) = 117px 고정.
- `stage_crest`: `frame_stage` 윗변 가운데에 얹는다 (문장 아래쪽 1/3이 판과 겹치게).
- `frame_portrait`: 가운데가 뚫린 고리. 초상화 그림을 뒤에 깔고 원형 마스크(지름 44 논리)로 자른다.
- `win_title`: 창 제목 판. 창 윗변 가운데에 절반쯤 걸치게 놓는다.

## 후드 화면용 부품 4개 (요청서 `Plan/requests/UI_Hood_Window.md`)
- `icon_goods_hood_exp`: Hood_Exp 재화 아이콘. 청록 별 모양이라 금화와 구분된다.
- `icon_predation`: 포식 효과 줄의 배지 아이콘. `badge_fill`에 청록 `#6CC6BE`(제안)를 입히고 `badge_ring`과 함께 쓴다.
- `win_bubble` + `win_bubble_tail`: 한마디 판. 판은 가로로 늘리고, 꼬리는 따로 판 윗변 가운데에 아래 2(논리) = 6px 겹치게 놓는다. 한 장으로 만들면 늘릴 때 가운데 꼬리가 같이 늘어나서 나눴다.
- 조립 예시: `Plan/mockups/review/hood_window_states.png`

## 하단 탭 아이콘 (기획 확정 6개)
- 순서: 성장 `tab_grow`, 인벤토리 `tab_inventory`, 스킬 `tab_skill`, 미션 `tab_mission`, 컨텐츠 `tab_content`, 상점 `tab_shop`.
- `tab_hood`, `tab_nest`는 하단 탭에서 빠졌다(후드는 성장 창 안, 둥지는 별도 아이콘). 다른 곳에 쓸 수 있어 남겨 둔다.

## 보스 도전 (시안 `Plan/mockups/review/boss_challenge_states.png`)
- 버튼 자리: 스테이지 바 바로 아래 가운데 (120×30 논리).
- 진행 중: `hud_pill` + `icon_skull_light` + 흐린 글자 `#C2A888`. 아래에 남은 처치 수 안내 한 줄.
- 도전 가능: `btn_main` + `icon_skull` + `#2A1A14` 글자, 오른쪽 위에 `dot_alert`. 둘레 빛(주황 그림자)은 엔진 효과로.
- 양끝 장식: `btn_boss_deco` (왼쪽), 오른쪽은 좌우 반전. 장식 테두리 여백(3) 포함 24×22(논리), 버튼 끝에 10(논리) 겹치게.
- 보스전: `frame_stage` 안을 `icon_skull_light` + 보스 이름 + 체력 %로 바꾸고, `bar_fill`을 `#C84B3A`로. 바 아래에 `hud_pill` + `icon_timer` + 남은 시간(`#E2765E`), 옆에 포기 버튼(`hud_pill`, 글자 `#E2765E`, 장식은 반투명).
- 보스전 분위기: 화면 가장자리 비네트(남보라 `rgba(43,42,76,.55)`)는 엔진 효과로.
- 자동 보스 도전: 보스 버튼 왼쪽에 `hud_pill`(62×22 논리) + "자동" 글자 + `switch_on`/`switch_off`. 보스전 중에는 숨긴다.
- 실패 직후(시간 초과·사망·포기): 스테이지 바 0/10, 버튼 꺼짐, 자동 스위치 꺼짐. 화면 가운데에 `win_row` 알림 판(260×58 논리, 2~3초 뒤 사라짐): `icon_skull_light` + "보스 도전 실패"(`#E2765E`) + 안내 2줄.
- 자동 전투: 재화 바로 아래 오른쪽에 `hud_pill`(92×22 논리) + "자동 전투" + 스위치.
- 카메라 줌: 오른쪽 사이드 버튼 아래 (아래 '카메라 줌').
- `switch_on`/`switch_off`(28×16 논리): 켜기/끄기 스위치 공용. 글자는 판 위에 엔진으로.

## 카메라 줌 (3단계)
- 버튼 하나: `hud_round` + `icon_zoom`. 누를 때마다 1 → 2 → 3 → 1.
- 버튼 바로 아래 가운데에 점 3개(6×6 논리, 간격 8). 지금 단계만 `dot_step_on`, 나머지는 `dot_step_off`. 왼쪽부터 1·2·3단계.
- 자리: 오른쪽 사이드 버튼(우편·랭킹·설정) 아래.

## 다시 만들기
`work/`에서: `python make_kit.py` → Edge 헤드리스로 `parts.html`을 3배·투명 배경 캡처해 `sheet.png` 저장 → `crop.ps1`.
"""
open(os.path.join(os.path.dirname(HERE), "NOTES.md"), "w", encoding="utf-8").write(notes)
