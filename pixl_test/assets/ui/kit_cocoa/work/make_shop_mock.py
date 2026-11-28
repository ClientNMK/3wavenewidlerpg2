# 상점 창 시안: ① 추천 탭(기간 한정 패키지 배너, 무료·광고 보상, 보석으로 사는 일일 상점) ② 보석 탭(보석 상품 6칸, 첫 구매 2배).
# BM은 아직 설계 전이라 상품·가격은 모두 가안. 상품 칸·배너는 CSS·SVG 초안이다.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_inventory_mock as I
S = I.S; B = I.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, ROOT = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.ROOT
SH = "text-shadow:0 0 2px #2A1A14,0 0 3px #2A1A14"

def gem_pile(cx, cy, n):
    # 보석 더미: 개수가 많을수록 크게
    pts = [(0, 0), (-9, 5), (9, 5), (-5, -6), (5, -6), (0, 9), (-14, -1), (14, -1)][:n]
    for dx, dy in pts: img("icon_gem", cx + dx - 10, cy + dy - 10, 20, 20)

def ticket(x, y, s=16): img("icon_ticket", x, y, s, s)

def card(x, y, w, h, hot=False):
    raw(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;box-sizing:border-box;border-radius:12px;background:linear-gradient(#4A2E22,#36211A);border:1.5px solid {"#F2B544" if hot else "rgba(230,194,122,.55)"}"></div>')

def price_btn(x, y, w, label, kind="won"):
    if kind == "free":
        img("btn_main", x, y, w, 24); txt(label, x, y + 5, 10, DARK, "center", w)
    elif kind == "ad":
        img("btn_main", x, y, w, 24); img("icon_ad", x + w / 2 - 22, y + 4); txt(label, x + w / 2 - 4, y + 5, 10, DARK, "left", 40)
    elif kind == "gem":
        img("hud_pill", x, y, w, 24); img("icon_gem", x + w / 2 - 20, y + 4); txt(label, x + w / 2 - 2, y + 5, 10, CREAM, "left", 40)
    elif kind == "done":
        img("btn_main_off", x, y, w, 24); txt(label, x, y + 5, 10, DARK, "center", w)
    else:
        img("btn_main", x, y, w, 24); txt(label, x, y + 5, 10, DARK, "center", w)

def frame(tab):
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right); S.group(.85, "180px 0", S.center)
    top = 120
    I.window("상점", top)
    for i, t in enumerate(["추천", "보석", "패키지"]):
        on = i == tab
        img("win_tab_on" if on else "win_tab_off", 16 + 111 * i, top + 22, 106, 28)
        txt(t, 16 + 111 * i, top + 29, 12, DARK if on else MUTED, "center", 106)
    if tab != 0: img("dot_alert", 116, top + 19)
    B.nav()
    img("hud_nav_on", 315, 589); img("tab_shop", 320, 594); txt("상점", 300, 622, 9, CREAM, "center", 60)
    return top

def recommend():
    top = frame(0)
    # 기간 한정 패키지 배너
    y = top + 60
    raw(f'<div style="position:absolute;left:16px;top:{y}px;width:328px;height:108px;border-radius:14px;overflow:hidden;background:url({ROOT}ArtSource/backgrounds/bg_dungeon_2001/bg_dungeon_2001.png) center 55% / 340px auto"></div>')
    img("card_shade", 17.5, y + 1.5, 325, 105); img("card_frame", 16, y, 328, 108)
    raw(f'<img src="{ROOT}ArtSource/girl_base/animations/idle/frames/frame_000.png" style="position:absolute;left:214px;top:{y - 24}px;width:132px;image-rendering:pixelated">')
    img("tag_first", 28, y + 10, 64, 16); txt("기간 한정", 28, y + 12, 9, CREAM, "center", 64)
    txt("성장 시작 패키지", 28, y + 32, 15, "#FFE7A0", "left", 200, SH)
    img("icon_gem", 28, y + 56); txt("3,000", 46, y + 58, 10, CREAM, "left", 50, SH)
    ticket(98, y + 56); txt("×10", 116, y + 58, 10, CREAM, "left", 40, SH)
    img("icon_gold", 146, y + 56); txt("100만", 164, y + 58, 10, CREAM, "left", 50, SH)
    img("icon_timer", 28, y + 84, 14, 14); txt("2일 13:20 남음", 46, y + 85, 9, CREAM, "left", 100, SH)
    img("btn_main", 238, y + 76, 96, 26); txt("₩ 5,500", 238, y + 82, 11, DARK, "center", 96)
    raw(f'<div style="position:absolute;left:176px;top:{y + 84}px;font-size:9px;color:#C2A888;text-decoration:line-through;{SH}">₩ 22,000</div>')
    # 무료·광고
    y = top + 180
    txt("매일 무료", 20, y, 10, MUTED, "left", 100); img("icon_timer", 250, y - 1, 12, 12); txt("초기화 07:42:10", 266, y, 8, MUTED, "left", 80)
    for i, (title, draw, kind, label) in enumerate([
            ("무료 보석", lambda cx, cy: gem_pile(cx, cy, 3), "free", "받기"),
            ("광고 보석", lambda cx, cy: gem_pile(cx, cy, 5), "ad", "2/3"),
            ("광고 티켓", lambda cx, cy: (ticket(cx - 12, cy - 12, 24)), "ad", "1/2")]):
        x = 16 + 112 * i
        card(x, y + 16, 104, 96, hot=(i == 0))
        draw(x + 52, y + 50)
        txt(title, x, y + 70, 9, CREAM, "center", 104)
        price_btn(x + 10, y + 84, 84, label, kind)
        if i == 0: img("dot_alert", x + 94, y + 12)
    # 일일 상점 (보석으로 구매)
    y = top + 306
    txt("일일 상점", 20, y, 10, MUTED, "left", 100); txt("보석으로 사요", 244, y, 8, MUTED, "right", 100)
    items = [("던전 티켓", "ticket", "×1", "50", "gem"), ("골드 묶음", "gold", "50만", "100", "gem"), ("Hood_Exp", "hood", "×200", "80", "gem"),
             ("소환 티켓", "ticket2", "×1", "150", "gem"), ("골드 묶음", "gold", "20만", "", "done"), ("꿈 조각", "drop", "×30", "60", "gem")]
    for i, (name, ic, amt, price, kind) in enumerate(items):
        col, rowi = i % 3, i // 3
        x, yy = 16 + 112 * col, y + 16 + 66 * rowi
        card(x, yy, 104, 60)
        icon = {"ticket": "icon_ticket", "ticket2": "icon_ticket", "gold": "icon_gold", "hood": "icon_goods_hood_exp", "drop": "icon_goods_drop"}[ic]
        img(icon, x + 8, yy + 8, 22, 22)
        if ic == "ticket2": raw(f'<div style="position:absolute;left:{x + 8}px;top:{yy + 8}px;width:22px;height:22px;filter:hue-rotate(200deg)"></div>')
        txt(name, x + 34, yy + 8, 9, CREAM, "left", 70); txt(amt, x + 34, yy + 20, 9, "#FFE7A0", "left", 70)
        price_btn(x + 8, yy + 32, 88, "구매 완료" if kind == "done" else price, kind)
    return "".join(B.out)

def gems():
    top = frame(1)
    y = top + 62
    packs = [(60, "₩ 1,200", 2), (330, "₩ 5,500", 3), (680, "₩ 11,000", 4), (1400, "₩ 22,000", 5), (3700, "₩ 55,000", 6), (7800, "₩ 110,000", 8)]
    for i, (n, price, pile) in enumerate(packs):
        col, rowi = i % 3, i // 3
        x, yy = 16 + 112 * col, y + 132 * rowi
        first = i < 4
        card(x, yy, 104, 124, hot=first)
        raw(f'<div style="position:absolute;left:{x + 22}px;top:{yy + 18}px;width:60px;height:50px;background:radial-gradient(circle closest-side,rgba(255,180,160,.35),rgba(238,122,92,0))"></div>')
        gem_pile(x + 52, yy + 42, pile)
        txt(f"{n:,}", x, yy + 66, 13, CREAM, "center", 104)
        if first:
            img("tag_first", x + 6, yy + 6, 50, 16); txt("첫 구매 2배", x + 6, yy + 8, 8, CREAM, "center", 50)
            txt(f"+{n:,} 보너스", x, yy + 84, 8, "#9BD65A", "center", 104)
        price_btn(x + 8, yy + 96, 88, price)
    txt("첫 구매 2배는 상품마다 한 번만 적용돼요", 16, y + 272, 8, MUTED, "center", 328)
    return "".join(B.out)

shots = [("① 추천: 기간 한정 패키지 · 매일 무료 · 일일 상점", recommend()), ("② 보석: 첫 구매 2배", gems())]
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 732px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "shop_mock.html"), "w", encoding="utf-8").write(html)
