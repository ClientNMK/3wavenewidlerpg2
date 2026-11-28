# 의상 창 시안: 캐릭터 / 후드 탭. 위 미리보기(입은 모습), 보유·착용 효과, 의상 칸(등급 테두리), 착용 버튼.
# CostumeChart_Dev 기준(Grade, 보유 효과 Have_Stat, 착용 효과 Equip_Stat). 캐릭터 의상은 아직 그림이 없어 알껍데기 후드(hood_0)를 임시로 쓴다.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_inventory_mock as I
S = I.S; B = I.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, ROOT = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.ROOT
SH = "text-shadow:0 0 2px #2A1A14,0 0 3px #2A1A14"
A = ROOT + "ArtSource/"
GIRL = A + "girl_base/animations/idle/frames/frame_000.png"
EGG = A + "hood_0/animations/idle/frames/frame_000.png"
PET = A + "pets/pet_hood/hood_001/animations/idle/frames/frame_000.png"
SHELL = A + "pets/pet_hood/hood_001_skin_shell/animations/idle/frames/frame_000.png"

def sprite(src, x, y, box_w, box_h, size, dx, dy, extra=""):
    # 128×128 프레임을 size로 키워 상자 안에 자른다. (dx, dy) = 상자 안에서 프레임 왼쪽 위
    raw(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{box_w}px;height:{box_h}px;overflow:hidden;border-radius:9px">'
        f'<img src="{src}" style="position:absolute;left:{dx}px;top:{dy}px;width:{size}px;image-rendering:pixelated;{extra}"></div>')

