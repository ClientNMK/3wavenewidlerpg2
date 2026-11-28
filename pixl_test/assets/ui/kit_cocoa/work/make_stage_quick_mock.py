# 스테이지 이동 창 (원래 방식: 칸 고르기 → 정보 → 이동) + 사냥터 기록. 아래 버튼 3개: 사냥터 / 최고 / 이동.
# ① 보스 실패 후 최고 2-2에서 열기 → '사냥터 1-3' ② 사냥터 1-3에서 열고 1-5를 고름 → '이동' 또는 '최고 2-2'.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_stage_mock as M
B = M.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, ROOT = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.ROOT
HUNT, BEST = (1, 3), (2, 2)
H_LABEL = lambda: f"사냥터 {HUNT[0]}-{HUNT[1]}"

def popup(world, sel):
    raw('<div style="position:absolute;inset:0;background:rgba(10,6,4,.62)"></div>')
    x0, y0, w, h = 24, 150, 312, 330
    img("win_popup", x0, y0, w, h, "filter:drop-shadow(0 6px 14px rgba(0,0,0,.6))")
    img("win_title", 110, y0 - 17, 140, 34); txt("스테이지 이동", 110, y0 - 6, 11, align="center", w=140)
    img("btn_close", x0 + w - 12, y0 - 12)
    for i in range(2):
        on = i == world; tx = 40 + 142 * i
        img("win_tab_on" if on else "win_tab_off", tx, y0 + 24, 138, 26)
        txt(f"월드 {i + 1}", tx, y0 + 31, 11, DARK if on else MUTED, "center", 138)
    bg = ["bg_world_1", "bg_world_2"][world]
    raw(f'<div style="position:absolute;left:40px;top:{y0 + 58}px;width:280px;height:40px;border-radius:10px;overflow:hidden;box-sizing:border-box;border:1.5px solid rgba(230,194,122,.6);'
        f'background:url({ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/{bg}.png) center 40% / 280px auto"></div>')
    raw(f'<div style="position:absolute;left:41.5px;top:{y0 + 59.5}px;width:277px;height:37px;border-radius:9px;background:linear-gradient(90deg,rgba(26,16,12,.85),rgba(26,16,12,.1) 75%)"></div>')
    done = [5, 1][world]
    txt(f"월드 {world + 1}", 52, y0 + 70, 12, CREAM, "left", 80, M.SHADOW)
    txt(f"{done} / 5 클리어", 104, y0 + 72, 9, "#FFE7A0", "left", 100, M.SHADOW)
    img("bar_track", 200, y0 + 74, 106, 8); tint("bar_fill", 201.5, y0 + 75.5, GOLD, round(103 * done / 5), 5)
    gy = y0 + 116
    for n in range(1, 6):
        st = "clear" if world == 0 or n < 2 else "best" if n == 2 else "lock"
        x = 40 + 58 * (n - 1)
        M.cell(x, gy, n, st, n == sel, world + 1)
        if (world + 1, n) == HUNT:
            img("tag_hunt", x + 6, gy + 31, 36, 12); txt("사냥터", x, gy + 31.5, 8, DARK, "center", 48)
    iy = gy + 64
    raw(f'<div style="position:absolute;left:40px;top:{iy}px;width:280px;height:1px;background:{GOLD};opacity:.6"></div>'); img("win_rule_gem", 175, iy - 4.5)
    here = M.HERE_AT
    txt(f"Stage {world + 1}-{sel}", 40, iy + 10, 13, GOLD, "left", 140); txt(f"지금 {here[0]}-{here[1]}에 있어요", 180, iy + 13, 9, MUTED, "right", 140)
    img("win_row", 36, iy + 30, 288, 30)
    txt("처치당", 46, iy + 39, 9, MUTED, "left", 60)
    img("icon_gold", 96, iy + 37); txt("+120", 116, iy + 39, 10, CREAM, "left", 60)
    img("icon_exp", 170, iy + 37); txt("+8", 190, iy + 39, 10, CREAM, "left", 60)
    best = (world + 1, sel) == BEST
    txt("도전 중" if best else "보스 깸", 250, iy + 39, 9, "#FFE7A0" if best else "#9BD65A", "right", 64)
    # 사냥터 저장 줄: 고른 칸이 최고보다 낮은 깬 곳이면 저장 버튼, 이미 사냥터면 저장됨 표시
    sy = iy + 70
    txt(f"지금 사냥터: {HUNT[0]}-{HUNT[1]}", 44, sy + 6, 9, MUTED, "left", 140)
    if (world + 1, sel) == HUNT:
        img("icon_check", 238, sy + 6); txt("사냥터로 저장됨", 254, sy + 5.5, 9, "#9BD65A", "left", 80)
    elif not best:
        img("hud_pill", 214, sy, 106, 24); img("tag_hunt", 222, sy + 6, 12, 12); txt("사냥터로 저장", 238, sy + 6.5, 9, CREAM, "left", 80)
    by = y0 + h - 48
    for bx, label, off, main in [(40, H_LABEL(), here == HUNT, False), (136, "최고 2-2", here == BEST, False), (232, "이동", (world + 1, sel) == here, True)]:
        if main: img("btn_main_off" if off else "btn_main", bx, by, 88, 34); txt(label, bx, by + 10, 12, DARK, "center", 88)
        else: img("hud_pill", bx, by, 88, 34, "opacity:.45" if off else ""); txt(label, bx, by + 10, 11, MUTED if off else CREAM, "center", 88)

shots = []
M.HERE_AT = BEST
M.battle("Stage 2-2"); popup(1, 2); shots.append(("① 보스 실패 후 2-2에서 열면: '사냥터 1-3' 한 번에", "".join(B.out)))
M.HERE_AT = HUNT
M.battle("Stage 1-3"); popup(0, 5); shots.append(("② 1-5를 고르면: '사냥터로 저장' 또는 '이동'", "".join(B.out)))
HUNT = (1, 5)
M.battle("Stage 1-3"); popup(0, 5); shots.append(("③ 저장하면: 꼬리표·아래 버튼이 1-5로", "".join(B.out)))
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1104px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "stage_quick_mock.html"), "w", encoding="utf-8").write(html)
