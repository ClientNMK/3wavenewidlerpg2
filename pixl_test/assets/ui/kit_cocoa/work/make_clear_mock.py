# 보스 클리어 연출 시안: ① 보스가 쓰러지는 순간(번쩍 + 띠 + VICTORY 문장 + 빛줄기 + 반짝임)
# ② 클리어 창: 같은 창 안에서 첫 클리어 보상 + 클리어 보상을 보여 줌 ③ 확인 → 보상이 재화 칸·인벤토리로 날아가고 다음 스테이지 시작.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_reward_mock as R
I = R.I; S = I.S; B = I.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK = B.GOLD, B.CREAM, B.MUTED, B.DARK
ROOT = B.ROOT
SHADOW = "text-shadow:0 0 2px #2A1A14,0 0 3px #2A1A14"

def sparkles(pts):
    for x, y, s, o in pts: img("fx_sparkle", x, y, s, s, f"opacity:{o}")
def scene(boss_down=True):
    raw(f'<div style="position:absolute;inset:0;background:url({ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png) center top / auto 640px"></div>')
    if boss_down: raw(f'<img src="{ROOT}ArtSource/monsters/mob_slime/animations/idle/frames/frame_000.png" style="position:absolute;left:170px;top:250px;width:140px;image-rendering:pixelated;transform:scaleX(-1) scaleY(.6);transform-origin:50% 100%;opacity:.55;filter:brightness(1.6)">')
    raw(f'<img src="{ROOT}ArtSource/girl_base/animations/idle/frames/frame_000.png" style="position:absolute;left:80px;top:300px;width:128px;image-rendering:pixelated">')

def moment():
    B.out = []
    scene()
    raw('<div style="position:absolute;inset:0;background:rgba(20,12,6,.45)"></div>')
    raw('<div style="position:absolute;inset:0;background:radial-gradient(circle at 50% 44%,rgba(255,243,220,.55),rgba(255,231,160,.12) 40%,rgba(255,231,160,0) 65%)"></div>')
    img("boss_intro_rays", 50, 150, 260, 260, "opacity:.95")
    img("clear_band", 0, 263, 360, 34)
    img("clear_emblem", 76, 213, 208, 139)
    sparkles([(70, 220, 14, 1), (300, 240, 18, .9), (110, 340, 10, .8), (262, 350, 12, 1), (180, 190, 10, .7), (40, 300, 9, .7), (320, 310, 9, .8)])
    txt("Stage 1-7 클리어!", 80, 362, 14, "#FFE7A0", "center", 200, SHADOW)
    return "".join(B.out)

def result():
    B.out = []
    scene()
    raw('<div style="position:absolute;inset:0;background:rgba(14,8,4,.68)"></div>')
    img("boss_intro_rays", 70, 30, 220, 220, "opacity:.6")
    img("clear_emblem", 96, 64, 168, 112)
    x0, y0, w = 24, 176, 312
    img("clear_panel", x0, y0, w, 340, "filter:drop-shadow(0 6px 14px rgba(0,0,0,.6))")
    txt("보스 · 해초 슬라임 왕", x0, y0 + 16, 11, CREAM, "center", w)
    txt("Stage 1-7 → 1-8", x0, y0 + 34, 13, GOLD, "center", w)
    raw(f'<div style="position:absolute;left:44px;top:{y0 + 58}px;width:272px;height:1px;background:{GOLD};opacity:.55"></div>'); img("win_rule_gem", 175, y0 + 53.5)
    # 첫 클리어 보상 (처음 한 번만)
    img("tag_first", 40, y0 + 70, 62, 16); txt("첫 클리어", 40, y0 + 73, 9, "#FFF3DC", "center", 62)
    R.reward("gem", 5, 40, y0 + 92, 50, "100")
    sparkles([(80, y0 + 86, 10, 1)])
    # 클리어 보상
    txt("클리어 보상", 120, y0 + 73, 9, MUTED, "left", 100)
    for i, (k, g, v) in enumerate([("gold", 4, "8,000"), ("exp", 2, "15"), ("sword", 3, "x1")]):
        R.reward(k, g, 120 + 62 * i, y0 + 92, 50, v)
    # 다음 목표
    raw(f'<div style="position:absolute;left:44px;top:{y0 + 172}px;width:272px;height:1px;background:{GOLD};opacity:.35"></div>')
    txt("다음 보스", 44, y0 + 184, 9, MUTED, "left", 100); txt("Stage 1-8 · 껍데기 게", 140, y0 + 184, 10, CREAM, "right", 176)
    txt("필요 처치", 44, y0 + 202, 9, MUTED, "left", 100); txt("10마리", 140, y0 + 202, 10, CREAM, "right", 176)
    txt("자동 도전", 44, y0 + 220, 9, MUTED, "left", 100); txt("켜짐 유지", 140, y0 + 220, 10, "#9BD65A", "right", 176)
    img("btn_main", 64, y0 + 280, 232, 36); txt("확인", 64, y0 + 291, 13, DARK, "center", 232)
    txt("3초 뒤 자동으로 닫혀요", x0, y0 + 254, 8, MUTED, "center", w)
    return "".join(B.out)

