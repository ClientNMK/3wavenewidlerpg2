// Dungeon pillar break: the nightmare toy-block tower (ArtSource dungeon pillar, 192 canvas) bursts apart.
// Block positions and sizes match the pillar sprite, so the engine hides the pillar and plays this at its pivot.
import { Particles, Bits, Lights, P, PF, B, Rng, band, clamp, smooth, ease, TAU, WHITE, hex, mix } from "../../core/index.js";

export const meta = {
  name: "Block tower shatter",
  desc: "A nightmare toy-block tower bursts: the glowing eye flashes and shuts, the big pastel blocks tumble out and bounce, chips and dust fly, shadow wisps fade.",
  tags: ["destroy", "break", "blocks", "debris", "dust", "one-shot"],
  size: [288, 224],
  origin: [144, 200],
  duration: 1.4,
  loop: false,
  fps: 30,
  bg: "ground",
  options: {
    blocks: ["#92A4AD", "#C18D92", "#D9C7AD", "#B095B5"],
    outline: "#2a1d33",
    eye: ["#5a4a3b", "#e8a53a", "#ffd86b", "#fff3b0"],
    shadow: "#2d1d36",
    dust: "#8a7f99",
    seed: 3,
  },
};

// the tower as drawn on the 192 canvas, relative to its pivot (96,176): [x, y of block centre, size, colour index]
const TOWER = [[-23, -27, 38, 0], [20, -27, 40, 1], [-20, -64, 28, 2], [12, -64, 28, 3], [-6, -91, 27, 0], [-2, -122, 32, 1]];
const EYE = 5;

