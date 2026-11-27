# HANDOFF — 몬스터: 단풍 버섯 파랑 (monster_stage_002_blue)

확정일: 2026-11-27. **원본 `monster_stage_002`(주황)의 갓 색만 바꾼 버전입니다.** 모양·프레임 수·속도·정렬·엔진 연결 방법은 모두 `../monster_stage_002/HANDOFF.md`와 같습니다.

## 폴더 구성 (원본과 동일)
- `pose/monster_stage_002_blue_se.png`: 기본 자세, 96×96
- `animations/idle`, `move`, `dead`: 128×128, 바닥선 y=112, 중심 x=64 (공격 모션 없음)
- `preview/monster_stage_002_blue_all_x3.gif`: idle×2 → move×2 → dead

## 만든 방법
PixelLab 생성 없이 로컬 색상 회전 (+175°, 채도 ×0.85, 밝기 0). 주황 갓만 바꾸고 단풍잎·크림 몸통·반점·포자·외곽선은 그대로. 도구: `pixl_test/assets/monster/monster_stage_002/work/tools/MushroomRecolor.cs`