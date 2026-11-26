# UI Art Guide — Shellgirl Idle RPG (소라게 후드 소녀)

> For an image AI (ChatGPT / Codex image generation). Read this whole file before generating anything.
> Version 2 (2026-11-26): colour and mood changed to **"creepy-cute storybook specimen journal"**. Ignore any older navy / violet / night-sea instructions.
> 한국어 요약은 맨 아래에 있습니다.

## 1. The game in one paragraph
A vertical mobile idle RPG. The heroine is a small doll-like girl with wavy ash-cream hair, golden eyes, a dark cocoa-brown dress with gold buttons, a big rust-red ribbon with a gold bell, cream frills and brown boots. She fights with one small dagger held low in reverse grip. When she defeats a boss she wears the boss's **shell as a hood**. The hood is a living parasite: an ivory bone-like shell with coral-red spots and **one big amber eye**, and when it awakens it opens into a huge **toothy maw** around her. The first hood is a cracked egg shell with one amber eye. Enemies are cute jelly slimes. Characters and monsters are top-down **pixel art**.

**Mood: creepy-cute storybook.** Cute girl, slightly eerie parasite hood. It feels like an old illustrated fairy-tale book or a naturalist's specimen journal: parchment paper, brown ink, hand-lettered labels, little vine flourishes. The sea / hermit-crab idea is only a motif, not the main theme.

