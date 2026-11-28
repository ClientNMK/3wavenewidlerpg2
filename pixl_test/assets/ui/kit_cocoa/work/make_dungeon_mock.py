# 던전(컨텐츠 탭) 시안: ① 던전 카드 목록 ② 단계 고르기 창(입장 / 소탕) ③ 던전 안 HUD.
# 차트 기준: 던전 1001 = 5단계·제한 60초, 1002 = 1단계·제한 30초, 입장권 = 던전 티켓(Goods 101, 102). 보상·이름·여는 조건은 가안.
# 카드 판, 티켓 아이콘, 단계 줄은 CSS·SVG 초안이다.
import os, sys
HERE = os.path.dirname(os.path.abspath(__file__)); sys.path.insert(0, HERE)
import make_inventory_mock as I
S = I.S; B = I.B
img, txt, raw, tint = B.img, B.txt, B.raw, B.tint
GOLD, CREAM, MUTED, DARK, ROOT = B.GOLD, B.CREAM, B.MUTED, B.DARK, B.ROOT
SHADOW = "text-shadow:0 0 2px #2A1A14,0 0 3px #2A1A14"
BG = f"{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/"
MON = f"{ROOT}ArtSource/monsters/"
DBG = f"{ROOT}ArtSource/backgrounds/bg_dungeon_2001/bg_dungeon_2001.png"

def ticket(x, y, s=16): img("icon_ticket", x, y, s, s)

def lock(x, y, s=14, color=MUTED):
    raw(f'<svg width="{s}" height="{s}" viewBox="0 0 24 24" style="position:absolute;left:{x}px;top:{y}px" fill="{color}" stroke="{DARK}" stroke-width="1.800" stroke-linejoin="round">'
        f'<path d="M7.500 11V8a4.500 4.500 0 019 0v3" fill="none" stroke="{color}" stroke-width="2.600"/><rect x="5" y="11" width="14" height="10" rx="2"/></svg>')

def card(y, name, bg, mon, cleared, tickets, reward, locked=None, alert=False):
    # 카드 한 장에서 바로 소탕 / 입장. cleared = 깬 최고 단계(0이면 소탕 꺼짐), reward = 그 단계 소탕 보상 [(아이콘, 수량)]
    x, w, h = 16, 328, 118
    raw(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{w}px;height:{h}px;border-radius:14px;overflow:hidden;'
        f'background:url({BG}{bg}.png) center 55% / 340px auto;{"filter:grayscale(.85) brightness(.5);" if locked else ""}"></div>')
    img("card_shade", x + 1.5, y + 1.5, w - 3, h - 3); img("card_frame_lock" if locked else "card_frame", x, y, w, h)
    if mon: raw(f'<img src="{MON}{mon}/animations/idle/frames/frame_000.png" style="position:absolute;left:{x + 206}px;top:{y - 22}px;width:120px;image-rendering:pixelated;transform:scaleX(-1);{"filter:brightness(0) opacity(.5);" if locked else ""}">')
    txt(name, x + 14, y + 10, 15, CREAM if not locked else MUTED, "left", 200, SHADOW)
    if not locked:  # 오른쪽 위: 제한 시간
        img("hud_pill", x + w - 66, y + 8, 56, 20); img("icon_timer", x + w - 61, y + 10); txt("30초", x + w - 43, y + 13, 9, CREAM, "left", 30)
    if locked:
        txt("준비 중", x + 14, y + 32, 9, MUTED, "left", 200, SHADOW)
        lock(x + 14, y + 80, 16, GOLD); txt(locked, x + 36, y + 82, 10, CREAM, "left", 260, SHADOW); return
    txt(f"{cleared}단계 클리어" if cleared else "아직 안 깸", x + 14, y + 32, 9, "#FFE7A0" if cleared else MUTED, "left", 200, SHADOW)
    # 소탕 보상 (깬 최고 단계 기준). 안 깼으면 1단계 보상을 흐리게
    img("card_chip", x + 10, y + 48, 200, 24)
    txt(f"{max(cleared, 1)}단계 보상", x + 20, y + 55, 9, MUTED, "left", 60)
    rx = x + 80
    for ic, v in reward:
        img(ic, rx, y + 52, extra="" if cleared else "opacity:.5"); txt(v, rx + 19, y + 54, 10, CREAM if cleared else MUTED, "left", 60); rx += 22 + len(v) * 7
    # 티켓 · 소탕 · 입장
    by = y + 82
    img("hud_pill", x + 10, by + 2, 64, 24); ticket(x + 16, by + 6); txt(tickets, x + 36, by + 8, 10, CREAM, "left", 36)
    img("hud_pill" if cleared else "btn_main_off", x + 82, by, 112, 28); txt("소탕", x + 82, by + 8, 12, CREAM if cleared else DARK, "center", 112)
    img("btn_main", x + 202, by, 116, 28); txt("입장", x + 202, by + 8, 12, DARK, "center", 116)
    if alert: img("dot_alert", x + 186, by - 3)

