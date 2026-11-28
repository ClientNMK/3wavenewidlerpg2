# 사냥 기록 창 시안: 전투 화면 왼쪽의 작은 창. 잰 시간, 분당 처치 수, 그동안 얻은 골드·경험치, 초기화. 접으면 작은 버튼.
# 판과 초기화·접기 아이콘은 CSS·SVG 초안이다.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_hud_scale_mock as S
B = S.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, ROOT = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.ROOT

def icon(x, y, d, s=14, color=GOLD, w=2.4):
    raw(f'<svg width="{s}" height="{s}" viewBox="0 0 24 24" style="position:absolute;left:{x}px;top:{y}px" fill="none" stroke="{color}" stroke-width="{w}" stroke-linecap="round" stroke-linejoin="round"><path d="{d}"/></svg>')
RESET = "M4 12a8 8 0 1 0 2.600-5.900M4 4v4.500h4.500"
CHART = "M4 20V10M10 20V4M16 20v-7M21 20H3"
FOLD = "M15 6l-6 6 6 6"

def panel(x, y, confirm=False):
    w, h = 124, 132
    img("hud_panel", x, y, w, h)
    # 머리줄: 제목 + 잰 시간 + 접기
    img("icon_chart", x + 8, y + 6, 12, 12); txt("사냥 기록", x + 24, y + 8, 9, CREAM, "left", 60)
    img("icon_fold", x + w - 20, y + 6)
    raw(f'<div style="position:absolute;left:{x + 8}px;top:{y + 24}px;width:{w - 16}px;height:1px;background:{GOLD};opacity:.4"></div>')
    rows = [("icon_timer", "잰 시간", "12:34", CREAM), ("icon_skull_light", "총 처치", "477마리", CREAM), ("icon_skull_light", "분당 처치", "38마리", "#FFE7A0"),
            ("icon_gold", "골드", "+24.6만", GOLD), ("icon_exp", "경험치", "+3.12만", "#9BD65A")]
    for i, (ic, name, val, col) in enumerate(rows):
        ry = y + 30 + 14 * i
        img(ic, x + 8, ry, 12, 12); txt(name, x + 24, ry + 1.5, 8, MUTED, "left", 50); txt(val, x + 50, ry + 0.5, 9, col, "right", w - 58)
    # 초기화
    img("hud_pill", x + 8, y + h - 24, w - 16, 18); img("icon_reset", x + 38, y + h - 21, 11, 11); txt("초기화", x + 52, y + h - 20, 9, CREAM, "left", 50)
    if confirm:
        img("hud_panel_dark", x, y, w, h)
        txt("기록을 지우고", x, y + 34, 9, CREAM, "center", w); txt("다시 잴까요?", x, y + 48, 9, CREAM, "center", w)
        img("hud_pill", x + 8, y + 82, 50, 22); txt("취소", x + 8, y + 88, 9, CREAM, "center", 50)
        img("btn_main", x + 66, y + 82, 50, 22); txt("초기화", x + 66, y + 88, 9, DARK, "center", 50)

def folded(x, y):
    img("hud_round", x, y); img("icon_chart", x + 8, y + 8)
    img("hud_pill", x + 26, y + 5, 62, 20); img("icon_skull_light", x + 31, y + 8, 12, 12); txt("38/분", x + 46, y + 10, 9, "#FFE7A0", "left", 40)

def screen(state):
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right)
    def center():
        B.stage_bar(7); B.boss_button(120, 126, 120, 30, "off"); B.toggle(40, 130, "자동", False, 62)
    S.group(.85, "180px 0", center); S.group(1, "180px 584px", S.bottom); B.nav()
    if state == 0: folded(8, 150)
    else: panel(8, 150, state == 2)
    return "".join(B.out)

shots = [("① 접힘: 버튼 + 분당 처치만", screen(0)), ("② 펼침: 잰 시간·총 처치·분당 처치·골드·경험치", screen(1)), ("③ 초기화를 누르면 한 번 물어봄", screen(2))]
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1104px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "stats_mock.html"), "w", encoding="utf-8").write(html)
