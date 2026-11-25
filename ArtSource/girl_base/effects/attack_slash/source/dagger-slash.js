// Adapted from melee/sword-slash (itself adapted from Sprite Fusion's open-source slash,
// https://www.spritefusion.com/pixel-art-vfx/slash, MIT License, Copyright (c) 2026 Hugo Duprez).
// A shorter, quicker crescent for a reverse-grip dagger: centre and size are options so it can sit on any character.
import { Particles, P, Rng, fit, sparkle, flare, clamp, smooth, ease, TAU } from "../../core/index.js";

export const meta = {
  name: "Dagger slash",
  desc: "A short, tight crescent snaps across the front in two frames, white at the rim cooling down its ramp, a glint at the tip and a flick of sparks.",
  tags: ["melee", "slash", "dagger", "combat", "directional", "one-shot"],
  size: [220, 140],
  origin: [40, 128],
  duration: 0.6,
  loop: false,
  fps: 30,
  bg: "ground",
  options: {
    palette: ["#2d2233", "#54405a", "#8a7488", "#c2b2b8", "#e9dfd2", "#fff7e8", "#ffffff"],
    sparks: ["#fff3a6", "#ffd86b", "#e8a33a", "#b8661d"],
    radius: 30,
    width: 9,
    cx: 18,
    cy: -30,
    sy: 0.55, // vertical squash of the arc's ellipse (1 = round)
    a0: -2.35, // start angle of the main cut (less negative = shorter arc, starts further forward)
    // extras for a flashier cut: spark/mote counts, tip glint size, an 8-ray flare and a ring at the tip,
    // and a second crescent that crosses back the other way a beat later (0 turns each off)
    sparkN: 7,
    moteN: 10,
    glint: 7,
    flareL: 0,
    ringR: 0,
    second: 0,
    seed: 2,
  },
};

function onArc(s, r, a) {
  const lx = Math.cos(a) * r, ly = Math.sin(a) * r * s.sy, c = Math.cos(s.rot), n = Math.sin(s.rot);
  return [s.cx + lx * c - ly * n, s.cy + lx * n + ly * c];
}

function crescent(p, s, ta, L, W, age) {
  if (Math.abs(L) < 0.02 || W < 0.3) return;
  const R = s.ramp, n = R.length, dir = Math.sign(L), span = Math.abs(L), c = Math.cos(s.rot), sn = Math.sin(s.rot), B = s.R + 2;
  p.shade(s.cx - B, s.cy - B, s.cx + B, s.cy + B, (x, y, out) => {
    const dx = x - s.cx, dy = y - s.cy, lx = dx * c + dy * sn, ly = (-dx * sn + dy * c) / s.sy, r = Math.hypot(lx, ly);
    if (r > s.R + 0.5 || r < s.R - W - 1) return;
    let u = (dir * (Math.atan2(ly, lx) - ta)) % TAU;
    if (u < 0) u += TAU;
    if (u > span) return;
    const v = u / span, w = W * Math.pow(Math.sin(Math.PI * Math.pow(v, 2.2)), 0.6);
    if (w < 0.35) return;
    const q = (s.R - r) / w;
    if (q < 0 || q > 1) return;
    const ci = q < 0.22 && age < 0.5 ? 0 : Math.round(q * (n - 1) * 0.75 + age * 2.4 - v * 0.8 + 0.4);
    out.c = R[n - 1 - clamp(ci, 0, n - 1)];
    out.a = Math.min(1, v / 0.4);
  });
  for (let u = 0; u <= span; u += 0.3) {
    const [x, y] = onArc(s, s.R - W * 0.4, ta + dir * u);
    p.glow(x, y, 4 + W, R[n - 3], 0.18 * (1 - age) * (u / span));
  }
}

function drawSlash(p, s, t) {
  const u = t - s.t0;
  if (u < 0) return;
  const span = s.a1 - s.a0, head = ease.outExpo(clamp(u / s.sweep)), tail = smooth(s.sweep + s.hold, s.sweep + s.hold + s.trail, u);
  const age = clamp(u / (s.sweep + s.hold + s.trail));
  if (tail < 1) crescent(p, s, s.a0 + span * tail, span * (head - tail), s.W * (1 - tail * 0.55), age);
  const th = 0.6 * (1 - clamp((u - s.sweep - s.hold) / (s.trail + 0.15)));
  if (tail > 0.02 && th > 0)
    for (let k = 0; k <= tail; k += 0.6 / (Math.abs(span) * s.R)) {
      const [x, y] = onArc(s, s.R - 0.5, s.a0 + span * k);
      p.dot(x, y, s.ramp[1], th * (0.4 + 0.6 * k));
    }
}

function tipOf(s) {
  const [x, y] = onArc(s, s.R - s.W * 0.3, s.a1), [x2, y2] = onArc(s, s.R - s.W * 0.3, s.a1 + Math.sign(s.a1 - s.a0) * 0.05);
  const l = Math.hypot(x2 - x, y2 - y) || 1;
  return { x, y, dx: (x2 - x) / l, dy: (y2 - y) / l };
}

