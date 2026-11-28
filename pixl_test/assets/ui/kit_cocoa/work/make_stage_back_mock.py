# 아래 스테이지에서 방치할 때 보스 버튼 자리: ① 최고 스테이지 진행 중(기존) ② 아래 스테이지 반복 중 → "1-9로 이동" ③ 누른 뒤 1-9 (이동만).
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_hud_scale_mock as S
B = S.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, ROOT = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.ROOT
SH = "text-shadow:0 0 2px #2A1A14,0 0 2px #2A1A14"

def bar(stage, right, fill, color, rcol=CREAM):
    img("frame_stage", 59, 71, 242, 50); img("stage_crest", 143, 59)
    txt(f"Stage {stage}", 104, 84); txt(right, 156, 84, 10, rcol, "right", 100)
    img("bar_track", 104, 98, 152, 8)
    if fill: tint("bar_fill", 105.5, 99.5, color, round(149 * fill), 5)

def go_best(best):
    x, y, w, h = 110, 126, 140, 30
    img("btn_main", x, y, w, h)
    img("btn_boss_deco", x - 20, y + 4); img("btn_boss_deco", x + w - 10, y + 4, extra="transform:scaleX(-1)")
    img("icon_go_best", x + 26, y + 7); txt(f"{best}로 이동", x + 46, y + 8.5, 12, DARK, "left", 100)

def screen(state):
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right)
    def center():
        if state == 0:
            bar("1-9", "7/10", .7, "#EE7A5C"); B.boss_button(120, 126, 120, 30, "off"); B.toggle(40, 130, "자동", False, 62)
            txt("10마리를 잡으면 도전할 수 있어요", 80, 162, 9, CREAM, "center", 200, SH)
        elif state == 1:
            bar("1-3", "반복 사냥", 1, "#C2A888", MUTED); go_best("1-9")
            txt("최고 스테이지로 돌아가서 보스에 도전해요", 60, 162, 9, "#FFE7A0", "center", 240, SH)
        else:
            bar("1-9", "0/10", 0, "#EE7A5C"); B.boss_button(120, 126, 120, 30, "off"); B.toggle(40, 130, "자동", False, 62)
            txt("10마리를 잡으면 도전할 수 있어요", 80, 162, 9, CREAM, "center", 200, SH)
    S.group(.85, "180px 0", center); S.group(1, "180px 584px", S.bottom); B.nav()
    return "".join(B.out)

shots = [("① 최고 스테이지 1-9 (지금 그대로)", screen(0)), ("② 아래 스테이지 1-3 반복 중: 버튼이 '1-9로 이동'", screen(1)), ("③ 누르면: 1-9로 이동만 → 평소 보스 도전 흐름", screen(2))]
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1104px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "stage_back_mock.html"), "w", encoding="utf-8").write(html)