from PIL import Image
def paint(name, x, y, color, w, h):
    # 흰 부품에 엔진에서 색 곱하기 (9-slice 테두리는 border-image 대신 mask로 미리보기)
    raw(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;background:{color};-webkit-mask:url(../parts/{name}.png) center/100% 100%"></div>')
def fit(src, x, y, box_w, box_h, body_h, cx, bottom, extra=""):
    # 보이는 몸(투명 아닌 영역) 높이를 body_h로, 몸 아래 가운데를 상자 안 (cx, bottom)에 둔다
    l, t, r, b = Image.open(src.replace(ROOT, "C:/3wavestudio/3wavenewidlerpg2/")).getbbox()
    k = body_h / (b - t)
    sprite(src, x, y, box_w, box_h, 128 * k, cx - (l + r) / 2 * k, bottom - b * k, extra)

def card(x, y, src, grade, pet, state, sel=False):
    # state: on(착용 중) / have(보유) / lock(미보유)
    s = 58
    c = I.GRADE[grade][1] if state != "lock" else "#6A5A4A"
    paint("costume_cell", x, y, c, s, s); paint("costume_cell_line", x, y, c, s, s)
    lock = "filter:brightness(0);opacity:.45" if state == "lock" else ""
    fit(src, x + 2, y + 2, s - 4, s - 4, 46, (s - 4) / 2, s - 8, lock)
    if state == "lock": img("icon_lock", x + 22, y + 22)
    if sel: img("stage_cell_sel", x - 1, y - 1, s + 2, s + 2, "transform:scale(1.2);transform-origin:center")
    if state == "on":
        img("tag_first", x + 14, y - 8, 30, 14); txt("착용", x + 14, y - 7, 7, CREAM, "center", 30)
    txt(I.GRADE[grade][0] if state != "lock" else "???", x, y + s + 2, 8, I.GRADE[grade][1] if state != "lock" else MUTED, "center", s, SH)

DATA = [
    # 이름, 등급, 그림, 상태, 보유 효과, 착용 효과
    [("소라게 외출복", 0, GIRL, "have", "공격력 +20", "없음"),
     ("알껍데기 후드", 2, EGG, "on", "공격력 +50", "치명타 확률 +10%")] + [("?", g, GIRL, "lock", "", "") for g in (1, 2, 3, 3, 4, 4, 5, 5)],
    [("별무늬 망토", 0, PET, "have", "공격력 +20", "없음"),
     ("조개 모자", 2, SHELL, "on", "공격력 +50", "포식 피해 +10%")] + [("?", g, PET, "lock", "", "") for g in (1, 2, 3, 3, 4, 4, 5, 5)],
]

def window(tab, sel, worn=True):
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right); S.group(.85, "180px 0", S.center)
    top = 120
    I.window("의상", top)
    for i, t in enumerate(["캐릭터", "후드"]):
        on = i == tab
        img("win_tab_on" if on else "win_tab_off", 16 + 166 * i, top + 22, 162, 28)
        txt(t, 16 + 166 * i, top + 29, 12, DARK if on else MUTED, "center", 162)
    pet = tab == 1
    name, g, src, state, have, equip = DATA[tab][sel]
    if not worn: state = "have"
    # 코스튬 포인트 (캐릭터·후드 함께, 모일수록 단계 스탯)
    py = top + 60
    img("win_chip", 16, py, 328, 28)
    hanger(24, py + 5); txt("160p", 46, py + 7, 11, "#FFE7A0", "left", 40)
    img("bar_track", 84, py + 10, 116, 8); tint("bar_fill", 85.5, py + 11.5, GOLD, round(113 * 160 / 200), 5)
    txt("받을 보상 2개", 208, py + 8.5, 8, "#FFE7A0", "left", 110)
    img("icon_fold", 324, py + 7, 14, 14, "transform:scaleX(-1)"); img("dot_alert", 336, py - 4)
    # 미리보기: 입은 모습 크게
    y = top + 96
    img("costume_stage", 16, y); img("costume_shadow", 185, y + 108)
    fit(src, 16, y, 328, 130, 100, 224, 116)
    img("card_frame", 16, y, 328, 130)
    paint("tag_grade", 28, y + 14, I.GRADE[g][1], 34, 16); txt(I.GRADE[g][0], 28, y + 17, 9, DARK, "center", 34)
    txt(name, 28, y + 36, 15, "#FFE7A0", "left", 170, SH)
    txt("후드 정령이 입어요" if pet else "소녀가 입어요", 28, y + 58, 9, MUTED, "left", 170, SH)
    hanger(28, y + 100, 14); txt("코스튬 포인트 +5p", 46, y + 101, 9, CREAM, "left", 120, SH)
    # 효과
    ey = y + 138
    img("win_chip", 16, ey, 328, 40)
    txt("보유 효과", 28, ey + 6, 9, MUTED, "left", 60); txt(have, 28, ey + 21, 10, CREAM, "left", 140)
    txt("착용 효과", 188, ey + 6, 9, MUTED, "left", 60); txt(equip, 188, ey + 21, 10, "#9BD65A" if equip != "없음" else MUTED, "left", 150)
    raw(f'<div style="position:absolute;left:180px;top:{ey + 8}px;width:1px;height:24px;background:{GOLD};opacity:.35"></div>')
    # 의상 칸 5열
    gy = ey + 50
    for i, (n, gg, s, st, *_ ) in enumerate(DATA[tab]):
        card(17 + 67 * (i % 5), gy + 72 * (i // 5), s, gg, pet, "have" if st == "on" and not worn else st, i == sel)
    # 버튼
    if state == "on": img("hud_pill", 60, 546, 240, 30); txt("해제", 60, 554, 13, CREAM, "center", 240)
    else: img("btn_main", 60, 546, 240, 30); txt("착용", 60, 554, 13, DARK, "center", 240)
    B.nav()
    return "".join(B.out)

def hanger(x, y, s=18): img("icon_costume_point", x, y, s, s)

STEPS = [(50, "공격력 +100", "on"), (100, "체력 +5%", "get"), (150, "치명타 확률 +3%", "get"), (200, "공격 속도 +5%", "next"), (300, "치명타 피해 +20%", "lock")]
def points():
    window(0, 1)
    raw('<div style="position:absolute;inset:0;background:rgba(10,6,4,.7)"></div>')
    x, y, w, h = 24, 130, 312, 400
    img("win_popup", x, y, w, h); img("win_title", 105, y - 17, 150, 34); txt("코스튬 포인트", 108, y - 6, 11, align="center", w=144); img("btn_close", 306, y - 13)
    hanger(x + 100, y + 28, 24); txt("160p", x + 132, y + 28, 20, "#FFE7A0", "left", 80)
    txt("의상을 얻을 때마다 포인트가 쌓여요 (캐릭터·후드 함께)", x, y + 60, 8, MUTED, "center", w)
    for i, (p, stat, st) in enumerate(STEPS):
        ry = y + 84 + 52 * i
        img("win_row", x + 14, ry, w - 28, 46, "opacity:.55" if st == "lock" else "")
        if st == "get": img("step_row_sel", x + 8, ry - 6, w - 16, 58)
        img("tag_best" if st != "lock" else "card_chip", x + 24, ry + 15, 44, 16); txt(f"{p}p", x + 24, ry + 17, 9, DARK if st != "lock" else MUTED, "center", 44)
        txt(stat, x + 78, ry + 9 if st == "next" else ry + 16, 11, CREAM if st != "lock" else MUTED, "left", 140)
        if st == "on": img("icon_check", x + 222, ry + 17); txt("적용 중", x + 238, ry + 16, 10, "#9BD65A", "left", 50)
        elif st == "get": img("btn_main", x + 222, ry + 9, 66, 28); txt("받기", x + 222, ry + 16, 11, DARK, "center", 66); img("dot_alert", x + 280, ry + 5)
        elif st == "next":
            img("bar_track", x + 78, ry + 28, 160, 8); tint("bar_fill", x + 79.5, ry + 29.5, GOLD, round(157 * 160 / 200), 5)
            txt("160/200", x + 244, ry + 26, 9, CREAM, "left", 50)
        else: img("icon_lock", x + 254, ry + 16)
    img("btn_main", x + 56, y + h - 52, 200, 34); txt("모두 받기", x + 56, y + h - 42, 13, DARK, "center", 200)
    return "".join(B.out)

shots = [("① 캐릭터: 입은 의상을 고르면 '해제'", window(0, 1)), ("② 해제한 뒤: 착용 표시 없음, 버튼 '착용'", window(0, 1, False)), ("③ 후드: 정령이 입는 의상", window(1, 1)), ("④ 포인트 바를 누르면: 단계 스탯 받기 · 모두 받기", points())]
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1476px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "costume_mock.html"), "w", encoding="utf-8").write(html)
