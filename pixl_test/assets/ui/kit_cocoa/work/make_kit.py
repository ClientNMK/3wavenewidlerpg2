# 코코아 + 금선 UI 키트의 단순 도형 부품을 HTML로 그려 parts.html과 manifest.json을 만든다.
# 기준: Plan/UI_Style_Guide.md. 크기는 360x640 논리 크기, 출력은 3배.
# 실행 순서: make_kit.py → Edge 헤드리스 캡처(sheet.png) → crop.ps1
import json, os
HERE = os.path.dirname(os.path.abspath(__file__))
SCALE = 3
GOLD = "#E6C27A"; DARK = "#2A1A14"; LW = "1.5px"; CREAM_HEX = "#C2A888"
# 반투명 농도 (0~1). 비교용으로 환경 변수 KIT_HUD_A, KIT_WIN_A로 바꿔 뽑을 수 있다
HUD_A = os.environ.get("KIT_HUD_A", "0.8"); WIN_A = os.environ.get("KIT_WIN_A", "1")
HUD = f"rgba(58,34,24,{HUD_A})"; HOLE = "rgba(0,0,0,.32)"; OFF = "rgba(120,96,80,.5)"

def box(bg, radius, border=True, extra=""):
    b = f"border:{LW} solid {GOLD};" if border else ""
    return f'<div style="width:100%;height:100%;box-sizing:border-box;background:{bg};{b}border-radius:{radius};{extra}"></div>'
def svg(size, body, vb=24):
    return f'<svg width="{size}" height="{size}" viewBox="0 0 {vb} {vb}" style="display:block">{body}</svg>'
def line_icon(size, d):
    return svg(size, f'<path d="{d}" fill="none" stroke="{GOLD}" stroke-width="2" stroke-linejoin="round" stroke-linecap="round"/>')
def stat_icon(d):
    return svg(18, f'<path d="{d}" fill="#fff" stroke="#3A251C" stroke-width="2" stroke-linejoin="round" stroke-linecap="round"/>')

LINE = {
    "icon_mail": "M3 6h18v12H3zM3 6l9 7 9-7", "icon_rank": "M5 20V11h4v9M10 20V5h4v15M15 20v-7h4v7",
    "icon_settings": "M12 8a4 4 0 100 8 4 4 0 000-8zM12 2v3M12 19v3M2 12h3M19 12h3M5 5l2 2M17 17l2 2M19 5l-2 2M7 17l-2 2" }
TAB = {
    "tab_grow": "M12 4l7 8h-4v8H9v-8H5z", "tab_hood": "M4 19C4 10 7 5 12 5s8 5 8 14zM12 11a2 2 0 100 4 2 2 0 000-4z",
    "tab_skill": "M12 2l2.6 6.2L21 9l-5 4.3L17.5 20 12 16.4 6.5 20 8 13.3 3 9l6.4-.8L12 2z",
    "tab_nest": "M3 13c2 5 6 7 9 7s7-2 9-7zM8 13a4 5 0 018 0", "tab_shop": "M5 9h14l-1 11H6zM9 9V7a3 3 0 016 0v2",
    # 인벤토리 = 가방, 미션 = 체크 두루마리, 컨텐츠 = 던전 문
    "tab_inventory": "M6 9h12a2 2 0 012 2v8a1 1 0 01-1 1H5a1 1 0 01-1-1v-8a2 2 0 012-2zM9 9V7a3 3 0 016 0v2M4 14h16M11 14v2h2v-2",
    "tab_mission": "M7 3h11a2 2 0 012 2v1h-3M7 3a2 2 0 00-2 2v14a2 2 0 002 2h9a2 2 0 002-2V6M7 3a2 2 0 012 2v0M9 11l2 2 4-4M9 17h6",
    "tab_content": "M5 21V10a7 7 0 0114 0v11zM3 21h18M12 3v2M9 21v-6a3 3 0 016 0v6" }
STAT = {
    "icon_atk": "M5 19l2-5L17 4h3v3L10 17l-5 2zM7 13l4 4", "icon_hp": "M12 20s-8-5-8-11a4.5 4.5 0 018-2.5A4.5 4.5 0 0120 9c0 6-8 11-8 11z",
    "icon_regen": "M12 20s-8-5-8-11a4.5 4.5 0 018-2.5A4.5 4.5 0 0120 9c0 6-8 11-8 11zM12 8v6M9 11h6", "icon_def": "M12 3l8 3v6c0 5-4 8-8 9-4-1-8-4-8-9V6l8-3z",
    "icon_pierce": "M13 4h7v7l-2.5-2.5L8 18l-4 2 2-4 9.5-9.5L13 4z", "icon_crit": "M12 2l2.6 6.2L21 9l-5 4.3L17.5 20 12 16.4 6.5 20 8 13.3 3 9l6.4-.8L12 2z" }

