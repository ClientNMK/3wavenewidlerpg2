# 보스전 진입 연출 새 시안 3가지 (정지 장면 비교용): A. VS 컷인, B. 경고 띠 + 실루엣, C. 보스 이름 컷인.
# 전부 CSS 초안이다. 고른 방향만 부품으로 만든다.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_hud_scale_mock as S
B = S.B
img, txt, raw = B.img, B.txt, B.raw
GOLD, CREAM, MUTED, DARK, ROOT = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.ROOT
BOSS = f"{ROOT}ArtSource/monsters/monster_dungeon_2001/animations/idle/frames/frame_000.png"
GIRL = f"{ROOT}ArtSource/girl_base/animations/idle/frames/frame_000.png"
SH = "text-shadow:0 2px 0 #2A1A14,0 0 4px #2A1A14"

def base():
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right); S.group(.85, "180px 0", S.center); S.group(1, "180px 584px", S.bottom); B.nav()
    raw('<div style="position:absolute;inset:0;background:rgba(10,6,4,.72)"></div>')

def vs():
    base()
    # 대각선으로 갈린 두 판: 왼쪽 소녀(금빛), 오른쪽 보스(붉은빛)
    raw(f'<div style="position:absolute;left:0;top:200px;width:360px;height:200px;overflow:hidden">'
        f'<div style="position:absolute;left:-40px;top:0;width:230px;height:200px;background:linear-gradient(90deg,#5A3A1E,#B8802E);transform:skewX(-14deg);border-right:3px solid {GOLD}"></div>'
        f'<div style="position:absolute;left:196px;top:0;width:230px;height:200px;background:linear-gradient(90deg,#7A2A22,#3A1418);transform:skewX(-14deg);border-left:3px solid #E2765E"></div>'
        f'<img src="{GIRL}" style="position:absolute;left:-34px;top:-70px;width:300px;image-rendering:pixelated">'
        f'<img src="{BOSS}" style="position:absolute;left:150px;top:-50px;width:260px;image-rendering:pixelated;transform:scaleX(-1)">'
        f'</div>')
    raw(f'<div style="position:absolute;left:0;top:198px;width:360px;height:2px;background:{GOLD}"></div><div style="position:absolute;left:0;top:400px;width:360px;height:2px;background:{GOLD}"></div>')
    txt("VS", 130, 268, 44, "#FFE7A0", "center", 100, "font-style:italic;text-shadow:0 3px 0 #2A1A14,0 0 12px #F2B544,0 0 3px #2A1A14")
    txt("소녀태수", 14, 374, 12, CREAM, "left", 120, SH); txt("뿔 달린 악몽", 226, 374, 12, "#FFD2C4", "right", 120, SH)

def warning():
    base()
    stripe = "repeating-linear-gradient(-45deg,#C84B3A 0 14px,#2A1A14 14px 28px)"
    for y in (214, 372):
        raw(f'<div style="position:absolute;left:0;top:{y}px;width:360px;height:16px;background:{stripe};border-top:1.5px solid {GOLD};border-bottom:1.5px solid {GOLD}"></div>')
    raw('<div style="position:absolute;left:0;top:232px;width:360px;height:140px;background:linear-gradient(rgba(120,24,20,.75),rgba(60,12,12,.9))"></div>')
    raw(f'<img src="{BOSS}" style="position:absolute;left:60px;top:176px;width:240px;image-rendering:pixelated;transform:scaleX(-1);filter:brightness(0) drop-shadow(0 0 6px #E2765E);opacity:.9">')
    raw('<div style="position:absolute;left:182px;top:258px;width:8px;height:8px;border-radius:50%;background:#fff;box-shadow:0 0 8px 3px #FF6A50"></div>')
    txt("WARNING", 30, 240, 30, "#FFE7A0", "center", 300, "letter-spacing:6px;text-shadow:0 3px 0 #2A1A14,0 0 10px #E2765E")
    txt("보스가 나타났다!", 30, 346, 12, CREAM, "center", 300, SH)
    raw('<div style="position:absolute;inset:0;box-shadow:inset 0 0 60px 20px rgba(200,40,30,.45)"></div>')

def name_cut():
    base()
    # 가로 띠 + 속도선, 보스가 오른쪽에서 크게 미끄러져 들어오고 이름판이 왼쪽에서 들어옴
    raw('<div style="position:absolute;left:0;top:214px;width:360px;height:172px;background:linear-gradient(90deg,rgba(26,14,18,.95),rgba(90,26,30,.95));border-top:2px solid #E6C27A;border-bottom:2px solid #E6C27A;overflow:hidden">'
        + "".join(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:2px;background:linear-gradient(90deg,rgba(255,231,160,0),rgba(255,231,160,.55))"></div>' for x, y, w in [(20, 18, 120), (140, 40, 200), (0, 70, 90), (200, 96, 150), (40, 128, 180), (220, 150, 120)])
        + f'<img src="{BOSS}" style="position:absolute;left:120px;top:-62px;width:300px;image-rendering:pixelated;transform:scaleX(-1)"></div>')
    img("icon_skull_light", 22, 262, 20, 20)
    txt("BOSS", 46, 262, 16, "#E2765E", "left", 100, SH + ";letter-spacing:3px")
    txt("뿔 달린 악몽", 20, 288, 24, "#FFE7A0", "left", 220, "text-shadow:0 3px 0 #2A1A14,0 0 6px #2A1A14")
    raw(f'<div style="position:absolute;left:20px;top:322px;width:150px;height:2px;background:linear-gradient(90deg,{GOLD},rgba(230,194,122,0))"></div>')
    txt("Stage 1-7", 20, 330, 10, CREAM, "left", 120, SH)

shots = []
vs(); shots.append(("A. VS 컷인: 소녀 vs 보스", "".join(B.out)))
warning(); shots.append(("B. 경고 띠 + 보스 실루엣", "".join(B.out)))
name_cut(); shots.append(("C. 보스 이름 컷인", "".join(B.out)))
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1104px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "boss_enter_mock.html"), "w", encoding="utf-8").write(html)
