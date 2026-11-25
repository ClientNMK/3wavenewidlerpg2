// Scalable copy of melee/double-slash for bigger characters (size, crossing point and palette as options).
// The crescent, trace, glint and spark recipe is adapted from Sprite Fusion's open-source slash
// (https://www.spritefusion.com/pixel-art-vfx/slash), MIT License, Copyright (c) 2026 Hugo Duprez.
import { Particles, Lights, P, Rng, fit, sparkle, flare, clamp, ease, WHITE } from "../../core/index.js";
import { through, slash, slashEnd, shavings, sliver } from "./_lib.js";

export const meta = {
  name: "Double slash (big)",
  desc: "Scalable double slash: a down cut, then an up cut crossing it; the X flashes white, the crossing bursts with light and sparks, a dotted X lingers.",
  tags: ["melee", "slash", "combo", "cross", "combat", "directional", "one-shot"],
  size: [240, 160],
  origin: [40, 148],
  duration: 0.8,
  loop: false,
  fps: 30,
  bg: "ground",
  options: {
    palette: ["#2a0a22", "#5e1238", "#a01f4e", "#e0386a", "#ff86a6", "#ffd2dd", "#ffffff"],
    sparks: ["#fff3a6", "#ffe26b", "#ff8a2a", "#f2561d"],
    size: 1, // 1 = the original double-slash
    cx: 34, cy: -23, // where the strokes cross, at size 1 (scaled by size)
    sparkN: 18,
    thin: 1, // stroke thickness multiplier (0.5 = half as fat)
    life: 1, // how long strokes, flash and crossing light linger (0.6 = 40% shorter)
    seed: 6,
  },
};

const UP = 0.14;

export function create(o) {
  const K = o.size, R0 = 62 * K;
  const C = fit(o.palette, 7), Rg = new Rng(o.seed), ps = new Particles({ rng: Rg, ground: () => 0, sparks: o.sparks }), lights = new Lights();
  const X = o.cx * K, Y = o.cy * K, V = Math.sqrt(K);
  const cut = (th, dir, extra) => through(X, Y, th, dir, R0, 0.84, 0.55, extra);
  const Lf = o.life, Th = o.thin;
  const down = cut(0.72, 1, { W: 10 * K * Th, t0: 0, sweep: 0.06, hold: 0.07 * Lf, trail: 0.24 * Lf, ramp: C, lens: true });
  const up = cut(-0.66, -1, { W: 11 * K * Th, t0: UP, sweep: 0.06, hold: 0.06 * Lf, trail: 0.26 * Lf, ramp: C, lens: true });
  const ends = [slashEnd(down), slashEnd(up)];
  const slivers = [-2.4, -1.1, 0.2, 1.6].map((a) => ({ a: a + Rg.range(-0.2, 0.2), r: Rg.range(5, 8) * K, v: Rg.range(70, 120) * K, arc: Rg.range(0.4, 0.7) }));
  const CLOSE = UP + 0.012;
  let t = 0, cut1 = false, cut2 = false, closed = false;
  return {
    update(dt) {
      t += dt;
      if (!cut1 && t >= down.sweep) {
        cut1 = true;
        shavings(ps, Rg, down, Math.round(10 * V), [C[4], C[5]]);
        for (let i = 0; i < 6; i++) {
          const a = Math.atan2(ends[0].dy, ends[0].dx) + Rg.range(-0.5, 0.5), sp = Rg.range(120, 300) * V;
          ps.spawn(P.SPARK, ends[0].x, ends[0].y, Math.cos(a) * sp, Math.sin(a) * sp, Rg.range(0.12, 0.3));
        }
      }
      if (!closed && t >= CLOSE) {
        closed = true;
        lights.add(X, Y, 56 * K, C[3], 0.45, 0.4);
        lights.add(X, Y, 18 * K, WHITE, 0.6, 0.08);
        for (let i = 0; i < o.sparkN; i++) {
          const a = Rg.range(-Math.PI, Math.PI) * 0.8, sp = Rg.range(140, 360) * V;
          ps.spawn(P.SPARK, X, Y, Math.cos(a) * sp, Math.sin(a) * sp - 40, Rg.range(0.14, 0.4));
        }
        for (let i = 0; i < 5; i++) ps.spawn(P.STAR, X + Rg.range(-14, 14) * K, Y + Rg.range(-12, 12) * K, 0, -4, Rg.range(0.2, 0.4), Rg.chance(0.5) ? C[5] : WHITE, Rg.range(1, 2.2) * V, Rg.range(0.05, 0.2));
      }
      if (!cut2 && t >= UP + up.sweep) {
        cut2 = true;
        shavings(ps, Rg, up, Math.round(12 * V), [C[4], C[5]]);
      }
      ps.update(dt);
      lights.update(dt);
    },
    draw(p) {
      const flash = t >= CLOSE && t < CLOSE + 0.07 * Lf ? 1 : 0;
      down.hot = up.hot = flash;
      slash(p, down, t);
      slash(p, up, t);
      for (const [s, e] of [[down, ends[0]], [up, ends[1]]]) {
        const g = t - s.t0 - s.sweep * 0.6;
        if (g > 0 && g < 0.2) sparkle(p, e.x, e.y, 7 * K * ease.outBack(Math.min(1, g / 0.05)) * (1 - g / 0.2), C[5], C[3], 0.5);
      }
      const q = t - CLOSE;
      if (q >= 0 && q < 0.26 * Lf) {
        const k = q / (0.26 * Lf);
        flare(p, X, Y, 22 * K * Math.pow(1 - k, 1.4), 1 - k, C);
        p.glow(X, Y, 34 * K, C[4], 0.35 * (1 - k));
      }
      if (q >= 0.03 && q < 0.15) {
        const k = (q - 0.03) / 0.12;
        for (const s of slivers) sliver(p, X, Y, s.a, s.r + s.v * (q - 0.03), s.arc * (1 - 0.4 * k), k < 0.4 ? C[6] : k < 0.7 ? C[5] : C[4], 1 - k);
      }
      for (const s of [down, up]) {
        const u = t - s.t0;
        if (u <= 0 || u >= 0.25) continue;
        const e = slashEnd({ ...s, span: s.span * ease.outExpo(clamp(u / s.sweep)) });
        p.glow(e.x, e.y, 40 * K, C[3], 0.5 * (1 - u / 0.25));
      }
      ps.draw(p);
      lights.draw(p);
    },
  };
}