## 2. UI art direction (most important)
- The UI is **NOT pixel art.** It is smooth, hand-illustrated mobile-game UI that sits on top of the pixel game.
- Materials: **aged parchment, bone-ivory shell, cocoa-brown leather, antique brass**, with **coral-rust red** and **amber** accents.
- Shading: soft painted shading, gentle warm highlights, slightly worn edges. A little gloss on buttons is fine, but **no plastic shine, no neon, no cold glow.**
- Signature motifs (use these to make the UI ours):
  1. **The amber eye** — a single round amber/gold eye with a dark slit pupil. Use as selection marker, alert marker, emblem on crests, and the "awakening" moment.
  2. **Shell ridges** — curved ivory shell bands with soft coral-red spots, for frame edges and end caps.
  3. **Teeth** — small ivory fang serrations, only for boss, danger and "awakened" frames.
  4. **Antenna curls** — thin curled stalks with a round bead tip (like the hood's eye stalks), as flourishes on corners and crests.
  5. **Storybook trim** — thin brown ink lines, tiny vine sprigs, small four-point stars, as on the concept sheet.
- Use motifs on corners, end caps and crests only, never in the middle of a stretchable area.
- Feel: cute first, eerie second. Never gory, never dark-horror.
- See the reference images (section 6) for quality and structure. **Do not copy their exact designs.**

## 2-1. UI weight by screen (must follow)
The game is played on a small vertical phone screen, and the battle field must stay visible. So UI has **three weights**:

| Weight | Where | Look |
|---|---|---|
| **Light (HUD)** | Always on screen during battle: currency, portrait, level, EXP bar, buff icons, quest tracker, side buttons | **Thin, small, semi-transparent** plates in smoky brown #3A2218 at about 70% opacity, a **very thin** antique-brass line, almost no ornaments, no glow. Icons do the talking. |
| **Medium (windows)** | Skill, inventory, shop, settings, stage windows | **Parchment** #E9D9C2 panels with a **cocoa-brown leather** #5A3826 frame, brass corner fittings, small shell-ridge or antenna-curl corners, ink-line inner border. Tabs and buttons in amber orange. |
| **Heavy (moments)** | Gacha / summon, boss clear, reward and result screens, tier frames, event banners | The hood "awakens": a glowing amber eye crest, shell ridges opening like a maw with ivory teeth around the reward, warm golden light rays, brass and bone ornaments, sparkles. Rich but still warm. |

Colour meaning (all weights):
- **Amber orange / gold** = can press, good thing (main buttons like "summon", "confirm", "claim")
- **Moss green** = buy more / plus / success
- **Coral-rust red** = close (X), alert dot, danger
- **Faded sand / gray-brown** = disabled

Design rule: if a part is visible during battle, make it **Light**. If unsure, go lighter.

## 3. Colour palette (use these exact colours)
| Name | Hex | Use |
|---|---|---|
| Parchment | #E9D9C2 | Window and panel fill |
| Bone ivory | #F3E3CC | Shell ridges, highlights, teeth |
| Faded sand | #C9A98A | Secondary fill, disabled |
| Cocoa brown | #5A3826 | Frames, dark windows, text outline |
| Smoky brown | #3A2218 | HUD semi-transparent plates, deepest shadow |
| Antique brass | #A86B3C | Metal fittings and their shadow |
| Eye gold | #F2B544 | Currency, rewards, metal highlights |
| Amber orange | #E58A4C | Main buttons, selected tab |
| Coral rust red | #C2583E | Accents, alert, close, ribbon |
| Moss green | #7E8F6E | Plus / buy / success |

Character colours for reference: hair #D8C0A8 / #F8E0C8, dress #583830 / #402820, ribbon #A03830, skin #F8C8A8, slime #90B098.

Avoid: saturated primaries, neon, cold navy or violet as a main colour, pure black, pure white.

## 4. Hard rules for every asset
1. **One asset per image.** Never put several parts in one image unless asked for a sheet.
2. **Transparent background** (real alpha PNG). If transparency is impossible, use a **flat pure #FF00FF magenta** background with no shadow on it, so it can be keyed out.
3. **No text, no numbers, no letters** inside any asset. Text is added in the engine. Simple symbols (X, +, !) are allowed only where the part list says so.
4. **Front view, flat-on**, no perspective, no tilt.
5. **Centered**, with about 8% empty margin on each side.
6. **Stretchable parts (bars, panels, buttons):** keep the middle section plain and uniform, so it can be 9-sliced. Put all ornaments in the corners, end caps or crest.
7. Glow and sparkles may extend outside the frame but must stay inside the canvas.
8. Same lighting on everything: warm light from the top.

## 5. Part list (make in this order)

### 5-A. Light — battle HUD (make these first)
| # | File name | What | Size (px) | Notes |
|---|---|---|---|---|
| H1 | hud_pill | Thin semi-transparent pill for currency numbers | 1000×140 | Smoky brown ~70% opacity, 1–2 px antique-brass line, plain, no ornaments |
| H2 | icon_gold | Single gold coin with a tiny amber-eye emblem | 400×400 | Sits on the left end of hud_pill, no socket ring |
| H3 | icon_gem | Amber gem / crystal with a warm glow | 400×400 | Second currency |
| H4 | btn_plus_small | Small moss-green plus button | 200×200 | "+" symbol allowed, slight gloss |
| H5 | frame_portrait_small | Small rounded-square portrait frame | 300×300 | Thin brass rim, dark inner, no ornaments |
| H6 | badge_level_small | Tiny level tab | 240×90 | Smoky brown, thin brass line |
| H7 | bar_exp | Very thin EXP bar trough | 1200×40 | Smoky brown, thin brass line |
| H8 | bar_exp_fill | EXP fill strip | 1200×30 | Amber to eye-gold gradient, no frame |
| H9 | card_quest | Semi-transparent quest tracker card | 700×220 | Smoky brown ~65% opacity, rounded, plain |
| H10 | btn_side_small | Small round side button plate | 240×240 | Smoky brown semi-transparent, thin brass rim, empty center |
| H11 | icon_mail, icon_rank, icon_shop, icon_settings, icon_chest, icon_boss | Side button icons | 300×300 each | Same painted style as icon_gold. icon_boss = small shell with amber eye and teeth |
| H12 | dot_alert | Coral-red alert dot | 100×100 | White "!" allowed |

### 5-B. Medium — windows
| # | File name | What | Size (px) | Notes |
|---|---|---|---|---|
| M1 | panel_window | Window panel | 1400×1600 | Parchment fill, cocoa leather frame, brass corners, small antenna-curl corner flourishes, ink-line inner border, plain middle |
| M2 | panel_header | Window title plate | 1000×200 | Cocoa leather plate with a small amber-eye crest on top, plain middle for text |
| M3 | tab_on, tab_off | Window tabs | 360×110 each | tab_on amber orange, tab_off faded sand |
| M4 | btn_primary | Main amber-orange button | 600×180 | Soft gloss, brass rim, plain middle (for "confirm", "summon") |
| M5 | btn_secondary | Cocoa-brown secondary button | 600×180 | Plain middle |
| M6 | btn_close | Round coral-red close button | 200×200 | "X" symbol allowed |
| M7 | slot_item | Item / skill slot | 260×260 | Parchment inner, cocoa rim, empty center |
| M8 | list_row | Row plate for lists | 1200×200 | Light parchment, thin cocoa line, plain middle |

### 5-C. Heavy — moments
| # | File name | What | Size (px) | Notes |
|---|---|---|---|---|
| X1 | crest_eye | Amber eye crest emblem | 600×400 | Big amber eye in a bone-shell frame with antenna curls. Signature piece |
| X2 | frame_reward_maw | Reward frame shaped like an opening shell maw | 900×900 | Ivory teeth around the edge, coral spots, empty center for the reward icon |
| X3 | banner_ribbon | Rust-red ribbon banner ("congratulations") | 1400×260 | Brass ends, plain middle for text |
| X4 | fx_rays | Warm golden light rays behind rewards | 1200×1200 | Transparent edges |
| X5 | panel_popup | Ornate popup panel | 1400×1000 | Parchment, leather frame, eye crest top center, shell-ridge corners, plain middle |
| X6 | frame_portrait | Ornate round portrait frame for profile screen | 600×600 | Bone shell ring, coral spots, antenna curls, small eye on top |
| X7 | fx_sparkle | Single 4-point sparkle | 200×200 | Warm white-gold |

Tier variants (later): the same Heavy frames per rarity, with more teeth, more eyes and richer brass as the tier goes up.

**Retired:** the navy/violet `bar_currency` made earlier is not in this palette. Do not use it as a style reference.

## 6. Reference images (attach these)
| File | Take from it |
|---|---|
| our_color_board.png | **Main colour and mood reference.** Palette, motifs, weight rules |
| concept_sheet.png | **Main mood reference.** Parchment storybook look, the hood's eye, shell and teeth |
| character_reference.png | Our heroine, slime and gold coins (pixel art). Match colours and cuteness, not the pixel style |
| ref_bars_round_socket_themed.jpg | Structure only: socket on the left of a bar, decorations per tier. Ignore its colours |
| ref_bars_sparkle_gradient.jpg | Structure only: sparkle accents, small tab at bottom-left. Ignore its colours |
| ref_panels_ornament_frame.jpg | Structure only: corner and crest ornaments on panels. Ignore its colours |
| ref_hud_layout_topbar.png | Layout only: portrait top-left, currency top-right, buff row, stage bar |
| (no file) 「레츠고 레전드!」 on uibowl.io | Source of the Light / Medium / Heavy split |

## 7. Prompt templates

Light (battle HUD):
```
Game UI asset for a cute mobile idle RPG battle HUD, [PART DESCRIPTION].
Style: clean, minimal, smooth hand-painted mobile game UI, not pixel art. Thin and small so the battle stays visible:
smoky brown #3A2218 semi-transparent fill (about 70% opacity), a very thin antique brass #A86B3C line,
subtle warm top highlight, no ornaments, no glow, no sparkles.
Front view, centered, 8% margin, transparent background PNG, no text, no numbers, no letters.
Size [W×H].
```

Medium (windows):
```
Game UI asset for a cute mobile idle RPG, [PART DESCRIPTION].
Style: smooth hand-painted mobile game UI, not pixel art. Creepy-cute storybook specimen journal:
aged parchment #E9D9C2 fill, cocoa-brown leather #5A3826 frame, antique brass #A86B3C corner fittings,
thin brown ink inner border, soft warm painted shading, slightly worn edges.
Small decorations only on the corners: [antenna curls with bead tips / ivory shell ridge with coral-red spots / tiny vine sprig].
Keep the middle section plain and uniform so it can be 9-sliced.
Front view, centered, 8% margin, transparent background PNG, no text, no numbers, no letters.
Size [W×H].
```

Heavy (moments):
```
Game UI asset for a cute mobile idle RPG reward moment, [PART DESCRIPTION].
Style: rich hand-painted mobile game UI, not pixel art. Creepy-cute storybook:
bone-ivory #F3E3CC shell with coral-rust #C2583E spots, a glowing amber #F2B544 eye with a dark slit pupil,
small ivory fangs, antique brass fittings, warm golden light and a few sparkles. Cute first, slightly eerie, never gory.
Front view, centered, 8% margin, transparent background PNG, no text, no numbers, no letters.
Size [W×H].
```

## 8. Delivery
- Save each asset as `<file name>.png` from the part list.
- If an asset is wrong (text appeared, ornament in the middle, background not transparent, cold colours), regenerate that one asset only.
- Put finished files in `pixl_test/assets/ui/gpt_out/`. Claude will clean the background, set 9-slice borders and assemble the preview.

---

## 한국어 요약 (v2)
- **분위기: 기괴하게 귀여운 동화 표본첩.** 오래된 동화책, 양피지, 뼈 껍데기, 산호빛 반점, 호박색 외눈, 살짝 섬뜩함. 바다는 소재일 뿐 메인 테마가 아님.
- **UI는 도트가 아니라 손그림 느낌의 매끈한 일러스트.** 재료는 양피지, 뼈 아이보리, 코코아 가죽, 앤틱 황동. 플라스틱 광택·네온·차가운 빛 금지.
- **시그니처 장식:** 호박색 외눈(선택·알림·문장), 껍데기 줄무늬, 이빨 톱니(보스·위험·각성), 더듬이 곡선, 동화책 잉크 선.
- **화면별 무게 3단계:**
  - **가벼움 (전투 HUD):** 그을린 갈색 반투명 판 + 얇은 황동선, 장식 없음.
  - **중간 (창):** 양피지 패널 + 코코아 가죽 테두리 + 황동 모서리, 주황 탭·버튼.
  - **무거움 (순간):** 후드가 깨어나는 연출. 빛나는 외눈 문장, 이빨 달린 껍데기가 보상을 감싸고, 금빛 빛줄기.
- **색 의미:** 주황·금 = 누를 것·좋은 것, 이끼 초록 = 추가 구매·성공, 산호 빨강 = 닫기·알림·위험, 바랜 모래 = 비활성.
- **10색:** 양피지 #E9D9C2, 뼈 아이보리 #F3E3CC, 바랜 모래 #C9A98A, 코코아 #5A3826, 그을린 갈색 #3A2218, 앤틱 황동 #A86B3C, 눈동자 금 #F2B544, 호박 주황 #E58A4C, 산호 빨강 #C2583E, 이끼 초록 #7E8F6E
- 예전 남보라 재화 바는 **폐기**. 스타일 참고로 쓰지 않음.
- **한 장에 부품 하나, 투명 배경, 글자·숫자 금지.** 결과물은 `pixl_test/assets/ui/gpt_out/` → Claude가 정리·조립.
