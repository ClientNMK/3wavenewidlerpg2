// Scalable copy of impacts/hit-spark (same look and palette) for bigger characters.
// The two-tone spark spray and the light pop are adapted from Sprite Fusion's open-source impact
// (https://www.spritefusion.com/pixel-art-vfx/impact), MIT License, Copyright (c) 2026 Hugo Duprez.
import { Particles, Lights, P, Rng, fit, WHITE } from "../../core/index.js";

export const meta = {
  name: "Hit spark (big)",
  desc: "Scalable hit spark: a lopsided cross flare round a white diamond, then sparks and hot chips that fall and bounce. size scales it, hold stretches the flash.",
  tags: ["impact", "hit", "spark", "combat", "directional", "one-shot"],
  size: [220, 140],
  origin: [70, 120],
  duration: 0.6,
  loop: false,
  fps: 30,
  bg: "ground",
  options: {
    palette: ["#4a1712", "#c9281f", "#f2561d", "#ff8a2a", "#ffe26b", "#fff3a6", "#ffffff"],
    height: 20, aim: 0.1,
    size: 1, // scales rays, core, glow and spark speed (1 = the original hit-spark)
    hold: 1, // stretches how long the flash stays on screen
    sparkN: 9, chipN: 6,
    seed: 4,
  },
};

function ray(p, x, y, a, L, w, C, from = 0) {
  const c = Math.cos(a), s = Math.sin(a);
  for (let d = from * L; d <= L; d += 0.5) {
    const u = d / L, hw = w * Math.pow(1 - u, 1.6);
    const col = u < 0.3 ? WHITE : u < 0.62 ? C[5] : u < 0.88 ? C[4] : C[3];
    p.px(x + c * d, y + s * d, col);
    if (hw > 0.45) for (let o = 0.5; o <= hw; o += 0.5) for (const e of [-o, o]) p.px(x + c * d - s * e, y + s * d + c * e, u < 0.3 && o < 1 ? C[5] : C[4]);
  }
}

function diamond(p, x, y, r, C) {
  for (let dy = -r - 1; dy <= r + 1; dy++)
    for (let dx = -r - 1; dx <= r + 1; dx++) {
      const m = Math.abs(dx) + Math.abs(dy);
      if (m <= r) p.px(x + dx, y + dy, WHITE);
      else if (m === r + 1) p.px(x + dx, y + dy, C[3]);
    }
}

export function create(o) {
  const K = o.size, H = o.hold;
  const C = fit(o.palette, 7), R = new Rng(o.seed), X = 0, Y = -o.height, A = o.aim, floor = () => 0;
  const ps = new Particles({ rng: R, ground: floor, sparks: [C[6], C[5], C[4], C[2]] });
  const lights = new Lights();
  const tilt = R.range(-0.08, 0.08);
  const rays = [
    { a: A + tilt, L: R.range(33, 37) * K, w: 2.2 * K },
    { a: A + Math.PI + tilt, L: R.range(8, 10) * K, w: 2.2 * K },
    { a: A - Math.PI / 2 + tilt, L: R.range(15, 18) * K, w: 2.6 * K },
    { a: A + Math.PI / 2 + tilt, L: R.range(10, 12) * K, w: 2.3 * K },
    { a: A - Math.PI / 4 + tilt, L: R.range(5, 7) * K, w: 1 * K },
  ];

  lights.add(X, Y, 38 * K, C[4], 0.35, 0.18 * H);
  lights.add(X + Math.cos(A) * 10 * K, Y + Math.sin(A) * 10 * K, 22 * K, C[5], 0.45, 0.1 * H);
  lights.add(X, -2, 26 * K, C[3], 0.22, 0.22 * H);
  for (let i = 0; i < o.sparkN; i++) {
    const a = A + R.range(-0.6, 0.6) - 0.15, v = R.range(170, 360) * Math.sqrt(K);
    ps.spawn(P.SPARK, X + 1, Y, Math.cos(a) * v, Math.sin(a) * v - 40, R.range(0.25, 0.45), WHITE, 1, i > o.sparkN * 0.75 ? R.range(0.01, 0.03) : 0);
  }
  for (let i = 0; i < o.chipN; i++) {
    const a = A + R.range(-1.1, 0.5) - 0.3, v = R.range(70, 170) * Math.sqrt(K);
    ps.spawn(P.GRIT, X + 1, Y, Math.cos(a) * v, Math.sin(a) * v - R.range(30, 80), R.range(0.35, 0.45), R.pick([C[5], C[4], C[4], C[3]]), 1);
  }

  let t = 0;
  return {
    update(dt) { t += dt; ps.update(dt); lights.update(dt); },
    draw(p) {
      ps.draw(p);
      if (t < 0.025 * H) {
        for (const r of rays) ray(p, X, Y, r.a, r.L, r.w, C);
        diamond(p, X, Y, Math.round(4 * K), C);
      } else if (t < 0.058 * H) {
        for (const r of rays) ray(p, X, Y, r.a, r.L * 1.2, r.w * 0.55, C, 0.12);
        diamond(p, X, Y, Math.round(3 * K), C);
      } else if (t < 0.092 * H) {
        const r = rays[0];
        ray(p, X, Y, r.a, r.L * 1.3, 0.6 * K, [C[0], C[1], C[2], C[3], C[3], C[4]], 0.4);
      }
      const pop = Math.exp(-t / (0.045 * H));
      if (pop > 0.02) {
        p.glow(X, Y, 22 * K, C[4], 0.3 * pop);
        p.glow(X, Y, 7 * K, WHITE, 0.15 * pop);
        p.glowLine(X, Y, X + Math.cos(A) * 26 * K, Y + Math.sin(A) * 26 * K, 4 * K, C[5], 0.25 * pop);
      }
      lights.draw(p);
    },
  };
}