# (이름, 가로, 세로, HTML, 9-slice [L, R, T, B] 논리 크기 또는 None, 용도)
P = [
    ("hud_pill", 44, 22, box(HUD, "11px"), [11, 11, 0, 0], "재화·이름·자동 버튼 판"),
    ("hud_round", 30, 30, box(HUD, "50%"), None, "사이드 둥근 버튼 판"),
    ("hud_slot", 40, 40, box(HUD, "11px"), None, "스킬 칸"),
    ("hud_slot_empty", 40, 40, box("rgba(58,34,24,.45)", "11px"), None, "빈 스킬 칸"),
    ("hud_nav", 48, 56, box("rgba(42,26,20,.88)", "16px 16px 0 0", False, f"border-top:{LW} solid {GOLD};"), [16, 16, 16, 0], "하단 탭 바"),
    ("hud_nav_on", 30, 30, box("#EE7A5C", "10px"), None, "선택된 하단 탭 표시"),
    ("btn_main", 60, 32, box("#F2B544", "16px"), [16, 16, 0, 0], "금색 주 버튼"),
    ("btn_main_off", 60, 32, box(OFF, "16px"), [16, 16, 0, 0], "비활성 버튼"),
    ("btn_plus", 16, 16, svg(16, f'<circle cx="8" cy="8" r="7.25" fill="#9BD65A" stroke="{GOLD}" stroke-width="1.5"/><path d="M8 4.5v7M4.5 8h7" stroke="#fff" stroke-width="2" stroke-linecap="round"/>', 16), None, "재화 추가 버튼"),
    ("btn_close", 24, 24, svg(24, f'<circle cx="12" cy="12" r="11.25" fill="#E2765E" stroke="{GOLD}" stroke-width="1.5"/><path d="M8 8l8 8M16 8l-8 8" stroke="#fff" stroke-width="2.2" stroke-linecap="round"/>'), None, "닫기"),
    ("dot_alert", 10, 10, svg(10, f'<circle cx="5" cy="5" r="4.25" fill="#E2765E" stroke="{GOLD}" stroke-width="1.5"/>', 10), None, "알림 점"),
    ("bar_track", 40, 8, box(HOLE, "4px"), [4, 4, 0, 0], "진행바 바닥"),
    ("bar_fill", 40, 5, box("#fff", "2.5px", False), [3, 3, 0, 0], "진행바 채움 (흰색, 엔진에서 색 입힘)"),
    ("icon_gold", 16, 16, svg(16, f'<circle cx="8" cy="8" r="7" fill="#F2B544" stroke="{DARK}" stroke-width="1.5"/><circle cx="8" cy="8" r="4" fill="none" stroke="#FFE7A0" stroke-width="1.2"/>', 16), None, "금화"),
    ("icon_gem", 16, 16, svg(16, f'<path d="M8 1.5L14.5 8 8 14.5 1.5 8z" fill="#EE7A5C" stroke="{DARK}" stroke-width="1.5" stroke-linejoin="round"/><path d="M8 4.5L11.5 8" stroke="#FFD2C4" stroke-width="1.2" stroke-linecap="round"/>', 16), None, "보석"),
    ("win_panel", 48, 48, f'<div style="position:relative;width:100%;height:100%;box-sizing:border-box;background:rgba(38,24,19,{WIN_A});border-top:{LW} solid {GOLD};border-radius:18px 18px 0 0"><div style="position:absolute;left:4px;right:4px;top:2.5px;bottom:0;border:1px solid {GOLD};border-bottom:0;border-radius:14px 14px 0 0;opacity:.85"></div></div>', [22, 22, 22, 2], "창 바탕 (안쪽 금선 포함)"),
    ("win_row", 48, 44, box("rgba(84,54,40,.75)", "12px"), [12, 12, 12, 12], "목록 줄"),
    ("win_chip", 20, 14, box(HOLE, "6px", False), [6, 6, 0, 0], "수치 칸"),
    ("win_tab_on", 32, 28, box(GOLD, "10px"), [10, 10, 0, 0], "선택된 창 탭"),
    ("win_tab_off", 32, 28, box(HOLE, "10px"), [10, 10, 0, 0], "꺼진 창 탭"),
    ("win_seg_on", 30, 20, box(GOLD, "10px"), [10, 10, 0, 0], "x1·x10·x100 선택"),
    ("win_seg_off", 30, 20, box("transparent", "10px"), [10, 10, 0, 0], "x1·x10·x100 안 선택"),
    ("win_rule_gem", 10, 10, svg(10, f'<path d="M5 0.5L9.5 5 5 9.5 0.5 5z" fill="{GOLD}"/>', 10), None, "구분선 가운데 마름모"),
    ("badge_fill", 30, 30, box("#fff", "50%", False), None, "능력치 배지 바탕 (흰색, 엔진에서 색 입힘)"),
    ("badge_ring", 30, 30, box("transparent", "50%"), None, "능력치 배지 금테"),
]
P += [(n, 16, 16, line_icon(16, d), None, "사이드 아이콘") for n, d in LINE.items()]
P += [(n, 20, 20, line_icon(20, d), None, "하단 탭 아이콘") for n, d in TAB.items()]
P += [(n, 18, 18, stat_icon(d), None, "능력치 아이콘") for n, d in STAT.items()]

# ---- 장식 부품: 금선 한 색. 늘어나는 가운데 구간에는 직선만 둔다 ----
# 밝은 배경에서도 보이도록 금선 둘레에 어두운 테두리를 두른다. 테두리 자리로 사방 3씩 여백을 더 둔다(AP)
AP = 3
def art(w, h, body):
    halo = f"drop-shadow(0 0 .7px {DARK}) drop-shadow(0 0 .7px {DARK}) drop-shadow(0 0 1.2px rgba(42,26,20,.85))"
    return f'<svg width="{w + 2 * AP}" height="{h + 2 * AP}" viewBox="{-AP} {-AP} {w + 2 * AP} {h + 2 * AP}" fill="none" stroke="{GOLD}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="display:block;filter:{halo}">{body}</svg>'
def both(w, left):  # 왼쪽 장식을 좌우 대칭으로 한 번 더
    return left + f'<g transform="translate({w} 0) scale(-1 1)">{left}</g>'
GEM = "#F2B544"
# 스테이지 바: 양끝이 뾰족한 라벨 모양 + 끝 소용돌이. 가운데(36~84)는 직선뿐이라 가로로 늘어남
stage_end = ('<path d="M16 13C10 10 5 12 6 16c1 3 5 2 4-1"/><path d="M16 31c-6 3-11 1-10-3 1-3 5-2 4 1"/>'
             f'<path d="M5 19l3 3-3 3-3-3z" fill="{GOLD}" stroke="none"/>')
frame_stage = art(120, 44, f'<path d="M34 4C26 4 26 14 18 16l-6 6 6 6c8 2 8 12 16 12h52c8 0 8-10 16-12l6-6-6-6c-8-2-8-12-16-12z" fill="rgba(58,34,24,.9)"/>'
                  '<path d="M36 8h48M36 36h48" stroke-width="1" opacity=".6"/>'
                  '<path d="M33 8c-4 1-5 8-10 10l-4 4 4 4c5 2 6 9 10 10" stroke-width="1" opacity=".6"/>'
                  '<path d="M87 8c4 1 5 8 10 10l4 4-4 4c-5 2-6 9-10 10" stroke-width="1" opacity=".6"/>' + both(120, stage_end))
# 스테이지 바 위 가운데에 얹는 문장
crest_side = ('<path d="M26 11C20 11 18 5 12 6c-4 1-4 6 0 6 2 0 3-2 2-3"/><path d="M26 13c-7 1-12 6-21 3"/>'
              f'<circle cx="3.5" cy="15.5" r="1.4" fill="{GOLD}" stroke="none"/><circle cx="22" cy="6" r="1.2" fill="{GOLD}" stroke="none"/>')
