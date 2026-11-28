// One dagger of light out of a rain of them (shellgirl): it streaks down at a slant, stabs into the floor with a
// flash, a small ground ring and sparks, stands there a moment, then breaks up into motes.
// The engine spawns many of these with staggered starts.
import { Particles, P, Rng, fit, flare, clamp, ease, TAU, WHITE } from "../../core/index.js";

export const meta = {
  name: "Blade drop",
  desc: "A dagger of light streaks down at a slant and stabs into the floor: flash, a small ring, sparks; the blade stands a moment and breaks up into motes.",
  tags: ["melee", "dagger", "rain", "projectile", "combat", "one-shot"],
  size: [112, 220],
  origin: [44, 184],
  duration: 0.7,
  loop: false,
  fps: 30,
  bg: "ground",
  options: {
    palette: ["#a89480", "#cdb9a4", "#e6d6c4", "#f3e6d8", "#fdf2e8", "#fffaf4", "#ffffff"],
    sparks: ["#ffffff", "#fdf2e8", "#e6d6c4", "#a89480"],
    seed: 3,
    fall: 0.1, // seconds until it lands
    dx: 52, // where it comes from, off the landing point
    dy: -180,
    len: 26, // blade length
    sparkN: 16,
  },
};

const SY = 0.42, STAND = 0.36;

export function create(o) {
  const C = fit(o.palette, 7), R = new Rng(o.seed), ps = new Particles({ rng: R, ground: () => 0, sparks: o.sparks });
  const L = Math.hypot(o.dx, o.dy), ux = o.dx / L, uy = o.dy / L, nx = -uy, ny = ux;
  let t = 0, landed = false, broke = false;
  // the blade: its tip at (x, y), lying back along the fall line; a fat middle, a crossguard, a short grip
  const blade = (p, x, y, a) => {
    for (let s = 0; s <= o.len; s += 0.5) {
      const w = s < o.len * 0.72 ? 0.4 + 1.6 * Math.sin((Math.PI * s) / (o.len * 0.72)) ** 0.6 : s < o.len * 0.8 ? 3.4 : 0.8;
      for (let k = -w; k <= w; k += 0.5) p.dot(x + ux * s + nx * k, y + uy * s + ny * k, Math.abs(k) < w * 0.5 ? WHITE : C[4], a);
    }
    p.glowLine(x, y, x + ux * o.len, y + uy * o.len, 7, C[3], 0.5 * a);
  };

  return {
    update(dt) {
      t += dt;
      if (!landed && t >= o.fall) {
        landed = true;
        for (let i = 0; i < o.sparkN; i++) {
          const an = R.range(0, TAU), sp = R.range(60, 190);
          ps.spawn(P.SPARK, 0, -1, Math.cos(an) * sp, Math.sin(an) * sp * SY - R.range(40, 150), R.range(0.18, 0.4));
        }
        for (let i = 0; i < 6; i++) {
          const an = R.range(0, TAU);
          ps.spawn(P.DUST, Math.cos(an) * 5, Math.sin(an) * 3, Math.cos(an) * 46, Math.sin(an) * 20 - 6, R.range(0.2, 0.34), C[1], R.range(1.5, 3));
        }
      }
      if (!broke && t >= STAND) {
        broke = true;
        for (let s = 2; s < o.len; s += 1.6)
          ps.spawn(P.STAR, ux * s + R.range(-1, 1), uy * s + R.range(-1, 1), R.range(-14, 14), -R.range(14, 46), R.range(0.14, 0.3), R.pick([WHITE, C[5], C[4]]), R.chance(0.3) ? 1.3 : 0.6);
      }
      ps.update(dt);
    },

    drawBack(p) {
      ps.draw(p, "back");
    },

    draw(p) {
      if (t < o.fall) {
        // falling: the blade and a long tail of light behind it
        const k = t / o.fall, x = o.dx * (1 - k), y = o.dy * (1 - k), T = 90;
        p.glowLine(x, y, x + ux * T, y + uy * T, 5, C[3], 0.35);
        for (let s = o.len; s < T; s++) p.dot(x + ux * s, y + uy * s, s < T * 0.5 ? C[5] : C[3], 1 - s / T);
        blade(p, x, y, 1);
      } else {
        const q = t - o.fall, e = ease.outExpo(clamp(q / 0.3));
        if (q < 0.14) {
          const a = q / 0.14;
          flare(p, 0, -2, 22 * Math.pow(1 - a, 1.4), 1 - a, C);
          p.glow(0, -2, 26, WHITE, 0.45 * (1 - a));
          // the streak it came down, fading
          for (let s = o.len; s < 110; s++) p.dot(ux * s, uy * s, C[5], (1 - a) * (1 - s / 110));
        }
        const r = 5 + 27 * e, ra = clamp(1.3 - q / 0.22);
        for (let i = 0, N = Math.ceil(r * 7); i < N && ra > 0; i++) {
          const an = (i / N) * TAU;
          p.dot(Math.cos(an) * r, Math.sin(an) * r * SY, q < 0.08 ? WHITE : C[4], ra);
          if (q < 0.1) p.dot(Math.cos(an) * (r - 1), Math.sin(an) * (r - 1) * SY, C[4], ra);
        }
        if (t < STAND) blade(p, ux * -3, uy * -3, 1);
        else if (t < STAND + 0.08) blade(p, ux * -3, uy * -3, 1 - (t - STAND) / 0.08);
      }
      ps.draw(p, "front");
    },
  };
}
