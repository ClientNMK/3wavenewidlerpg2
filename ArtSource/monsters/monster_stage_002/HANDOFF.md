# HANDOFF — 몬스터: 단풍 버섯 (monster_stage_002)

확정일: 2026-11-27. **이 폴더에는 확정본만 둡니다.** 짝 배경: `../../backgrounds/bg_world_2/`

## 한 줄 요약
주황 갓에 크림 반점, 갓 위에 단풍잎 한 장. 통통한 크림색 몸통에 작은 발, 점 눈과 뾰로통한 입. 키는 슬라임과 비슷(메인 캐릭터의 절반쯤). **공격 모션 없음** (사용자 결정 2026-11-27: 몬스터 attack은 모두 뺌).

## 폴더 구성
```
ArtSource/monsters/monster_stage_002/
├─ HANDOFF.md
├─ pose/monster_stage_002_se.png     ← 기본 자세, 오른쪽 반측면, 96×96
├─ animations/                       ← 모두 128×128, 바닥선 y=112, 중심 x=64
│  ├─ idle/  frames 9장, 0.12초/프레임 반복 (살짝 눌렸다 늘어남)
│  ├─ move/  frames 9장, 0.08초/프레임 반복 (제자리 통 튀기, 이동은 엔진)
│  └─ dead/  frames 9장, 0.10초/프레임 1회 (납작 → 포자 연기 → 단풍잎만 남음)
└─ preview/
   ├─ monster_stage_002_all_x3.gif   ← idle×2 → move×2 → dead
   ├─ monster_stage_002_scale_x3.png ← 메인 캐릭터·슬라임 옆 크기
   ├─ monster_stage_002_skins_x3.png ← 주황(원본)·빨강·보라·파랑 비교
   └─ monster_stage_002_on_forest.png ← 숲 배경 위 폰 화면
```
이름: 2026-11-27 `mob_mushroom` → `monster_stage_002` (게임의 monster_stage_001과 같은 규칙).

## 색 바꾼 버전
`../monster_stage_002_red`, `../monster_stage_002_purple`, `../monster_stage_002_blue` — 모든 프레임에서 주황 갓만 HSL 색상 회전 (빨강 −22°, 보라 +255°, 파랑 +175°). 단풍잎·몸통·포자는 그대로. 도구 `pixl_test/assets/monster/monster_stage_002/work/tools/MushroomRecolor.cs` (갓 후보 픽셀 중 큰 덩어리만 바꿈 → move에서 밝아지는 잎은 안 바뀜).
각 동작 폴더에 `<동작>_strip.png`(가로 스트립), `<동작>_preview_x3.gif`가 있다.

## 엔진 연결 방법
- pivot (64,112), 왼쪽은 좌우 반전 (슬라임과 같음).
- dead: 1회 재생 후 마지막 프레임(바닥의 단풍잎)에서 멈추거나 페이드아웃. 4번째 프레임은 외곽선 없는 포자 덩어리라 급하게 바뀌어 보일 수 있음(통과).

## PixelLab ID
| 이름 | ID | 비고 |
|---|---|---|
| 오브젝트 | `d59444f4-21c3-4863-850f-92904dc6c33c` | create_object_pro_flash 96×96, 8방향(7회), style_image = 슬라임 pose 96px(색 복사 안 함) |
| idle | group `9e62cd3e-68d6-42c0-ba06-c2d43927a1d3` | animate_object v3, south-east 8프레임(+시작 1) |
| move | group `203693d0-adc3-414f-ab0f-1a880e1640ff` | 〃 |
| dead | group `58d377e3-15a3-4f30-9e94-1a082e9b1c44` | 〃, end_frame = 첫 dead의 마지막 프레임에서 잎만 남긴 것 → **확정** |
| └ dead (탈락) | group `d46b27cb-8fd1-467c-a4fe-ee056910ce3b` | 잎만 떨어지고 몸이 안 사라짐 |

정렬: 96×96 원본을 128×128에 (dx=18, dy=38)로 배치 (발 바닥 73행 → 111행).

오브젝트 설명: "a cute small autumn forest mushroom monster for an idle RPG: a chubby round creature whose body is a plump cream stem with two tiny stubby feet, wearing a big round orange-amber mushroom cap with cream spots, a fallen maple leaf stuck on top of the cap, two dot eyes and a small grumpy mouth on the stem, thick dark outline, chunky pixel art"