stage_crest = art(68, 22, both(68, crest_side) + f'<path d="M34 3l6 8-6 8-6-8z" fill="{GEM}"/><path d="M34 7l3 4-3 4-3-4z" stroke-width="1" stroke="#FFE7A0"/>')
# 초상화 틀: 안이 뚫린 고리 + 위 문장 + 좌우 마름모 + 아래 받침
ring_side = ('<path d="M29 12C24 6 16 6 15 11c-1 4 4 5 5 2"/><path d="M22 17C15 14 9 17 10 22c1 3 5 2 4-1"/>'
             f'<path d="M5 35l3 4-3 4-3-4z" fill="{GOLD}" stroke="none"/><path d="M27 66c-5 1-9-1-11-4"/>'
             f'<circle cx="14.5" cy="60" r="1.4" fill="{GOLD}" stroke="none"/>')
frame_portrait = art(72, 74, '<circle cx="36" cy="39" r="25" stroke="#3A251C" stroke-width="5.5"/><circle cx="36" cy="39" r="27.75"/><circle cx="36" cy="39" r="22.25" stroke-width="1" opacity=".8"/>'
                     + both(72, ring_side) + f'<path d="M36 2l6 8-6 8-6-8z" fill="{GEM}"/><path d="M36 6l3 4-3 4-3-4z" stroke-width="1" stroke="#FFE7A0"/>'
                     f'<path d="M36 64l4 4-4 4-4-4z" fill="{GOLD}" stroke="none"/>')
# 창 제목: 알약 판 + 양옆 소용돌이. 가운데(36~54)가 늘어남
title_side = ('<path d="M24 14H11c-6 0-9-5-5-7 3-1 5 2 3 4"/><path d="M20 14c-3 6-10 7-14 4"/><path d="M20 14c-2-5-6-8-10-8" stroke-width="1" opacity=".6"/>'
              f'<circle cx="4.5" cy="19.5" r="1.4" fill="{GOLD}" stroke="none"/>')
win_title = art(90, 28, '<rect x="24" y="2" width="42" height="24" rx="12" fill="#3A251C"/>' + both(90, title_side))
P += [
    ("frame_stage", 120 + 2 * AP, 44 + 2 * AP, frame_stage, [36 + AP, 36 + AP, 0, 0], "스테이지 바 (장식, 양끝 고정)"),
    ("stage_crest", 68 + 2 * AP, 22 + 2 * AP, stage_crest, None, "스테이지 바 윗변 가운데 문장"),
    ("frame_portrait", 72 + 2 * AP, 74 + 2 * AP, frame_portrait, None, "초상화 틀 (장식, 가운데 뚫림)"),
    ("win_title", 90 + 2 * AP, 28 + 2 * AP, win_title, [36 + AP, 36 + AP, 0, 0], "창 제목 판 (장식, 양끝 고정)"),
]

# ---- 창 안 구분선 장식 (불투명한 창 위에 놓여 어두운 테두리 없음). 가로 선은 엔진에서 1px 금색 줄로 긋고 그 위에 얹는다 ----
def plain(w, h, body):
    return f'<svg width="{w}" height="{h}" viewBox="0 0 {w} {h}" fill="none" stroke="{GOLD}" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="display:block">{body}</svg>'
# 여는 장식: 소용돌이가 선 위로 크게 말리고 가운데에 보석
rule_side = ('<path d="M50 11H2"/><path d="M48 11C42 11 40 3 33 3c-5 0-6 6-1 6 2 0 3-2 1-4"/><path d="M40 11c-4 0-6 6-12 6-3 0-3-4 0-4"/>'
             '<path d="M30 11C26 11 25 6 20 6" stroke-width="1" opacity=".7"/>'
             f'<path d="M54 8l3 3-3 3-3-3z" fill="{GOLD}" stroke="none"/><circle cx="14" cy="11" r="1.6" fill="{GOLD}" stroke="none"/><circle cx="22" cy="15.5" r="1.1" fill="{GOLD}" stroke="none"/>')
win_rule_center = plain(132, 22, both(132, rule_side) + f'<path d="M66 3l7 8-7 8-7-8z" fill="{GEM}"/><path d="M66 7l3.5 4-3.5 4-3.5-4z" stroke-width="1" stroke="#FFE7A0"/>')
# 닫는 장식: 같은 소용돌이를 위아래로 뒤집고(선 아래로 말림), 가운데는 보석 대신 아래로 맺히는 방울
win_rule_close = plain(132, 26, f'<g transform="translate(0 22) scale(1 -1)">{both(132, rule_side)}</g>'
                       f'<path d="M66 5l5 6-5 6-5-6z" fill="{GOLD}" stroke="none"/><path d="M66 18c2.5 3 2.5 5.5 0 7-2.500-1.500-2.500-4 0-7z" fill="{GOLD}" stroke="none"/>')
# 선 끝 마감 (왼쪽용, 오른쪽은 좌우 반전)
win_rule_end = plain(16, 10, f'<path d="M4 2l3 3-3 3-3-3z" fill="{GOLD}" stroke="none"/><path d="M8 5h8"/><circle cx="10.5" cy="5" r="1.2" fill="{GOLD}" stroke="none"/>')
P += [
    ("win_rule_center", 132, 22, win_rule_center, None, "구분선 가운데 장식 (여는 쪽, 탭 아래)"),
    ("win_rule_close", 132, 26, win_rule_close, None, "닫는 장식 (목록 끝 아래, 여는 쪽을 위아래로 뒤집은 문양)"),
    ("win_rule_end", 16, 10, win_rule_end, None, "구분선 양끝 마감 (오른쪽은 좌우 반전)"),
]

# ---- 후드 화면용 (Plan/requests/UI_Hood_Window.md) ----
TEAL = "#6CC6BE"; BUBBLE = "#4A2E22"
P += [
    ("icon_goods_hood_exp", 16, 16, svg(16, f'<path d="M8 1c1 4 3 6 7 7-4 1-6 3-7 7-1-4-3-6-7-7 4-1 6-3 7-7z" fill="{TEAL}" stroke="{DARK}" stroke-width="1.5" stroke-linejoin="round"/><path d="M8 5.500c.5 1.500 1 2 2.500 2.500" stroke="#E8FFFB" stroke-width="1.200" fill="none" stroke-linecap="round"/>', 16), None, "Hood_Exp 재화 아이콘 (청록 별)"),
    ("icon_predation", 18, 18, stat_icon("M3 11c0-5 4-8 9-8s9 3 9 8l-3 2.500-3-2.500-3 2.500-3-2.500-3 2.500zM5 16.500l3.500-2 3.500 2 3.500-2 3.500 2c-1 3-3.500 4.500-7 4.500s-6-1.500-7-4.500z"), None, "포식 효과 아이콘 (배지 색은 청록 #6CC6BE 제안)"),
    ("win_bubble", 48, 24, box(BUBBLE, "12px"), [12, 12, 0, 0], "한마디 판"),
    ("win_bubble_tail", 12, 8, f'<svg width="12" height="8" viewBox="0 0 12 8" style="display:block"><path d="M0 8L6 1.500 12 8z" fill="{BUBBLE}"/><path d="M0.500 7.500L6 1.500l5.500 6" fill="none" stroke="{GOLD}" stroke-width="1.500" stroke-linejoin="round" stroke-linecap="round"/></svg>', None, "한마디 판 꼬리 (판 윗변 가운데에 2 겹치게)"),
]

