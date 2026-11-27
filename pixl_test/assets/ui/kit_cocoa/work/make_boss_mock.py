# 보스 도전 시안. 기획 방향대로 보스 도전 버튼을 스테이지 바 바로 아래에 둔다.
# 상태 4개(진행 중, 도전 가능, 보스전, 실패 직후)를 나란히 놓은 boss_mock.html을 만든다. 각 화면은 360x640 논리 크기.
# 전부 parts/의 PNG로 조립한다.
import json, os
HERE = os.path.dirname(os.path.abspath(__file__))
M = {p["name"]: p for p in json.load(open(os.path.join(HERE, "manifest.json"), encoding="utf-8"))["parts"]}
ROOT = "../../../../../"
GOLD = "#E6C27A"; CREAM = "#FFF3DC"; MUTED = "#C2A888"; DARK = "#2A1A14"; RED = "#E2765E"; BOSSRED = "#C84B3A"
out = []
def img(name, x, y, w=None, h=None, extra=""):
    p = M[name]; w = w or p["w"] // 3; h = h or p["h"] // 3
    if p["nine"]:
        L, R, T, B = p["nine"]
        out.append(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;box-sizing:border-box;border-style:solid;border-width:{T/3}px {R/3}px {B/3}px {L/3}px;border-image:url(../parts/{name}.png) {T} {R} {B} {L} fill stretch;{extra}"></div>')
    else:
        out.append(f'<img src="../parts/{name}.png" style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;{extra}">')
def txt(s, x, y, size=10, color=CREAM, align="left", w=100, extra=""):
    out.append(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;text-align:{align};font-size:{size}px;color:{color};white-space:nowrap;{extra}">{s}</div>')
def tint(name, x, y, color, w=None, h=None):
    p = M[name]; w = w or p["w"] // 3; h = h or p["h"] // 3
    out.append(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;background:{color};-webkit-mask:url(../parts/{name}.png) center/100% 100%;border-radius:{h}px"></div>')
def raw(s): out.append(s)
def sprite(path, x, y, size=70, flip=False, extra=""):
    raw(f'<img src="{ROOT}ArtSource/{path}" style="position:absolute;left:{x}px;top:{y}px;width:{size}px;image-rendering:pixelated;{"transform:scaleX(-1);" if flip else ""}{extra}">')

# 초안 부품
def skull(x, y, size=16, color=CREAM, eye=DARK):
    raw(f'<svg width="{size}" height="{size}" viewBox="0 0 24 24" style="position:absolute;left:{x}px;top:{y}px" fill="{color}" stroke="{DARK}" stroke-width="2" stroke-linejoin="round">'
        '<path d="M12 2.500c-5 0-8.500 3.500-8.500 8 0 3 1.500 5 3.500 6v3.500h10V16.500c2-1 3.500-3 3.500-6 0-4.500-3.500-8-8.500-8z"/>'
        f'<circle cx="8.500" cy="11" r="2.200" fill="{eye}" stroke="none"/><circle cx="15.500" cy="11" r="2.200" fill="{eye}" stroke="none"/><path d="M12 14l-1.200 2h2.400z" fill="{eye}" stroke="none"/>'
        '<path d="M10 20v-2M14 20v-2" stroke-width="1.500"/></svg>')
def boss_button(x, y, w, h, state):
    # state: off(진행 중), on(도전 가능), give(보스전 중 포기)
    if state == "on":
        raw(f'<div class="glow" style="position:absolute;left:{x - 4}px;top:{y - 4}px;width:{w + 8}px;height:{h + 8}px;border-radius:{(h + 8) / 2}px;box-shadow:0 0 10px 3px rgba(242,181,68,.55)"></div>')
    img("btn_main" if state == "on" else "hud_pill", x, y, w, h)
    # 양끝 장식
    op = "" if state != "give" else "opacity:.5;"
    img("btn_boss_deco", x - 20, y + h / 2 - 11, extra=op); img("btn_boss_deco", x + w - 10, y + h / 2 - 11, extra="transform:scaleX(-1);" + op)
    label = {"off": "보스 도전", "on": "보스 도전", "give": "포기"}[state]
    if state == "give": txt(label, x, y + h / 2 - 6.5, 11, RED, "center", w); return
    color = {"on": DARK, "off": MUTED, "give": RED}[state]
    iw = 16 if state != "give" else 0
    tw = len(label) * 11
    sx = x + (w - tw - iw - (4 if iw else 0)) / 2
    if iw: img("icon_skull" if state == "on" else "icon_skull_light", sx, y + h / 2 - 8)
    txt(label, sx + iw + (4 if iw else 0), y + h / 2 - 6.5, 12, color, "left", 80)

def toggle(x, y, label, on, w):
    img("hud_pill", x, y, w, 22); txt(label, x + 9, y + 5.5, 9, CREAM if on else MUTED, "left", w)
    img("switch_on" if on else "switch_off", x + w - 34, y + 3)
AUTO_BATTLE = True
def hud():
    raw(f'<div style="position:absolute;left:14px;top:19px;width:44px;height:44px;border-radius:50%;image-rendering:pixelated;background:#3A251C url({ROOT}ArtSource/girl_base/animations/idle/frames/frame_000.png) -108px -122px / 256px 256px"></div>')
    img("frame_portrait", -3, -1); img("btn_main", 17, 62, 38, 15); txt("Lv.1", 17, 64, 9, DARK, "center", 38)
    img("hud_pill", 74, 14, 100, 22); txt("소라게 소녀", 84, 19)
    img("hud_pill", 232, 8, 56, 22); img("icon_gold", 236, 11); txt("137", 256, 13)
    img("hud_pill", 293, 8, 61, 22); img("icon_gem", 297, 11); txt("0", 317, 13); img("btn_plus", 335, 11)
    toggle(262, 36, "자동 전투", AUTO_BATTLE, 92)
    for i, n in enumerate(["icon_mail", "icon_rank", "icon_settings"]):
        img("hud_round", 324, 168 + 36 * i); img(n, 331, 175 + 36 * i)
    img("hud_round", 324, 276); img("icon_zoom", 331, 283)
    for i in range(3): img("dot_step_on" if i == ZOOM else "dot_step_off", 328 + 8 * i, 310)
def nav():
    img("hud_nav", 0, 584, 360, 56)
    for i, (ic, t) in enumerate([("tab_grow", "성장"), ("tab_inventory", "인벤토리"), ("tab_skill", "스킬"), ("tab_mission", "미션"), ("tab_content", "컨텐츠"), ("tab_shop", "상점")]):
        cx = 30 + 60 * i
        img(ic, cx - 10, 594); txt(t, cx - 30, 622, 9, MUTED, "center", 60)
def slots():
    for i in range(5):
        x = 76 + 44 * i
        img("hud_slot" if i < 2 else "hud_slot_empty", x, 530, 40, 40)
    txt("2.9", 76, 543, 11, CREAM, "center", 40)

ZOOM = 1
def stage_bar(count):
    img("frame_stage", 59, 71, 242, 50); img("stage_crest", 143, 59)
    txt("Stage 1-7", 104, 84); txt(f"{count}/10", 156, 84, align="right", w=100)
    img("bar_track", 104, 98, 152, 8)
    if count: tint("bar_fill", 105.5, 99.5, "#EE7A5C", round(149 * count / 10), 5)
def field():
    sprite("monsters/mob_slime/animations/idle/frames/frame_000.png", 110, 300); sprite("monsters/mob_slime_blue/animations/idle/frames/frame_000.png", 200, 286, flip=True)
    sprite("girl_base/animations/idle/frames/frame_000.png", 150, 340)
def screen(state):
    # 0 진행 중(자동 도전 꺼짐), 1 도전 가능(자동 도전 켜짐), 2 보스전, 3 실패 직후
    hud()
    # 보스 버튼 왼쪽: 자동 도전 스위치 (보스전 중에는 숨김)
    if state == 0:
        stage_bar(7); boss_button(120, 126, 120, 30, "off"); toggle(40, 130, "자동", False, 62)
        txt("10마리를 잡으면 도전할 수 있어요", 80, 162, 9, CREAM, "center", 200, "text-shadow:0 0 2px #2A1A14,0 0 2px #2A1A14")
        field()
    elif state == 1:
        stage_bar(10); boss_button(120, 126, 120, 30, "on"); img("dot_alert", 236, 122); toggle(40, 130, "자동", True, 62)
        txt("자동 도전: 곧 보스전을 시작해요", 80, 162, 9, "#FFE7A0", "center", 200, "text-shadow:0 0 2px #2A1A14,0 0 2px #2A1A14")
        field()
    elif state == 2:
        # 보스전: 스테이지 바 자리가 보스 바로 바뀜. 체력 바는 빨강, 아래에 남은 시간과 포기 버튼
        img("frame_stage", 59, 71, 242, 50); img("stage_crest", 143, 59)
        img("icon_skull_light", 102, 82); txt("보스 · 해초 슬라임 왕", 122, 84, 10, CREAM, "left", 140)
        img("bar_track", 104, 98, 152, 8); tint("bar_fill", 105.5, 99.5, BOSSRED, 92, 5)
        txt("62%", 156, 84, 10, CREAM, "right", 100)
        img("hud_pill", 146, 126, 68, 22)
        img("icon_timer", 154, 129)
        txt("0:24", 172, 131, 11, RED, "left", 40)
        boss_button(240, 126, 64, 22, "give")
        sprite("monsters/mob_slime/animations/idle/frames/frame_000.png", 150, 230, 140, flip=True)
        sprite("girl_base/animations/idle/frames/frame_000.png", 96, 330)
        raw('<div style="position:absolute;inset:0;pointer-events:none;box-shadow:inset 0 0 60px 20px rgba(43,42,76,.55)"></div>')
    else:
        # 실패 직후: 처치 수 0, 자동 도전 꺼짐 (알림 판 없음)
        stage_bar(0); boss_button(120, 126, 120, 30, "off"); toggle(40, 130, "자동", False, 62)
        field()
    slots(); nav()

phones = []
for s, label in enumerate(["1. 진행 중 (자동 도전 꺼짐)", "2. 도전 가능 (자동 도전 켜짐)", "3. 보스전 (남은 시간·포기)", "4. 실패 직후 (0/10, 자동 꺼짐)"]):
    out = []; screen(s)
    phones.append(f'<div class="col"><div class="cap">{label}</div><div class="phone">{"".join(out)}</div></div>')
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1476px; }}
.cap {{ color: #ddd; font: 13px/28px 'Malgun Gothic', sans-serif; height: 28px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{"".join(phones)}"""
open(os.path.join(HERE, "boss_mock.html"), "w", encoding="utf-8").write(html)
