// A blade gale: a funnel of pale wind whips up off the floor, cuts flash through it one after another, then it
// pulls tight and bursts into a crossing slash, a ground ring and flying shards.
// The funnel (snaking axis, sheared body, wrapped wind bands, dust at its foot) is a one-shot copy of nature/tornado.
import { fit, rand, rng, smooth, clamp, ease, TAU, WHITE, tileNoise, sparkle, flare } from "../../core/index.js";

export const meta = {
  name: "Blade gale",
  desc: "A pale whirlwind rises, white cuts flash through it in quick succession and wind is dragged in along the floor; it winds tight, then bursts: a crossing slash, a ring racing over the ground, shards and sparks.",
  tags: ["melee", "wind", "tornado", "slash", "aoe", "combat", "one-shot"],
  size: [360, 270],
  origin: [180, 215],
  duration: 1.75,
  loop: false,
  fps: 30,
  bg: "ground",
  options: {
    palette: ["#8a7c6e", "#a89a8c", "#c8bcae", "#e0d6ca", "#efe8de", "#f8f3ec", "#fdfaf6", "#ffffff"],
    thick: 2, // wind band thickness in px
    dust: ["#4e3e2e", "#76624a", "#a08a6a", "#cbb894", "#e8dcc0"],
    height: 134,
    cuts: 10, // cuts that flash through the funnel before it bursts (each one is a hit)
    seed: 5,
  },
};

const SY = 0.28, RISE = 0.22, TIGHT = 1.0, BURST = 1.2, SPIN = 0.62;

function geo(o, t) {
  const g = ease.outExpo(clamp(t / RISE)), sq = smooth(TIGHT, BURST, t), q = t - BURST, s = Math.max(0, t - TIGHT);
  const blow = q > 0 ? ease.outExpo(clamp(q / 0.3)) : 0, W = (0.25 + 0.75 * g) * (1 - 0.6 * sq) * (1 + 3.2 * blow);
  return {
    FH: o.height * g * (1 + 0.16 * sq),
    turn: TAU * (t / SPIN + s * s * 7),
    fade: q > 0 ? clamp(1 - q / 0.26) : 1,
    ax: (u) => (7 * Math.sin((TAU * t) / 0.9 - 2.6 * u) * (0.2 + 0.8 * u) + 2.5 * Math.sin((2 * TAU * t) / 0.9 + 1.3 - 4.5 * u) * u) * (1 - sq),
    rad: (u) => (5 + 30 * Math.pow(u, 1.4) + 17 * Math.pow(smooth(0.8, 1, u), 2)) * W,
  };
}

function body(p, C, g, seed, hot) {
  const { FH, ax, rad } = g;
  for (let y = -Math.ceil(FH) + 2; y <= 0; y++) {
    const u = -y / FH, cx = ax(u), r = rad(u), rot = g.turn * (2 + (rand(y, seed) < 0.35 ? 1 : 0));
    const fade = Math.min(1, u * 8 + 0.25) * (1 - smooth(0.9, 1.02, u));
    for (let x = Math.floor(cx - r); x <= Math.ceil(cx + r); x++) {
      const c = (x + 0.5 - cx) / r;
      if (Math.abs(c) > 1) continue;
      const n = tileNoise(((Math.acos(c) + rot) * 6) / TAU, y * 0.34, 6, 0, seed), v = -c * 0.55 + 0.15 + n * 0.55 + hot * 0.5;
      p.blend(x, y, v > 0.6 ? C[6] : v > 0.28 ? C[5] : v > -0.05 ? C[4] : v > -0.4 ? C[3] : C[2], (0.24 + 0.44 * Math.sqrt(1 - c * c) + hot * 0.25) * fade);
    }
  }
}

function bands(p, C, g, seed, back, hot, thick) {
  const { FH, ax, rad } = g;
  for (let j = 0; j < 76; j++) {
    const R = rng(seed, j), u = (j + R.range(0.1, 0.9)) / 76, K = R.int(2, 4), span = R.range(0.7, 2.1), lvl = R.next();
    const cx = ax(u), r = rad(u) * R.range(1.0, 1.12), y0 = -u * FH, head = R.range(0, TAU) + g.turn * K * 0.5;
    if (u > 0.97) continue;
    for (let s = 0; s <= span; s += 0.5 / Math.max(2, r)) {
      const th = head - s, sn = Math.sin(th);
      if (sn > 0 === back) continue;
      const k = s / span, x = cx + r * Math.cos(th), y = y0 + r * SY * sn, lit = Math.cos(th) < 0.2;
      let i = lvl < 0.18 ? 2 : k < 0.15 && lvl > 0.5 ? 7 : k < 0.45 ? (lit ? 6 : 5) : lit ? 5 : 4;
      if (hot > 0.5) i = Math.min(7, i + 1);
      if (back) i = Math.max(1, i - 2);
      for (let d = 0; d < (back ? 1 : thick); d++) p.blend(x, y + d, C[d ? Math.max(1, i - 1) : i], (k < 0.5 ? 1 : 2 * (1 - k)) * g.fade);
    }
    if (!back && lvl > 0.55 && Math.sin(head) > 0 && g.fade > 0.5) {
      const hx = cx + r * Math.cos(head), hy = y0 + r * SY * Math.sin(head);
      p.dot(hx, hy, WHITE);
      p.glow(hx, hy, 5, C[6], 0.3);
    }
  }
}

