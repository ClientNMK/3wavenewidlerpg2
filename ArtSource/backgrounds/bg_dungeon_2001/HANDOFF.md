# HANDOFF — 배경: 악몽 아이 방 (bg_dungeon_2001)

확정일: 2026-11-28. **이 폴더에는 확정본만 둡니다.** 짝 보스: `../../monsters/monster_dungeon_2001/`

## 한 줄 요약
악몽에 물든 밤의 아이 방. 가운데 둥근 러그(싸우는 곳), 위쪽 달빛 창, 왼쪽 침대(밑에서 빛나는 눈), 오른쪽 열린 옷장·수면등·곰인형·블록, 가장자리에서 보라 그림자 촉수가 스며듦. 스토리 '악몽은 소녀가 무서워했던 것에서 태어난다'에 맞춘 악몽 던전. 색은 차가운 보라·남색, 수면등만 따뜻한 호박색. 9:16 세로.

## 폴더 구성
```
ArtSource/backgrounds/bg_dungeon_2001/
├─ HANDOFF.md
├─ bg_dungeon_2001.png                  ← 게임용 1152×2048 (원본 3배, 위아래 8px 자름)
├─ source/
│  ├─ bg_dungeon_2001_384x688.png       ← 아래 빈칸을 메운 원본
│  └─ bg_dungeon_2001_raw_pro.png       ← PixelLab이 준 그대로 (600행 아래 흰 빈칸)
└─ preview/bg_dungeon_2001_screen_preview.png ← 왼쪽 전체(노란 틀 = 기본 줌 화면), 오른쪽 폰 화면
```
이름: 2026-11-28 `bg_nightmare_room` → `bg_dungeon_2001` (pixl_test 작업 폴더는 옛 이름 그대로).

## 엔진 연결 방법
- bg_world_2(가을 숲)와 같은 규격: Point 필터, 압축 없음, PPU 100, Max 2048 → 11.52×20.48칸.
- 어느 월드 번호로 쓸지는 기획에서 정함 (이름을 bg_world_N으로 바꾸면 이 문서도 함께 고칠 것).

## 만든 방법
`create_image_pro` 384×688 (25회). style_image = 옛 bg_world_2(모래 바닷가)를 512로 줄인 것, style_copy = outline·detail·shading. reference = bg_world_2(가을 숲) 384×688 ("layout only: open floor in the middle, props framing the edges, same camera angle and scale").
- PixelLab 결과가 384×600만 그려지고 아래 88행이 흰색이었음 → 512~599행을 위아래 뒤집어 600~687행에 붙여 메움(생성 0회). 그 부분은 기본 줌 화면 밖이거나 아래 HUD에 가려진다.

## PixelLab ID
- job `5fba6f3f-3696-4918-87e5-98f5a5b43b66` (asset `91ec6272-a04b-5492-b65a-a3aa5f9785a2`)
