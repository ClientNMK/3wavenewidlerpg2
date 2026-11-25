# HANDOFF — 몬스터: 슬라임 금색 (mob_slime_gold)

확정일: 2026-11-25. **원본 `mob_slime`(초록)의 색만 바꾼 버전입니다.** 모양·프레임 수·속도·정렬·엔진 연결 방법은 모두 `../mob_slime/HANDOFF.md`와 같습니다.

## 폴더 구성 (원본과 동일)
- `pose/mob_slime_gold_se.png`: 기본 자세, 96×96
- `animations/idle`, `move`, `attack`: 128×128, 바닥선 y=112, 중심 x=64
- `projectile/fly` (32×32, 4프레임 반복), `projectile/hit` (48×48, 7프레임)
- 구체 생성: attack frame_007, 입 위치 (90, 95)

## 만든 방법
PixelLab 생성 없이 로컬에서 색만 바꿈 (HSL 색상 회전). 해초(올리브색), 하얀 광택, 입 빨강은 그대로 두고 몸 색(초록 계열 밝은 톤 + 어두운 외곽선)만 회전.
