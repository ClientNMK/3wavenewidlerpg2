# 스테이지 이동 시안: 스테이지 바를 누르면 창이 뜬다. 월드 탭 + 스테이지 칸(깬 곳 / 최고 / 잠김), 깬 곳과 최고 스테이지로만 이동.
# 월드당 10 스테이지로 가정(StageChart_Dev 1월드 10행). 칸·자물쇠·체크는 CSS·SVG 초안이다.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_hud_scale_mock as S
B = S.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, ROOT = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.ROOT
SHADOW = "text-shadow:0 0 2px #2A1A14,0 0 2px #2A1A14"
HERE_AT = (1, 24)
BEST, NOW, SEL, TOTAL = 27, 24, 26, 50  # 월드당 50개 가정. 최고 도달 1-27(아직 못 깸), 지금 1-24, 1-26을 고름

def lock(x, y, s=14, color=MUTED):
    raw(f'<svg width="{s}" height="{s}" viewBox="0 0 24 24" style="position:absolute;left:{x}px;top:{y}px" fill="{color}" stroke="{DARK}" stroke-width="1.800" stroke-linejoin="round">'
        '<path d="M7.500 11V8a4.500 4.500 0 019 0v3" fill="none" stroke="' + color + '" stroke-width="2.600"/><rect x="5" y="11" width="14" height="10" rx="2"/></svg>')
def check(x, y, s=12):
    raw(f'<svg width="{s}" height="{s}" viewBox="0 0 24 24" style="position:absolute;left:{x}px;top:{y}px"><circle cx="12" cy="12" r="10" fill="#9BD65A" stroke="{DARK}" stroke-width="2.400"/>'
        f'<path d="M7 12.500l3.300 3.300L17 9" fill="none" stroke="{DARK}" stroke-width="2.800" stroke-linecap="round" stroke-linejoin="round"/></svg>')

def cell(x, y, n, state, selected, wd=1):
    # state: clear / best / lock. 전부 parts/ 부품으로 조립
    s = 48
    img({"clear": "stage_cell", "best": "stage_cell_best", "lock": "stage_cell_lock"}[state], x, y, s, s)
    if selected: img("stage_cell_sel", x - 6, y - 6)
    if state == "lock":
        img("icon_lock", x + 17, y + 12); txt(f"{wd}-{n}", x, y + 30, 9, "rgba(194,168,136,.5)", "center", s); return
    txt(f"{wd}-{n}", x, y + 12, 13, CREAM if state == "clear" else "#FFE7A0", "center", s)
    if state == "clear": img("icon_check", x + s - 10, y - 4)
    if state == "best":
        img("tag_best", x + 6, y + 31, s - 12, 12); txt("최고", x, y + 31.5, 8, DARK, "center", s)
    if (wd, n) == HERE_AT:  # 지금 있는 곳: 소녀 얼굴 표시
        raw(f'<div style="position:absolute;left:{x - 6}px;top:{y - 7}px;width:20px;height:20px;border-radius:50%;image-rendering:pixelated;background:#3A251C url({ROOT}ArtSource/girl_base/animations/idle/frames/frame_000.png) -50px -58px / 120px 120px"></div>')
        img("frame_face", x - 6, y - 7)

def stage_bar_entry(name):
    # 스테이지 바를 누르면 창이 열린다. 이름 옆 작은 ▼ 로 누를 수 있음을 알림
    img("frame_stage", 59, 71, 242, 50); img("stage_crest", 143, 59)
    txt(name, 104, 84); txt("▼", 104 + len(name) * 6.2 + 4, 85, 7, GOLD); txt("6/10", 156, 84, align="right", w=100)
    img("bar_track", 104, 98, 152, 8); tint("bar_fill", 105.5, 99.5, "#EE7A5C", 89, 5)

def battle(name="Stage 1-4"):
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right)
    def center():
        stage_bar_entry(name); B.boss_button(120, 126, 120, 30, "off"); B.toggle(40, 130, "자동", False, 62)
    S.group(.85, "180px 0", center); S.group(1, "180px 584px", S.bottom); B.nav()

def page_chip(x, y, w, label, state):
    # state: on(보는 쪽) / clear(다 깸) / open(진행 중) / lock
    on = state == "on"
    img("win_seg_on" if on else "win_seg_off", x, y, w, 22)
    if state == "lock": lock(x + 4, y + 5, 11, MUTED)
    txt(label, x + (6 if state == "lock" else 0), y + 6, 9, DARK if on else (MUTED if state == "lock" else CREAM), "center", w)
    if state == "clear": check(x + w - 9, y - 4, 11)

