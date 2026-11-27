# 후드 화면 시안 (Plan/requests/UI_Hood_Window.md). kit_cocoa PNG 부품으로 조립하고, 전부 parts/의 PNG로 조립한다.
# 상태 3개(기본, 모자람, 최대 레벨)를 나란히 놓은 hood_mock.html을 만든다. 각 화면은 360x640 논리 크기.
import json, os
HERE = os.path.dirname(os.path.abspath(__file__))
M = {p["name"]: p for p in json.load(open(os.path.join(HERE, "manifest.json"), encoding="utf-8"))["parts"]}
ROOT = "../../../../../"
GOLD = "#E6C27A"; CREAM = "#FFF3DC"; MUTED = "#C2A888"; DARK = "#2A1A14"; TEAL = "#6CC6BE"
out = []
def img(name, x, y, w=None, h=None, extra=""):
    p = M[name]; w = w or p["w"] // 3; h = h or p["h"] // 3
    if p["nine"]:
        L, R, T, B = p["nine"]
        out.append(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;box-sizing:border-box;border-style:solid;border-width:{T/3}px {R/3}px {B/3}px {L/3}px;border-image:url(../parts/{name}.png) {T} {R} {B} {L} fill stretch;{extra}"></div>')
    else:
        out.append(f'<img src="../parts/{name}.png" style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;{extra}">')
def txt(s, x, y, size=10, color=CREAM, align="left", w=100):
    out.append(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;text-align:{align};font-size:{size}px;color:{color};white-space:nowrap">{s}</div>')
def tint(name, x, y, color, w=None, h=None):
    p = M[name]; w = w or p["w"] // 3; h = h or p["h"] // 3
    out.append(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;background:{color};-webkit-mask:url(../parts/{name}.png) center/100% 100%;border-radius:{h}px"></div>')
def raw(s): out.append(s)

def bubble(text, y):
    img("win_bubble", 60, y, 240, 26); img("win_bubble_tail", 174, y - 6)
    txt(text, 60, y + 7, 10, CREAM, "center", 240)

def screen(state):
    # state: 0 기본, 1 모자람, 2 최대 레벨
    raw(f'<img src="{ROOT}ArtSource/girl_base/animations/idle/frames/frame_000.png" style="position:absolute;left:116px;top:150px;width:70px;image-rendering:pixelated">'
        f'<img src="{ROOT}ArtSource/monsters/mob_slime_blue/animations/idle/frames/frame_000.png" style="position:absolute;left:196px;top:156px;width:70px;image-rendering:pixelated;transform:scaleX(-1)">')
    # 상단 HUD (기존 그대로)
    raw(f'<div style="position:absolute;left:14px;top:19px;width:44px;height:44px;border-radius:50%;image-rendering:pixelated;background:#3A251C url({ROOT}ArtSource/girl_base/animations/idle/frames/frame_000.png) -108px -122px / 256px 256px"></div>')
    img("frame_portrait", -3, -1); img("btn_main", 17, 62, 38, 15); txt("Lv.1", 17, 64, 9, DARK, "center", 38)
    img("hud_pill", 74, 14, 100, 22); txt("소라게 소녀", 84, 19)
    img("hud_pill", 232, 8, 56, 22); img("icon_gold", 236, 11); txt("137", 256, 13)
    img("hud_pill", 293, 8, 61, 22); img("icon_gem", 297, 11); txt("0", 317, 13); img("btn_plus", 335, 11)
    img("frame_stage", 59, 71, 242, 50); img("stage_crest", 143, 59); txt("Stage 1-1", 104, 84); txt("7/10", 156, 84, align="right", w=100)
    img("bar_track", 104, 98, 152, 8); tint("bar_fill", 105.5, 99.5, "#EE7A5C", 106, 5)
    # 성장 창 틀 (기존 그대로)
    img("win_panel", 0, 248, 360, 336); img("btn_close", 326, 235)
    img("win_title", 119, 231, 122, 34); txt("성장", 122, 242, 11, align="center", w=116)
    for i, t in enumerate(["강화", "레벨", "후드", "전직", "어빌리티"]):
        on = i == 2
        img("win_tab_on" if on else "win_tab_off", 10 + 69 * i, 266, 64, 28); txt(t, 10 + 69 * i, 275, 10, DARK if on else MUTED, "center", 64)
    raw(f'<div style="position:absolute;left:16px;right:16px;top:305.5px;height:1px;background:{GOLD}"></div>')
    img("win_rule_center", 114, 295); img("win_rule_end", 10, 301); img("win_rule_end", 334, 301, extra="transform:scaleX(-1)")
    # ---- 탭 안쪽 내용 (이 요청의 범위) ----
    lv = ["Lv.12", "Lv.12", "Lv.500"][state]; have, need = [(34, 13), (5, 13), (0, 0)][state]
    # 1 후드 무대: 가운데 후드, 왼쪽에 레벨, 오른쪽에 Hood_Exp (무대의 빈 양옆을 채우고 가장 중요한 숫자를 크게)
    img("win_chip", 10, 318, 340, 98)
    raw('<div style="position:absolute;left:150px;top:400px;width:60px;height:10px;border-radius:50%;background:rgba(0,0,0,.4)"></div>'
        f'<img src="{ROOT}ArtSource/pets/pet_hood/hood_001/animations/idle/frames/frame_000.png" style="position:absolute;left:52px;top:182px;width:256px;height:256px;image-rendering:pixelated">')
    txt("레벨", 20, 346, 9, MUTED, "center", 100); txt(lv, 20, 360, 17, GOLD, "center", 100)
    txt("Hood_Exp", 240, 346, 9, MUTED, "center", 100)
    num = "MAX" if state == 2 else f"{have} / {need}"
    w = len(num) * 9 + 24; x0 = 290 - w / 2
    img("icon_goods_hood_exp", x0, 359, 20, 20); txt(num, x0 + 24, 361, 15, CREAM if state != 1 else "#E2765E", "left", 90)
    # 2 한마디
    bubble(["딸랑딸랑! 더 먹고 싶어 해요", "꼬르륵… 배가 고픈가 봐요", "딸랑~ 배가 불러요"][state], 422)
    # 3 경험치 바
    img("bar_track", 20, 455, 320, 10)
    fill = 1 if state != 1 else have / need
    tint("bar_fill", 21.5, 456.5, "#9BD65A", round(317 * fill), 7)
    # 4 효과 줄
    img("win_row", 10, 472, 340, 40)
    tint("badge_fill", 18, 477, TEAL); img("badge_ring", 18, 477); img("icon_predation", 24, 483)
    txt("포식 가능 체력", 56, 479, 11)
    delta = "55%" if state == 2 else "50.12% → 50.13%"
    img("win_chip", 56, 493, len(delta) * 6 + 16, 14); txt(delta, 63, 495, 9, MUTED)
    # 5 수량 선택 (넣을지 미정, 자리만 확인용으로 흐리게)
    raw('<div style="position:absolute;left:0;top:0;opacity:.4">')
    txt("한 번에", 10, 524, 9, MUTED)
    for i, t in enumerate(["x1", "x10", "MAX"]):
        img("win_seg_on" if i == 0 else "win_seg_off", 46 + 44 * i, 518, 38, 20); txt(t, 46 + 44 * i, 523, 10, DARK if i == 0 else CREAM, "center", 38)
    raw('</div>')
    # 6 먹이기 버튼
    ok = state == 0
    img("btn_main" if ok else "btn_main_off", 10, 546, 340, 32)
    txt("MAX" if state == 2 else "먹이기", 10, 555, 12, DARK if ok else MUTED, "center", 340)
    # 하단 탭 (기획 확정: 6개. 아이콘 3개는 아직 없어 글자만)
    img("hud_nav", 0, 584, 360, 56)
    for i, (ic, t) in enumerate([("tab_grow", "성장"), ("tab_inventory", "인벤토리"), ("tab_skill", "스킬"), ("tab_mission", "미션"), ("tab_content", "컨텐츠"), ("tab_shop", "상점")]):
        cx = 30 + 60 * i
        if i == 0: img("hud_nav_on", cx - 15, 589)
        if ic: img(ic, cx - 10, 594)
        else: raw(f'<div style="position:absolute;left:{cx - 9}px;top:595px;width:18px;height:18px;border:1.5px dashed {MUTED};border-radius:5px;box-sizing:border-box;opacity:.6"></div>')
        txt(t, cx - 30, 622, 9, CREAM if i == 0 else MUTED, "center", 60)

phones = []
for s, label in enumerate(["1. 기본 (Hood_Exp 충분)", "2. 모자람", "3. 최대 레벨"]):
    out = []; screen(s)
    phones.append(f'<div class="col"><div class="cap">{label}</div><div class="phone">{"".join(out)}</div></div>')
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1104px; }}
.cap {{ color: #ddd; font: 13px/28px 'Malgun Gothic', sans-serif; height: 28px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{"".join(phones)}"""
open(os.path.join(HERE, "hood_mock.html"), "w", encoding="utf-8").write(html)
