// Move trail for the floating hood pet (shellgirl). Faces right: motes and soft puffs shed off the hem drift back (left).
import { fit, cycle } from "../../core/index.js";

export const meta = {
  name: "Hood trail",
  desc: "A floating pet's move trail: warm motes and twinkles shed off its hem drift back and sink, with soft little puffs (wisp) behind. Faces right.",
  tags: ["pet", "trail", "move", "motes", "sparkle", "loop"],
  size: [96, 56],
  origin: [64, 48],
  duration: 2,
  loop: true,
  fps: 30,
  bg: "ground",
  options: {
    palette: ["#6b3a22", "#a8643a", "#e0a35a", "#f6d58c", "#fdf2e8", "#ffffff"],
    seed: 5,
    count: 14,
    wisp: 1,
    hem: 10,
    speed: 22,
  },
};

const D = 2;

export function draw(p, t, o) {
  const C = fit(o.palette, 6), hem = o.hem ?? 10, sp = o.speed ?? 22;
  // soft puffs: a short wake of round cream blobs that shrink as they fall behind
  if (o.wisp) {
    cycle(t, D, 9, o.seed * 7 + 3, (r) => r.range(0.45, 0.7), (age, L, r) => {
      const k = age / L, x = -4 - sp * 0.9 * age + r.range(-2, 2), y = -hem * 0.6 + r.range(-3, 3) + 4 * age;
      const rad = (r.range(3, 4.6)) * (1 - k * 0.85);
      if (rad < 0.7) return;
      p.disc(x + 0.4, y + 0.5, rad, C[3], 1);
      p.disc(x - 0.3, y - 0.4, rad * 0.75, C[4], 1);
    });
  }
  // motes and twinkles drifting back and down
  cycle(t, D, o.count ?? 14, o.seed * 3 + 1, (r) => r.range(0.8, 1.4), (age, L, r, i) => {
    const k = age / L;
    const x = -3 - sp * age * r.range(0.8, 1.3) + r.range(-3, 3);
    const y = -hem + r.range(-6, 4) + r.range(4, 9) * age + Math.sin(age * 9 + i) * 1.2;
    const a = Math.min(1, k * 8) * Math.min(1, (1 - k) * 2.5), tw = 0.5 + 0.5 * Math.sin(age * 11 + i * 1.9);
    if (r.chance(0.28) && k < 0.6) {
      // a little 4-point twinkle
      const c = tw > 0.6 ? C[5] : C[3];
      p.dot(x, y, c, a);
      if (tw > 0.35) { p.dot(x - 1, y, C[3], a * 0.8); p.dot(x + 1, y, C[3], a * 0.8); p.dot(x, y - 1, C[3], a * 0.8); p.dot(x, y + 1, C[3], a * 0.8); }
    } else {
      p.dot(x, y, tw > 0.7 ? C[4] : k > 0.6 ? C[2] : C[3], a);
    }
    p.glow(x, y, 5, C[3], 0.3 * a * tw);
  });
}