def top_tabs(top, on):
    # 창 안 위쪽 탭 2개: 던전(티켓 보상) / 도전(단계 오르기)
    for i, t in enumerate(["던전", "도전"]):
        img("win_tab_on" if i == on else "win_tab_off", 16 + 166 * i, top + 22, 162, 28)
        txt(t, 16 + 166 * i, top + 29, 12, DARK if i == on else MUTED, "center", 162)
    if on == 0: img("dot_alert", 316, top + 19)  # 도전 탭에 받을 첫 클리어 보상이 있을 때

def base_window(on):
    B.out = []
    B.field()
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right); S.group(.85, "180px 0", S.center)
    top = 120
    I.window("컨텐츠", top); top_tabs(top, on)
    return top

def nav_on():
    B.nav()
    img("hud_nav_on", 255, 589); img("tab_content", 260, 594)
    txt("컨텐츠", 240, 622, 9, CREAM, "center", 60)

def tab_screen():
    top = base_window(0)
    card(top + 62, "던전 1", "bg_world_2", "mob_slime_gold", 3, "2/2", [("icon_gold", "6,000"), ("icon_gem", "60")], alert=True)
    card(top + 190, "던전 2", "bg_world_1", "monster_stage_002_purple", 0, "1/1", [("icon_goods_hood_exp", "20")])
    card(top + 318, "던전 3", "bg_world_1", None, 0, "", [], locked="Stage 2-5를 깨면 열려요")
    nav_on()

def challenge_row(y, n, state, selected):
    # 도전 단계 줄: 첫 클리어 보상(보석)만, 받으면 체크. state: clear / next / lock
    x, w, h = 16, 328, 40
    img({"clear": "step_row", "next": "step_row_next", "lock": "step_row_lock"}[state], x, y, w, h)
    if selected: img("step_row_sel", x - 6, y - 6, w + 12, h + 12)
    dim = state == "lock"
    if dim: img("icon_lock", x + 12, y + 13)
    txt(f"{n}단계", x + (34 if dim else 14), y + 13, 12, "rgba(194,168,136,.55)" if dim else CREAM if state == "clear" else "#FFE7A0", "left", 80)
    txt("첫 클리어", x + 120, y + 15, 9, MUTED, "left", 60)
    img("icon_gem", x + 176, y + 12, extra="opacity:.5" if dim else ""); txt(f"+{n * 50}", x + 196, y + 14, 10, MUTED if dim else CREAM, "left", 50)
    if state == "clear":
        txt("받음", x + w - 52, y + 15, 9, "#9BD65A", "right", 26); img("icon_check", x + w - 22, y + 14)
    elif state == "next":
        img("tag_best", x + w - 52, y + 12, 42, 16); txt("도전", x + w - 52, y + 13.5, 9, DARK, "center", 42)

