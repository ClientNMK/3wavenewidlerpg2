# 잘라 낸 PNG 부품만으로 화면을 조립해 preview.html을 만든다 (9-slice 값 검증용). 장식 3종은 아직 없어 민무늬 판으로 대신함.
import json, os
HERE = os.path.dirname(os.path.abspath(__file__))
M = {p["name"]: p for p in json.load(open(os.path.join(HERE, "manifest.json"), encoding="utf-8"))["parts"]}
ROOT = "../../../../../"
out = []
def img(name, x, y, w=None, h=None, extra=""):
    p = M[name]; w = w or p["w"] // 3; h = h or p["h"] // 3
    if p["nine"]:
        L, R, T, B = p["nine"]
        out.append(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;box-sizing:border-box;border-style:solid;border-width:{T/3}px {R/3}px {B/3}px {L/3}px;border-image:url(../parts/{name}.png) {T} {R} {B} {L} fill stretch;{extra}"></div>')
    else:
        out.append(f'<img src="../parts/{name}.png" style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;{extra}">')
def txt(s, x, y, size=10, color="#FFF3DC", align="left", w=100):
    out.append(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;text-align:{align};font-size:{size}px;color:{color};white-space:nowrap">{s}</div>')
def tint(name, x, y, color, w=None, h=None):
    p = M[name]; w = w or p["w"] // 3; h = h or p["h"] // 3
    out.append(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;background:{color};-webkit-mask:url(../parts/{name}.png) center/100% 100%;border-radius:{h}px"></div>')

# 상단 HUD
img("hud_pill", 74, 14, 100, 22); txt("소라게 소녀", 84, 19)
img("bar_track", 80, 40, 88, 8); tint("bar_fill", 81.5, 41.5, "#9BD65A", 36, 5)
img("hud_pill", 232, 8, 56, 22); img("icon_gold", 236, 11); txt("137", 256, 13)
img("hud_pill", 293, 8, 61, 22); img("icon_gem", 297, 11); txt("0", 317, 13); img("btn_plus", 335, 11)
out.append(f'<div style="position:absolute;left:14px;top:19px;width:44px;height:44px;border-radius:50%;image-rendering:pixelated;background:#3A251C url({ROOT}ArtSource/girl_base/animations/idle/frames/frame_000.png) -108px -122px / 256px 256px"></div>')
img("frame_portrait", -3, -1); img("btn_main", 17, 62, 38, 15); txt("Lv.1", 17, 64, 9, "#2A1A14", "center", 38)
img("frame_stage", 59, 71, 242, 50); img("stage_crest", 143, 59); txt("Stage 1-1", 104, 84); txt("7/10", 156, 84, align="right", w=100)
img("bar_track", 104, 98, 152, 8); tint("bar_fill", 105.5, 99.5, "#EE7A5C", 106, 5)
for i, n in enumerate(["icon_mail", "icon_rank", "icon_settings"]):
    img("hud_round", 324, 130 + 36 * i); img(n, 331, 137 + 36 * i)
img("dot_alert", 346, 127)
# 창
img("win_panel", 0, 248, 360, 336)
img("btn_close", 326, 235)
img("win_title", 119, 231, 122, 34); txt("성장", 122, 242, 11, align="center", w=116)
for i, t in enumerate(["강화", "레벨", "전직", "어빌리티"]):
    img("win_tab_on" if i == 0 else "win_tab_off", 10 + 86.5 * i, 266, 80.5, 28)
    txt(t, 10 + 86.5 * i, 274, 11, "#2A1A14" if i == 0 else "#C2A888", "center", 80.5)
out.append('<div style="position:absolute;left:16px;right:16px;top:305.5px;height:1px;background:#E6C27A"></div>'); img("win_rule_center", 114, 295); img("win_rule_end", 10, 301); img("win_rule_end", 334, 301, extra="transform:scaleX(-1)")
txt("한 번에", 10, 322, 9, "#C2A888")
for i, t in enumerate(["x1", "x10", "x100"]):
    img("win_seg_on" if i == 0 else "win_seg_off", 46 + 44 * i, 316, 38, 20); txt(t, 46 + 44 * i, 321, 10, "#2A1A14" if i == 0 else "#FFF3DC", "center", 38)
stats = [("icon_atk", "공격력", "Lv.13만", "562만 → 712만", "45양", "#FF8A5C", True), ("icon_hp", "체력", "Lv.11.4만", "57만2000 → 57만2005", "2067자", "#FF6F7D", True),
         ("icon_regen", "체력 재생", "Lv.12.1만", "1194자 → 1195자", "2774자", "#8CCB4F", True), ("icon_def", "방어력", "Lv.14.1만", "14만1101 → 14만1102", "1961자", "#6FB6F2", True),
         ("icon_pierce", "관통", "Lv.16.5만", "16만5101 → 16만5102", "55양", "#B58CF0", False)]
for i, (ic, name, lv, delta, cost, col, ok) in enumerate(stats):
    y = 343 + 50 * i
    if y + 44 > 584: break
    img("win_row", 10, y, 340, 44)
    tint("badge_fill", 18, y + 7, col); img("badge_ring", 18, y + 7); img(ic, 24, y + 13)
    txt(name, 56, y + 7, 11); txt(lv, 56 + len(name) * 11 + 6, y + 9, 9, "#C2A888")
    img("win_chip", 56, y + 23, len(delta) * 6 + 14, 14); txt(delta, 63, y + 25, 9, "#C2A888")
    img("btn_main" if ok else "btn_main_off", 268, y + 6, 74, 32)
    txt("강화", 268, y + 10, 10, "#2A1A14" if ok else "#C2A888", "center", 74); txt(cost, 268, y + 22, 8, "#2A1A14" if ok else "#C2A888", "center", 74)
out.append('<div style="position:absolute;left:16px;right:16px;top:558.5px;height:1px;background:#E6C27A"></div>')
img("win_rule_close", 114, 548); img("win_rule_end", 10, 554); img("win_rule_end", 334, 554, extra="transform:scaleX(-1)")
# 하단 탭
img("hud_nav", 0, 584, 360, 56)
for i, (ic, t) in enumerate([("tab_grow", "성장"), ("tab_inventory", "인벤토리"), ("tab_skill", "스킬"), ("tab_mission", "미션"), ("tab_content", "컨텐츠"), ("tab_shop", "상점")]):
    cx = 30 + 60 * i
    if i == 0: img("hud_nav_on", cx - 15, 589)
    img(ic, cx - 10, 594); txt(t, cx - 30, 622, 9, "#FFF3DC" if i == 0 else "#C2A888", "center", 60)

html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style><div class="phone">{''.join(out)}</div>"""
open(os.path.join(HERE, "preview.html"), "w", encoding="utf-8").write(html)
