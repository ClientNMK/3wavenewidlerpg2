# 장비 소환 시안 (상점 탭): ① 소환 화면(소환 레벨, 확률 보기, 광고 1회 / 10회 / 30회) ② 10회 결과.
# 장비 등급·그림은 인벤토리 시안의 초안(6등급, 임시 아이템 그림)을 쓴다. 소환 티켓 = Goods 1001~1003. 가격·확률은 가안.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_inventory_mock as I
S = I.S; B = I.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, ROOT = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.ROOT
SH = "text-shadow:0 0 2px #2A1A14,0 0 3px #2A1A14"

def frame(tab):
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right); S.group(.85, "180px 0", S.center)
    top = 120
    I.window("소환", top)
    for i, t in enumerate(["무기", "방어구", "장신구"]):
        on = i == tab
        img("win_tab_on" if on else "win_tab_off", 16 + 111 * i, top + 22, 106, 28)
        txt(t, 16 + 111 * i, top + 29, 12, DARK if on else MUTED, "center", 106)
    B.nav()
    img("hud_nav_on", 315, 589); img("tab_shop", 320, 594); txt("상점", 300, 622, 9, CREAM, "center", 60)
    return top

def summon_btn(x, y, w, title, cost_icon, cost, main=True, sub=None):
    img("btn_main" if main else "hud_pill", x, y, w, 46)
    txt(title, x, y + 7, 12, DARK if main else CREAM, "center", w)
    if cost_icon == "ad":
        c = DARK if main else CREAM
        raw(f'<svg width="14" height="14" viewBox="0 0 24 24" style="position:absolute;left:{x + w / 2 - 20}px;top:{y + 27}px" fill="none" stroke="{c}" stroke-width="2.200" stroke-linejoin="round"><rect x="2.500" y="5" width="19" height="14" rx="3"/><path d="M10 9.200v5.600l4.800-2.800z" fill="{c}"/></svg>')
        txt(cost, x + w / 2 - 2, y + 26, 10, c, "left", 60)
    else: img(cost_icon, x + w / 2 - 22, y + 25, 16, 16); txt(cost, x + w / 2 - 2, y + 26, 10, DARK if main else CREAM, "left", 60)
    if sub: img("tag_first", x + w - 46, y - 8, 50, 16); txt(sub, x + w - 46, y - 6, 8, CREAM, "center", 50)

def main():
    top = frame(0)
    # 소환 무대: 어두운 띠 + 빛 + 떠 있는 장비 3개
    y = top + 60
    raw(f'<div style="position:absolute;left:16px;top:{y}px;width:328px;height:200px;border-radius:14px;overflow:hidden;background:radial-gradient(ellipse at 50% 60%,#6A3A6E 0%,#3A1E36 45%,#1E1218 100%)"></div>')
    img("boss_intro_rays", 80, y + 10, 200, 200, "opacity:.35")
    raw(f'<div style="position:absolute;left:110px;top:{y + 150}px;width:140px;height:26px;border-radius:50%;background:radial-gradient(ellipse,rgba(255,231,160,.55),rgba(255,231,160,0) 70%)"></div>')
    I.item_slot(72, y + 70, 52, "armor", 3)
    I.item_slot(150, y + 40, 64, "sword", 5)
    I.item_slot(240, y + 70, 52, "ring", 4)
    for x, yy, s in [(60, y + 30, 12), (290, y + 40, 14), (140, y + 130, 10), (230, y + 20, 10)]: img("fx_sparkle", x, yy, s, s)
    img("card_frame", 16, y, 328, 200)
    txt("무기 소환", 30, y + 12, 15, "#FFE7A0", "left", 150, SH)
    img("hud_pill", 252, y + 10, 80, 22); txt("확률 보기", 252, y + 15, 10, CREAM, "center", 80)
    # 소환 레벨
    ly = y + 212
    img("win_chip", 16, ly, 328, 40)
    txt("소환 레벨", 28, ly + 6, 9, MUTED, "left", 60); txt("Lv.3", 78, ly + 5, 11, "#FFE7A0", "left", 40)
    txt("레벨이 오르면 높은 등급이 더 잘 나와요", 336, ly + 6, 8, MUTED, "right", 200)
    img("bar_track", 28, ly + 24, 272, 8); tint("bar_fill", 29.5, ly + 25.5, GOLD, 108, 5)
    txt("120/300", 300, ly + 22, 8, CREAM, "right", 40, SH)
    # 보유 티켓 / 보석
    ty = ly + 50
    img("hud_pill", 16, ty, 160, 24); img("icon_ticket", 22, ty + 4); txt("무기 소환 티켓", 42, ty + 6, 9, MUTED, "left", 90); txt("12", 138, ty + 5, 10, CREAM, "right", 30)
    img("hud_pill", 184, ty, 160, 24); img("icon_gem", 190, ty + 4); txt("보석", 210, ty + 6, 9, MUTED, "left", 40); txt("4,280", 276, ty + 5, 10, CREAM, "right", 60)
    # 소환 버튼
    by = ty + 40
    summon_btn(16, by, 100, "광고 1회", "ad", "2/3", main=False)
    summon_btn(124, by, 106, "10회 소환", "icon_ticket", "10")
    summon_btn(238, by, 106, "30회 소환", "icon_gem", "1,350", sub="10% 할인")
    img("dot_alert", 108, by - 4)
    return "".join(B.out)

