# skill_004 "screen cut" demo at the real screen size (900x1600 = 9x16 units at 100 PPU), saved at half size.
import glob, json, math, os, random, sys
from PIL import Image, ImageChops, ImageDraw, ImageFont, ImageOps

ART = r'C:\3wavestudio\3wavenewidlerpg2\ArtSource'
FONT = r'C:\3wavestudio\3wavenewidlerpg2\3wavenewidlerpg2\Assets\Resource\Font\DNFBitBitv2.ttf'
FX = sys.argv[1]            # base path of the screen-cut sheet (no extension)
OUT = sys.argv[2]
W, H, PAD, FPS = 900, 1600, 200, 30
random.seed(7)

def frames(path, flip=False):
    fs = [Image.open(f).convert('RGBA') for f in sorted(glob.glob(os.path.join(ART, path, 'frames', '*.png')))]
    return [ImageOps.mirror(f) for f in fs] if flip else fs

def sheet(base, scale=1):
    m = json.load(open(base + '.json', encoding='utf8'))['meta']['pixelfx']
    im = Image.open(base + '.png').convert('RGBA'); fw, fh = m['frameSize']; c = m['columns']
    fr = [im.crop(((i % c) * fw, (i // c) * fh, (i % c) * fw + fw, (i // c) * fh + fh)) for i in range(m['frames'])]
    if scale > 1: fr = [f.resize((fw * scale, fh * scale), Image.NEAREST) for f in fr]
    return fr, [m['pivot'][0] * scale, m['pivot'][1] * scale]

def white(im):
    w = Image.new('RGBA', im.size, (255, 255, 255, 255)); w.putalpha(im.getchannel('A')); return w

def out_expo(k): return 1 if k >= 1 else 1 - 2 ** (-10 * max(0, k))

bg = Image.open(os.path.join(ART, 'backgrounds', 'bg_world_2', 'bg_world_2.png')).convert('RGBA').crop((126, 224, 126 + W, 224 + H))
g_idle, g_atk = frames('girl_base/animations/idle'), frames('girl_base/animations/attack')
KINDS = ['monster_stage_002', 'monster_stage_002_blue', 'monster_stage_002_red', 'monster_stage_002_purple', 'mob_slime', 'mob_slime_blue', 'mob_slime_gold', 'mob_slime_pink']
anims = {(k, fl): (frames('monsters/%s/animations/move' % k, fl), frames('monsters/%s/animations/dead' % k, fl)) for k in KINDS for fl in (False, True)}
spark, spark_piv = sheet(os.path.join(ART, 'girl_base', 'effects', 'hit_spark', 'hit-spark'))
cut, cut_piv = sheet(FX, 2)

GX, GY = 330, 856
CX, CY = W // 2, H // 2                       # the cut sheet sits on the middle of the screen
LA, LB = (CX - 492 - 60, CY + 122 + 15), (CX + 492 + 60, CY - 122 - 15)
LL = math.hypot(LB[0] - LA[0], LB[1] - LA[1]); UX, UY = (LB[0] - LA[0]) / LL, (LB[1] - LA[1]) / LL
T_ATK = 0.2; T_SWING = T_ATK + 0.49; T_SPLIT = T_SWING + 0.3; T_SNAP = T_SWING + 0.7; T_END = 3.2
SLIDE = 26

mobs = []; tries = 0
while len(mobs) < 46 and tries < 6000:
    tries += 1
    x, y = random.uniform(70, W - 70), random.uniform(330, 1250)
    if math.hypot(x - GX, (y - GY) * 1.3) < 90: continue
    if any(math.hypot(x - m['x'], (y - m['y']) * 1.6) < 62 for m in mobs): continue
    d = math.hypot(x - GX, y - GY)
    mobs.append(dict(x=x, y=y, kind=random.choice(KINDS), ph=random.uniform(0, 0.7), die=T_SNAP + 0.02 + 0.35 * d / 900 + random.uniform(0, 0.05),
                     dmg=random.randint(12000, 98000) * random.choice([1, 1, 1, 10]), crit=False))
for m in random.sample(mobs, 9): m['crit'] = True; m['dmg'] *= 7
font_s, font_b = ImageFont.truetype(FONT, 24), ImageFont.truetype(FONT, 40)
SHAKE = [(16, -8), (-14, 8), (12, -6), (-10, 4), (8, -4), (-6, 2), (4, -2), (-2, 2)]

def put(canvas, im, x, y): canvas.alpha_composite(im, (int(round(x)) + PAD, int(round(y)) + PAD))

def render(t):
    cv = Image.new('RGBA', (W + 2 * PAD, H + 2 * PAD), (0, 0, 0, 255)); cv.paste(bg, (PAD, PAD))
    tm = min(t, T_SWING)
    sh = Image.new('RGBA', cv.size, (0, 0, 0, 0)); sd = ImageDraw.Draw(sh); draws = []
    for m in mobs:
        dx, dy = GX - m['x'], GY - m['y']; d = math.hypot(dx, dy)
        x, y = m['x'] + dx / d * 40 * tm, m['y'] + dy / d * 40 * tm
        mv, dead = anims[(m['kind'], dx < 0)]; age = t - m['die']; m['px'], m['py'] = x, y
        if age < 0: im = mv[int((tm + m['ph']) / 0.08) % len(mv)]
        elif age < 0.07: im = white(mv[int((tm + m['ph']) / 0.08) % len(mv)])
        else:
            i = int((age - 0.07) / 0.09)
            if i >= len(dead): continue
            im = dead[i]; k = out_expo(age / 0.35); x -= dx / d * 18 * k; y -= dy / d * 18 * k
        if age < 0.3: sd.ellipse((x - 17 + PAD, y - 6 + PAD, x + 17 + PAD, y + 5 + PAD), fill=(40, 20, 10, 70))
        draws.append((y, im, x - 64, y - 112))
    sd.ellipse((GX - 18 + PAD, GY - 6 + PAD, GX + 18 + PAD, GY + 5 + PAD), fill=(40, 20, 10, 80))
    if t < T_ATK: gi = g_idle[int(t / 0.12) % len(g_idle)]
    elif t < T_SWING: gi = g_atk[int((t - T_ATK) / 0.07)]
    elif t < T_SNAP + 0.1: gi = g_atk[7]
    elif t < T_SNAP + 0.1 + 3 * 0.07: gi = g_atk[8 + int((t - T_SNAP - 0.1) / 0.07)]
    else: gi = g_idle[int(t / 0.12) % len(g_idle)]
    draws.append((GY, gi, GX - 64, GY - 112))
    cv.alpha_composite(sh)
    for _, im, x, y in sorted(draws, key=lambda a: a[0]): put(cv, im, x, y)
    fxl = Image.new('RGBA', cv.size, (0, 0, 0, 255))
    for m in mobs:
        i = int((t - m['die']) * 30)
        if 0 <= i < len(spark): put(fxl, spark[i], m['px'] - spark_piv[0], m['py'] - spark_piv[1] + 4)
    scene = ImageChops.add(cv.convert('RGB'), fxl.convert('RGB')).convert('RGBA')

    slide = 0
    if T_SPLIT <= t < T_SNAP: slide = SLIDE * out_expo((t - T_SPLIT) / 0.16) + 6 * (t - T_SPLIT) / (T_SNAP - T_SPLIT)
    if slide > 0:
        P = PAD; out = Image.new('RGBA', scene.size, (0, 0, 0, 255)); up = Image.new('L', scene.size, 0)
        ImageDraw.Draw(up).polygon([(LA[0] - 300 * UX + P, LA[1] - 300 * UY + P), (LB[0] + 300 * UX + P, LB[1] + 300 * UY + P), (W + 2 * P, 0), (0, 0)], fill=255)
        ox, oy = int(round(UX * slide)), int(round(UY * slide))
        a = Image.new('RGBA', scene.size, (0, 0, 0, 255)); a.paste(scene, (ox, oy)); out.paste(a, (0, 0), up)
        b = Image.new('RGBA', scene.size, (0, 0, 0, 255)); b.paste(scene, (-ox, -oy)); out.paste(b, (0, 0), ImageOps.invert(up))
        ImageDraw.Draw(out).line([(LA[0] + P, LA[1] + P), (LB[0] + P, LB[1] + P)], fill=(8, 6, 12, 255), width=int(6 + slide * 0.5))
    else: out = scene
    # the cut sheet on top, added as light
    i = int((t - T_SWING) * 30 + 1e-3)
    if 0 <= i < len(cut):
        fxl = Image.new('RGBA', out.size, (0, 0, 0, 255)); put(fxl, cut[i], CX - cut_piv[0], CY - cut_piv[1])
        solid = Image.new('RGBA', out.size, (0, 0, 0, 0)); put(solid, cut[i], CX - cut_piv[0], CY - cut_piv[1])
        out = ImageChops.add(out.convert('RGB'), fxl.convert('RGB')).convert('RGBA')
    d = ImageDraw.Draw(out); d.fontmode = '1'
    for m in mobs:
        a = t - m['die'] - 0.03
        if a < 0 or a > 0.95: continue
        f = font_b if m['crit'] else font_s; txt = format(m['dmg'], ','); k = out_expo(a / 0.5)
        tw = d.textlength(txt, font=f); x = min(W - tw - 6, max(6, m['px'] - tw / 2)) + PAD; y = m['py'] - 70 - (50 if m['crit'] else 34) * k + PAD
        al = int(255 * min(1, (0.95 - a) / 0.2))
        d.text((x, y), txt, font=f, fill=((255, 244, 200, al) if m['crit'] else (255, 226, 107, al)), stroke_width=3 if m['crit'] else 2, stroke_fill=((150, 30, 20, al) if m['crit'] else (58, 26, 10, al)))
    sx = sy = 0; j = int((t - T_SNAP) * 30)
    if 0 <= j < len(SHAKE): sx, sy = SHAKE[j]
    return out.crop((PAD - sx, PAD - sy, PAD - sx + W, PAD - sy + H)).convert('RGB')

n = int(T_END * FPS); os.makedirs(os.path.join(OUT, 'frames'), exist_ok=True); small = []
for i in range(n):
    f = render(i / FPS + 1e-4)
    if i in (0, 21, 26, 34, 41, 43, 46, 52, 60, 80): f.save(os.path.join(OUT, 'frames', 'full_%03d.png' % i))
    small.append(f.resize((W // 2, H // 2), Image.BOX))
pal = [f.quantize(colors=255, method=Image.MEDIANCUT, dither=Image.NONE) for f in small]
name = os.path.join(OUT, 'skill_004_demo_half.gif')
pal[0].save(name, save_all=True, append_images=pal[1:], duration=[30] * (n - 1) + [700], loop=0, optimize=False)
print(os.path.getsize(name) // 1024, 'KB', n, 'frames', len(mobs), 'mobs', 'snap frame', int(T_SNAP * 30))
idx = [0, 21, 26, 34, 41, 43, 46, 52, 60, 80]
m = Image.new('RGB', (225 * 5, 400 * 2))
for k, i in enumerate(idx): m.paste(small[i].resize((225, 400), Image.BOX), ((k % 5) * 225, (k // 5) * 400))
m.save(os.path.join(OUT, 'check.png'))
