# "blade rain" demo at the real screen size (900x1600), saved at half and third size.
import glob, json, math, os, random, sys
from PIL import Image, ImageChops, ImageDraw, ImageFont, ImageOps

ART = r'C:\3wavestudio\3wavenewidlerpg2\ArtSource'
FONT = r'C:\3wavestudio\3wavenewidlerpg2\3wavenewidlerpg2\Assets\Resource\Font\DNFBitBitv2.ttf'
FX = sys.argv[1]; OUT = sys.argv[2]
W, H, PAD, FPS = 900, 1600, 200, 30
random.seed(5)

def frames(path, flip=False):
    fs = [Image.open(f).convert('RGBA') for f in sorted(glob.glob(os.path.join(ART, path, 'frames', '*.png')))]
    return [ImageOps.mirror(f) for f in fs] if flip else fs

def sheet(base, box=None):
    m = json.load(open(base + '.json', encoding='utf8'))['meta']['pixelfx']
    im = Image.open(base + '.png').convert('RGBA'); fw, fh = m['frameSize']; c = m['columns']
    fr = [im.crop(((i % c) * fw, (i // c) * fh, (i % c) * fw + fw, (i // c) * fh + fh)) for i in range(m['frames'])]
    piv = list(m['pivot'])
    if box: fr = [f.crop(box) for f in fr]; piv = [piv[0] - box[0], piv[1] - box[1]]
    return fr, piv

def tint(im, rgb, a):
    w = Image.new('RGBA', im.size, rgb + (255,)); w.putalpha(im.getchannel('A').point(lambda v: int(v * a))); return w

def out_expo(k): return 1 if k >= 1 else 1 - 2 ** (-10 * max(0, k))

bg = Image.open(os.path.join(ART, 'backgrounds', 'bg_world_2', 'bg_world_2.png')).convert('RGBA').crop((126, 224, 126 + W, 224 + H))
g_idle, g_atk = frames('girl_base/animations/idle'), frames('girl_base/animations/attack2')
KINDS = ['monster_stage_002', 'monster_stage_002_blue', 'monster_stage_002_red', 'monster_stage_002_purple', 'mob_slime', 'mob_slime_blue', 'mob_slime_gold', 'mob_slime_pink']
anims = {(k, fl): (frames('monsters/%s/animations/move' % k, fl), frames('monsters/%s/animations/dead' % k, fl)) for k in KINDS for fl in (False, True)}
drop, drop_piv = sheet(FX)
drop = [f.resize((f.width * 2, f.height * 2), Image.NEAREST) for f in drop]; drop_piv = [drop_piv[0] * 2, drop_piv[1] * 2]   # shown at 2x
glint, glint_piv = sheet(FX.replace('blade-drop', 'blade-glint'))
glint = [f.resize((f.width * 2, f.height * 2), Image.NEAREST) for f in glint]; glint_piv = [glint_piv[0] * 2, glint_piv[1] * 2]
up = ImageOps.flip(drop[1]); LAND = 3                 # the frame the blade lands on

GX, GY = 450, 1320
T_CAST = 0.2; T_THROW = T_CAST + 0.28; T_GLINT = T_THROW + 0.22; T_RAIN = T_GLINT + 0.36; SWEEP = 0.6; T_END = 3.3
mobs = []; tries = 0
while len(mobs) < 46 and tries < 8000:
    tries += 1
    x, y = random.uniform(80, W - 80), random.uniform(330, 1230)
    if any(math.hypot(x - m['x'], (y - m['y']) * 1.6) < 62 for m in mobs): continue
    mobs.append(dict(x=x, y=y, kind=random.choice(KINDS), ph=random.uniform(0, 0.7), die=T_RAIN + 0.1 + (y - 330) / 900 * SWEEP + random.uniform(0, 0.14),
                     dmg=random.randint(12000, 98000) * random.choice([1, 1, 1, 10]), crit=False))
for m in random.sample(mobs, 9): m['crit'] = True; m['dmg'] *= 7
extra = []                                            # blades that miss and stab the floor
for i in range(70):
    y = random.uniform(120, 1500); extra.append((random.uniform(20, W - 20), y, T_RAIN + (y - 120) / 1380 * (SWEEP + 0.25) + random.uniform(0, 0.3)))
T_LAST = max(m['die'] for m in mobs)
font_s, font_b = ImageFont.truetype(FONT, 24), ImageFont.truetype(FONT, 40)

def put(canvas, im, x, y): canvas.alpha_composite(im, (int(round(x)) + PAD, int(round(y)) + PAD))

def render(t):
    cv = Image.new('RGBA', (W + 2 * PAD, H + 2 * PAD), (0, 0, 0, 255)); cv.paste(bg, (PAD, PAD))
    sh = Image.new('RGBA', cv.size, (0, 0, 0, 0)); sd = ImageDraw.Draw(sh); draws = []
    for m in mobs:
        tm = min(t, m['die'])
        dx, dy = GX - m['x'], GY - m['y']; d = math.hypot(dx, dy)
        x, y = m['x'] + dx / d * 40 * tm, m['y'] + dy / d * 40 * tm
        mv, dead = anims[(m['kind'], dx < 0)]; age = t - m['die']; m['px'], m['py'] = x, y
        im = mv[int((tm + m['ph']) / 0.08) % len(mv)]
        if age >= 0.07:
            i = int((age - 0.07) / 0.09)
            if i >= len(dead): continue
            im = dead[i]
        elif age >= 0: im = tint(im, (255, 255, 255), 1)
        if age < 0.3: sd.ellipse((x - 17 + PAD, y - 6 + PAD, x + 17 + PAD, y + 5 + PAD), fill=(40, 20, 10, 70))
        draws.append((y, im, x - 64, y - 112))
    sd.ellipse((GX - 18 + PAD, GY - 6 + PAD, GX + 18 + PAD, GY + 5 + PAD), fill=(40, 20, 10, 80))
    a = t - T_CAST
    gi = g_atk[int(a / 0.07)] if 0 <= a < len(g_atk) * 0.07 else g_idle[int(t / 0.12) % len(g_idle)]
    draws.append((GY, gi, GX - 64, GY - 112))
    cv.alpha_composite(sh)
    for _, im, x, y in sorted(draws, key=lambda a: a[0]): put(cv, im, x, y)
    fxl = Image.new('RGB', cv.size, (0, 0, 0))
    def add(im, x, y):
        nonlocal fxl
        layer = Image.new('RGBA', cv.size, (0, 0, 0, 255)); put(layer, im, x, y); fxl = ImageChops.add(fxl, layer.convert('RGB'))
    # the one dagger thrown up: it leaves the top of the screen in 0.2 s
    k = (t - T_THROW) / 0.22
    if 0 <= k < 1: add(up, GX + 20 + 190 * k - up.width / 2, GY - 120 - (GY - 270) * k - up.height / 2)
    i = int((t - T_GLINT) * 30 + 1e-3)
    if 0 <= i < len(glint): add(glint[i], GX + 150 - glint_piv[0], 150 - glint_piv[1])
    for x, y, tl in [(m['px'], m['py'] - 6, m['die']) for m in mobs] + extra:
        i = int((t - tl) * 30 + 1e-3) + LAND
        if 0 <= i < len(drop): add(drop[i], x - drop_piv[0], y - drop_piv[1])
    out = ImageChops.add(cv.convert('RGB'), fxl).convert('RGBA')
    d = ImageDraw.Draw(out); d.fontmode = '1'
    for m in mobs:
        a = t - m['die'] - 0.03
        if a < 0 or a > 0.95: continue
        f = font_b if m['crit'] else font_s; txt = format(m['dmg'], ','); k = out_expo(a / 0.5)
        tw = d.textlength(txt, font=f); x = min(W - tw - 6, max(6, m['px'] - tw / 2)) + PAD; y = m['py'] - 70 - (50 if m['crit'] else 34) * k + PAD
        al = int(255 * min(1, (0.95 - a) / 0.2))
        d.text((x, y), txt, font=f, fill=((255, 244, 200, al) if m['crit'] else (255, 226, 107, al)), stroke_width=3 if m['crit'] else 2, stroke_fill=((150, 30, 20, al) if m['crit'] else (58, 26, 10, al)))
    sx = sy = 0
    if T_RAIN + 0.08 < t < T_LAST + 0.1: sx, sy = random.choice([(5, -3), (-5, 3), (3, 4), (-3, -4), (0, 5), (0, -5)])
    return out.crop((PAD - sx, PAD - sy, PAD - sx + W, PAD - sy + H)).convert('RGB')

n = int(T_END * FPS); os.makedirs(OUT, exist_ok=True); small = []
for i in range(n): small.append(render(i / FPS + 1e-4).resize((W // 2, H // 2), Image.BOX))
for nm, sz in (('half', (450, 800)), ('third', (300, 534))):
    pal = [(f if sz[0] == 450 else f.resize(sz, Image.BOX)).quantize(colors=255, method=Image.MEDIANCUT, dither=Image.NONE) for f in small]
    name = os.path.join(OUT, 'blade_rain_demo_%s.gif' % nm)
    pal[0].save(name, save_all=True, append_images=pal[1:], duration=[30] * (n - 1) + [700], loop=0, optimize=False)
    print(nm, os.path.getsize(name) // 1024, 'KB')
print(n, 'frames', 'rain frame', int(T_RAIN * 30), 'last', int(T_LAST * 30))
idx = [10, 15, 18, 24, 29, 31, 33, 36, 39, 42, 46, 50, 56, 64, 80]
m = Image.new('RGB', (225 * 5, 400 * 3))
for k, i in enumerate(idx): m.paste(small[i].resize((225, 400), Image.BOX), ((k % 5) * 225, (k // 5) * 400))
m.save(os.path.join(OUT, 'check.png'))
