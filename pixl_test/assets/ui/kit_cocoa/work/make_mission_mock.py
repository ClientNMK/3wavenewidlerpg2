# 미션 창 시안: 일일 / 주간 / 반복 탭, 미션 포인트 바, 미션 줄(진행·받기·완료), 모두 받기.
# MissionChart_Dev 기준(종류 3개, 일일 미션마다 포인트 10, 주간 미션 '포인트 150 모으기'). 미션 이름·보상 수량은 차트 숫자로 만든 가안.
# 미션 줄 칸·로그인 아이콘은 CSS·SVG 초안이다.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_inventory_mock as I
S = I.S; B = I.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, ROOT = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.ROOT
GREEN = "#9BD65A"

def login_icon(x, y, s=20):
    raw(f'<svg width="{s}" height="{s}" viewBox="0 0 24 24" style="position:absolute;left:{x}px;top:{y}px" fill="none" stroke="{GOLD}" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><rect x="3.500" y="5" width="17" height="15" rx="2"/><path d="M3.500 10h17M8 3v4M16 3v4M9 15l2 2 4-4"/></svg>')

ICON = {"login": "icon_login", "time": "icon_timer", "kill": "icon_skull_light", "dungeon": "tab_content", "daily": "icon_check"}

def row(y, kind, name, have, need, reward, state, times=1):
    # state: go(진행 중) / get(받기) / done(받음). times = 반복 미션이 목표의 몇 배 쌓였는지 (2 이상이면 ×N)
    x, w, h = 16, 328, 56
    done = state == "done"
    img("win_row", x, y, w, h, "opacity:.55" if done else "")
    if state == "get": img("step_row_sel", x - 6, y - 6, w + 12, h + 12)
    img("icon_ring", x + 10, y + 10)
    img(ICON[kind], x + 18, y + 18, 20, 20)
    txt(name, x + 56, y + 10, 11, MUTED if done else CREAM, "left", 170)
    # 진행 바 (반복 미션은 목표를 넘겨도 실제 수를 보여 줌)
    img("bar_track", x + 56, y + 32, 140, 8)
    fw = round(137 * min(1, have / need))
    if fw: tint("bar_fill", x + 57.5, y + 33.5, GREEN if have >= need else "#EE7A5C", fw, 5)
    shown = have if times > 1 else min(have, need)
    txt(f"{shown:,}/{need:,}", x + 56, y + 31, 8, CREAM, "right", 136, "text-shadow:0 0 2px #2A1A14,0 0 2px #2A1A14")
    # 보상 (×N이면 합친 수량)
    rx = x + 206
    img("card_chip", rx, y + 16, 50, 24, "opacity:.6" if done else "")
    img("icon_gem", rx + 6, y + 20); txt(f"{reward * times:,}", rx + 24, y + 22, 10, MUTED if done else CREAM, "left", 30)
    # 버튼
    bx = x + 262
    if state == "get":
        img("btn_main", bx, y + 14, 58, 28); txt("받기", bx, y + 21, 11, DARK, "center", 58)
        if times > 1: img("tag_first", bx + 34, y + 6, 30, 16); txt(f"×{times}", bx + 34, y + 8, 9, CREAM, "center", 30)
        else: img("dot_alert", bx + 50, y + 10)
    elif state == "go":
        img("hud_pill", bx, y + 14, 58, 28); txt("이동", bx, y + 21, 11, CREAM, "center", 58)
    else:
        img("icon_check", bx + 12, y + 22); txt("완료", bx + 28, y + 21, 10, GREEN, "left", 30)

ROWS = [
    # 일일
    [("kill", "몬스터 500마리 처치", 500, 500, 100, "get"), ("time", "10분 동안 플레이", 600, 600, 100, "get"),
     ("dungeon", "던전 3번 플레이", 1, 3, 100, "go"), ("kill", "보스 3번 처치", 2, 3, 100, "go"),
     ("login", "로그인", 1, 1, 100, "done")],
    # 주간 (받기 → 진행 중 → 받음)
    [("kill", "몬스터 2,000마리 처치", 2000, 2000, 200, "get"), ("daily", "일일 미션 15개 완료", 12, 15, 200, "go"),
     ("dungeon", "던전 20번 플레이", 6, 20, 200, "go"), ("time", "30분 동안 플레이", 1800, 1800, 200, "done"),
     ("login", "로그인", 1, 1, 200, "done")],
    # 반복 (목표마다 계속 받음, 2배 이상 쌓이면 ×N)
    [("kill", "몬스터 1,000마리 처치", 2350, 1000, 10, "get", 2), ("time", "100분 동안 플레이", 6000, 6000, 10, "get", 1),
     ("dungeon", "던전 10번 플레이", 4, 10, 10, "go")],
]
RESET = ["초기화 07:42:10", "초기화 3일 07시간", "목표를 채울 때마다 받을 수 있어요"]

def window(tab):
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right); S.group(.85, "180px 0", S.center)
    top = 120
    I.window("미션", top)
    for i, t in enumerate(["일일", "주간", "반복"]):
        on = i == tab
        img("win_tab_on" if on else "win_tab_off", 16 + 111 * i, top + 22, 106, 28)
        txt(t, 16 + 111 * i, top + 29, 12, DARK if on else MUTED, "center", 106)
    for i in range(3):
        if i != tab: img("dot_alert", 116 + 111 * i, top + 19)
    # 초기화 안내 한 줄
    py = top + 60
    img("win_chip", 16, py, 328, 28)
    if tab < 2: img("icon_timer", 26, py + 7, 14, 14)
    txt(RESET[tab], 44 if tab < 2 else 16, py + 8, 9, CREAM if tab < 2 else MUTED, "left" if tab < 2 else "center", 280 if tab < 2 else 328)
    ly = top + 100
    for i, r in enumerate(ROWS[tab]): row(ly + 62 * i, *r)
    img("btn_main", 60, 540, 240, 36); txt("모두 받기", 60, 551, 13, DARK, "center", 240)
    B.nav()
    img("hud_nav_on", 195, 589); img("tab_mission", 200, 594); txt("미션", 180, 622, 9, CREAM, "center", 60)
    return "".join(B.out)

shots = [("① 일일", window(0)), ("② 주간: 일일 미션 15개 완료, 받기 → 진행 중 → 받음", window(1)), ("③ 반복: 2배 이상 쌓이면 받기에 ×2", window(2))]
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1104px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "mission_mock.html"), "w", encoding="utf-8").write(html)
