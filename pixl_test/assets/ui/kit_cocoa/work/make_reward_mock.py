# 공용 보상 획득 연출 시안 (미션, 우편, 상점, 방치 보상, 보스 클리어 등 모든 재화·아이템 받기에 같은 연출).
# ① 1개 ② 여러 개(한 줄 5개까지) ③ 많을 때(두 줄, 넘치면 스크롤) ④ 닫으면 재화는 위 재화 칸으로, 아이템은 하단 인벤토리 탭으로 날아감.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_inventory_mock as I
S = I.S; B = I.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK = B.GOLD, B.CREAM, B.MUTED, B.DARK
SHADOW = "text-shadow:0 0 2px #2A1A14,0 0 3px #2A1A14"

def base():
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right); S.group(.85, "180px 0", S.center)
    S.group(1, "180px 584px", S.bottom); B.nav()

def reward(kind, grade, x, y, s, label, pop=False):
    c = I.GRADE[grade][1]
    sc = "transform:scale(1.18);" if pop else ""
    raw(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{s}px;height:{s}px;box-sizing:border-box;border-radius:12px;border:2px solid {c};background:radial-gradient(circle,{c}55,rgba(38,24,19,.95) 70%);{sc}"></div>')
    if kind in ("gold", "gem", "exp"):
        ic = {"gold": "icon_gold", "gem": "icon_gem", "exp": "icon_goods_hood_exp"}[kind]
        img(ic, x + s * .22, y + s * .22, s * .56, s * .56)
    else:
        I.item_icon(kind, x + s * .2, y + s * .16, s * .6)
    txt(label, x - 12, y + s + 4, 11 if s >= 56 else 10, GOLD, "center", s + 24, SHADOW)

def popup(items, step=None, scroll=False):
    raw('<div style="position:absolute;inset:0;background:rgba(10,6,4,.7)"></div>')
    n = len(items)
    rows = 1 if n <= 5 else 2
    cy = 300 if rows == 1 else 290
    img("boss_intro_rays", 40, cy - 140, 280, 280, "opacity:.8")
    img("win_title", 109, cy - 108 - (24 if rows == 2 else 0), 142, 38); txt("보상 획득", 112, cy - 96 - (24 if rows == 2 else 0), 13, CREAM, "center", 136)
    s = 64 if n == 1 else 52
    gap = 10
    per = min(n, 5)
    for i, (kind, grade, label) in enumerate(items[:10]):
        r, col = divmod(i, 5)
        cnt = per if r == 0 else min(n - 5, 5)
        w = cnt * s + (cnt - 1) * gap
        x = 180 - w / 2 + col * (s + gap)
        y = cy - s / 2 - (40 if rows == 2 and r == 0 else 0) + (40 if rows == 2 and r == 1 else 0) - 10
        if step is not None and i > step: continue
        reward(kind, grade, x, y, s, label, pop=(step == i))
    if scroll:
        txt("▼ 더 있어요 (스크롤)", 80, cy + 92, 9, MUTED, "center", 200)
    txt("화면을 누르면 닫혀요", 80, cy + 118 if rows == 2 else cy + 96, 9, MUTED, "center", 200)

def fly():
    raw(f'<svg width="360" height="640" style="position:absolute;left:0;top:0">'
        f'<path d="M160 300 C 170 190, 220 90, 248 19" fill="none" stroke="{GOLD}" stroke-width="1.200" stroke-dasharray="3 4" opacity=".75"/>'
        f'<path d="M200 300 C 240 220, 300 110, 309 19" fill="none" stroke="#E8604E" stroke-width="1.200" stroke-dasharray="3 4" opacity=".75"/>'
        f'<path d="M180 320 C 160 420, 110 520, 90 600" fill="none" stroke="#B47CF0" stroke-width="1.200" stroke-dasharray="3 4" opacity=".75"/></svg>')
    for i, (x, y, s) in enumerate([(164, 232, 16), (184, 164, 14), (214, 98, 13), (238, 46, 12)]):
        img("icon_gold", x, y, s, s, f"opacity:{.5 + i * .15}")
    for i, (x, y, s) in enumerate([(224, 230, 14), (262, 150, 12), (292, 72, 11)]):
        img("icon_gem", x, y, s, s, f"opacity:{.5 + i * .2}")
    for i, (x, y, s) in enumerate([(166, 380, 26), (128, 470, 22), (100, 550, 18)]):
        I.item_slot(x, y, s, "sword", 3)
    raw('<div style="position:absolute;left:228px;top:6px;width:64px;height:26px;border-radius:13px;box-shadow:0 0 10px 3px rgba(242,181,68,.7)"></div>')
    raw('<div style="position:absolute;left:66px;top:588px;width:48px;height:46px;border-radius:12px;box-shadow:0 0 12px 4px rgba(180,124,240,.6)"></div>')
    txt("+5,000", 150, 13, 11, GOLD, "right", 76, SHADOW)
    img("dot_alert", 102, 590)

one = [("gold", 4, "5,000")]
three = [("gold", 4, "5,000"), ("gem", 5, "50"), ("sword", 3, "x1")]
many = [("gold", 4, "12,000"), ("gem", 5, "100"), ("exp", 2, "30"), ("sword", 4, "x1"), ("armor", 3, "x2"),
        ("hat", 2, "x1"), ("ring", 1, "x3"), ("sword", 0, "x4"), ("armor", 0, "x2"), ("ring", 3, "x1")]
shots = []
base(); popup(one); shots.append(("① 보상 1개: 큰 칸 하나", "".join(B.out)))
base(); popup(three, step=2); shots.append(("② 여러 개: 왼쪽부터 0.12초 간격으로 튀어나옴", "".join(B.out)))
base(); popup(many, scroll=True); shots.append(("③ 많을 때: 5개씩 두 줄, 넘치면 스크롤", "".join(B.out)))
base(); fly(); shots.append(("④ 닫으면: 재화는 위로, 아이템은 인벤토리 탭으로", "".join(B.out)))
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{B.ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1476px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{B.ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "reward_mock.html"), "w", encoding="utf-8").write(html)