def after():
    B.out = []
    raw(f'<div style="position:absolute;inset:0;background:url({ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png) center top / auto 640px"></div>')
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right)
    def center():
        img("frame_stage", 59, 71, 242, 50); img("stage_crest", 143, 59)
        txt("Stage 1-8", 104, 84); txt("0/10", 156, 84, align="right", w=100); img("bar_track", 104, 98, 152, 8)
        B.boss_button(120, 126, 120, 30, "off"); B.toggle(40, 130, "자동", True, 62)
    S.group(.85, "180px 0", center); S.group(1, "180px 584px", S.bottom); B.nav()
    R.fly()
    txt("Stage 1-8", 80, 150, 16, "#FFE7A0", "center", 200, SHADOW)
    txt("새 스테이지 시작", 80, 172, 10, CREAM, "center", 200, SHADOW)
    return "".join(B.out)


# ---- 한 장면 버전: 문장 + 보상을 같은 화면에서, 창 없이 ----
def one_scene(step, first=True):
    # step 0: 문장 등장, 1: 보상이 하나씩 튀어나오는 중, 2: 다 나옴
    B.out = []
    scene()
    raw('<div style="position:absolute;inset:0;background:rgba(20,12,6,.55)"></div>')
    raw('<div style="position:absolute;inset:0;background:radial-gradient(circle at 50% 40%,rgba(255,243,220,.45),rgba(255,231,160,.1) 40%,rgba(255,231,160,0) 65%)"></div>')
    img("boss_intro_rays", 30, 110, 300, 300, "opacity:.9")
    img("clear_band", 0, 213, 360, 34)
    img("clear_emblem", 76, 163, 208, 139)
    sparkles([(64, 176, 14, 1), (300, 190, 18, .9), (190, 140, 10, .7), (40, 250, 9, .7), (318, 262, 10, .8)])
    if step == 0: return "".join(B.out)
    items = ([("gem", 5, "100", True)] if first else []) + [("gold", 4, "8,000", False), ("exp", 2, "15", False), ("sword", 3, "x1", False)]
    s, gap = 60, 14
    w = len(items) * s + (len(items) - 1) * gap
    shown = len(items) if step == 2 else 2
    for i, (k, g, v, is_first) in enumerate(items):
        if i >= shown: continue
        x = 180 - w / 2 + i * (s + gap); y = 330
        R.reward(k, g, x, y, s, v, pop=(step == 1 and i == shown - 1))
        if is_first:
            img("tag_first", x - 4, y - 14, s + 8, 16); txt("첫 클리어", x - 4, y - 11, 9, "#FFF3DC", "center", s + 8)
            sparkles([(x + s - 8, y - 6, 12, 1)])
    if step == 2:
        sparkles([(70, 392, 10, .8), (292, 380, 12, .9)])
        txt("화면을 누르면 바로 받아요", 80, 428, 9, CREAM, "center", 200, SHADOW)
        img("bar_track", 130, 444, 100, 6); tint("bar_fill", 131.5, 445.5, GOLD, 40, 3)
    return "".join(B.out)

shots = [("① 쓰러지는 순간: VICTORY 등장", one_scene(0)), ("② 보상이 문장 아래에서 하나씩 튀어나옴", one_scene(1)),
         ("③ 다 나옴: 2초 뒤(또는 누르면) 보상이 날아가며 닫힘", one_scene(2)), ("④ 날아가고 다음 스테이지 시작", after())]
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1476px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: #000; }}
</style>{cols}"""
open(os.path.join(HERE, "clear_mock.html"), "w", encoding="utf-8").write(html)
