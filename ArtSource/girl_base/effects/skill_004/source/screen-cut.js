// A screen cut: melee/delayed-cut stretched edge to edge across the screen (drawn at half size, shown at 2x).
// One thin straight line is drawn across, hangs still, then bursts open along its length.
// The glint, flare and spark recipe is adapted from Sprite Fusion's open-source slash
// (https://www.spritefusion.com/pixel-art-vfx/slash), MIT License, Copyright (c) 2026 Hugo Duprez.
import { Particles, Lights, P, Rng, fit, sparkle, flare, clamp, ease, WHITE } from "../../core/index.js";
import { sliver } from "./_lib.js";

export const meta = {
  name: "Screen cut",
  desc: "A thin white line whips through the target and hangs; after a beat it flashes fat, splits open and throws sparks and slivers off both sides.",
  tags: ["melee", "slash", "iai", "finisher", "screen", "aoe", "one-shot"],
  size: [480, 200],
  origin: [240, 100],
  duration: 1.2,
  loop: false,
  fps: 30,
  bg: "ground",
  options: {
    palette: ["#a89480", "#cdb9a4", "#e6d6c4", "#f3e6d8", "#fdf2e8", "#fffaf4", "#ffffff"],
    sparks: ["#ffffff", "#fdf2e8", "#e6d6c4", "#a89480"],
    x0: -246, y0: 61, x1: 246, y1: -61, // the line, edge to edge through the middle of the screen
    hit: 0.5, // where along the line the target is (0..1)
    hold: 0.65, // seconds the line hangs before it bursts
    sparkN: 170,
    cuts: 0, // extra cuts that flick through the target when it bursts
    power: 1.3, // how fat the flash and how big the flare (1 = the plain version)
    seed: 3,
  },
};

const DRAW = 0.05, FLASH = 0.08, OPEN = 0.4;

