// Level-up for the floating hood pet (shellgirl): ui/level-up's particles and release burst, centred over the
// pet's head (no pillar, rings, plate or title), sized for a ~40px pet.
import { Particles, P, PF, Rng, fit, sparkle, flare, seg, TAU, WHITE } from "../../core/index.js";

export const meta = {
  name: "Hood level-up",
  desc: "Warm streaks gather over the pet's head, then it bursts: a short flash shoots up, speed lines and sparks fly out, twinkles drift and fade.",
  tags: ["pet", "level", "particles", "one-shot"],
  size: [112, 128],
  origin: [56, 112],
  duration: 1.6,
  loop: false,
  fps: 30,
  bg: "ground",
  options: { palette: ["#5a2e14", "#8a4a1e", "#c0702a", "#e0a35a", "#f6d58c", "#fde9b8", "#fdf2e8", "#ffffff"], seed: 5, count: 1, head: -46 },
};

const REL = 0.28;

export function create(o) {
  const C = fit(o.palette, 8), R = new Rng(o.seed), n = o.count ?? 1, HY = o.head ?? -46;
  const ps = new Particles({ rng: R, ground: () => 0, sparks: [C[7], C[6], C[5], C[3]] });
  const spawn = (k, x, y, vx, vy, life, ...rest) => ps.spawn(k, x, y, vx, vy, Math.min(life, meta.duration - 0.05 - t), ...rest);
  const rush = [];
  let t = 0, released = false, gather = 0;
  // a gathering streak: a warm tail and a white head, running straight in
  const drawRush = (p, back) => {
    for (const m of rush) {
      if (m.back !== back) continue;
      const sp = Math.hypot(m.vx, m.vy), ux = m.vx / sp, uy = m.vy / sp, L = m.len;
      p.line(m.x - ux * L, m.y - uy * L, m.x - ux * L * 0.4, m.y - uy * L * 0.4, m.c);
      p.line(m.x - ux * L * 0.35, m.y - uy * L * 0.35, m.x, m.y, WHITE);
    }
  };

  return {
    update(dt) {
      t += dt;
      // streaks pour in from all round to the point over the head
      for (gather += t < REL ? dt * 160 * n : 0; gather >= 1; gather--) {
        const an = R.range(0, TAU), sp = R.range(90, 150) * (1 + t * 2), D = R.range(20, 40);
        const x = Math.cos(an) * D, y = HY + Math.sin(an) * D * 0.8, tx = Math.cos(an) * 3, ty = HY + Math.sin(an) * 2;
        const dx = tx - x, dy = ty - y, l = Math.hypot(dx, dy) || 1;
        rush.push({ x, y, vx: (dx / l) * sp, vy: (dy / l) * sp, life: Math.min(l / sp, REL + 0.04 - t), len: R.range(3, 6), c: R.chance(0.45) ? C[6] : C[5], back: R.chance(0.4) });
      }
      for (let i = rush.length - 1; i >= 0; i--) {
        const m = rush[i];
        if ((m.life -= dt) <= 0) rush.splice(i, 1);
        else (m.x += m.vx * dt), (m.y += m.vy * dt);
      }
      // the burst: speed lines flung out, sparks and twinkles
      if (!released && t >= REL) {
        released = true;
        for (let i = 0; i < 18 * n; i++) {
          const an = (i / (18 * n)) * TAU + R.range(-0.12, 0.12), sp = R.range(180, 320);
          spawn(P.MOTE, Math.cos(an) * 4, HY + Math.sin(an) * 3, Math.cos(an) * sp, Math.sin(an) * sp * 0.8, R.range(0.08, 0.15),R.chance(0.4) ? C[7] : C[6], 1);
        }
        for (let i = 0; i < 14 * n; i++) {
          const an = -Math.PI / 2 + R.range(-1.3, 1.3), sp = R.range(70, 170);
          spawn(P.SPARK, R.range(-3, 3), HY, Math.cos(an) * sp, Math.sin(an) * sp, R.range(0.3, 0.6));
        }
        for (let i = 0; i < 10 * n; i++)
          spawn(P.STAR, R.range(-10, 10), HY + R.range(-10, 6), R.range(-25, 25), -R.range(10, 40), R.range(0.5, 1.0), R.pick([C[7], C[6], C[5]]), R.chance(0.3) ? R.range(1.2, 2) : R.range(0.4, 0.8), 0, R.chance(0.4) ? PF.BACK : 0);
      }
      // twinkles round the head for a moment
      if (t > REL && t < 1.2 && R.chance(0.2 * n)) {
        spawn(P.STAR, R.range(-22, 22), HY + R.range(-20, 14), R.range(-5, 5), -R.range(4, 12), R.range(0.35, 0.6), R.pick([C[7], C[6], C[5]]), R.chance(0.3) ? 1.5 : 0.6, 0, R.chance(0.3) ? PF.BACK : 0);
      }
      ps.update(dt);
    },

    drawBack(p) {
      ps.draw(p, "back");
      drawRush(p, true);
    },

    draw(p) {
      if (t < REL) {
        // a spark swelling over the head
        const k = seg(t, 0.05, REL);
        sparkle(p, 0, HY, 1 + 3 * k * k, C[6], C[5], 0.3 + 0.4 * k);
      } else if (t < REL + 0.22) {
        // the flash, and a short streak of light shooting up out of it
        const a = (t - REL) / 0.22, top = HY - 6 - 30 * Math.min(1, a * 3);
        flare(p, 0, HY, 16 * Math.pow(1 - a, 1.5), 1 - a, C);
        p.glow(0, HY, 26, WHITE, 0.25 * (1 - a));
        p.glowLine(0, top, 0, HY - 4, 4, C[5], 0.35 * (1 - a));
        for (let y = Math.ceil(top); y < HY - 4; y++) p.dot(0, y, (y - top) < 3 ? WHITE : C[6], (1 - a) * 1.4);
      }
      ps.draw(p, "front");
      drawRush(p, false);
    },
  };
}
