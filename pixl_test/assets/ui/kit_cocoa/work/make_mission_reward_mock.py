# 미션 보상 획득 연출 시안: ① 완료한 미션의 받기 버튼 → ② 보상 팝업(빛줄기 + 보상이 하나씩 튀어나옴) → ③ 팝업을 닫으면 재화가 위쪽 재화 칸으로 날아감 → ④ 줄이 '받음'으로 바뀜.
# 체크 도장, 미션 탭 내용은 SVG·CSS 초안이다.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_inventory_mock as I
S = I.S; B = I.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, RED = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.RED
OK = "#9BD65A"

def check(x, y, s=18):
    raw(f'<svg width="{s}" height="{s}" viewBox="0 0 24 24" style="position:absolute;left:{x}px;top:{y}px"><circle cx="12" cy="12" r="10.500" fill="{OK}" stroke="{DARK}" stroke-width="1.500"/>'
        f'<path d="M7 12.500l3.300 3.300L17 9" fill="none" stroke="{DARK}" stroke-width="2.600" stroke-linecap="round" stroke-linejoin="round"/></svg>')
def reward_icon(kind, x, y, s=16):
    if kind == "gold": img("icon_gold", x, y, s, s)
    elif kind == "gem": img("icon_gem", x, y, s, s)
    else: img("icon_goods_hood_exp", x, y, s, s)

MISSIONS = [
    ("몬스터 100마리 처치", 100, 100, [("gold", "5,000"), ("gem", "50")]),
    ("보스 1회 도전", 1, 1, [("gem", "30")]),
    ("골드 강화 10회", 6, 10, [("gold", "3,000")]),
    ("후드 먹이기 5회", 2, 5, [("exp", "20")]),
]
def mission_window(claimed=False, highlight=False):
    top = 200
    I.window("미션", top)
    for i, t in enumerate(["전직", "일일", "반복"]):
        on = i == 1
        img("win_tab_on" if on else "win_tab_off", 10 + 115 * i, top + 16, 110, 28); txt(t, 10 + 115 * i, top + 24, 11, DARK if on else MUTED, "center", 110)
        if i == 1 and not claimed: img("dot_alert", 10 + 115 * i + 102, top + 12)
    for n, (name, have, need, rewards) in enumerate(MISSIONS):
        y = top + 56 + 64 * n
        done = have >= need
        got = done and claimed and n < 2
        img("win_row", 10, y, 340, 58, "opacity:.55" if got else "")
        txt(name, 22, y + 8, 11, CREAM if not got else MUTED, "left", 200)
        img("bar_track", 22, y + 28, 150, 8); tint("bar_fill", 23.5, y + 29.5, OK if done else GOLD, round(147 * min(1, have / need)), 5)
        txt(f"{have}/{need}", 176, y + 26, 9, MUTED, "left", 60)
        rx = 22
        for k, v in rewards:
            reward_icon(k, rx, y + 40, 12); txt(v, rx + 15, y + 41, 9, CREAM, "left", 60); rx += 60
        bx = 262
        if got:
            img("hud_pill", bx, y + 15, 78, 28); check(bx + 10, y + 20); txt("받음", bx + 32, y + 23, 11, OK, "left", 40)
        elif done:
            if highlight and n == 0:
                raw(f'<div style="position:absolute;left:{bx - 4}px;top:{y + 11}px;width:86px;height:36px;border-radius:18px;box-shadow:0 0 12px 4px rgba(242,181,68,.7)"></div>')
            img("btn_main", bx, y + 15, 78, 28); txt("받기", bx, y + 23, 11, DARK, "center", 78); img("dot_alert", bx + 70, y + 11)
        else:
            img("hud_pill", bx, y + 15, 78, 28); txt("진행 중", bx, y + 23, 10, MUTED, "center", 78)
    img("btn_main", 10, 548, 340, 30); txt("모두 받기", 10, 556, 12, DARK, "center", 340)

def base():
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right); S.group(.85, "180px 0", S.center)

def reward_popup(step):
    raw('<div style="position:absolute;inset:0;background:rgba(10,6,4,.7)"></div>')
    sc = 1.1 if step == 0 else 1
    img("boss_intro_rays", 60, 160, 240, 240, f"transform:rotate({20 + step * 15}deg) scale({sc});opacity:.85")
    img("win_title", 109, 186, 142, 38); txt("보상 획득", 112, 198, 13, CREAM, "center", 136)
    items = [("gold", "5,000", "#F2B544"), ("gem", "50", "#E8604E")]
    shown = [1, 2][step]
    for i, (k, v, c) in enumerate(items):
        x = 116 + 72 * i
        if i >= shown: continue
        pop = "transform:scale(1.2);" if (step == 0 and i == 0) or (step == 1 and i == 1) else ""
        raw(f'<div style="position:absolute;left:{x}px;top:248px;width:56px;height:56px;box-sizing:border-box;border-radius:12px;border:2px solid {c};background:radial-gradient(circle,{c}55,rgba(38,24,19,.95) 70%);{pop}"></div>')
        reward_icon(k, x + 12, 260, 32)
        txt(v, x - 10, 310, 12, GOLD, "center", 76, "text-shadow:0 0 2px #2A1A14,0 0 3px #2A1A14")
    if step == 1:
        txt("화면을 누르면 닫혀요", 80, 380, 9, MUTED, "center", 200)

def fly():
    # 재화가 위쪽 재화 칸으로 날아가는 모습 (점선은 이동 경로)
    raw(f'<svg width="360" height="640" style="position:absolute;left:0;top:0"><path d="M150 300 C 160 180, 220 90, 248 19" fill="none" stroke="{GOLD}" stroke-width="1.200" stroke-dasharray="3 4" opacity=".7"/>'
        f'<path d="M200 300 C 230 200, 300 100, 309 19" fill="none" stroke="#E8604E" stroke-width="1.200" stroke-dasharray="3 4" opacity=".7"/></svg>')
    for i, (x, y, s) in enumerate([(160, 230, 16), (182, 160, 14), (214, 96, 13), (238, 44, 12)]):
        img("icon_gold", x, y, s, s, f"opacity:{.5 + i * .15}")
    for i, (x, y, s) in enumerate([(214, 228, 14), (254, 150, 12), (290, 70, 11)]):
        img("icon_gem", x, y, s, s, f"opacity:{.5 + i * .2}")
    raw('<div style="position:absolute;left:228px;top:6px;width:64px;height:26px;border-radius:13px;box-shadow:0 0 10px 3px rgba(242,181,68,.7)"></div>')
    txt("+5,000", 228, 34, 10, GOLD, "center", 64, "text-shadow:0 0 2px #2A1A14,0 0 2px #2A1A14")
    txt("+50", 292, 34, 10, "#FF9C8A", "center", 64, "text-shadow:0 0 2px #2A1A14,0 0 2px #2A1A14")

shots = []
base(); mission_window(highlight=True); B.nav(); shots.append(("① 미션 완료: 받기 버튼이 빛남", "".join(B.out)))
base(); mission_window(); B.nav(); reward_popup(0); shots.append(("② 누르면: 보상 팝업, 빛줄기, 보상이 하나씩 튀어나옴", "".join(B.out)))
base(); mission_window(); B.nav(); reward_popup(1); shots.append(("③ 다 나오면: 누르면 닫힘", "".join(B.out)))
base(); mission_window(claimed=True); B.nav(); fly(); shots.append(("④ 닫으면: 재화가 위로 날아가고 숫자 증가, 줄은 '받음'", "".join(B.out)))
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{B.ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1476px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{B.ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "mission_reward_mock.html"), "w", encoding="utf-8").write(html)