# 도전 보스 5마리 가안: (스프라이트, 얼굴 크롭 [폭, 왼쪽, 위], 상태, 최고 단계)
BOSSES = [("monster_dungeon_2001", (130, -43, -38), "open", 3), ("mob_slime_gold", (84, -20, -36), "alert", 1),
          ("monster_stage_002_purple", (84, -20, -30), "open", 0), ("monster_stage_002_red", (84, -20, -30), "lock", 0),
          ("mob_slime_pink", (84, -20, -36), "lock", 0)]

def boss_face(x, y, i, selected):
    mon, (w, l, t), st, best = BOSSES[i]
    s = 44
    raw(f'<div style="position:absolute;left:{x}px;top:{y}px;width:{s}px;height:{s}px;border-radius:50%;overflow:hidden;background:{"#3A251C" if st != "lock" else "#1E1410"}">'
        f'<img src="{MON}{mon}/animations/idle/frames/frame_000.png" style="position:absolute;left:{l}px;top:{t}px;width:{w}px;image-rendering:pixelated;transform:scaleX(-1);{"filter:brightness(0) opacity(.55);" if st == "lock" else ""}"></div>')
    img("face_ring_sel", x - 6, y - 6) if selected else img("face_ring_lock" if st == "lock" else "face_ring", x, y)
    if st == "lock": img("icon_lock", x + 15, y + 15)
    if st == "alert": img("dot_alert", x + 34, y)
    label = "잠김" if st == "lock" else f"{best}단계" if best else "도전 전"
    txt(label, x - 8, y + s + 4, 8, CREAM if selected else MUTED, "center", s + 16)

def challenge_screen(sel=0):
    top = base_window(1)
    # 보스 얼굴 줄 (누르면 그 보스로). 6마리 이상이면 가로 스크롤
    for i in range(5): boss_face(16 + 71 * i, top + 62, i, i == sel)
    mon, _, st, best = BOSSES[sel]
    locked = st == "lock"
    # 보스 띠
    y = top + 126
    raw(f'<div style="position:absolute;left:16px;top:{y}px;width:328px;height:84px;border-radius:14px;overflow:hidden;background:url({DBG}) center 55% / 340px auto;{"filter:grayscale(.85) brightness(.5);" if locked else ""}"></div>')
    img("card_shade", 17.5, y + 1.5, 325, 81); img("card_frame_lock" if locked else "card_frame", 16, y, 328, 84)
    big = 130 if mon == "monster_dungeon_2001" else 110
    raw(f'<div style="position:absolute;left:16px;top:{y - 30}px;width:328px;height:{84 + 30}px;overflow:hidden;border-radius:0 0 14px 14px"><img src="{MON}{mon}/animations/idle/frames/frame_000.png" style="position:absolute;left:{196 if big == 130 else 210}px;top:{0 if big == 130 else -4}px;width:{big}px;image-rendering:pixelated;transform:scaleX(-1);{"filter:brightness(0) opacity(.5);" if locked else ""}"></div>')
    txt(f"도전 보스 {sel + 1}", 30, y + 10, 15, CREAM if not locked else MUTED, "left", 200, SHADOW)
    if locked:
        lock(30, y + 50, 16, GOLD); txt("도전 보스 3을 5단계까지 깨면 열려요", 52, y + 52, 10, CREAM, "left", 260, SHADOW)
    else:
        txt(f"최고 {best}단계 · 단계마다 강해져요", 30, y + 32, 9, MUTED, "left", 200, SHADOW)
        img("icon_timer", 30, y + 56); txt("제한 60초 · 티켓 없이 계속 도전", 50, y + 58, 9, CREAM, "left", 200, SHADOW)
    # 단계 목록 (위로 갈수록 높은 단계, 스크롤)
    ly = top + 220
    txt("단계", 30, ly, 9, MUTED, "left", 60)
    raw(f'<div style="position:absolute;left:0;top:{ly + 14}px;width:360px;height:186px;overflow:hidden">')
    rows = [(n, "lock" if locked or n > best + 1 else "next" if n == best + 1 else "clear") for n in (5, 4, 3, 2, 1)]
    for i, (n, st2) in enumerate(rows): challenge_row(8 + 46 * i, n, st2, not locked and n == best + 1)
    raw('</div>')
    raw(f'<div style="position:absolute;left:10px;top:{ly + 170}px;width:340px;height:30px;background:linear-gradient(rgba(38,24,19,0),rgb(38,24,19) 70%)"></div>')
    img("btn_main_off" if locked else "btn_main", 60, 540, 240, 36); txt("잠김" if locked else f"{best + 1}단계 도전", 60, 551, 13, DARK, "center", 240)
    nav_on()