// dust boiling round the foot, born all through the spin
function skirt(p, D, g, t, seed, back) {
  for (let j = 0; j < 44; j++) {
    const R = rng(seed, j), born = R.range(0, BURST - 0.1), life = R.range(0.5, 0.9), age = t - born;
    if (age < 0 || age > life) continue;
    const k = age / life, th = R.range(0, TAU) + R.range(5, 9) * age, sn = Math.sin(th);
    if (sn > 0 === back) continue;
    const rr = 4 + k * R.range(14, 30), x = g.ax(0) + rr * Math.cos(th), y = -1.5 - k * R.range(4, 16) + rr * 0.3 * sn, s = R.range(3, 6) * (0.6 + 0.8 * k), a = Math.min(1, k * 6) * (1 - k) * 0.55;
    p.disc(x, y + s * 0.2, s, D[back ? 1 : 2], a);
    p.disc(x - s * 0.2, y - s * 0.3, s * 0.55, D[back ? 2 : 3], a * 0.8);
  }
}

// a tapered straight stroke from (x, y) along (dx, dy), s0 to s1 px, W fat in the middle
function stroke(p, x, y, dx, dy, s0, s1, W, core, rim, a) {
  if (s1 - s0 < 1) return;
  for (let s = s0; s <= s1; s += 0.5) {
    const w = W * Math.pow(Math.sin(Math.PI * clamp((s - s0) / (s1 - s0), 0.02, 0.98)), 0.55), px = x + dx * s, py = y + dy * s;
    if (w < 1) { p.dot(px, py, core, a); continue; }
    for (let d = -w / 2; d <= w / 2; d += 0.5) p.dot(px - dy * d, py + dx * d, Math.abs(d) > w * 0.28 ? rim : core, a);
  }
}

const cutTimes = (o) => Array.from({ length: o.cuts }, (_, i) => RISE + 0.03 + (i * (TIGHT - RISE - 0.06)) / Math.max(1, o.cuts - 1));

export function drawBack(p, t, o) {
  const C = fit(o.palette, 8), D = fit(o.dust, 5), g = geo(o, t), hot = smooth(TIGHT, BURST, t);
  if (t < BURST) skirt(p, D, g, t, o.seed * 5 + 1, true);
  bands(p, C, g, o.seed * 3 + 5, true, hot, o.thick);
  if (t < BURST) body(p, C, g, o.seed, hot);
}

