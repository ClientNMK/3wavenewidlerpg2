# 의상 창 여는 사이드 버튼: 오른쪽 사이드 버튼 줄 맨 아래(줌 점 아래)에 추가. ① 전투 화면 ② 사이드 줄 확대.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_hud_scale_mock as S
B = S.B
img, txt, raw = B.img, B.txt, B.raw
ROOT = B.ROOT

def screen():
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right)
    def center(): B.stage_bar(7); B.boss_button(120, 126, 120, 30, "off"); B.toggle(40, 130, "자동", False, 62)
    S.group(.85, "180px 0", center); S.group(1, "180px 584px", S.bottom); B.nav()
    img("hud_round", 324, 322); img("icon_costume", 331, 329)
    return "".join(B.out)

s = screen()
big = '<div class="phone" style="width:240px;height:640px;background:#3A2A20">' + "".join([
    f'<img src="../parts/hud_round.png" style="position:absolute;left:60px;top:{y}px;width:120px">' +
    f'<img src="../parts/{n}.png" style="position:absolute;left:88px;top:{y + 28}px;width:64px">' for y, n in [(40, "icon_mail"), (190, "icon_zoom"), (340, "icon_costume")]]) + '</div>'
shots = [("① 전투 화면: 줌 아래에 의상 버튼", f'<div class="phone">{s}</div>'), ("② 확대 (우편 · 줌 · 의상)", big)]
cols = "".join(f'<div><div class="cap">{l}</div>{b}</div>' for l, b in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 612px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "side_costume_mock.html"), "w", encoding="utf-8").write(html)
