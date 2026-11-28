# 3 second ad hook 1: the screen is cut in two. Top-down field, a horde closes in, one swing, the screen
# slides apart along the cut, snaps back and everything bursts.
import glob, json, math, os, random, sys
from PIL import Image, ImageChops, ImageDraw, ImageFont, ImageOps

ART = r'C:\3wavestudio\3wavenewidlerpg2\ArtSource'
FONT = r'C:\3wavestudio\3wavenewidlerpg2\3wavenewidlerpg2\Assets\Resource\Font\DNFBitBitv2.ttf'
OUT = sys.argv[1] if len(sys.argv) > 1 else '.'
W, H, PAD, FPS = 360, 640, 160, 30
random.seed(11)

def frames(path, flip=False):
    fs = [Image.open(f).convert('RGBA') for f in sorted(glob.glob(os.path.join(ART, path, 'frames', '*.png')))]
    return [ImageOps.mirror(f) for f in fs] if flip else fs

def sheet(base):
    m = json.load(open(base + '.json', encoding='utf8'))['meta']['pixelfx']
    im = Image.open(base + '.png').convert('RGBA'); fw, fh = m['frameSize']
    return [im.crop(((i % m['columns']) * fw, (i // m['columns']) * fh, (i % m['columns']) * fw + fw, (i // m['columns']) * fh + fh)) for i in range(m['frames'])], m['pivot']

def white(im):
    w = Image.new('RGBA', im.size, (255, 255, 255, 255)); w.putalpha(im.getchannel('A')); return w

def out_expo(k): return 1 if k >= 1 else 1 - 2 ** (-10 * max(0, k))

bg = Image.open(os.path.join(ART, 'backgrounds', 'bg_world_2', 'bg_world_2.png')).convert('RGBA').crop((396, 690, 396 + W, 690 + H))
g_idle, g_back = frames('girl_base/animations/idle'), frames('girl_base/animations/backstab')
KINDS = ['monster_stage_002', 'monster_stage_002_blue', 'monster_stage_002_red', 'monster_stage_002_purple', 'mob_slime', 'mob_slime_blue', 'mob_slime_gold', 'mob_slime_pink']
anims = {}
for k in KINDS:
    for fl in (False, True):
        anims[(k, fl)] = (frames('monsters/%s/animations/move' % k, fl), frames('monsters/%s/animations/dead' % k, fl))
spark, spark_piv = sheet(os.path.join(ART, 'girl_base', 'effects', 'hit_spark', 'hit-spark'))

GX, GY = 104, 446          # girl pivot
LA, LB = (-30, 492), (W + 30, 268)   # the cut, left end to right end
LL = math.hypot(LB[0] - LA[0], LB[1] - LA[1]); UX, UY = (LB[0] - LA[0]) / LL, (LB[1] - LA[1]) / LL
def line_y(x): return LA[1] + (LB[1] - LA[1]) * (x - LA[0]) / (LB[0] - LA[0])

T_WIND, T_SWING, T_SPLIT, T_SNAP, T_END = 0.40, 0.47, 0.90, 1.45, 3.0

mobs = []
tries = 0
while len(mobs) < 44 and tries < 4000:
    tries += 1
    x, y = random.uniform(24, W - 20), random.uniform(120, H - 40)
    if math.hypot(x - GX, (y - GY) * 1.3) < 62: continue
    if any(math.hypot(x - m['x'], (y - m['y']) * 1.6) < 34 for m in mobs): continue
    d = math.hypot(x - GX, y - GY)
    mobs.append(dict(x=x, y=y, kind=random.choice(KINDS), ph=random.uniform(0, 0.7), die=T_SNAP + 0.02 + 0.3 * d / 420 + random.uniform(0, 0.05),
                     dmg=random.randint(12000, 98000) * random.choice([1, 1, 1, 10]), crit=False))
for m in random.sample(mobs, 9): m['crit'] = True; m['dmg'] *= 7

font_s, font_b = ImageFont.truetype(FONT, 14), ImageFont.truetype(FONT, 22)
SHAKE = [(8, -4), (-7, 4), (6, -3), (-5, 2), (4, -2), (-3, 1), (2, -1), (-1, 1)]

def put(canvas, im, x, y):
    canvas.alpha_composite(im, (int(round(x)) + PAD, int(round(y)) + PAD))

def render(t):
    cv = Image.new('RGBA', (W + 2 * PAD, H + 2 * PAD), (0, 0, 0, 255))
    cv.paste(bg, (PAD, PAD))
    tm = min(t, T_SWING)                       # the horde freezes the moment she swings
    sh = Image.new('RGBA', cv.size, (0, 0, 0, 0)); sd = ImageDraw.Draw(sh)
    draws = []
    for m in mobs:
        dx, dy = GX - m['x'], GY - m['y']; d = math.hypot(dx, dy)
        x, y = m['x'] + dx / d * 34 * tm, m['y'] + dy / d * 34 * tm
        mv, dead = anims[(m['kind'], dx < 0)]
        age = t - m['die']
        if age < 0: im = mv[int((tm + m['ph']) / 0.08) % len(mv)]
        elif age < 0.07: im = white(mv[int((tm + m['ph']) / 0.08) % len(mv)])
        else:
            i = int((age - 0.07) / 0.09)
            if i >= len(dead): continue
            im = dead[i]; k = out_expo(age / 0.35); x -= dx / d * 14 * k; y -= dy / d * 14 * k
        if age < 0.3: sd.ellipse((x - 15 + PAD, y - 5 + PAD, x + 15 + PAD, y + 4 + PAD), fill=(40, 20, 10, 70))
        draws.append((y, im, x - 64, y - 112))
        m['px'], m['py'] = x, y
    sd.ellipse((GX - 17 + PAD, GY - 5 + PAD, GX + 17 + PAD, GY + 4 + PAD), fill=(40, 20, 10, 80))
    if t < T_WIND: gi = g_idle[int(t / 0.12) % len(g_idle)]
    elif t < T_SWING: gi = g_back[3]
    elif t < T_SNAP + 0.25: gi = g_back[4]
    elif t < T_SNAP + 0.25 + 6 * 0.07: gi = g_back[5 + int((t - T_SNAP - 0.25) / 0.07)]
    else: gi = g_idle[int(t / 0.12) % len(g_idle)]
    draws.append((GY, gi, GX - 64, GY - 112))
    cv.alpha_composite(sh)
    for _, im, x, y in sorted(draws, key=lambda a: a[0]): put(cv, im, x, y)
    fxl = Image.new('RGBA', cv.size, (0, 0, 0, 255))   # sparks are light: added, not painted over
    for m in mobs:
        i = int((t - m['die']) * 30)
        if 0 <= i < len(spark): put(fxl, spark[i], m['px'] - spark_piv[0], m['py'] - spark_piv[1] + 4)
    scene = ImageChops.add(cv.convert('RGB'), fxl.convert('RGB')).convert('RGBA')

    # the cut
    out = Image.new('RGBA', scene.size, (0, 0, 0, 255))
    q = t - T_SWING
    slide = 0
    if T_SPLIT <= t < T_SNAP: slide = 13 * out_expo((t - T_SPLIT) / 0.16) + 3 * (t - T_SPLIT) / (T_SNAP - T_SPLIT)
    if slide > 0:
        P = PAD
        up = Image.new('L', scene.size, 0)
        ImageDraw.Draw(up).polygon([(LA[0] - 200 * UX + P, LA[1] - 200 * UY + P), (LB[0] + 200 * UX + P, LB[1] + 200 * UY + P), (W + 2 * P, 0), (0, 0)], fill=255)
        ox, oy = int(round(UX * slide)), int(round(UY * slide))
        a = Image.new('RGBA', scene.size, (0, 0, 0, 255)); a.paste(scene, (ox, oy)); out.paste(a, (0, 0), up)
        b = Image.new('RGBA', scene.size, (0, 0, 0, 255)); b.paste(scene, (-ox, -oy)); out.paste(b, (0, 0), ImageOps.invert(up))
    else:
        out = scene
    d = ImageDraw.Draw(out)
    def seg(s0, s1, off=0): return [(LA[0] + UX * s0 - UY * off + PAD, LA[1] + UY * s0 + UX * off + PAD), (LA[0] + UX * s1 - UY * off + PAD, LA[1] + UY * s1 + UX * off + PAD)]
    def lens(s0, s1, wd, col):
        n = 24; top = []; bot = []
        for i in range(n + 1):
            k = i / n; s = s0 + (s1 - s0) * k; w = wd / 2 * math.sin(math.pi * min(0.98, max(0.02, k))) ** 0.5
            top.append((LA[0] + UX * s - UY * w + PAD, LA[1] + UY * s + UX * w + PAD)); bot.append((LA[0] + UX * s + UY * w + PAD, LA[1] + UY * s - UX * w + PAD))
        d.polygon(top + bot[::-1], fill=col)
    if 0 <= q and t < T_SNAP:
        head = LL * out_expo(q / 0.06)
        if slide > 0:
            d.line(seg(-20, LL + 20), fill=(8, 6, 12, 255), width=int(3 + slide * 0.5))
            for off in (-(2 + slide * 0.25), 2 + slide * 0.25): d.line(seg(-20, LL + 20, off), fill=(255, 255, 255, 255), width=1)
        else:
            young = max(0, 1 - q / 0.12)
            lens(0, head, 2 + 6 * young, (255, 250, 240, 255)); lens(0, head, 1 + 2.5 * young, (255, 255, 255, 255))
            gk = (t - T_SWING - 0.08) / (T_SPLIT - T_SWING - 0.08)
            if 0 < gk < 1:
                gx, gy = LA[0] + UX * LL * gk + PAD, LA[1] + UY * LL * gk + PAD
                d.line([(gx - 6, gy), (gx + 6, gy)], fill=(255, 255, 255, 255)); d.line([(gx, gy - 6), (gx, gy + 6)], fill=(255, 255, 255, 255))
    fq = t - T_SNAP
    if 0 <= fq < 0.3:
        k = fq / 0.3
        if fq < 0.07: lens(-30, LL + 30, 30, (255, 246, 230, 255)); lens(-30, LL + 30, 18, (255, 255, 255, 255))
        else:
            gap = 3 + 16 * out_expo(k)
            for sgn in (-1, 1):
                top = seg(-20 + 60 * k, LL + 20 - 40 * k, sgn * gap)
                d.line(top, fill=(255, 250, 240, int(255 * (1 - k))), width=max(1, int(5 * (1 - k))))
    # damage numbers
    for m in mobs:
        a = t - m['die'] - 0.03
        if a < 0 or a > 0.95: continue
        f = font_b if m['crit'] else font_s
        txt = format(m['dmg'], ','); k = out_expo(a / 0.5)
        tw = d.textlength(txt, font=f); x = min(W - tw - 4, max(4, m['px'] - tw / 2)) + PAD; y = m['py'] - 40 - (34 if m['crit'] else 24) * k + PAD
        al = int(255 * min(1, (0.95 - a) / 0.2))
        d.fontmode = '1'
        d.text((x, y), txt, font=f, fill=((255, 244, 200, al) if m['crit'] else (255, 226, 107, al)), stroke_width=2 if m['crit'] else 1, stroke_fill=((150, 30, 20, al) if m['crit'] else (58, 26, 10, al)))
    sx = sy = 0
    j = int(fq * 30)
    if 0 <= j < len(SHAKE): sx, sy = SHAKE[j]
    return out.crop((PAD - sx, PAD - sy, PAD - sx + W, PAD - sy + H)).convert('RGB')

n = int(T_END * FPS)
fr = [render(i / FPS + 1e-4) for i in range(n)]
os.makedirs(os.path.join(OUT, 'frames'), exist_ok=True)
for i, f in enumerate(fr): f.save(os.path.join(OUT, 'frames', 'f_%03d.png' % i))
for scale, name in ((1, 'ad_hook1_screen_cut_x1.gif'), (2, 'ad_hook1_screen_cut_x2.gif')):
    big = [f.resize((W * scale, H * scale), Image.NEAREST).quantize(colors=255, method=Image.MEDIANCUT, dither=Image.NONE) for f in fr]
    big[0].save(os.path.join(OUT, name), save_all=True, append_images=big[1:], duration=[30] * (n - 1) + [700], loop=0, optimize=False)
    print(name, os.path.getsize(os.path.join(OUT, name)) // 1024, 'KB')
print('frames', n, 'mobs', len(mobs))