def grid(gx, gy, first):
    for n in range(first, first + 10):
        col, row = (n - first) % 5, (n - first) // 5
        cell(gx + 58 * col, gy + 62 * row, n, "clear" if n < BEST else "best" if n == BEST else "lock", n == SEL)

def popup(mode):
    raw('<div style="position:absolute;inset:0;background:rgba(10,6,4,.62)"></div>')
    x0, y0, w, h = 24, 96, 312, 460
    img("win_popup", x0, y0, w, h, "filter:drop-shadow(0 6px 14px rgba(0,0,0,.6))")
    img("win_title", 110, y0 - 17, 140, 34); txt("스테이지 이동", 110, y0 - 6, 11, align="center", w=140)
    img("btn_close", x0 + w - 12, y0 - 12)
    for i in range(3):
        on = i == 0; locked = i >= 1; tx = 40 + 94 * i
        img("win_tab_on" if on else "win_tab_off", tx, y0 + 24, 88, 26)
        if locked: lock(tx + 18, y0 + 30, 13, MUTED)
        txt(f"월드 {i + 1}", tx + (8 if locked else 0), y0 + 31, 11, DARK if on else MUTED, "center", 88)
    # 월드 그림 띠 (얇게)
    raw(f'<div style="position:absolute;left:40px;top:{y0 + 58}px;width:280px;height:40px;border-radius:10px;overflow:hidden;box-sizing:border-box;border:1.5px solid rgba(230,194,122,.6);'
        f'background:url({ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_1.png) center 40% / 280px auto"></div>')
    raw(f'<div style="position:absolute;left:41.5px;top:{y0 + 59.5}px;width:277px;height:37px;border-radius:9px;background:linear-gradient(90deg,rgba(26,16,12,.85),rgba(26,16,12,.1) 75%)"></div>')
    txt("월드 1", 52, y0 + 70, 12, CREAM, "left", 80, SHADOW)
    txt(f"{BEST - 1} / {TOTAL} 클리어", 104, y0 + 72, 9, "#FFE7A0", "left", 100, SHADOW)
    img("bar_track", 200, y0 + 74, 106, 8); tint("bar_fill", 201.5, y0 + 75.5, GOLD, round(103 * (BEST - 1) / TOTAL), 5)
    top = y0 + 108
    if mode == "page":
        # 10개씩 쪽 칩: 다 깬 쪽은 체크, 아직 못 간 쪽은 자물쇠
        for i in range(5):
            st = "on" if i == 2 else "clear" if (i + 1) * 10 < BEST else "open" if i * 10 < BEST else "lock"
            page_chip(40 + 57 * i, top, 52, f"{i * 10 + 1}~{i * 10 + 10}", st)
        grid(40, top + 38, 21)
        area_end = top + 38 + 124
    else:
        # 스크롤: 10개 묶음마다 구분 줄, 열 때 지금 있는 곳이 보이게 자동 스크롤
        vh = 200
        raw(f'<div style="position:absolute;left:30px;top:{top}px;width:300px;height:{vh}px;overflow:hidden">')
        block = 18 + 124 + 6
        off = -(2 * block) + 64  # 1-21 묶음이 보이게 내려온 상태
        for bi in range(5):
            by = off + bi * block
            if by > vh or by + block < 0: continue
            raw(f'<div style="position:absolute;left:10px;top:{by + 6}px;width:280px;height:1px;background:{GOLD};opacity:.35"></div>')
            txt(f"&nbsp;1-{bi * 10 + 1} ~ 1-{bi * 10 + 10}&nbsp;", 10, by, 9, MUTED, "center", 280, "background:none")
            raw(f'<div style="position:absolute;left:115px;top:{by}px;width:70px;height:12px;background:rgb(58,36,25)"></div>')
            txt(f"1-{bi * 10 + 1} ~ {bi * 10 + 10}", 10, by, 9, MUTED, "center", 280)
            grid(10, by + 20, bi * 10 + 1)
        raw('</div>')
        # 위아래 흐림 + 스크롤 막대
        raw(f'<div style="position:absolute;left:30px;top:{top}px;width:300px;height:22px;background:linear-gradient(rgb(58,36,25),rgba(58,36,25,0))"></div>')
        raw(f'<div style="position:absolute;left:30px;top:{top + vh - 22}px;width:300px;height:22px;background:linear-gradient(rgba(58,36,25,0),rgb(58,36,25))"></div>')
        raw(f'<div style="position:absolute;left:326px;top:{top + 4}px;width:3px;height:{vh - 8}px;border-radius:2px;background:rgba(230,194,122,.18)"></div>')
        raw(f'<div style="position:absolute;left:326px;top:{top + 4 + (vh - 8) * .38}px;width:3px;height:{(vh - 8) * .25}px;border-radius:2px;background:{GOLD}"></div>')
        area_end = top + vh
    # 고른 스테이지 정보
    iy = area_end + 12
    raw(f'<div style="position:absolute;left:40px;top:{iy}px;width:280px;height:1px;background:{GOLD};opacity:.6"></div>'); img("win_rule_gem", 175, iy - 4.5)
    txt(f"Stage 1-{SEL}", 40, iy + 10, 13, GOLD, "left", 140); txt(f"지금 1-{NOW}에 있어요", 180, iy + 13, 9, MUTED, "right", 140)
    img("win_row", 36, iy + 30, 288, 30)
    txt("처치당", 46, iy + 39, 9, MUTED, "left", 60)
    img("icon_gold", 96, iy + 37); txt("+120", 116, iy + 39, 10, CREAM, "left", 60)
    img("icon_exp", 170, iy + 37); txt("+8", 190, iy + 39, 10, CREAM, "left", 60)
    txt("보스 깸", 250, iy + 39, 9, "#9BD65A", "right", 64)
    by = y0 + h - 48
    img("hud_pill", 40, by, 132, 34); txt(f"최고 1-{BEST}로", 40, by + 10, 12, CREAM, "center", 132)
    img("btn_main", 188, by, 132, 34); txt("이동", 188, by + 10, 12, DARK, "center", 132)

