# HANDOFF — 몬스터: 따개비 고블린 (mob_barnacle_goblin)

확정일: 2026-11-25. **이 폴더에는 확정본만 둡니다.**

## 한 줄 요약
해변 고블린 몬스터. 청록 피부, 뾰족 귀, 노란 눈, 등에 따개비 붙은 소라껍데기, 해초 허리옷, 유목 몽둥이. 메인 캐릭터(소라게 후드 소녀)와 같은 화풍.

## 폴더 구성
```
ArtSource/monsters/mob_barnacle_goblin/
├─ HANDOFF.md
├─ pose/mob_barnacle_goblin_se.png   ← 기본 자세, 오른쪽 반측면, 96×96
└─ animations/
   ├─ idle/    frames/frame_000~008.png, idle_strip.png, idle_x1.gif (0.12초), idle_preview_x3.gif
   └─ attack/  frames/frame_000~008.png, attack_strip.png, attack_x1.gif (0.08초), attack_preview_x3.gif
```

## 규칙 (메인 캐릭터와 동일)
- **오른쪽 반측면(south-east) 한 방향만** 만든다. 왼쪽을 볼 때는 엔진에서 좌우 반전. 모든 몬스터·캐릭터에 적용
- 128×128 캔버스, 발바닥 111행(바닥선 y=112), 가로 중심 x=64. 엔진 pivot (64,112)
- frame_000 = 시작 자세(기본 자세와 같음), 마지막 프레임은 시작 자세로 복귀

## PixelLab ID
| 이름 | ID | 비고 |
|---|---|---|
| 캐릭터 | `05532ed6-b941-403b-a189-72ff7576f92d` | pro 모드, `style_character_id` = 메인 캐릭터 `91ce750a-…`, 96px |
| idle | group `67bac0fa-a71e-476a-8035-f424cc44a84d` | v3 custom 8프레임(+시작 1), end_frame = south-east 회전 이미지. 정렬 (dx=14, dy=14) |
| attack | group `a19b1879-51b2-4d0e-90f3-f39e72454278` | v3 custom 8프레임(+시작 1), end_frame = south-east 회전 이미지. 정렬 (dx=14, dy=12) |

## 새 몬스터 만드는 방법 (이 고블린 기준)
1. `create_character` mode=pro, `style_character_id`=메인 캐릭터 ID, size 96 → 디자인 확인받기 (15회)
2. south-east 회전 이미지를 업로드해 `end_frame_url`로 넣고, v3 custom으로 idle·attack 생성 (각 2회)
3. 128×128에 발바닥 111행, 머리 중심 x=64로 정렬 → GIF로 확인받기 → 확정본만 커밋

## 알려진 점
- idle 가운데 프레임에서 고개가 살짝 앞으로 돌고 눈을 감음 (숨쉬는 느낌으로 통과)
- attack은 내려찍기가 아니라 앞으로 휘두르는 동작이고, 몽둥이 끝에 노란 타격 효과가 조금 그려짐