export function create(o) {
  const C = fit(o.palette, 7), Rg = new Rng(o.seed), ps = new Particles({ rng: Rg, ground: () => 0, sparks: o.sparks });
  const R0 = o.radius, a0 = o.a0, a1 = 0.85;
  const A = { cx: o.cx, cy: o.cy, R: R0, sy: o.sy, rot: -0.3, a0, a1, W: o.width, t0: 0, sweep: 0.05, hold: 0.05, trail: 0.2, ramp: C };
  const E = { ...A, R: R0 * 0.78, W: o.width * 0.38, a0: a0 + (a1 - a0) * 0.1, a1: a1 - (a1 - a0) * 0.04, t0: 0.017, hold: 0.03, trail: 0.16, ramp: C.slice(0, 6) };
  const tip = tipOf(A);
  // the return cut: same centre, tilted the other way, sweeping back from lower right to upper left
  const B2 = o.second ? { ...A, cx: A.cx + 6, cy: A.cy + 4, R: R0 * 0.9, W: o.width * 0.8, rot: 0.62, sy: o.sy * 0.9, a0: 0.9, a1: -1.35, t0: 0.11, ramp: C } : null;
  const tip2 = B2 ? tipOf(B2) : null;
  let t = 0, landed = false, landed2 = false;
  function burst(tp, n, m, arc) {
    for (let i = 0; i < n; i++) {
      const a = Math.atan2(tp.dy, tp.dx) + Rg.range(-0.7, 0.7), sp = Rg.range(120, 320);
      ps.spawn(P.SPARK, tp.x, tp.y, Math.cos(a) * sp, Math.sin(a) * sp, Rg.range(0.1, 0.3));
    }
    for (let i = 0; i < m; i++) {
      const a = arc.a0 + (arc.a1 - arc.a0) * Rg.range(0.3, 1), [x, y] = onArc(arc, arc.R, a);
      const ox = x - arc.cx, oy = y - arc.cy, l = Math.hypot(ox, oy) || 1, sp = Rg.range(25, 90);
      ps.spawn(P.MOTE, x, y, (ox / l) * sp, (oy / l) * sp - 10, Rg.range(0.2, 0.5), C[4 + Rg.int(0, 2)], 0.6);
    }
  }
  // flare and ring at a tip, u seconds after it lands
  function pop(p, tp, u, scale, withRing) {
    if (u < 0 || u > 0.16) return;
    const k = 1 - u / 0.16;
    if (o.flareL > 0 && u < 0.1) flare(p, tp.x, tp.y, o.flareL * scale * ease.outBack(Math.min(1, u / 0.03)) * (1 - u / 0.1), 0.8, o.palette);
    // a fat white shock ring that snaps out and thins away quickly (only after the flare is mostly gone)
    if (withRing && o.ringR > 0 && u > 0.02) {
      const v = (u - 0.02) / 0.14, r = 3 + o.ringR * scale * ease.outQuart(v);
      p.ring(tp.x, tp.y, r, Math.max(1, 3.5 * (1 - v)), C[6], Math.max(0, 1 - v * 1.1));
    }
  }
  return {
    update(dt) {
      t += dt;
      if (!landed && t >= A.sweep) { landed = true; burst(tip, o.sparkN, o.moteN, A); }
      if (B2 && !landed2 && t >= B2.t0 + B2.sweep) { landed2 = true; burst(tip2, Math.round(o.sparkN * 0.7), Math.round(o.moteN * 0.6), B2); }
      ps.update(dt);
    },
    draw(p) {
      drawSlash(p, E, t);
      drawSlash(p, A, t);
      if (B2) drawSlash(p, B2, t);
      const g = t - A.sweep * 0.6;
      if (g > 0 && g < 0.2) sparkle(p, tip.x, tip.y, o.glint * ease.outBack(Math.min(1, g / 0.05)) * (1 - g / 0.2), C[5], C[3], 0.6);
      pop(p, tip, t - A.sweep, 1, true);
      if (B2) {
        const g2 = t - B2.t0 - B2.sweep * 0.6;
        if (g2 > 0 && g2 < 0.18) sparkle(p, tip2.x, tip2.y, o.glint * 0.8 * ease.outBack(Math.min(1, g2 / 0.05)) * (1 - g2 / 0.18), C[5], C[3], 0.6);
        pop(p, tip2, t - B2.t0 - B2.sweep, 0.7, false);
      }
      if (t < 0.25) {
        const [x, y] = onArc(A, A.R - 3, A.a0 + (A.a1 - A.a0) * ease.outExpo(clamp(t / A.sweep)));
        p.glow(x, y, 30, C[3], 0.4 * (1 - t / 0.25));
      }
      ps.draw(p);
    },
  };
}
