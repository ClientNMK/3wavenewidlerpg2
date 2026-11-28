# 타이틀 로고 — 소녀 태수 키우기 (가제)

확정: 2026-11-28. 시안 A(크림 글자 + 금선 두 겹). 제목은 가제라 바뀔 수 있음.

## 파일
| 파일 | 내용 |
|---|---|
| `title_logo.png` | 로고. 784×346, 투명 배경. "소녀 태수" + "키우기" + 양옆 금선·마름모가 한 장 |
| `preview_on_title.png` | 타이틀 배경(1080×1920) 위에 얹은 확인 그림 |
| `work/logo.html` | 원본. 글자만 바꿔 다시 뽑을 수 있음 |

## 붙이는 값
- 1080 폭 기준 그림. 720 캔버스에서는 약 523×231.
- 위치: 가로 가운데, 위에서 270px (1080×1920 기준) = 720×1280에서는 위에서 180.
- 임포트: Sprite (2D and UI), Bilinear, Compression None, sRGB 켬 (도트 아님, kit_cocoa와 같음).
- "화면을 터치하세요" 같은 글자는 로고에 없음.

## 색·글꼴
글자 `#FFF3DC`, 안쪽 선 `#2A1A14`, 바깥 금선 `#E6C27A`, 글꼴 DNFBitBitv2. 기준: `Plan/UI_Style_Guide.md`.

## 다시 뽑는 법
`work/logo.html`의 글자를 고친 뒤 Edge 헤드리스로 3배 스크린샷(`--default-background-color=00000000 --force-device-scale-factor=3 --window-size=360,220`)을 찍고 투명 여백을 자른다.