export function draw(p, t, o) {
  const C = fit(o.palette, 8), D = fit(o.dust, 5), g = geo(o, t), hot = smooth(TIGHT, BURST, t), q = t - BURST, H = o.height;
  bands(p, C, g, o.seed * 3 + 5, false, hot, o.thick);
  if (q < 0) {
    skirt(p, D, g, t, o.seed * 5 + 1, false);
    p.glowLine(g.ax(0.1), -g.FH * 0.1, g.ax(1), -g.FH * 0.9, 30, C[6], 0.16 + 0.3 * hot);
    p.glow(g.ax(0), -2, 30, D[3], 0.12);
    // wind dragged in along the floor
    for (let j = 0; j < 44; j++) {
      const R = rng(o.seed + 17, j), born = R.range(0.1, BURST - 0.25), life = R.range(0.18, 0.3), k = (t - born) / life;
      if (k < 0 || k > 1) continue;
      const side = j % 2 ? 1 : -1, x = side * (R.range(95, 170) * (1 - ease.outExpo(k) * 0.85) + 14), y = -R.range(2, 60), len = R.range(12, 34) * (1 - k);
      p.line(x, y, x + side * len, y, k < 0.4 ? C[7] : C[5], 0.8 * (1 - k));
    }
  }
  // the cuts
  cutTimes(o).forEach((t0, i) => {
    const age = t - t0;
    if (age < 0 || age > 0.17) return;
    const R = rng(o.seed + 31, i), u = R.range(0.25, 0.78), a = (i % 2 ? -1 : 1) * R.range(0.18, 0.75) + (i % 3 === 0 ? Math.PI : 0), len = R.range(108, 150), dx = Math.cos(a), dy = Math.sin(a);
    const x = g.ax(u) + R.range(-5, 5), y = -u * H, head = ease.outExpo(clamp(age / 0.04)), v = clamp((age - 0.06) / 0.11), ci = Math.min(4, Math.round(v * 5));
    const sx = x - (dx * len) / 2, sy = y - (dy * len) / 2;
    if (age < 0.06) p.glowLine(sx, sy, sx + dx * len * head, sy + dy * len * head, 7, C[5], 0.4);
    stroke(p, sx, sy, dx, dy, len * v * 0.6, len * head, age < 0.06 ? 7 : 5 * (1 - v), C[7 - ci], C[Math.max(1, 5 - ci)], 1 - v * v);
    if (age < 0.09) sparkle(p, x, y, 11 * (1 - age / 0.09), C[7], C[5], 0.6);
  });
  if (q < 0) return;
  const cy = -H * 0.42;
  // the crossing slash
  for (const [i, a] of [[0, -0.5], [1, 0.62], [2, -1.32]]) {
    const u = q - i * 0.03, len = i === 2 ? 200 : 260, dx = Math.cos(a), dy = Math.sin(a);
    if (u < 0 || u > 0.34) continue;
    const head = ease.outExpo(clamp(u / 0.04)), v = clamp((u - 0.09) / 0.25), ci = Math.min(4, Math.round(v * 5));
    const sx = (-dx * len) / 2, sy = cy - (dy * len) / 2;
    if (u < 0.09) p.glowLine(sx, sy, -sx, cy + (dy * len) / 2, 14, C[6], 0.5);
    stroke(p, sx, sy, dx, dy, len * v * 0.5, len * head, u < 0.09 ? 17 : 10 * (1 - v), C[7 - ci], C[Math.max(1, 6 - ci)], 1 - v * v);
  }
  if (q < 0.34) {
    const k = q / 0.34;
    flare(p, 0, cy, 74 * Math.pow(1 - k, 1.4), 1 - k, C);
    p.glow(0, cy, 100, C[6], 0.55 * (1 - k));
    // a pillar of light punches up through where the funnel stood
    const up = H * 1.5 * ease.outExpo(clamp(q / 0.06));
    p.glowLine(0, 0, 0, -up, 18, C[6], 0.5 * (1 - k));
    stroke(p, 0, 2, 0, -1, up * k * 0.7, up, 15 * Math.pow(1 - k, 1.5), C[7], C[5], 1 - k * k);
  }
  // a ring racing out over the floor
  for (const [d0, R1] of [[0, 158], [0.07, 110]]) {
    if (q < d0 || q >= d0 + 0.42) continue;
    const k = (q - d0) / 0.42, r = 16 + R1 * ease.outExpo(k), n = Math.ceil(r * 5);
    for (let j = 0; j < n; j++) {
      const th = (j / n) * TAU, x = Math.cos(th) * r, y = -2 + Math.sin(th) * r * 0.2;
      p.dot(x, y, k < 0.3 ? C[7] : C[5], (1 - k) * (rand(j, o.seed) < 0.75 ? 1 : 0.3));
      if (k < 0.5) p.dot(x * 0.96, y * 0.96 - 0.5, C[6], (1 - k * 2) * 0.8);
    }
    p.glow(0, -2, 60 + 60 * k, C[5], 0.25 * (1 - k));
  }
  // shards flung out
  for (let j = 0; j < 28; j++) {
    const R = rng(o.seed + 43, j), a = (j / 28) * TAU + R.range(-0.14, 0.14), life = R.range(0.26, 0.4), k = q / life;
    if (k > 1) continue;
    const d = R.range(90, 170) * ease.outExpo(k), len = R.range(18, 40) * (1 - k * 0.7), dx = Math.cos(a), dy = Math.sin(a) * 0.8;
    stroke(p, dx * d, cy + dy * d, dx, dy, 0, len, 4.4 * (1 - k), k < 0.4 ? C[7] : C[6], C[4], 1 - k * k);
  }
  for (let j = 0; j < 150; j++) {
    const R = rng(o.seed + 59, j), a = R.range(0, TAU), v = R.range(90, 540), life = R.range(0.22, 0.55), k = q / life;
    if (k > 1) continue;
    const d = v * q * (1 - 0.45 * k), x = Math.cos(a) * d, y = Math.min(0, cy + Math.sin(a) * d * 0.8 + 240 * q * q);
    p.dot(x, y, k < 0.35 ? C[7] : k < 0.7 ? C[5] : C[3], 1 - k * k);
    if (k < 0.5) p.dot(x - Math.cos(a) * 1.5, y - Math.sin(a) * 1.2, C[6], 0.6);
  }
  for (let j = 0; j < 18; j++) {
    const R = rng(o.seed + 71, j), born = R.range(0.02, 0.22), life = R.range(0.2, 0.34), k = (q - born) / life;
    if (k < 0 || k > 1) continue;
    sparkle(p, R.range(-90, 90), cy + R.range(-66, 50), R.range(3, 7) * Math.sin(Math.PI * k), C[7], C[5], 0.6);
  }
}