# ---- 보스 도전 (Plan/mockups/review/boss_challenge_states.png) ----
SKULL = ('<path d="M12 2.500c-5 0-8.500 3.500-8.500 8 0 3 1.500 5 3.500 6v3.500h10V16.500c2-1 3.500-3 3.500-6 0-4.500-3.500-8-8.500-8z"/>'
         '<path d="M10 20v-2M14 20v-2" stroke-width="1.500"/>')
def skull_icon(fill):  # 눈·코는 뚫어서(투명) 어떤 판 위에서도 판 색이 비친다
    return svg(16, f'<defs><mask id="sk{fill[1:]}"><rect width="24" height="24" fill="#fff"/><circle cx="8.500" cy="11" r="2.200"/><circle cx="15.500" cy="11" r="2.200"/><path d="M12 14l-1.200 2h2.400z"/></mask></defs>'
                   f'<g mask="url(#sk{fill[1:]})" fill="{fill}" stroke="{DARK}" stroke-width="2" stroke-linejoin="round">{SKULL}</g>')
P += [
    ("btn_boss_deco", 18 + 2 * AP, 16 + 2 * AP, art(18, 16, '<path d="M17 8H8C4 8 2 5 4 3.500c2-1 3.500 1 2 2.500"/><path d="M12 8c-2 3.500-6 4.500-9 3"/>'
                                          f'<path d="M2 7l1.500 1.500L2 10 .500 8.500z" fill="{GOLD}" stroke="none"/>'), None, "보스 도전 버튼 왼쪽 끝 장식 (오른쪽은 좌우 반전)"),
    ("icon_skull", 16, 16, skull_icon(DARK), None, "해골 (금색 버튼 위)"),
    ("icon_skull_light", 16, 16, skull_icon(CREAM_HEX), None, "해골 (어두운 판 위: 보스 바, 꺼진 버튼)"),
    ("icon_timer", 16, 16, svg(16, f'<g fill="none" stroke="{GOLD}" stroke-width="2.500" stroke-linecap="round"><circle cx="12" cy="13" r="8"/><path d="M12 9v4l3 2M9 3h6"/></g>'), None, "남은 시간 아이콘"),
]

# ---- 카메라 줌 (3단계, 버튼 하나를 누를 때마다 1 → 2 → 3 → 1). 버튼은 hud_round + icon_zoom, 아래에 점 3개 ----
P += [
    ("icon_zoom", 16, 16, line_icon(16, "M10.500 4a6.500 6.500 0 110 13 6.500 6.500 0 010-13zM15.300 15.300L20 20M10.500 6.800v4.400M8.300 9h4.400M8.300 13.600h4.400"), None, "카메라 줌 버튼 아이콘 (돋보기 ±)"),
    ("dot_step_on", 6, 6, svg(6, f'<circle cx="3" cy="3" r="2.400" fill="#FFE7A0" stroke="{DARK}" stroke-width=".7"/>', 6), None, "단계 점 (지금 단계)"),
    ("dot_step_off", 6, 6, svg(6, f'<circle cx="3" cy="3" r="1.800" fill="rgba(58,34,24,.85)" stroke="{GOLD}" stroke-width=".7" stroke-opacity=".7"/>', 6), None, "단계 점 (다른 단계)"),
]

# ---- 켜기/끄기 스위치 (자동 전투, 자동 보스 도전 공용). 글자는 옆에 엔진으로 ----
def switch(on):
    track = "#9BD65A" if on else "rgba(0,0,0,.45)"
    kx = 19 if on else 9
    return svg(28, f'<rect x="1" y="2.250" width="26" height="11.500" rx="5.750" fill="{track}" stroke="{GOLD}" stroke-width="1.500"/>'
                   f'<circle cx="{kx}" cy="8" r="4.300" fill="{"#FFF3DC" if on else "#8A7462"}" stroke="{DARK}" stroke-width="1"/>', 28).replace('viewBox="0 0 28 28"', 'viewBox="0 0 28 16"').replace('height="28"', 'height="16"')
P += [
    ("switch_on", 28, 16, switch(True), None, "켜기/끄기 스위치 켜짐 (초록, 손잡이 오른쪽)"),
    ("switch_off", 28, 16, switch(False), None, "켜기/끄기 스위치 꺼짐 (어두움, 손잡이 왼쪽)"),
]

# ---- 보스 등장 연출 (검은 화면 위에 띠 → 문장 + 빛줄기 → 번쩍). 움직임은 엔진에서 ----
BRED = "#8E2A22"; BRED2 = "#C84B3A"
rays = "".join(f'<path d="M100 100L{100 + 100 * __import__("math").cos(a / 18 * 3.14159 * 2 - .07):.1f} {100 + 100 * __import__("math").sin(a / 18 * 3.14159 * 2 - .07):.1f}L{100 + 100 * __import__("math").cos(a / 18 * 3.14159 * 2 + .07):.1f} {100 + 100 * __import__("math").sin(a / 18 * 3.14159 * 2 + .07):.1f}z"/>' for a in range(18))
boss_rays = (f'<svg width="200" height="200" viewBox="0 0 200 200" style="display:block"><defs><radialGradient id="rg"><stop offset="0" stop-color="#FFE7A0" stop-opacity=".9"/>'
             f'<stop offset=".55" stop-color="{GOLD}" stop-opacity=".35"/><stop offset="1" stop-color="{GOLD}" stop-opacity="0"/></radialGradient></defs><g fill="url(#rg)">{rays}</g>'
             '<circle cx="100" cy="100" r="34" fill="url(#rg)" opacity=".7"/></svg>')
emblem_side = ('<path d="M22 50C12 50 6 42 10 35c3-5 10-3 8 3"/><path d="M22 58c-10 2-17 9-14 16 2 4 8 3 7-2"/>'
               f'<path d="M4 51l3 3-3 3-3-3z" fill="{GOLD}" stroke="none"/><circle cx="18" cy="38" r="1.600" fill="{GOLD}" stroke="none"/>')