def step_row(y, n, state, selected):
    # state: clear / next / lock
    x, w, h = 36, 288, 40
    img({"clear": "step_row", "next": "step_row_next", "lock": "step_row_lock"}[state], x, y, w, h)
    if selected: img("step_row_sel", x - 6, y - 6, w + 12, h + 12)
    if state == "lock":
        lock(x + 12, y + 13, 14); txt(f"{n}단계", x + 34, y + 14, 11, "rgba(194,168,136,.55)", "left", 80); return
    txt(f"{n}단계", x + 14, y + 13, 12, CREAM if state == "clear" else "#FFE7A0", "left", 80)
    img("icon_gem", x + 92, y + 12); txt(f"+{n * 20}", x + 112, y + 14, 10, CREAM, "left", 50)
    img("icon_gold", x + 152, y + 12); txt(f"+{n * 2},000", x + 172, y + 14, 10, CREAM, "left", 60)
    if state == "clear":
        img("icon_check", x + w - 24, y + 14)
    else:
        img("tag_best", x + w - 52, y + 12, 42, 16); txt("도전", x + w - 52, y + 13.5, 9, DARK, "center", 42)

def step_popup():
    tab_screen()
    raw('<div style="position:absolute;inset:0;background:rgba(10,6,4,.62)"></div>')
    x0, y0, w, h = 24, 104, 312, 432
    img("win_popup", x0, y0, w, h, "filter:drop-shadow(0 6px 14px rgba(0,0,0,.6))")
    img("win_title", 119, y0 - 17, 122, 34); txt("던전 1", 122, y0 - 6, 11, align="center", w=116)
    img("btn_close", x0 + w - 12, y0 - 12)
    # 그림 띠
    raw(f'<div style="position:absolute;left:40px;top:{y0 + 24}px;width:280px;height:64px;border-radius:10px;overflow:hidden;box-sizing:border-box;border:1.5px solid rgba(230,194,122,.6);background:url({BG}bg_world_2.png) center 55% / 300px auto"></div>')
    raw(f'<div style="position:absolute;left:41.5px;top:{y0 + 25.5}px;width:277px;height:61px;border-radius:9px;background:linear-gradient(90deg,rgba(30,18,12,.88),rgba(30,18,12,.1) 75%)"></div>')
    raw(f'<img src="{MON}mob_slime_gold/animations/idle/frames/frame_000.png" style="position:absolute;left:224px;top:{y0 + 8}px;width:90px;image-rendering:pixelated;transform:scaleX(-1)">')
    img("icon_timer", 52, y0 + 38); txt("제한 30초", 72, y0 + 40, 10, CREAM, "left", 100, SHADOW)
    txt("시간 안에 쓰러뜨리면 다음 단계가 열려요", 52, y0 + 62, 8, MUTED, "left", 220, SHADOW)
    # 단계 목록 (위아래 스크롤)
    txt("단계", 40, y0 + 100, 9, MUTED, "left", 60); txt("보상", 128, y0 + 100, 9, MUTED, "left", 60)
    states = ["clear", "clear", "clear", "next", "lock"]
    for i, st in enumerate(states): step_row(y0 + 116 + 46 * i, i + 1, st, i == 3)
    # 티켓 + 버튼
    by = y0 + h - 52
    raw(f'<div style="position:absolute;left:40px;top:{by - 14}px;width:280px;height:1px;background:{GOLD};opacity:.5"></div>')
    ticket(40, by - 32 + 0); txt("던전 티켓 2 / 2", 60, by - 30, 10, CREAM, "left", 120); txt("매일 0시에 다시 채워져요", 180, by - 30, 8, MUTED, "right", 140)
    img("hud_pill", 40, by, 132, 34); txt("3단계 소탕", 40, by + 6, 11, CREAM, "center", 132); ticket(84, by + 19, 11); txt("1", 97, by + 19.5, 9, MUTED, "left", 20)
    img("btn_main", 188, by, 132, 34); txt("4단계 입장", 188, by + 6, 11, DARK, "center", 132); ticket(232, by + 19, 11); txt("1", 245, by + 19.5, 9, DARK, "left", 20)