def result():
    B.out = []
    B.field()
    raw('<div style="position:absolute;inset:0;background:rgba(10,6,4,.94)"></div>')
    img("boss_intro_rays", 60, 70, 240, 240, "opacity:.5")
    txt("소환 결과", 0, 108, 16, "#FFE7A0", "center", 360, SH)
    txt("무기 10회", 0, 130, 10, CREAM, "center", 360, SH)
    got = [("sword", 1), ("sword", 0), ("sword", 2), ("sword", 0), ("sword", 5), ("sword", 1), ("sword", 0), ("sword", 3), ("sword", 1), ("sword", 0)]
    for i, (k, g) in enumerate(got):
        col, row = i % 5, i // 5
        x, y = 28 + 62 * col, 170 + 74 * row
        if g >= 4: raw(f'<div style="position:absolute;left:{x - 4}px;top:{y - 4}px;width:62px;height:62px;border-radius:12px;box-shadow:0 0 12px 4px {I.GRADE[g][1]}aa"></div>')
        I.item_slot(x, y, 54, k, g)
        txt(I.GRADE[g][0], x, y + 58, 8, I.GRADE[g][1], "center", 54, SH)
        if g >= 4: img("fx_sparkle", x + 40, y - 6, 14, 14); img("tag_first", x - 2, y - 8, 30, 14); txt("NEW", x - 2, y - 7, 7, CREAM, "center", 30)
    txt("가장 높은 등급: 신화 · 별빛 단검", 0, 330, 10, "#F2B544", "center", 360, SH)
    img("hud_pill", 28, 470, 96, 36); txt("닫기", 28, 481, 12, CREAM, "center", 96)
    img("btn_main", 132, 470, 96, 36); txt("10회 더", 132, 476, 11, DARK, "center", 96); img("icon_ticket", 158, 490, 12, 12); txt("10", 174, 490, 9, DARK, "left", 30)
    img("btn_main", 236, 470, 96, 36); txt("30회 더", 236, 476, 11, DARK, "center", 96); img("icon_gem", 254, 490, 12, 12); txt("1,350", 270, 490, 9, DARK, "left", 40)
    txt("빈 곳을 누르면 바로 다 열려요", 0, 520, 8, MUTED, "center", 360)
    return "".join(B.out)

shots = [("① 장비 소환 (상점 탭)", main()), ("② 10회 결과: 등급 색, 높은 등급은 빛남", result())]
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 732px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "summon_mock.html"), "w", encoding="utf-8").write(html)
