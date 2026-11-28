# 미션 창

시안: `Plan/mockups/review/mission_window.png` (일일 / 주간 / 반복). 좌표는 360×640 논리 크기(720×1280에서는 ×2).
기획 확정(2026-11-28): 미션 포인트 없음, 주간 첫 미션은 "일일 미션 15개 완료", 정렬은 받기 → 진행 중 → 받음, 반복 미션은 2배 이상 쌓이면 한 번에 받기(×N).

## 창
| 요소 | 부품 | 위치 | 크기 |
|---|---|---|---|
| 창 바탕 | win_panel | (0, 120) | 360×464 |
| 제목 판 + "미션" 11pt | win_title | (119, 103) | 122×34 |
| 닫기 | btn_close | (326, 107) | |
| 탭 "일일" / "주간" / "반복" | win_tab_on / win_tab_off + 12pt (켜짐 #2A1A14, 꺼짐 #C2A888) | x = 16, 127, 238, y = 142 | 106×28 |
| └ 알림 | dot_alert (그 탭에 받을 미션이 있을 때) | 탭 오른쪽 위 (100, -3) | |
| 안내 칸 | win_chip | (16, 180) | 328×28 |
| └ 일일·주간 | icon_timer 14×14 (10, 7) + "초기화 07:42:10" / "초기화 3일 07시간" 9pt #FFF3DC (28, 8) | | |
| └ 반복 | "목표를 채울 때마다 받을 수 있어요" 9pt #C2A888 가운데 | | |
| 미션 목록 (세로 스크롤) | 아래 '미션 줄' | x=16, y=220부터 간격 62 | 328×56 |
| 모두 받기 | btn_main + "모두 받기" 13pt #2A1A14 (받을 것이 없으면 btn_main_off) | (60, 540) | 240×36 |
| 하단 탭 | 미션 탭 켜짐: hud_nav_on (195, 589) + tab_mission (200, 594), 글자 #FFF3DC | | |

## 미션 줄 (줄 안 기준)
| 요소 | 부품 | 위치 | 크기 |
|---|---|---|---|
| 판 | win_row (받음은 알파 0.55) | (0, 0) | 328×56 |
| 받기 강조 | step_row_sel (받기 가능일 때만, 줄 위에 겹침) | (-6, -6) | 340×68 |
| 아이콘 | icon_ring + 종류 아이콘 20×20 (8, 8) | (10, 10) | 36×36 |
| 이름 | 11pt #FFF3DC (받음 #C2A888) | (56, 10) | |
| 진행 바 | bar_track + bar_fill (채움 초록 #9BD65A, 진행 중 산호 #EE7A5C) | (56, 32) | 140×8 |
| └ 숫자 | "320/500" 8pt #FFF3DC, 오른쪽 끝 x=192, 어두운 글자 테두리 | | |
| 보상 | card_chip (받음은 알파 0.6) + icon_gem (6, 4) + 수량 10pt (24, 6) | (206, 16) | 50×24 |
| 버튼: 받기 | btn_main + "받기" 11pt #2A1A14 + dot_alert (50, -4) | (262, 14) | 58×28 |
| 버튼: 진행 중 | hud_pill + "이동" 11pt #FFF3DC (누르면 해당 화면으로) | (262, 14) | 58×28 |
| 버튼: 받음 | icon_check (12, 8) + "완료" 10pt #9BD65A (28, 7) | (262, 14) | |

- 종류 아이콘: 몬스터·보스 처치 icon_skull_light, 플레이 시간 icon_timer, 던전 tab_content, 로그인 icon_login, 일일 미션 N개 완료 icon_check.
- 정렬: 받기 가능 → 진행 중 → 받음. 같은 상태 안에서는 차트 순서.

## 반복 미션 ×N (목표의 2배 이상 쌓였을 때)
- 진행 숫자는 목표를 넘어도 실제 수를 보여 준다: "2,350/1,000". 바는 꽉 찬 채로.
- 받기 버튼 오른쪽 위에 dot_alert 대신 tag_first (34, -8) 30×16 + "×2" 9pt #FFF3DC 가운데. N = 쌓인 수 ÷ 목표(내림).
- 보상 수량은 합친 값(10 × 2 = "20").
- 받으면 N번 몫을 한 번에 주고, 남은 수(2,350 - 2,000 = 350)부터 다시 센다.

## 부품
- 새로 추가: icon_ring, icon_login
- 기존: win_panel, win_title, btn_close, win_tab_on, win_tab_off, dot_alert, win_chip, icon_timer, win_row, step_row_sel, bar_track, bar_fill, card_chip, icon_gem, btn_main, btn_main_off, hud_pill, icon_check, tag_first, icon_skull_light, tab_content, hud_nav_on, tab_mission
