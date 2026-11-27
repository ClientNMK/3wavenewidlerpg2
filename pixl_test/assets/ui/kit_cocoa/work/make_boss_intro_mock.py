# 보스 등장 연출 순서 시안: 검은 화면 위에 ① 띠가 들어옴 → ② 문장 + 빛줄기가 튀어나옴 → ③ 번쩍 → ④ 사라지며 보스전 시작
import json, os
HERE = os.path.dirname(os.path.abspath(__file__))
M = {p["name"]: p for p in json.load(open(os.path.join(HERE, "manifest.json"), encoding="utf-8"))["parts"]}
ROOT = "../../../../../"
def img(name, x, y, w=None, h=None, extra=""):
    p = M[name]; w = w or p["w"] // 3; h = h or p["h"] // 3
    if p["nine"]:
        L, R, T, B = p["nine"]
        return f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;box-sizing:border-box;border-style:solid;border-width:{T/3}px {R/3}px {B/3}px {L/3}px;border-image:url(../parts/{name}.png) {T} {R} {B} {L} fill stretch;{extra}"></div>'
    return f'<img src="../parts/{name}.png" style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;{extra}">'
BG = f"url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px"
CY = 280  # 연출 가운데 높이
def frame(step):
    o = [f'<div style="position:absolute;inset:0;background:{BG};filter:brightness(.35) saturate(.6)"></div>',
         f'<div style="position:absolute;inset:0;background:rgba(0,0,0,{[.55, .7, .7, .4][step]})"></div>']
    band_w = [180, 360, 360, 360][step]
    o.append(img("boss_intro_band", (360 - band_w) / 2, CY - 17, band_w, 34, "opacity:%s" % [1, 1, 1, .6][step]))
    if step >= 1:
        sc = [0, 1.15, 1, .9][step]
        o.append(img("boss_intro_rays", 80, CY - 100, 200, 200, f"transform:rotate({step * 12}deg) scale({sc});opacity:{[0, 1, .8, .3][step]}"))
        o.append(img("boss_intro_emblem", 84, CY - 59, 192, 128, f"transform:scale({sc});opacity:{[0, 1, 1, .5][step]}"))
    if step == 2:
        o.append('<div style="position:absolute;inset:0;background:radial-gradient(circle at 50% 44%,rgba(255,243,220,.85),rgba(255,231,160,.25) 45%,rgba(255,231,160,0) 70%)"></div>')
    if step == 3:
        o.append(f'<img src="{ROOT}ArtSource/monsters/mob_slime/animations/idle/frames/frame_000.png" style="position:absolute;left:110px;top:330px;width:140px;image-rendering:pixelated;transform:scaleX(-1);opacity:.85">')
    return "".join(o)
labels = ["① 0.0초: 화면 어두워지고 띠가 가운데서 양옆으로", "② 0.25초: 문장이 커졌다 작아지며 등장, 빛줄기 회전", "③ 0.6초: 번쩍 (흰빛 0.1초 × 2번)", "④ 1.2초: 흐려지며 보스전 시작"]
cols = "".join(f'<div><div class="cap">{labels[i]}</div><div class="phone">{frame(i)}</div></div>' for i in range(4))
html = f"""<!doctype html><meta charset="utf-8"><style>
body {{ margin: 0; background: #20201f; display: flex; gap: 12px; padding: 0 12px; width: 1476px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: #000; }}
</style>{cols}"""
open(os.path.join(HERE, "boss_intro_mock.html"), "w", encoding="utf-8").write(html)
