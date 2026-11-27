# HUD 크기 비교 시안: 같은 전투 화면(도전 가능 상태)을 HUD 100% / 85% / 75%로 나란히.
# 묶음마다 붙는 쪽을 기준으로 줄인다: 왼쪽 위(초상화·이름), 오른쪽 위(재화·자동 전투·사이드 버튼), 가운데 위(스테이지 바·보스 버튼), 아래(스킬 칸, 하단 탭 바 위를 기준). 하단 탭 바는 화면 폭을 채워야 해서 100% 그대로.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_boss_mock as B
ROOT = B.ROOT

def group(scale, origin, draw):
    B.out.append(f'<div style="position:absolute;inset:0;transform:scale({scale});transform-origin:{origin}">'); draw(); B.out.append("</div>")
def left():
    B.raw(f'<div style="position:absolute;left:14px;top:19px;width:44px;height:44px;border-radius:50%;image-rendering:pixelated;background:#3A251C url({ROOT}ArtSource/girl_base/animations/idle/frames/frame_000.png) -108px -122px / 256px 256px"></div>')
    B.img("frame_portrait", -3, -1); B.img("btn_main", 17, 62, 38, 15); B.txt("Lv.1", 17, 64, 9, B.DARK, "center", 38)
    B.img("hud_pill", 74, 14, 100, 22); B.txt("소라게 소녀", 84, 19)
def right():
    B.img("hud_pill", 232, 8, 56, 22); B.img("icon_gold", 236, 11); B.txt("137", 256, 13)
    B.img("hud_pill", 293, 8, 61, 22); B.img("icon_gem", 297, 11); B.txt("0", 317, 13); B.img("btn_plus", 335, 11)
    B.toggle(262, 36, "자동 전투", True, 92)
    for i, n in enumerate(["icon_mail", "icon_rank", "icon_settings"]):
        B.img("hud_round", 324, 168 + 36 * i); B.img(n, 331, 175 + 36 * i)
    B.img("hud_round", 324, 276); B.img("icon_zoom", 331, 283)
    for i in range(3): B.img("dot_step_on" if i == 1 else "dot_step_off", 328 + 8 * i, 310)
def center():
    B.stage_bar(10); B.boss_button(120, 126, 120, 30, "on"); B.img("dot_alert", 236, 122); B.toggle(40, 130, "자동", True, 62)
def bottom():
    B.slots()

def screen(s):
    B.out = []
    B.field()
    # 왼쪽 위·가운데 위만 줄이고, 오른쪽 위(재화·자동 전투·사이드 버튼)와 스킬 칸은 100%
    group(s, "0 0", left); group(1, "360px 0", right); group(s, "180px 0", center); group(1, "180px 584px", bottom); B.nav()
    return "".join(B.out)
cols = "".join(f'<div><div class="cap">{"지금 (100%)" if s == 1 else "동그라미 부분만 85%"}</div><div class="phone">{screen(s)}</div></div>' for s in (1, .85))
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 732px; }}
.cap {{ color: #ddd; font: 13px/28px 'Malgun Gothic', sans-serif; height: 28px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "hud_scale_mock.html"), "w", encoding="utf-8").write(html)
