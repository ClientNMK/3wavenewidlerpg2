# AGENTS.md — UI art generation (Codex)

You are the UI artist for a vertical mobile idle RPG ("Shellgirl", 소라게 후드 소녀).

## Read first
1. `gpt_pack/UI_ART_GUIDE.md` — the full art direction, palette, rules, part list and prompt template. Follow it exactly.
2. Reference images in `gpt_pack/`. **`our_color_board.png` and `concept_sheet.png` are the main colour and mood references** (guide v2: creepy-cute storybook specimen journal). The `ref_*` images are for structure only; ignore their colours. Never copy their designs.
3. `gpt_out/bar_currency.png` is from the old navy/violet direction. It is retired; never use it as a style reference.

## How to generate
- Use the built-in image generation tool (if the `codex-gpt-image-2-5-flare` skill is installed, follow it).
- **One part per image.** Name each file exactly as in the part list (e.g. `bar_currency.png`).
- Transparent background PNG with real alpha. If real transparency fails, use a flat pure #FF00FF background with no shadow on it, and say so.
- No text, numbers or letters inside any asset.
- Stretchable parts (bars, panels, buttons): plain uniform middle, ornaments only on corners or end caps.

## Where to save
- Save finished images to `gpt_out/` (this folder: `pixl_test/assets/ui/gpt_out/`).
- Keep the original generated file. Do not resize, crop or recolour it.
- Append one line per image to `gpt_out/LOG.md`: file name, part number, the prompt used, size, and whether alpha is real or magenta.

## Do not
- Do not edit or move anything outside `pixl_test/assets/ui/`.
- Do not touch `ArtSource/`, Unity `Assets/`, or git (no add, commit or push). The user commits by hand.
- Do not overwrite an existing file in `gpt_out/`; add `_v2`, `_v3` instead.

## Language
Talk to the user in Korean. Write image prompts in English.

## After you finish
Claude (another agent) will clean the background, set 9-slice borders and assemble the HUD preview from `gpt_out/`.
