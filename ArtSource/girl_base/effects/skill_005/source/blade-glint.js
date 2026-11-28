// The glint before a rain of daggers (shellgirl): a four-armed star of light swells where the thrown dagger went,
// then flares and scatters small twinkles as the rain begins.
import { Particles, P, Rng, fit, flare, clamp, TAU, WHITE } from "../../core/index.js";

export const meta = {
  name: "Blade glint",
  desc: "A four-armed star of light swells in the sky, then flares and scatters twinkles.",
  tags: ["melee", "dagger", "rain", "glint", "one-shot"],
  size: [200, 200],
  origin: [100, 100],
  duration: 0.6,
  loop: false,
  fps: 30,
  bg: "ground",
  options: { palette: ["#a89480", "#cdb9a4", "#e6d6c4", "#f3e6d8", "#fdf2e8", "#fffaf4", "#ffffff"], seed: 3, burst: 0.36, arm: 78 },
};

export function create(o) {
  const C = fit(o.palette, 7), R = new Rng(o.seed), ps = new Particles({ rng: R });
  let t = 0, burst = false;
  // an arm: fat at the heart, thinning to a point
  const arm = (p, dx, dy, L, W, a) => {
    for (let s = 0; s <= L; s += 0.5) {
      const w = W * Math.pow(1 - s / L, 1.6);
      for (let k = -w; k <= w; k += 0.5) p.dot(dx * s - dy * k, dy * s + dx * k, Math.abs(k) < w * 0.55 ? WHITE : C[4], a);
    }
  };
  return {
    update(dt) {
      t += dt;
      if (!burst && t >= o.burst) {
        burst = true;
        for (let i = 0; i < 26; i++) {
          const an = R.range(0, TAU), sp = R.range(60, 260);
          ps.spawn(P.STAR, Math.cos(an) * 6, Math.sin(an) * 6, Math.cos(an) * sp, Math.sin(an) * sp, Math.min(R.range(0.1, 0.2), 0.22), R.pick([WHITE, C[5], C[4]]), R.chance(0.3) ? 1.6 : 0.7);
        }
      }
      ps.update(dt);
    },
    draw(p) {
      const k = clamp(t / o.burst), q = t - o.burst;
      if (q < 0) {
        const L = 4 + (o.arm - 4) * k * k, W = 1 + 3.5 * k;
        p.glow(0, 0, 14 + 40 * k, C[3], 0.25 + 0.35 * k);
        arm(p, 1, 0, L, W, 1), arm(p, -1, 0, L, W, 1), arm(p, 0, 1, L * 0.8, W, 1), arm(p, 0, -1, L * 0.8, W, 1);
        for (const [x, y] of [[0.7, 0.7], [-0.7, 0.7], [0.7, -0.7], [-0.7, -0.7]]) arm(p, x, y, L * 0.3, W * 0.6, 1);
      } else if (q < 0.16) {
        const a = q / 0.16, L = o.arm * (1 + 0.25 * a), W = 4.5 * (1 - a);
        flare(p, 0, 0, 60 * Math.pow(1 - a, 1.3), 1 - a, C);
        p.glow(0, 0, 80, WHITE, 0.5 * (1 - a));
        arm(p, 1, 0, L, W, 1 - a), arm(p, -1, 0, L, W, 1 - a), arm(p, 0, 1, L * 0.8, W, 1 - a), arm(p, 0, -1, L * 0.8, W, 1 - a);
      }
      ps.draw(p, "front");
    },
  };
}