boss_emblem = art(168, 104,
    # 띠(리본) 판: 양끝이 갈라진 리본
    f'<path d="M20 44h128l-8 10 8 10H20l8-10z" fill="{BRED}"/><path d="M26 48h116M26 60h116" stroke-width="1" opacity=".7"/>'
    + both(168, emblem_side)
    # 위 해골 문장: 마름모 판 + 해골
    + f'<path d="M84 4l22 22-22 22-22-22z" fill="{DARK}"/><path d="M84 10l16 16-16 16-16-16z" stroke-width="1" opacity=".7"/>'
    f'<g transform="translate(73 15) scale(.92)" fill="#FFF3DC" stroke="{DARK}" stroke-width="2" stroke-linejoin="round"><path d="M12 2.500c-5 0-8.500 3.500-8.500 8 0 3 1.500 5 3.500 6v3.500h10V16.500c2-1 3.500-3 3.500-6 0-4.500-3.500-8-8.500-8z"/>'
    f'<circle cx="8.500" cy="11" r="2.200" fill="{BRED2}" stroke="none"/><circle cx="15.500" cy="11" r="2.200" fill="{BRED2}" stroke="none"/></g>'
    # BOSS 글자 (이 부품만 글자 포함: 연출용 로고)
    + f'<text x="84" y="62" text-anchor="middle" font-family="DNF" font-size="20" fill="#FFE7A0" stroke="{DARK}" stroke-width="3" paint-order="stroke" letter-spacing="3">BOSS</text>'
    # 아래 작은 장식
    + f'<path d="M64 72h14M90 72h14" stroke-width="1.200"/><path d="M84 68l4 4-4 4-4-4z" fill="{GOLD}" stroke="none"/>')
boss_band = (f'<div style="width:100%;height:100%;box-sizing:border-box;background:rgba(20,12,9,.92);border-top:1.5px solid {GOLD};border-bottom:1.5px solid {GOLD}">'
             f'<div style="margin-top:3px;height:1px;background:{GOLD};opacity:.45"></div><div style="margin-top:26px;height:1px;background:{GOLD};opacity:.45"></div></div>')
P += [
    ("boss_intro_rays", 200, 200, boss_rays, None, "보스 등장 빛줄기 (문장 뒤에서 천천히 회전)"),
    ("boss_intro_emblem", 168 + 2 * AP, 104 + 2 * AP, boss_emblem, None, "보스 등장 문장 (해골 + BOSS 글자 리본, 연출용으로 글자 포함)"),
    ("boss_intro_band", 40, 34, boss_band, [0, 0, 6, 6], "보스 등장 가로 띠 (화면 폭으로 가로 늘림)"),
]

# ---- 실패 연출 문장 (보스 실패·사망). 보스 문장과 같은 틀을 어둡고 차갑게, 금이 간 리본 ----
FGRAY = "#4A4458"
fail_emblem = art(168, 104,
    f'<path d="M20 44h128l-8 10 8 10H20l8-10z" fill="{FGRAY}"/><path d="M26 48h116M26 60h116" stroke-width="1" opacity=".5"/>'
    + both(168, emblem_side)
    + f'<path d="M84 4l22 22-22 22-22-22z" fill="{DARK}"/><path d="M84 10l16 16-16 16-16-16z" stroke-width="1" opacity=".5"/>'
    f'<g transform="translate(73 15) scale(.92)" fill="#B9B0C4" stroke="{DARK}" stroke-width="2" stroke-linejoin="round"><path d="M12 2.500c-5 0-8.500 3.500-8.500 8 0 3 1.500 5 3.500 6v3.500h10V16.500c2-1 3.500-3 3.500-6 0-4.500-3.500-8-8.500-8z"/>'
    f'<circle cx="8.500" cy="11" r="2.200" fill="{DARK}" stroke="none"/><circle cx="15.500" cy="11" r="2.200" fill="{DARK}" stroke="none"/><path d="M13 3l-1.500 4 2 2-1.500 3" fill="none" stroke-width="1.200"/></g>'
    + f'<text x="84" y="62" text-anchor="middle" font-family="DNF" font-size="17" fill="#D8CFC4" stroke="{DARK}" stroke-width="3" paint-order="stroke" letter-spacing="2">DEFEAT</text>'
    # 리본에 간 금
    + f'<path d="M58 44l4 6-3 5 5 9M112 44l-3 7 4 4-2 9" stroke="{DARK}" stroke-width="1.400" fill="none"/>'
    + f'<path d="M64 72h14M90 72h14" stroke-width="1.200" opacity=".6"/><path d="M84 68l4 4-4 4-4-4z" fill="{GOLD}" stroke="none" opacity=".6"/>')
P += [("fail_emblem", 168 + 2 * AP, 104 + 2 * AP, fail_emblem, None, "실패 문장 (해골 + DEFEAT 글자 리본, 연출용으로 글자 포함)")]

# ---- 실패 결과 판 (회보라, 둥근 모서리, 이중 테두리, 네 모서리 작은 장식) + 처음부터 흑백인 띠 ----
FSIL = "#A79FB4"
fcorner = (f'<path d="M3 15V9a6 6 0 016-6h6" stroke="{FSIL}" stroke-width="1.500" fill="none"/>'
           f'<path d="M8 3.500l2.500 2.500L8 8.500 5.500 6z" fill="{FSIL}"/>')
def corner_set(w, h):
    g = fcorner
    return (g + f'<g transform="translate({w} 0) scale(-1 1)">{g}</g>' + f'<g transform="translate(0 {h}) scale(1 -1)">{g}</g>'
            + f'<g transform="translate({w} {h}) scale(-1 -1)">{g}</g>')
fail_panel = (f'<svg width="48" height="48" viewBox="0 0 48 48" style="display:block">'
              f'<rect x=".75" y=".75" width="46.500" height="46.500" rx="12" fill="#2A1E27" stroke="#8A8096" stroke-width="1.500"/>'
              f'<rect x="5" y="5" width="38" height="38" rx="8" fill="none" stroke="{FSIL}" stroke-width=".8" opacity=".45"/>'
              f'<rect x="1.500" y="1.500" width="45" height="14" rx="11" fill="#FFFFFF" opacity=".04"/>'
              f'<g transform="translate(1.500 1.500) scale(.94)">{corner_set(48, 48)}</g></svg>')
fail_band = (f'<div style="width:100%;height:100%;box-sizing:border-box;background:rgba(14,12,16,.92);border-top:1.5px solid {FSIL};border-bottom:1.5px solid {FSIL}">'
             f'<div style="margin-top:3px;height:1px;background:{FSIL};opacity:.4"></div><div style="margin-top:26px;height:1px;background:{FSIL};opacity:.4"></div></div>')
