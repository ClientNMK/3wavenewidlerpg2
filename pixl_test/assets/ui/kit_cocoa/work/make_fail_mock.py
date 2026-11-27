# 실패 연출 시안: ① 보스 실패 순간(화면 흑백 + 빨간 비네트, DEFEAT 문장이 떨어지며 흔들림) ② 실패 결과 판(이유 + 강해지는 방법 바로가기)
# ③ 일반 스테이지에서 쓰러졌을 때(가벼운 판, 자동 다시 시작) ④ 돌아온 화면(0/10, 자동 도전 꺼짐).
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_inventory_mock as I
S = I.S; B = I.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, RED = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.RED
ROOT = B.ROOT
SHADOW = "text-shadow:0 0 2px #2A1A14,0 0 3px #2A1A14"

def hud(count=10, boss_on=False):
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right)
    def center():
        B.stage_bar(count); B.boss_button(120, 126, 120, 30, "on" if count >= 10 else "off"); B.toggle(40, 130, "자동", boss_on, 62)
    S.group(.85, "180px 0", center); S.group(1, "180px 584px", S.bottom); B.nav()
def gray_field(boss=True):
    raw(f'<div style="position:absolute;inset:0;background:url({ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png) center top / auto 640px;filter:grayscale(.85) brightness(.55)"></div>')
    if boss: raw(f'<img src="{ROOT}ArtSource/monsters/mob_slime/animations/idle/frames/frame_000.png" style="position:absolute;left:170px;top:230px;width:140px;image-rendering:pixelated;transform:scaleX(-1);filter:grayscale(.7) brightness(.7)">')
    raw(f'<img src="{ROOT}ArtSource/girl_base/animations/dead/frames/frame_010.png" style="position:absolute;left:70px;top:300px;width:128px;image-rendering:pixelated;filter:grayscale(.6) brightness(.8)">')
    raw('<div style="position:absolute;inset:0;box-shadow:inset 0 0 80px 30px rgba(140,30,24,.55)"></div>')

def fail_moment():
    B.out = []
    gray_field()
    img("fail_band", 0, 263, 360, 34)
    img("fail_emblem", 84, 221, 192, 128, "transform:rotate(-3deg)")
    txt("시간 초과", 80, 352, 13, "#E2765E", "center", 200, SHADOW)
    return "".join(B.out)

def fail_result():
    B.out = []
    gray_field()
    x0, y0, w, h = 24, 150, 312, 340
    img("fail_emblem", 104, y0 - 60, 152, 101)
    img("fail_panel", x0, y0 + 40, w, h - 40, "filter:drop-shadow(0 6px 14px rgba(0,0,0,.6))")
    txt("보스 · 해초 슬라임 왕", x0, y0 + 56, 11, CREAM, "center", w)
    txt("시간 초과  ·  남은 체력 38%", x0, y0 + 74, 10, "#E2765E", "center", w)
    img("bar_track", 60, y0 + 92, 240, 8); tint("bar_fill", 61.5, y0 + 93.5, B.BOSSRED, 90, 5)
    raw(f'<div style="position:absolute;left:44px;top:{y0 + 116}px;width:272px;height:1px;background:#A79FB4;opacity:.5"></div>')
    img("win_rule_gem", 175, y0 + 111.5, extra="filter:grayscale(1) brightness(1.1)")
    txt("이렇게 강해질 수 있어요", x0, y0 + 124, 10, GOLD, "center", w)
    tips = [("tab_grow", "골드 강화", "공격력 Lv.13 → 강화 가능"), ("tab_grow", "후드 먹이기", "Hood_Exp 34개 보유"), ("tab_inventory", "장비", "더 좋은 장비가 있어요")]
    for i, (ic, t, sub) in enumerate(tips):
        y = y0 + 144 + 44 * i
        img("fail_row", 36, y, 288, 38)
        img("hud_round", 42, y + 4); img(ic, 49, y + 11)
        txt(t, 80, y + 6, 11, CREAM, "left", 150); txt(sub, 80, y + 22, 8, MUTED, "left", 170)
        img("btn_main", 262, y + 7, 54, 24); txt("이동", 262, y + 13, 10, DARK, "center", 54)
        if i == 0: img("dot_alert", 309, y + 3)
    img("hud_pill", 104, y0 + 284, 152, 32); txt("확인", 104, y0 + 293, 12, CREAM, "center", 152)
    return "".join(B.out)

def normal_death():
    B.out = []
    gray_field(boss=False)
    hud(6)
    img("win_row", 70, 236, 220, 78)
    txt("쓰러졌어요", 70, 250, 14, "#E2765E", "center", 220)
    txt("잠시 뒤 다시 일어나요", 70, 272, 9, CREAM, "center", 220)
    img("bar_track", 100, 292, 160, 8); tint("bar_fill", 101.5, 293.5, GOLD, 52, 5)
    txt("3초 뒤 다시 시작", 70, 302, 8, MUTED, "center", 220)
    return "".join(B.out)

def back():
    B.out = []
    B.field()
    hud(0)
    img("win_row", 50, 196, 260, 58)
    img("icon_skull_light", 64, 206)
    txt("보스 도전 실패", 84, 207, 12, "#E2765E", "left", 200)
    txt("처치 수가 0으로 돌아갔어요", 64, 226, 9, CREAM, "left", 240)
    txt("자동 도전이 꺼졌어요", 64, 239, 9, MUTED, "left", 240)
    return "".join(B.out)

shots = [("① 보스 실패 순간: 흑백 + 빨간 테두리, 문장이 떨어지며 흔들림", fail_moment()), ("② 실패 결과: 이유 + 강해지는 방법", fail_result()),
         ("③ 일반 스테이지에서 쓰러짐: 가벼운 판, 자동 다시 시작", normal_death())]
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1104px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "fail_mock.html"), "w", encoding="utf-8").write(html)
