# 획득처 팝업 시안: 재화·아이템을 누르면 어디서 얻는지와 바로가기를 보여 준다.
# 예시 2개: ① 후드 화면에서 Hood_Exp가 모자라 아이콘을 눌렀을 때 ② 인벤토리에서 장비 획득처. 자물쇠 아이콘과 팝업 판은 초안.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_hood_mock as H
import make_inventory_mock as I
B = I.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, RED = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.RED

def lock(x, y, s=14):
    raw(f'<svg width="{s}" height="{s}" viewBox="0 0 24 24" style="position:absolute;left:{x}px;top:{y}px" fill="none" stroke="{MUTED}" stroke-width="2.200" stroke-linejoin="round" stroke-linecap="round">'
        f'<rect x="5" y="10.500" width="14" height="10" rx="2" fill="rgba(0,0,0,.35)"/><path d="M8 10.500V8a4 4 0 018 0v2.500M12 14.500v2.500"/></svg>')
def popup(head, name, own, desc, rows):
    raw('<div style="position:absolute;inset:0;background:rgba(10,6,4,.6)"></div>')
    h = 132 + 52 * len(rows)
    y0 = 320 - h / 2
    raw(f'<div style="position:absolute;left:24px;top:{y0}px;width:312px;height:{h}px;box-sizing:border-box;border-radius:12px;background:#3A2419;border:1.5px solid {GOLD};box-shadow:0 6px 24px rgba(0,0,0,.6)"></div>')
    img("btn_close", 324, y0 - 12)
    # 머리: 아이콘 칸 + 이름 + 보유 + 설명
    head(40, y0 + 16)
    txt(name, 100, y0 + 18, 13, CREAM, "left", 200)
    txt(own, 100, y0 + 37, 10, MUTED, "left", 200)
    txt(desc, 100, y0 + 54, 9, MUTED, "left", 220, "white-space:normal;line-height:13px")
    # 구분선 + 제목
    raw(f'<div style="position:absolute;left:40px;top:{y0 + 96}px;width:280px;height:1px;background:{GOLD};opacity:.6"></div>')
    img("win_rule_gem", 175, y0 + 91.5)
    txt("획득처", 40, y0 + 106, 10, GOLD, "left", 100)
    for i, (icon, title, sub, state) in enumerate(rows):
        y = y0 + 124 + 52 * i
        img("win_row", 36, y, 288, 46, "opacity:%s" % (".55" if state == "lock" else "1"))
        img("hud_round", 44, y + 8); img(icon, 51, y + 15)
        txt(title, 82, y + 9, 11, CREAM if state != "lock" else MUTED, "left", 160)
        txt(sub, 82, y + 26, 9, MUTED if state != "lock" else "#E2765E", "left", 170)
        if state == "go":
            img("btn_main", 258, y + 10, 58, 26); txt("이동", 258, y + 17, 11, DARK, "center", 58)
        elif state == "here":
            img("hud_pill", 258, y + 10, 58, 26); txt("진행 중", 258, y + 17, 10, "#9BD65A", "center", 58)
        else:
            img("hud_pill", 258, y + 10, 58, 26); lock(266, y + 16); txt("잠김", 280, y + 17, 10, MUTED, "left", 40)

# ① 후드 화면(모자람) 위에서 Hood_Exp 획득처
H.out = []; H.screen(1); s1 = "".join(H.out)
B.out = []
def head1(x, y):
    raw(f'<div style="position:absolute;left:{x}px;top:{y}px;width:48px;height:48px;box-sizing:border-box;border-radius:10px;border:2px solid #6CC6BE;background:radial-gradient(circle,#6CC6BE55,rgba(38,24,19,.9))"></div>')
    img("icon_goods_hood_exp", x + 10, y + 10, 28, 28)
popup(head1, "Hood_Exp", "보유 5 / 필요 13", "후드에게 먹여 레벨을 올리는 꿈 조각", [
    ("icon_skull_light", "스테이지 사냥", "몬스터를 잡으면 확률로 얻어요", "here"),
    ("tab_content", "던전", "Stage 2-1을 클리어하면 열려요", "lock"),
    ("tab_shop", "상점", "보석으로 살 수 있어요", "go"),
])
s1 += "".join(B.out)

# ② 인벤토리 위에서 장비 획득처
s2 = I.screen(0)
B.out = []
def head2(x, y):
    I.item_slot(x, y, 48, "sword", 4)
popup(head2, "별빛 단검", "보유 2 / 합성 3", "같은 장비 3개를 모으면 다음 등급으로 합성", [
    ("tab_shop", "장비 뽑기", "상점에서 보석으로 뽑아요", "go"),
    ("tab_inventory", "합성", "아래 등급 장비 3개를 합쳐요", "go"),
    ("tab_mission", "미션 보상", "주간 미션을 끝내면 받아요", "go"),
])
s2 += "".join(B.out)

cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in [("1. Hood_Exp 획득처 (후드 화면에서)", s1), ("2. 장비 획득처 (인벤토리에서)", s2)])
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{B.ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 732px; }}
.cap {{ color: #ddd; font: 13px/28px 'Malgun Gothic', sans-serif; height: 28px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{B.ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "source_mock.html"), "w", encoding="utf-8").write(html)