P += [
    ("fail_panel", 48, 48, fail_panel, [16, 16, 16, 16], "실패 결과 판 (둥근 모서리, 이중 테두리, 모서리 장식)"),
    ("fail_band", 40, 34, fail_band, [0, 0, 6, 6], "실패 가로 띠 (처음부터 흑백, 셰이더 필요 없음)"),
    ("fail_vignette", 90, 160, '<div style="width:100%;height:100%;background:radial-gradient(ellipse 75% 70% at 50% 50%,rgba(140,30,24,0) 45%,rgba(140,30,24,.55) 80%,rgba(90,16,14,.8) 100%)"></div>', None, "실패 화면 가장자리 빨간 비네트 (화면 크기로 늘림)"),
    ("fail_row", 48, 38, box("rgba(255,255,255,.05)", "10px", False, f"border:1px solid rgba(167,159,180,.45);"), [10, 10, 10, 10], "실패 판 안 바로가기 줄"),
]

# ---- 보스 클리어 연출 (실패 연출과 같은 구조를 금색·따뜻하게) ----
CGOLD = "#B8862E"
crown = (f'<path d="M72 30l-4-16 9 8 7-12 7 12 9-8-4 16z" fill="#F2B544" stroke="{DARK}" stroke-width="1.500" stroke-linejoin="round"/>'
         f'<circle cx="68" cy="14" r="2" fill="#FFE7A0" stroke="{DARK}" stroke-width="1"/><circle cx="84" cy="9" r="2.200" fill="#FFE7A0" stroke="{DARK}" stroke-width="1"/><circle cx="100" cy="14" r="2" fill="#FFE7A0" stroke="{DARK}" stroke-width="1"/>'
         f'<path d="M73 26h22" stroke="{DARK}" stroke-width="1.200"/><path d="M84 18l2 3-2 3-2-3z" fill="#E8604E" stroke="none"/>')
clear_emblem = art(168, 104,
    f'<path d="M20 44h128l-8 10 8 10H20l8-10z" fill="{CGOLD}"/><path d="M26 48h116M26 60h116" stroke="#FFE7A0" stroke-width="1" opacity=".7"/>'
    + both(168, emblem_side)
    + f'<path d="M84 2l24 24-24 24-24-24z" fill="{DARK}"/><path d="M84 8l18 18-18 18-18-18z" stroke-width="1" opacity=".7"/>'
    + crown.replace('M72 30', 'M72 34').replace('l-4-16', 'l-4-14')
    + f'<text x="84" y="62" text-anchor="middle" font-family="DNF" font-size="17" fill="#FFF8DC" stroke="{DARK}" stroke-width="3" paint-order="stroke" letter-spacing="2">VICTORY</text>'
    + f'<path d="M58 72h20M90 72h20" stroke-width="1.200"/><path d="M84 67l5 5-5 5-5-5z" fill="#F2B544" stroke="none"/>')
clear_panel = (f'<svg width="48" height="48" viewBox="0 0 48 48" style="display:block">'
               f'<rect x=".75" y=".75" width="46.500" height="46.500" rx="12" fill="#3A2419" stroke="{GOLD}" stroke-width="1.500"/>'
               f'<rect x="5" y="5" width="38" height="38" rx="8" fill="none" stroke="{GOLD}" stroke-width=".8" opacity=".45"/>'
               f'<rect x="1.500" y="1.500" width="45" height="14" rx="11" fill="#FFE7A0" opacity=".06"/>'
               f'<g transform="translate(1.500 1.500) scale(.94)">{corner_set(48, 48).replace(FSIL, GOLD)}</g></svg>')
clear_band = (f'<div style="width:100%;height:100%;box-sizing:border-box;background:linear-gradient(rgba(60,36,14,.92),rgba(40,24,10,.92));border-top:1.5px solid {GOLD};border-bottom:1.5px solid {GOLD}">'
              f'<div style="margin-top:3px;height:1px;background:{GOLD};opacity:.5"></div><div style="margin-top:26px;height:1px;background:{GOLD};opacity:.5"></div></div>')
fx_sparkle = svg(16, '<path d="M8 .500c.6 4 3.500 6.900 7.500 7.500-4 .6-6.900 3.500-7.500 7.500C7.400 11.500 4.500 8.600.500 8 4.500 7.400 7.400 4.500 8 .500z" fill="#FFF3C4"/>', 16)
tag_first = box("#E8604E", "6px", False, f"border:1px solid {DARK};")
P += [
    ("clear_emblem", 168 + 2 * AP, 104 + 2 * AP, clear_emblem, None, "클리어 문장 (왕관 + VICTORY 글자 리본, 연출용으로 글자 포함)"),
    ("clear_panel", 48, 48, clear_panel, [16, 16, 16, 16], "클리어 결과 판 (금테, 둥근 모서리, 모서리 장식)"),
    ("clear_band", 40, 34, clear_band, [0, 0, 6, 6], "클리어 가로 띠"),
    ("fx_sparkle", 16, 16, fx_sparkle, None, "반짝임 (엔진에서 여러 개 흩뿌림)"),
    ("tag_first", 32, 14, tag_first, [6, 6, 0, 0], "'첫 클리어' 꼬리표 판 (빨강, 글자는 엔진)"),
]

# ---- 방치 보상 ----
# 보물상자: 살짝 열린 뚜껑 사이로 금빛과 동전, 나무 결 명암, 금테·리벳, 자물쇠 (64 기준 좌표)
CHEST_DEF = ('<defs><linearGradient id="cw" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="#B07A48"/><stop offset="1" stop-color="#6E4428"/></linearGradient>'
             '<linearGradient id="cl" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="#C48A54"/><stop offset="1" stop-color="#8A5A34"/></linearGradient>'
             '<linearGradient id="cg" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="#FFE7A0"/><stop offset="1" stop-color="#D9952E"/></linearGradient>'
             '<radialGradient id="cin" cx=".5" cy=".7" r=".6"><stop offset="0" stop-color="#FFF6C8"/><stop offset=".6" stop-color="#F2B544"/><stop offset="1" stop-color="#B8701E"/></radialGradient></defs>')