export function create(o) {
  const C = fit(o.palette, 7), Rg = new Rng(o.seed), ps = new Particles({ rng: Rg, ground: () => 999, sparks: o.sparks }), lights = new Lights();
  const L = Math.hypot(o.x1 - o.x0, o.y1 - o.y0), ux = (o.x1 - o.x0) / L, uy = (o.y1 - o.y0) / L, nx = -uy, ny = ux, B = DRAW + o.hold;
  const at = (s, off = 0) => [o.x0 + ux * s + nx * off, o.y0 + uy * s + ny * off];
  const [HX, HY] = at(L * o.hit);
  // a tapered stroke from s0 to s1, W px fat in the middle, pushed `off` px off the line
  const lens = (p, s0, s1, W, off, core, rim, a = 1) => {
    for (let s = s0; s <= s1; s += 0.5) {
      const w = W * Math.pow(Math.sin(Math.PI * clamp((s - s0) / (s1 - s0), 0.02, 0.98)), 0.55);
      if (w < 1) { const [x, y] = at(s, off); p.dot(x, y, core, a); continue; }
      for (let d = -w / 2; d <= w / 2; d += 0.5) { const [x, y] = at(s, off + d); p.dot(x, y, Math.abs(d) > w * 0.28 ? rim : core, a); }
    }
  };
  const streaks = [], slivers = [], cuts = [], K = o.power;
  // extra cuts: each a straight stroke through the target at its own angle, a beat apart
  const base = Math.atan2(uy, ux);
  for (let i = 0; i < o.cuts; i++) {
    const a = base + [0.5, -0.42, 1.05, -0.95, 0.2, -0.16][i % 6] + Rg.range(-0.06, 0.06), len = Rg.range(95, 130) * (Math.abs(Math.sin(a - base)) > 0.7 ? 0.8 : 1), mid = Rg.range(0.4, 0.6), dir = i % 2 ? -1 : 1;
    const x = HX + Rg.range(-6, 6), y = HY + Rg.range(-5, 5), c = Math.cos(a) * dir, n = Math.sin(a) * dir;
    cuts.push({ ax: x - c * len * mid, ay: y - n * len * mid, cx: c, cy: n, len, t0: 0.025 + i * 0.03 });
  }
  const stroke = (p, k, s0, s1, W, core, rim, a) => {
    for (let s = s0; s <= s1; s += 0.5) {
      const w = W * Math.pow(Math.sin(Math.PI * clamp((s - s0) / (s1 - s0), 0.02, 0.98)), 0.55), x = k.ax + k.cx * s, y = k.ay + k.cy * s;
      if (w < 1) { p.dot(x, y, core, a); continue; }
      for (let d = -w / 2; d <= w / 2; d += 0.5) p.dot(x - k.cy * d, y + k.cx * d, Math.abs(d) > w * 0.28 ? rim : core, a);
    }
  };
  for (let i = 0; i < 44; i++) streaks.push({ s: Rg.range(-0.05, 0.95) * L, off: Rg.range(5, 34) * (i % 2 ? 1 : -1), len: Rg.range(10, 34), v: Rg.range(260, 520), d: Rg.range(0, 0.05) });
  for (let i = 0; i < 26; i++) slivers.push({ s: Rg.range(0.03, 0.97) * L, side: i % 2 ? 1 : -1, r: Rg.range(4, 8), v: Rg.range(90, 210), arc: Rg.range(0.5, 0.9) });
  let t = 0, burst = false;
  return {
    update(dt) {
      t += dt;
      if (!burst && t >= B) {
        burst = true;
        lights.add(HX, HY, 80 * K, C[3], 0.5, 0.4);
        lights.add(HX, HY, 30, WHITE, 0.7, 0.09);
        for (let i = 0; i < o.sparkN; i++) {
          const near = Rg.chance(0.2), s = near ? L * o.hit + Rg.range(-16, 16) : Rg.range(0.04, 0.96) * L, side = Rg.chance(0.5) ? 1 : -1, sp = Rg.range(90, near ? 480 : 280), al = Rg.range(-80, 140);
          const [x, y] = at(s);
          ps.spawn(P.SPARK, x, y, nx * side * sp + ux * al, ny * side * sp + uy * al - 30, Rg.range(0.14, 0.42));
        }
        for (let i = 0; i < 70; i++) {
          const [x, y] = at(Rg.range(0.02, 0.98) * L), side = Rg.chance(0.5) ? 1 : -1, sp = Rg.range(25, 90);
          ps.spawn(P.MOTE, x, y, nx * side * sp + ux * Rg.range(0, 60), ny * side * sp - 8, Rg.range(0.3, 0.6), Rg.pick([C[4], C[5], C[6]]), 0.6);
        }
        for (let i = 0; i < 12; i++) ps.spawn(P.STAR, HX + Rg.range(-34, 40), HY + Rg.range(-26, 24), 0, -4, Rg.range(0.22, 0.45), Rg.chance(0.5) ? C[5] : WHITE, Rg.range(1.2, 2.6), Rg.range(0.03, 0.2));
      }
      ps.update(dt);
      lights.update(dt);
    },
    draw(p) {
      const q = t - B;
      if (q < 0) {
        const len = L * ease.outExpo(clamp(t / DRAW)), [ex, ey] = at(len), young = clamp(1 - (t - DRAW) / 0.06);
        p.glowLine(o.x0, o.y0, ex, ey, 5, C[3], 0.18 + 0.3 * young);
        lens(p, 0, len, 0.9 + 2.4 * young, 0, C[6], C[4]);
        const g = t / 0.16;
        if (g < 1) sparkle(p, ex, ey, 9 * ease.outBack(Math.min(1, t / 0.05)) * (1 - g), C[6], C[3], 0.6);
        // a glint slides down the hanging line toward the target, then winks on it
        const w = clamp((t - DRAW) / (o.hold * 0.7));
        if (t > DRAW && w < 1) { const [gx, gy] = at(L * (1 - w * (1 - o.hit))); sparkle(p, gx, gy, 3, C[6], C[4], 0.5); }
        const k = (B - t) / 0.06;
        if (k < 1) sparkle(p, HX, HY, 12 * (1 - k), C[6], C[4], 0.7);
      } else {
        if (q < FLASH) {
          p.glowLine(...at(-10), ...at(L + 10), 14, C[4], 0.55);
          lens(p, -12, L + 12, 9 * K, 0, C[6], C[5]);
        } else if (q < OPEN) {
          const k = (q - FLASH) / (OPEN - FLASH), gap = 1.5 + 9 * K * ease.outExpo(k), W = 4.5 * K * Math.pow(1 - k, 1.3), ci = Math.min(6, Math.round(k * 5));
          p.glowLine(...at(0), ...at(L), 8, C[3], 0.3 * (1 - k));
          for (const side of [-1, 1]) lens(p, -8 + k * 20, L + 8 - k * 14, W, side * gap, C[6 - Math.min(ci, 4)], C[Math.max(0, 4 - ci)], 1 - k * k);
        }
        if (q < 0.5) {
          const a = 0.75 * (1 - q / 0.5);
          for (let s = (q * 40) % 4; s < L; s += 4) { const [x, y] = at(s); p.dot(x, y, C[2], a * (0.5 + 0.5 * s / L)); }
        }
        for (const st of streaks) {
          const u = q - st.d;
          if (u < 0 || u > 0.14) continue;
          const s = st.s + st.v * u, [xa, ya] = at(s, st.off), [xb, yb] = at(s + st.len * (1 - u / 0.14), st.off);
          p.line(xa, ya, xb, yb, u < 0.05 ? C[6] : C[4], 1 - u / 0.14);
        }
        if (q < 0.3) {
          const k = q / 0.3;
          flare(p, HX, HY, 30 * K * Math.pow(1 - k, 1.4), 1 - k, C);
          p.glow(HX, HY, 40 * K, C[4], 0.4 * (1 - k));
        }
        for (const k of cuts) {
          const u = q - k.t0;
          if (u < 0 || u > 0.24) continue;
          const head = ease.outExpo(clamp(u / 0.035)), v = clamp((u - 0.07) / 0.17), ci = Math.min(4, Math.round(v * 5));
          if (u < 0.07) p.glowLine(k.ax, k.ay, k.ax + k.cx * k.len * head, k.ay + k.cy * k.len * head, 6, C[4], 0.4);
          stroke(p, k, k.len * v * 0.6, k.len * head, u < 0.07 ? 4.5 : 3 * (1 - v), C[6 - ci], C[Math.max(0, 4 - ci)], 1 - v * v);
          if (u < 0.1) sparkle(p, k.ax + k.cx * k.len * head, k.ay + k.cy * k.len * head, 6 * (1 - u / 0.1), C[6], C[4], 0.6);
        }
        if (q >= 0.02 && q < 0.2) {
          const k = (q - 0.02) / 0.18, base = Math.atan2(ny, nx);
          for (const s of slivers) { const [x, y] = at(s.s); sliver(p, x, y, base + (s.side > 0 ? 0 : Math.PI), s.r + s.v * (q - 0.02), s.arc * (1 - 0.4 * k), k < 0.4 ? C[6] : k < 0.7 ? C[5] : C[4], 1 - k); }
        }
      }
      ps.draw(p);
      lights.draw(p);
    },
  };
}