def inside():
    B.out = []
    raw(f'<div style="position:absolute;inset:0;background:url({DBG}) center top / auto 640px"></div>')
    raw(f'<img src="{MON}monster_dungeon_2001/animations/idle/frames/frame_000.png" style="position:absolute;left:150px;top:230px;width:180px;image-rendering:pixelated;transform:scaleX(-1)">')
    B.sprite("girl_base/animations/idle/frames/frame_000.png", 60, 330, 128)
    S.group(.85, "0 0", S.left); S.group(1, "360px 0", S.right)
    def center():
        img("frame_stage", 59, 71, 242, 50); img("stage_crest", 143, 59)
        B.skull(102, 82, 16, CREAM); txt("던전 1 · 4단계", 122, 84, 10, CREAM, "left", 140)
        img("bar_track", 104, 98, 152, 8); tint("bar_fill", 105.5, 99.5, B.BOSSRED, 104, 5)
        # 남은 시간: 줄어드는 금색 바 + 숫자
        img("hud_pill", 112, 126, 136, 26); img("icon_timer", 120, 131); txt("0:42", 140, 132, 12, "#FFE7A0", "left", 50)
        img("bar_track", 178, 135, 60, 8); tint("bar_fill", 179.5, 136.5, GOLD, 40, 5)
    S.group(.85, "180px 0", center); S.group(1, "180px 584px", S.bottom)
    img("hud_pill", 14, 594, 78, 30); txt("나가기", 14, 602, 11, B.RED, "center", 78)
    txt("시간이 다 되면 실패 (티켓은 돌려받음)", 80, 506, 9, CREAM, "center", 200, SHADOW)

shots = []
tab_screen(); shots.append(("① 던전 탭: 카드에서 바로 소탕 / 입장", "".join(B.out)))
challenge_screen(0); shots.append(("② 도전 탭: 보스 얼굴 줄 (5마리 예)", "".join(B.out)))
challenge_screen(3); shots.append(("③ 잠긴 보스를 눌렀을 때", "".join(B.out)))
inside(); shots.append(("④ 안에 들어가면: 보스 체력 + 남은 시간 (둘 다 같음)", "".join(B.out)))
cols = "".join(f'<div><div class="cap">{l}</div><div class="phone">{s}</div></div>' for l, s in shots)
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin: 0; background: #20201f; font-family: 'DNF', 'Malgun Gothic', sans-serif; display: flex; gap: 12px; padding: 0 12px; width: 1476px; }}
.cap {{ color: #ddd; font: 12px/28px 'Malgun Gothic', sans-serif; height: 28px; white-space: nowrap; overflow: hidden; width: 360px; }}
.phone {{ width: 360px; height: 640px; position: relative; overflow: hidden; background: url('{BG}bg_world_2.png') center top / auto 640px; }}
</style>{cols}"""
open(os.path.join(HERE, "dungeon_mock.html"), "w", encoding="utf-8").write(html)
