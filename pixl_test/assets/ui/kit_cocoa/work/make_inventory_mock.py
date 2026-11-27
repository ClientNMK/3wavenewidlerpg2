# 인벤토리(장비) 창 시안. 기획: 장비는 소녀만 찬다, 뽑기로 얻고 같은 장비를 합쳐 등급을 올린다(추천안, 미확정).
# 상태 2개(장비 목록, 아이템 상세)를 나란히 놓은 inventory_mock.html. 등급 칸·아이템 그림은 CSS·SVG 초안이다.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_hud_scale_mock as S
B = S.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
ROOT = B.ROOT; GOLD = B.GOLD; CREAM = B.CREAM; MUTED = B.MUTED; DARK = B.DARK; RED = B.RED

GRADE = [("일반", "#9A8A7A"), ("고급", "#7CC46A"), ("희귀", "#5FA8E8"), ("영웅", "#B47CF0"), ("전설", "#F2B544"), ("신화", "#E8604E")]
ITEM = {  # 임시 아이템 그림 (흰 채움 + 어두운 선, 배경은 등급 색)
    "sword": "M5 19l2-5L17 4h3v3L10 17l-5 2zM7 13l4 4",
    "armor": "M8 3h8l1 3 3 1v6l-3 1v7H7v-7l-3-1V7l3-1z",
    "hat": "M3 17c3-1 5-1 9-1s6 0 9 1l-2 3H5zM6 16c0-6 2-11 6-11s6 5 6 11",
    "ring": "M12 9a6 6 0 110 12 6 6 0 010-12zM9 6l3-3 3 3-3 3z",
}
def item_icon(kind, x, y, size):
    raw(f'<svg width="{size}" height="{size}" viewBox="0 0 24 24" style="position:absolute;left:{x}px;top:{y}px" fill="#FFF3DC" stroke="#3A251C" stroke-width="1.800" stroke-linejoin="round" stroke-linecap="round"><path d="{ITEM[kind]}"/></svg>')