CHEST = (
    # 안쪽 빛 + 동전 더미 (뚜껑과 몸통 사이)
    '<path d="M10 30h44l-2 6H12z" fill="url(#cin)"/>'
    '<g stroke="#8A5410" stroke-width="1"><ellipse cx="22" cy="31" rx="5" ry="2.200" fill="#F7C64A"/><ellipse cx="32" cy="29.500" rx="5.500" ry="2.400" fill="#FFD966"/><ellipse cx="42" cy="31" rx="5" ry="2.200" fill="#F7C64A"/><ellipse cx="27" cy="27.500" rx="4.500" ry="2" fill="#FFE37A"/></g>'
    # 몸통
    '<path d="M8 34h48v19a4 4 0 01-4 4H12a4 4 0 01-4-4z" fill="url(#cw)" stroke="#2A1A14" stroke-width="2"/>'
    '<path d="M12 40h40M12 47h40" stroke="#5A3420" stroke-width="1" opacity=".55"/>'
    # 몸통 금테
    '<path d="M8 34h48v4H8z" fill="url(#cg)" stroke="#2A1A14" stroke-width="1.500"/>'
    '<path d="M17 38v19M47 38v19" stroke="#2A1A14" stroke-width="5"/><path d="M17 38v19M47 38v19" stroke="url(#cg)" stroke-width="3"/>'
    '<g fill="#FFF3C4" stroke="#2A1A14" stroke-width=".8"><circle cx="17" cy="45" r="1.300"/><circle cx="17" cy="52" r="1.300"/><circle cx="47" cy="45" r="1.300"/><circle cx="47" cy="52" r="1.300"/></g>'
    # 뚜껑 (살짝 열림, 뒤로 기울어짐)
    '<path d="M9 28c0-11 9.500-18 23-18s23 7 23 18z" fill="url(#cl)" stroke="#2A1A14" stroke-width="2" transform="rotate(-6 32 28)"/>'
    '<g transform="rotate(-6 32 28)"><path d="M9.500 26.500h45" stroke="url(#cg)" stroke-width="3.500"/><path d="M9.500 26.500h45" stroke="#2A1A14" stroke-width=".8" opacity=".6"/>'
    '<path d="M17 25.500C17 17 20 12.500 23 11.500M47 25.500C47 17 44 12.500 41 11.500" stroke="#2A1A14" stroke-width="5" fill="none"/>'
    '<path d="M17 25.500C17 17 20 12.500 23 11.500M47 25.500C47 17 44 12.500 41 11.500" stroke="url(#cg)" stroke-width="3" fill="none"/>'
    '<path d="M14 20c4-5 9-7 14-7.500" stroke="#FFF3C4" stroke-width="1.400" fill="none" opacity=".55" stroke-linecap="round"/></g>'
    # 자물쇠
    '<path d="M27.500 35h9v10l-4.500 3.500-4.500-3.500z" fill="url(#cg)" stroke="#2A1A14" stroke-width="1.500" stroke-linejoin="round"/>'
    '<circle cx="32" cy="39.500" r="1.600" fill="#2A1A14"/><path d="M32 40.500v3" stroke="#2A1A14" stroke-width="1.400"/>'
    # 반짝임
    '<path d="M50 8c.5 3 2 4.500 5 5-3 .5-4.500 2-5 5-.5-3-2-4.500-5-5 3-.5 4.500-2 5-5z" fill="#FFF8DC"/>'
    '<path d="M12 12c.3 2 1.200 2.800 3 3-1.800.3-2.700 1.200-3 3-.3-1.800-1.200-2.700-3-3 1.800-.2 2.700-1 3-3z" fill="#FFF8DC" opacity=".85"/>'
)
def chest_svg(size):
    return f'<svg width="{size}" height="{size}" viewBox="0 0 64 64" style="display:block">{CHEST_DEF}{CHEST}</svg>'
popup_panel = (f'<svg width="48" height="48" viewBox="0 0 48 48" style="display:block">'
               f'<rect x=".75" y=".75" width="46.500" height="46.500" rx="12" fill="#3A2419" stroke="{GOLD}" stroke-width="1.500"/>'
               f'<rect x="1.500" y="1.500" width="45" height="14" rx="11" fill="#FFE7A0" opacity=".05"/></svg>')
P += [
    ("icon_chest", 24, 24, chest_svg(24), None, "방치 보상 상자 (메인 버튼용)"),
    ("icon_chest_big", 60, 60, chest_svg(60), None, "방치 보상 상자 (팝업용 큰 것)"),
    ("icon_ad", 16, 16, svg(16, f'<g fill="none" stroke="{DARK}" stroke-width="2.200" stroke-linejoin="round"><rect x="2.500" y="5" width="19" height="14" rx="3"/><path d="M10 9.200v5.600l4.800-2.800z" fill="{DARK}"/></g>'), None, "광고 보기 아이콘 (금색 버튼 위)"),
    ("icon_exp", 16, 16, svg(16, f'<circle cx="8" cy="8" r="7" fill="#9BD65A" stroke="{DARK}" stroke-width="1.500"/><path d="M8 3.500l1.300 3 3.200.3-2.400 2.100.7 3.100L8 10.400 5.200 12l.7-3.100L3.500 6.800l3.200-.3z" fill="#FFF8DC" stroke="{DARK}" stroke-width=".8" stroke-linejoin="round"/>', 16), None, "경험치 아이콘 (초록 원 + 별)"),
    ("win_popup", 48, 48, popup_panel, [16, 16, 16, 16], "불투명 팝업 판 (금테, 둥근 모서리)"),
]

# 스테이지 이동 창
def cell_box(bg, bd, bw=1.5):
    return f'<div style="width:100%;height:100%;box-sizing:border-box;background:{bg};border:{bw}px solid {bd};border-radius:10px"></div>'
P += [
    ("stage_cell", 48, 48, cell_box("#4A2E22", "rgba(230,194,122,.55)"), [10, 10, 10, 10], "스테이지 칸 (깬 곳)"),
    ("stage_cell_best", 48, 48, cell_box("#5A3A1E", "#F2B544", 2), [10, 10, 10, 10], "스테이지 칸 (최고 스테이지)"),
    ("stage_cell_lock", 48, 48, cell_box("rgba(0,0,0,.35)", "rgba(230,194,122,.2)"), [10, 10, 10, 10], "스테이지 칸 (잠김)"),
    ("stage_cell_sel", 60, 60, f'<div style="position:absolute;left:6px;top:6px;width:48px;height:48px;box-sizing:border-box;border:2px solid {GOLD};border-radius:10px;box-shadow:0 0 8px 3px rgba(242,181,68,.6)"></div>', None, "고른 칸 테두리+빛 (칸 위에 겹침, 사방 6 바깥)"),
    ("tag_best", 36, 12, box("#F2B544", "6px", False), [6, 6, 0, 0], "'최고' 꼬리표 판 (글자는 엔진)"),
    ("icon_lock", 14, 14, svg(14, f'<path d="M7.500 11V8a4.500 4.500 0 019 0v3" fill="none" stroke="{CREAM_HEX}" stroke-width="2.600"/><rect x="5" y="11" width="14" height="10" rx="2" fill="{CREAM_HEX}" stroke="{DARK}" stroke-width="1.800" stroke-linejoin="round"/>'), None, "자물쇠 (흐린 크림, 엔진에서 색 입혀도 됨)"),
    ("icon_check", 12, 12, svg(12, f'<circle cx="12" cy="12" r="10" fill="#9BD65A" stroke="{DARK}" stroke-width="2.400"/><path d="M7 12.500l3.300 3.300L17 9" fill="none" stroke="{DARK}" stroke-width="2.800" stroke-linecap="round" stroke-linejoin="round"/>'), None, "깬 곳 체크"),
    ("frame_face", 20, 20, f'<div style="width:100%;height:100%;box-sizing:border-box;border-radius:50%;border:1.5px solid {GOLD}"></div>', None, "지금 위치 얼굴 테두리 (안은 비어 있음, 초상화를 원형 마스크로 아래에)"),
]