def popup_real(world):
    # 지금 데이터: 월드 1·2 각 5개. 최고 2-2(못 깸), 지금 1-3에서 사냥 중. 칸은 스크롤 목록 안에 있고, 10개 이하면 구분 줄 없이 한 묶음.
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
    txt(f"월드 {world + 1}", 52, y0 + 70, 12, CREAM, "left", 80, SHADOW)
    txt(f"{done} / 5 클리어", 104, y0 + 72, 9, "#FFE7A0", "left", 100, SHADOW)
    img("bar_track", 200, y0 + 74, 106, 8); tint("bar_fill", 201.5, y0 + 75.5, GOLD, round(103 * done / 5), 5)
    gy = y0 + 116
    sel = [5, 2][world]
    for n in range(1, 6):
        st = "clear" if world == 0 or n < 2 else "best" if n == 2 else "lock"
        cell(40 + 58 * (n - 1), gy, n, st, n == sel, world + 1)
    iy = gy + 64
    raw(f'<div style="position:absolute;left:40px;top:{iy}px;width:280px;height:1px;background:{GOLD};opacity:.6"></div>'); img("win_rule_gem", 175, iy - 4.5)
    txt(f"Stage {world + 1}-{sel}", 40, iy + 10, 13, GOLD, "left", 140); txt("지금 1-3에 있어요", 180, iy + 13, 9, MUTED, "right", 140)
    img("win_row", 36, iy + 30, 288, 30)
    txt("처치당", 46, iy + 39, 9, MUTED, "left", 60)
    img("icon_gold", 96, iy + 37); txt("+120", 116, iy + 39, 10, CREAM, "left", 60)
    img("icon_exp", 170, iy + 37); txt("+8", 190, iy + 39, 10, CREAM, "left", 60)
    txt("보스 깸" if world == 0 else "도전 중", 250, iy + 39, 9, "#9BD65A" if world == 0 else "#FFE7A0", "right", 64)
    by = y0 + h - 48
    img("hud_pill", 40, by, 132, 34); txt("최고 2-2로", 40, by + 10, 12, CREAM, "center", 132)
    img("btn_main", 188, by, 132, 34); txt("이동", 188, by + 10, 12, DARK, "center", 132)

shots = []
HERE_AT = (1, 3)
battle("Stage 1-3"); popup_real(0); shots.append(("지금 데이터 · 월드 1 (5개 다 깸, 지금 1-3)", "".join(B.out)))
battle("Stage 1-3"); popup_real(1); shots.append(("지금 데이터 · 월드 2 (최고 2-2)", "".join(B.out)))
HERE_AT = (1, 24)
battle("Stage 1-24"); popup("scroll"); shots.append(("나중에 늘어나면 · 같은 창이 스크롤됨 (50개 예)", "".join(B.out)))
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1104px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "stage_mock.html"), "w", encoding="utf-8").write(html)