def item_slot(x, y, s, kind, grade, lv=None, count=None, equipped=False, empty_label=None):
    if kind is None:
        raw(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{s}px;height:{s}px;box-sizing:border-box;border-radius:10px;background:rgba(0,0,0,.32);border:1.5px dashed rgba(230,194,122,.5)"></div>')
        if empty_label: txt(empty_label, x, y + s / 2 - 5, 8, MUTED, "center", s)
        return
    c = GRADE[grade][1]
    raw(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{s}px;height:{s}px;box-sizing:border-box;border-radius:10px;border:2px solid {c};'
        f'background:radial-gradient(circle at 50% 40%,{c}66,{c}22 60%,rgba(38,24,19,.9));box-shadow:inset 0 0 0 1px rgba(42,26,20,.8)"></div>')
    item_icon(kind, x + s * .2, y + s * .16, s * .6)
    if lv: txt(f"Lv.{lv}", x + 4, y + s - 13, 8, CREAM, "left", s, "text-shadow:0 0 2px #2A1A14,0 0 2px #2A1A14")
    if count:
        have, need = count
        raw(f'<div style="position:absolute;left:{x + 4}px;top:{y + s - 4.5}px;width:{s - 8}px;height:3px;border-radius:2px;background:rgba(0,0,0,.5)"><div style="width:{min(1, have / need) * 100}%;height:100%;border-radius:2px;background:{"#9BD65A" if have >= need else GOLD}"></div></div>')
        txt(f"{have}/{need}", x, y + 3, 8, CREAM, "right", s - 4, "text-shadow:0 0 2px #2A1A14,0 0 2px #2A1A14")
    if equipped:
        raw(f'<div style="position:absolute;left:{x - 3}px;top:{y - 3}px;width:15px;height:15px;border-radius:50%;background:{GOLD};border:1.5px solid {DARK};box-sizing:border-box;font-size:8px;line-height:12px;text-align:center;color:{DARK}">E</div>')
    if count and count[0] >= count[1]: img("dot_alert", x + s - 7, y - 3)

def window(title, top):
    img("win_panel", 0, top, 360, 584 - top); img("btn_close", 326, top - 13)
    img("win_title", 119, top - 17, 122, 34); txt(title, 122, top - 6, 11, align="center", w=116)

def screen(state):
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right); S.group(.85, "180px 0", S.center)
    top = 200
    window("인벤토리", top)
    # 장착 영역: 가운데 소녀, 양옆 장비 칸 4개
    img("win_chip", 10, top + 16, 340, 112)
    # 소녀는 장착 영역 안에서만 보이게 자른다 (1.5배)
    raw(f'<div style="position:absolute;left:10px;top:{top + 16}px;width:340px;height:112px;overflow:hidden;border-radius:10px">'
        f'<div style="position:absolute;left:130px;top:96px;width:80px;height:12px;border-radius:50%;background:rgba(0,0,0,.4)"></div>'
        f'<img src="{ROOT}ArtSource/girl_base/animations/idle/frames/frame_000.png" style="position:absolute;left:74px;top:-62px;width:192px;image-rendering:pixelated"></div>')
    item_slot(22, top + 26, 42, "sword", 4, 12, equipped=False)
    item_slot(22, top + 76, 42, "armor", 2, 7)
    item_slot(296, top + 26, 42, "hat", 3, 9)
    item_slot(296, top + 76, 42, None, 0, empty_label="장신구")
    img("hud_pill", 120, top + 104, 120, 22); txt("전투력", 132, top + 110, 9, MUTED, "left", 40); txt("12,345", 168, top + 108, 12, GOLD, "left", 80)
    # 분류
    for i, t in enumerate(["전체", "무기", "갑옷", "모자", "장신구"]):
        on = i == 0
        img("win_tab_on" if on else "win_tab_off", 10 + 69 * i, top + 136, 64, 24); txt(t, 10 + 69 * i, top + 143, 10, DARK if on else MUTED, "center", 64)
    # 장비 목록 (5칸 × 2.5줄, 아래는 스크롤)
    items = [("sword", 4, 12, (2, 3), True), ("sword", 2, 5, (5, 5), False), ("armor", 2, 7, (1, 3), True), ("hat", 3, 9, (2, 3), True), ("ring", 1, 3, (3, 3), False),
             ("armor", 1, 4, (4, 5), False), ("hat", 0, 2, (1, 5), False), ("ring", 5, 1, (0, 1), False), ("sword", 0, 1, (2, 5), False), ("armor", 0, 1, (1, 5), False),
             ("ring", 3, 2, (1, 3), False), ("hat", 1, 3, (2, 5), False), ("sword", 1, 2, (3, 5), False), ("armor", 3, 1, (0, 3), False), ("ring", 0, 1, (4, 5), False)]
    raw(f'<div style="position:absolute;left:0;top:{top + 168}px;width:360px;height:176px;overflow:hidden">')
    for n, (k, g, lv, cnt, eq) in enumerate(items):
        col, row = n % 5, n // 5
        item_slot(24 + 64 * col, 4 + 64 * row, 56, k, g, lv, cnt, eq)
    raw('</div>')
    raw(f'<div style="position:absolute;left:10px;top:{top + 316}px;width:340px;height:28px;background:linear-gradient(rgba(38,24,19,0),rgb(38,24,19))"></div>')
    # 아래 버튼: 합성(보조) / 장착(주)
    img("hud_pill", 10, 548, 166, 30); txt("일괄 합성", 10, 556, 12, CREAM, "center", 166); img("dot_alert", 168, 545)
    img("btn_main", 184, 548, 166, 30); txt("일괄 장착", 184, 556, 12, DARK, "center", 166)
    B.nav()
    if state == 1:
        # 아이템 상세: 화면을 어둡게 하고 가운데 카드
        raw('<div style="position:absolute;inset:0;background:rgba(10,6,4,.6)"></div>')
        g = 4; c = GRADE[g][1]
        raw(f'<div style="position:absolute;left:30px;top:150px;width:300px;height:330px;box-sizing:border-box;border-radius:12px;background:#3A2419;border:1.5px solid {GOLD};box-shadow:0 6px 24px rgba(0,0,0,.6)"></div>')
        raw(f'<div style="position:absolute;left:31.5px;top:151.5px;width:297px;height:33px;border-radius:11px 11px 0 0;background:linear-gradient({c}dd,{c}66)"></div>')
        txt("전설", 30, 160, 11, DARK, "center", 300)
        img("btn_close", 318, 138)
        item_slot(140, 198, 80, "sword", g, 12)
        txt("별빛 단검", 30, 288, 14, CREAM, "center", 300)
        txt("Lv.12 / 20", 30, 307, 9, MUTED, "center", 300)
        for i, (a, b2) in enumerate([("공격력", "+1,240"), ("치명타", "+3.5%"), ("공격 속도", "+2%")]):
            y = 330 + 22 * i
            txt(a, 52, y, 10, MUTED, "left", 120); txt(b2, 188, y, 10, CREAM, "right", 120)
        raw(f'<div style="position:absolute;left:52px;top:398px;width:256px;height:1px;background:{GOLD};opacity:.5"></div>')
        txt("합성 재료", 52, 406, 9, MUTED, "left", 100); txt("2 / 3", 188, 406, 9, CREAM, "right", 120)
        img("bar_track", 52, 420, 256, 8); tint("bar_fill", 53.5, 421.5, GOLD, 168, 5)
        img("btn_main_off", 44, 438, 132, 30); txt("합성", 44, 446, 12, MUTED, "center", 132)
        img("btn_main", 184, 438, 132, 30); txt("장착", 184, 446, 12, DARK, "center", 132)
    return "".join(B.out)

cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{screen(i)}</div></div>' for i, l in enumerate(["1. 장비 목록", "2. 아이템을 눌렀을 때 (상세)"]))
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 732px; }}
.cap {{ color: #ddd; font: 13px/28px 'Malgun Gothic', sans-serif; height: 28px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "inventory_mock.html"), "w", encoding="utf-8").write(html)