# 던전·도전 (컨텐츠 창)
def ring(bd, bw, extra=""):
    return f'<div style="width:100%;height:100%;box-sizing:border-box;border-radius:50%;border:{bw}px solid {bd};{extra}"></div>'
P += [
    ("card_frame", 40, 40, f'<div style="width:100%;height:100%;box-sizing:border-box;border-radius:14px;border:1.5px solid {GOLD}"></div>', [14, 14, 14, 14], "던전 카드·보스 띠 금테 (안은 비어 있음, 그림 위에 겹침)"),
    ("card_frame_lock", 40, 40, f'<div style="width:100%;height:100%;box-sizing:border-box;border-radius:14px;border:1.5px solid rgba(230,194,122,.35)"></div>', [14, 14, 14, 14], "잠긴 카드 테두리"),
    ("card_shade", 120, 40, '<div style="width:100%;height:100%;border-radius:12.5px;background:linear-gradient(90deg,rgba(30,18,12,.92) 0%,rgba(30,18,12,.75) 50%,rgba(30,18,12,.15) 90%)"></div>', [13, 13, 13, 13], "카드 그림 위 어두운 그라데이션 (왼쪽 진함, 늘려 씀)"),
    ("card_chip", 32, 24, box("rgba(0,0,0,.35)", "12px", False), [12, 12, 0, 0], "카드 안 보상 칸 바닥"),
    ("icon_ticket", 16, 16, svg(16, f'<path d="M3 7a2 2 0 012-2h14a2 2 0 012 2v2.500a2.500 2.500 0 000 5V17a2 2 0 01-2 2H5a2 2 0 01-2-2v-2.500a2.500 2.500 0 000-5z" fill="#EE7A5C" stroke="{DARK}" stroke-width="1.800" stroke-linejoin="round"/><path d="M15 6v12" stroke="#FFD2C4" stroke-width="1.400" stroke-dasharray="2 2"/><path d="M8.500 10l1 2 2 .3-1.500 1.400.4 2-1.900-1-1.900 1 .4-2L5.500 12.300l2-.3z" fill="#FFF3DC"/>'), None, "던전 티켓"),
    ("step_row", 40, 40, cell_box("rgba(84,54,40,.75)", "rgba(230,194,122,.45)").replace("10px", "12px"), [12, 12, 12, 12], "도전 단계 줄 (깬 단계)"),
    ("step_row_next", 40, 40, cell_box("#5A3A1E", "#F2B544", 2).replace("10px", "12px"), [12, 12, 12, 12], "도전 단계 줄 (다음 도전)"),
    ("step_row_lock", 40, 40, cell_box("rgba(0,0,0,.3)", "rgba(230,194,122,.18)").replace("10px", "12px"), [12, 12, 12, 12], "도전 단계 줄 (잠김)"),
    ("step_row_sel", 52, 52, f'<div style="position:absolute;left:6px;top:6px;width:40px;height:40px;box-sizing:border-box;border:2px solid {GOLD};border-radius:12px;box-shadow:0 0 8px 3px rgba(242,181,68,.55)"></div>', [18, 18, 18, 18], "고른 단계 줄 테두리+빛 (줄 위에 겹침, 사방 6 바깥)"),
    ("face_ring", 44, 44, ring("rgba(230,194,122,.55)", 1.5), None, "보스 얼굴 테두리 (초상화는 원형 마스크로 아래에)"),
    ("face_ring_lock", 44, 44, ring("rgba(230,194,122,.25)", 1.5), None, "잠긴 보스 얼굴 테두리"),
    ("face_ring_sel", 56, 56, f'<div style="position:absolute;left:6px;top:6px;width:44px;height:44px;box-sizing:border-box;border-radius:50%;border:2px solid {GOLD};box-shadow:0 0 8px 3px rgba(242,181,68,.6)"></div>', None, "고른 보스 얼굴 테두리+빛 (사방 6 바깥)"),
]

# 격자 배치
W = 400; PAD = 6; x = PAD; y = PAD; rowh = 0; items = []; html = []
for name, w, h, body, nine, use in P:
    if x + w + PAD > W: x = PAD; y += rowh + PAD; rowh = 0
    html.append(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px">{body}</div>')
    items.append({"name": name, "x": x * SCALE, "y": y * SCALE, "w": w * SCALE, "h": h * SCALE,
                  "nine": [v * SCALE for v in nine] if nine else None, "use": use})
    x += w + PAD; rowh = max(rowh, h)
H = y + rowh + PAD
open(os.path.join(HERE, "parts.html"), "w", encoding="utf-8").write(
    f'<!doctype html><meta charset="utf-8"><style>@font-face{{font-family:DNF;src:url("../../../../../3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf")}}</style><body style="margin:0;background:transparent;width:{W}px;height:{H}px;position:relative">' + "".join(html) + "</body>")
json.dump({"width": W, "height": H, "scale": SCALE, "parts": items}, open(os.path.join(HERE, "manifest.json"), "w", encoding="utf-8"), ensure_ascii=False, indent=1)

# 부품 표
rows = ["| 파일 | 크기(px) | 9-slice L R T B (px) | 용도 |", "|---|---|---|---|"]
for it in items:
    nine = " ".join(str(v) for v in it["nine"]) if it["nine"] else "—"
    rows.append(f'| {it["name"]} | {it["w"]}×{it["h"]} | {nine} | {it["use"]} |')
open(os.path.join(HERE, "parts_table.md"), "w", encoding="utf-8").write("\n".join(rows) + "\n")
print(W, H, len(items))
