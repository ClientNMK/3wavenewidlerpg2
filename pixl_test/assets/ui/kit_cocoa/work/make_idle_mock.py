# 방치(오프라인) 보상 시안: 다시 접속했을 때 10분 이상 비웠으면 자동으로 뜨는 보상 팝업. 메인 화면 버튼은 없다.
# 보물상자 아이콘, 광고 아이콘, 팝업 판은 SVG·CSS 초안이다.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_hud_scale_mock as S
B = S.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, RED = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.RED

def chest(x, y, s, glow=False):
    if glow: raw(f'<div style="position:absolute;left:{x - 4}px;top:{y - 4}px;width:{s + 8}px;height:{s + 8}px;border-radius:50%;box-shadow:0 0 12px 4px rgba(242,181,68,.6)"></div>')
    raw(f'<svg width="{s}" height="{s}" viewBox="0 0 24 24" style="position:absolute;left:{x}px;top:{y}px" stroke="{DARK}" stroke-width="1.400" stroke-linejoin="round">'
        '<path d="M3.500 11h17v8.500a1 1 0 01-1 1h-15a1 1 0 01-1-1z" fill="#8A5A34"/><path d="M3.500 11c0-4 3.500-6.500 8.500-6.500s8.500 2.500 8.500 6.500z" fill="#A86E3E"/>'
        f'<path d="M3.500 11h17M9 4.900v15.600M15 4.900v15.600" stroke="{GOLD}" stroke-width="1.600"/><rect x="10.200" y="9.500" width="3.600" height="4.500" rx="1" fill="#F2B544"/></svg>')
def ad_icon(x, y, s=14, color=DARK):
    raw(f'<svg width="{s}" height="{s}" viewBox="0 0 24 24" style="position:absolute;left:{x}px;top:{y}px" fill="none" stroke="{color}" stroke-width="2.200" stroke-linejoin="round"><rect x="2.500" y="5" width="19" height="14" rx="3"/><path d="M10 9.200v5.600l4.800-2.800z" fill="{color}"/></svg>')
def idle_button(x, y, full):
    if full: raw(f'<div style="position:absolute;left:{x - 4}px;top:{y - 4}px;width:46px;height:46px;border-radius:50%;box-shadow:0 0 12px 4px rgba(242,181,68,.6)"></div>')
    img("hud_round", x, y, 38, 38); img("icon_chest", x + 7, y + 6)
    img("hud_pill", x - 6, y + 34, 50, 18)
    txt("가득!" if full else "3:12", x - 6, y + 38, 9, "#FFE7A0" if full else CREAM, "center", 50)
    if full: img("dot_alert", x + 30, y - 2)

def base(full):
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right); S.group(.85, "180px 0", S.center)
    S.group(1, "180px 584px", S.bottom); B.nav()

def popup():
    raw('<div style="position:absolute;inset:0;background:rgba(10,6,4,.62)"></div>')
    x0, y0, w, h = 24, 120, 312, 400
    img("win_popup", x0, y0, w, h, "filter:drop-shadow(0 6px 14px rgba(0,0,0,.6))")
    img("win_title", 119, y0 - 17, 122, 34); txt("방치 보상", 122, y0 - 6, 11, align="center", w=116)
    img("btn_close", x0 + w - 12, y0 - 12)
    raw(f'<div style="position:absolute;left:110px;top:{y0 - 20}px;width:140px;height:140px;background:radial-gradient(circle closest-side,rgba(255,231,160,.5),rgba(242,181,68,.18) 55%,rgba(242,181,68,0) 100%)"></div>'); img("icon_chest_big", 140, y0 + 10, 80, 80)
    # 방치 시간: 지금 / 최대
    txt("방치한 시간", 40, y0 + 92, 9, MUTED, "left", 100); txt("3시간 12분 / 최대 12시간", 180, y0 + 92, 9, CREAM, "right", 140)
    img("bar_track", 40, y0 + 108, 280, 8); tint("bar_fill", 41.5, y0 + 109.5, GOLD, 74, 5)
    raw(f'<div style="position:absolute;left:40px;top:{y0 + 128}px;width:280px;height:1px;background:{GOLD};opacity:.6"></div>')
    img("win_rule_gem", 175, y0 + 123.5)
    # 얻은 것: 아이콘 · 이름 · 시간당 · 이번에 얻은 양
    txt("시간당", 186, y0 + 138, 8, MUTED, "right", 50); txt("획득", 262, y0 + 138, 8, MUTED, "right", 56)
    rows = [("icon_gold", "골드", "1,240", "3,968"), ("icon_exp", "경험치", "860", "2,752")]
    for i, (ic, name, per, got) in enumerate(rows):
        y = y0 + 152 + 38 * i
        img("win_row", 36, y, 288, 32)
        img(ic, 46, y + 8)
        txt(name, 70, y + 10, 10, CREAM, "left", 100)
        txt(f"+{per}", 186, y + 10, 10, MUTED, "right", 50)
        txt(f"+{got}", 262, y + 9, 12, GOLD, "right", 56)
    # 아이템: 지금은 없음. 장비가 생기면 이 자리에 칸이 들어간다 (시안에서는 점선 자리로 표시)
    txt("아이템", 40, y0 + 236, 9, MUTED, "left", 100)
    raw(f'<div style="position:absolute;left:36px;top:{y0 + 252}px;width:288px;height:52px;box-sizing:border-box;border-radius:10px;border:1.5px dashed rgba(230,194,122,.4)"></div>')
    txt("장비가 생기면 여기에 표시 (지금은 숨김)", 36, y0 + 272, 9, MUTED, "center", 288)
    # 버튼: 받기(보조) / 광고 보고 2배(주)
    img("hud_pill", 40, y0 + 348, 132, 34); txt("받기", 40, y0 + 358, 12, CREAM, "center", 132)
    img("btn_main", 188, y0 + 348, 132, 34); img("icon_ad", 214, y0 + 357); txt("2배 받기", 234, y0 + 358, 12, DARK, "left", 80)

shots = []
base(False); popup(); shots.append(("다시 접속했을 때 (10분 이상 비웠으면 자동으로 뜸)", "".join(B.out)))
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{B.ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 360px; }}
.cap {{ color: #ddd; font: 13px/28px 'Malgun Gothic', sans-serif; height: 28px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{B.ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "idle_mock.html"), "w", encoding="utf-8").write(html)