export function create(o) {
  const R = new Rng(o.seed), OL = hex(o.outline), SH = hex(o.shadow), DUST = hex(o.dust), D = meta.duration;
  const COL = o.blocks.map((c) => { const b = hex(c); return { base: b, lit: mix(b, WHITE, 0.28), dark: mix(b, OL, 0.38), mark: mix(b, OL, 0.22) }; });
  const EY = o.eye.map(hex);
  const ps = new Particles({ rng: R, ground: () => 0, sparks: [EY[3], EY[2], EY[1], EY[0]] });
  const lights = new Lights();
  // each block keeps its bottom on the floor line (y = bottom edge), drawn rotated about its centre
  const blocks = new Bits({ ground: () => 0 });
  for (let i = 0; i < TOWER.length; i++) {
    const [x, y, s, ci] = TOWER[i], up = -y / 122, side = x === 0 ? R.sign() : Math.sign(x);
    blocks.add({ kind: B.CHUNK, x, y: y + s / 2, vx: side * R.range(40, 90) * (0.7 + 0.3 * up), vy: -R.range(120, 220) * (0.5 + up), g: 1, drag: 0.3, bounce: 0.3,
      life: D, a: 0, va: side * R.range(3, 7) * (0.5 + up), s, ci, eye: i === EYE });
  }
  // the burst: a flash at the eye, light through the room, chips, dust, the shadow letting go
  lights.add(0, -90, 110, EY[2], 0.55, 0.45);
  lights.add(0, -122, 40, WHITE, 0.9, 0.12);
  for (let i = 0; i < 30; i++) {
    const a = R.range(0, TAU), sp = R.range(120, 320);
    ps.spawn(P.SPARK, -2, -122, Math.cos(a) * sp, Math.sin(a) * sp - 60, R.range(0.2, 0.45));
  }
  for (let i = 0; i < 90; i++) {
    const [x, y, s, ci] = TOWER[R.int(0, TOWER.length - 1)], a = -Math.PI / 2 + R.range(-1.4, 1.4), sp = R.range(90, 300);
    ps.spawn(P.GRIT, x + R.range(-s, s) * 0.4, y + R.range(-s, s) * 0.4, Math.cos(a) * sp, Math.sin(a) * sp, R.range(0.6, 1.2), R.pick([COL[ci].base, COL[ci].lit, COL[ci].dark]), R.chance(0.4) ? 2 : 1);
  }
  for (let i = 0; i < 22; i++) {
    const s = R.sign(), h = R.range(0, 110);
    ps.spawn(P.SMOKE, s * R.range(0, 30), -h, s * R.range(20, 80), -R.range(10, 50), R.range(0.7, 1.2), DUST, R.range(5, 10), R.range(0, 0.1), PF.BIG);
  }
  for (let i = 0; i < 12; i++) {
    const s = R.sign();
    ps.spawn(P.SMOKE, s * R.range(4, 40), -R.range(1, 5), s * R.range(40, 120), -R.range(2, 10), R.range(0.6, 1.0), DUST, R.range(6, 9), 0, PF.BIG);
  }
  for (let i = 0; i < 10; i++) {
    const s = R.sign();
    ps.spawn(P.SMOKE, s * R.range(30, 48), -R.range(0, 60), s * R.range(5, 25), -R.range(20, 50), R.range(0.5, 0.8), SH, R.range(3, 5), R.range(0, 0.15));
  }

  let t = 0;
  for (const b of blocks.list) b.pvy = 0;

  const drawBlock = (p, b, fade) => {
    const S = b.s, h = S / 2, cx = b.x, cy = b.y - h, ca = Math.cos(b.a), sa = Math.sin(b.a), C = COL[b.ci], r = h * 1.45;
    p.shade(cx - r, cy - r, cx + r, cy + r, (x, y, out) => {
      const dx = x - cx, dy = y - cy, u = dx * ca + dy * sa, v = -dx * sa + dy * ca, au = Math.abs(u), av = Math.abs(v);
      if (au > h || av > h) return;
      out.a = fade;
      if (au > h - 1.5 || av > h - 1.5) return OL;
      if (b.eye && t < 0.6) {
        // the eye: wide, then squeezed shut
        const open = 1 - smooth(0.15, 0.45, t), er = S * 0.26;
        if (open > 0.05 && (u * u) / (er * er) + (v * v) / (er * er * open * open) < 1) return Math.abs(u) < 1.2 ? EY[0] : u * u + v * v < er * er * 0.25 ? EY[3] : EY[2];
        if (open <= 0.05 && av < 1 && au < er) return OL;
      }
      const m = S * 0.24;
      if (!b.eye && Math.max(au, av) < m && Math.max(au, av) > m - 1.6) return C.mark;
      if (u < -h + 4 || v < -h + 4) return C.lit;
      if (u > h - 4 || v > h - 4) return C.dark;
      return C.base;
    });
  };

  return {
    update(dt) {
      t += dt;
      for (const b of blocks.list) b.pvy = b.vy;
      blocks.update(dt);
      for (const b of blocks.list) {
        // stay inside the frame, and settle flat on a face once down (the floor test ignores rotation)
        const lim = 144 - b.s * 0.75;
        if (Math.abs(b.x) > lim) (b.x = Math.sign(b.x) * lim), (b.vx *= -0.4);
        if (b.rest || b.y > -2) b.a += (Math.round(b.a / (Math.PI / 2)) * (Math.PI / 2) - b.a) * Math.min(1, dt * 10);
      }
      // a heavy block landing: a puff and a few chips
      for (const b of blocks.list)
        if (b.pvy > 80 && b.vy <= 0) {
          for (let i = 0; i < 4; i++) ps.spawn(P.SMOKE, b.x + R.range(-b.s / 2, b.s / 2), -1, R.sign() * R.range(15, 50), -R.range(4, 16), R.range(0.4, 0.7), DUST, R.range(3, 6), 0, PF.BIG);
          for (let i = 0; i < 4; i++) ps.spawn(P.GRIT, b.x, -1, R.range(-80, 80), -R.range(40, 120), R.range(0.4, 0.8), COL[b.ci].dark, 1);
          if (b.rest) b.va = 0;
          b.pvy = 0;
        }
      ps.update(dt);
      lights.update(dt);
    },
    drawBack(p) {
      const k = t / 0.4;
      if (k < 1) band(p, 0, 0.5, 90 * ease.outCubic(k), 0.18, Math.max(1, 4 * (1 - k)), 1.1 - k, [DUST, mix(DUST, WHITE, 0.4), EY[2], WHITE], (g) => Math.sin(g) < 0);
      ps.draw(p, "back");
    },
    draw(p) {
      const k = t / 0.4;
      if (k < 1) band(p, 0, 0.5, 90 * ease.outCubic(k), 0.18, Math.max(1, 4 * (1 - k)), 1.1 - k, [DUST, mix(DUST, WHITE, 0.4), EY[2], WHITE], (g) => Math.sin(g) >= 0);
      const fade = 1 - smooth(D - 0.35, D - 0.02, t);
      for (const b of [...blocks.list].sort((a, c) => a.s - c.s)) drawBlock(p, b, fade);
      ps.draw(p, "front");
      if (t < 0.07) {
        p.circle(-2, -122, 20 * (1 - t / 0.07) + 6, EY[3]);
        p.circle(-2, -122, 10 * (1 - t / 0.07) + 3, WHITE);
      }
      if (t < 0.3) p.glow(-2, -122, 40, EY[2], 0.6 * (1 - t / 0.3));
      lights.draw(p);
    },
  };
}
