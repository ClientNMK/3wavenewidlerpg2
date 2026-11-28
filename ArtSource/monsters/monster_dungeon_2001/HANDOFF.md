# HANDOFF — 보스 몬스터: 뿔 달린 악몽 (monster_dungeon_2001)

확정일: 2026-11-28. **이 폴더에는 확정본만 둡니다.** 짝 배경: `../../backgrounds/bg_dungeon_2001/`

## 한 줄 요약
악몽 던전 보스. 덩치 큰 곱사등 몸에 진한 회보라 외투, 작은 머리에 하얀 외눈, 양옆으로 뻗어 끝이 말린 뼈 뿔(검은 실로 감고 바느질 인형 해골을 매달음), 크림색 붕대 팔, 짧은 다리, 가로로 든 긴 창. 키는 메인 캐릭터의 약 2배. 스토리 '큰 악몽(보스)'용 — 색은 악몽 규칙(차가운 보라·회색), 뼈·붕대만 밝아 어두운 배경에서도 보인다.

## 폴더 구성
```
ArtSource/monsters/monster_dungeon_2001/
├─ HANDOFF.md
├─ pose/
│  ├─ monster_dungeon_2001_se.png     ← 기본 자세, 오른쪽 반측면, 192×192
│  └─ keyframes/                     ← 모션 생성에 쓴 끝 자세 (attack 찌른 정점, skill 내리꽂은 끝)
├─ portrait/portrait_monster_dungeon_2001.png ← 도전 탭 보스 얼굴 (2026-11-28). 기본 자세 (57,34)부터 96×96을 2배(192×192) 최근접 확대. 원형 마스크는 엔진, Point 필터
├─ animations/                       ← 모두 192×192, 바닥선 y=176(발 175행), 중심 x=96
│  ├─ idle/    9장, 0.12초/프레임 반복 (어깨 오르내림, 해골 흔들림)
│  ├─ move/    9장, 0.10초/프레임 반복 (쿵쿵 제자리걸음, 이동은 엔진)
│  ├─ attack/  13장, 0.08초/프레임 1회 (웅크림 → 내딛으며 찌르기 → 복귀) ★ 타격 frame_008
│  ├─ skill/   10장, 0.09초/프레임 1회 (창을 머리 위로 → 앞 바닥에 내리꽂기) ★ 타격 frame_006, 끝나면 idle로
│  └─ dead/    9장, 0.10초/프레임 1회 (휘청 → 창 떨굼 → 무릎 → 엎어짐, 연기 없음)
├─ effects/                          ← skill(땅 찍기) 이펙트 확정본 (2026-11-28, pixelfx)
│  ├─ skill_slam/                    ← 페이즈1
│  │  ├─ ground_slam/  ground_slam.png/.json (칸 288×208, 49프레임 1.6초, pivot (144,164)), frames/, ground_slam_x1.gif, source/
│  │  ├─ earth_spike/  earth_spike.png/.json (칸 272×150, 55프레임 1.8초, pivot (136,132)), frames/, earth_spike_x1.gif, source/
│  │  └─ skill_slam_demo_x2.gif      ← 꽂는 자리 충격파 + 주인공 발밑 돌가시
│  └─ skill_slam_phase2/             ← 페이즈2: 8갈래로 뻗는 돌가시
│     ├─ spike_s1~s4/  spike_sN.png/.json (칸 272×150, 55프레임 1.8초, pivot (136,132)), frames/, spike_sN_x1.gif, source/ — 가시 높이 22·30·40·52px
│     └─ skill_slam_phase2_demo_x1.gif
└─ preview/
   ├─ monster_dungeon_2001_all_x2.gif ← idle×2 → move×2 → attack×2 → skill×2 → dead
   ├─ skill_with_idle_x2.gif         ← idle → skill → idle
   ├─ monster_dungeon_2001_design_steps_x3.png ← 시안 4번 / 중간판 / 확정 디자인
   └─ monster_dungeon_2001_on_room.png ← 악몽 방 위 폰 화면
```
각 동작 폴더에 `<동작>_strip.png`, `<동작>_preview_x2.gif`가 있다.
이름: 2026-11-28 `monster_boss_horned` → `monster_dungeon_2001` (pixl_test 작업 폴더는 옛 이름 그대로).

## 엔진 연결 방법
- **캔버스가 다른 캐릭터와 다르다**: 192×192, pivot **(96,176)** (일반 몬스터는 128×128, (64,112)). 같은 PPU면 그대로 2배 크기로 보인다.
- 왼쪽은 좌우 반전.
- attack 타격: frame_008(가장 멀리 찌른 순간). skill 타격: frame_006(창이 바닥에 닿는 순간).
- dead: 1회 재생 후 마지막 프레임(엎어진 몸)에서 엔진 페이드아웃 (보라 연기는 사용자 결정으로 뺌).

## skill 이펙트 (땅 찍기) 엔진 연결
이펙트는 모두 30fps 1회, 보스와 별도 스프라이트. 기준점 = **창이 꽂히는 자리 = 보스 pivot에서 앞(오른쪽)으로 50px, 바닥선** (192 캔버스 (146,176)). 왼쪽을 볼 때는 −50px.
- 시작 시각 = skill frame_006 (0.54초, 창이 바닥에 닿는 순간)

**페이즈1**
1. `skill_slam/ground_slam`: pivot (144,164)을 기준점에 맞춰 0.54초에 재생. 보스 **앞**에 그림 (먼지가 보스 발을 덮음)
2. `skill_slam/earth_spike`: pivot (136,132)을 **주인공 발 위치(바닥선)** 에 맞춰 0.48초에 재생 (가시는 시작 후 0.3초에 솟으므로 꽂고 약 0.24초 뒤 솟음). 타격 판정은 솟는 순간(이펙트 0.3초)

**페이즈2** (페이즈1의 ground_slam + 아래 8갈래, 페이즈1 earth_spike는 안 씀)
1. 기준점에서 8방향(0°, 45°, … 315°)으로 가시를 줄줄이 찍는다. k번째(k=1,2,3…) 가시 위치 = 기준점 + (cos θ, sin θ × **0.5**) × (18 + 40k) px. 세로 0.5는 탑뷰 시점이라 위·아래 간격을 줄인 것
2. k번째 가시 재생 시각 = 0.54초 + 0.07초 × k. 화면 밖으로 나가면 그 방향은 멈춤
3. 크기: k=1~2 `spike_s1`, 3~4 `spike_s2`, 5~6 `spike_s3`, 7부터 `spike_s4` (멀어질수록 커짐. 유니티 Scale로 키우지 않고 크기별 시트 사용)
4. 그리는 순서: 가시는 y가 작은(화면 위쪽) 것부터. 보스 발보다 위쪽 가시는 보스 뒤, 아래쪽 가시는 보스 앞
5. 데모: `effects/skill_slam_phase2/skill_slam_phase2_demo_x1.gif` (주인공은 크기 참고용)

**만든 방법**: pixelfx `impacts/ground-slam`·`nature/earth-spike`를 복사한 `boss/ground-slam`·`boss/earth-spike` (원본은 그대로). 색을 뼈 크림·회보라로, 길이를 절반으로(먼지·돌 수명과 단계 시간 단축). `boss/earth-spike`에 `debris`(부스러기 양 배율)·`crack`(바닥 갈라짐 길이 배율) 옵션 추가, 페이즈2는 debris 0.5·crack 0.5. 다시 렌더: pixelfx 폴더에서 `node tools/render.mjs boss/<이름> --sheet --no-actor --opts '<source/options.json 내용>'`

## 만든 방법 (디자인 결정 순서)
1. 레퍼런스(사용자 제공: 큰 뿔·외눈·해골·창의 덩치 큰 괴물)는 방향으로만 사용.
2. `create_image_pro` 128×128 시안 4개(10회) → 사용자가 **2번** 선택, 등은 **4번처럼** 요청.
3. `create_image_pro` 192×192, reference = 시안 2번(디자인)·4번(등)만 (10회) → 등이 너무 솟아 `edit_image_pro_flash`로 등 낮추고 왼팔 내림(6회) = 확정 디자인.
4. 192 캔버스 발 정렬(발 바닥 175행, 가로 중심 96) 후 `import_object`로 1방향 오브젝트화.
5. idle·move·dead: `animate_object` v3 (각 5회). dead는 연기 없이 다시 뽑음.
6. attack·skill: 끝 자세를 `edit_image_pro_flash`로 먼저 그림(각 6회) → `animate_image`(first=기본 자세, last=끝 자세, 8프레임, 각 5회). attack은 끝에 6·4·2·0 프레임을 거꾸로 붙여 복귀, skill은 마지막 프레임을 한 번 더 붙여 멈춤.

## PixelLab ID
| 이름 | ID | 비고 |
|---|---|---|
| 시안 4개 | job `6d8d3e9d-91ff-4de8-9d8b-310e20877f88` | create_image_pro 128, 2번 선택 |
| 192 디자인 | job `d61774fb-5b42-434e-b1c9-f16b76145428` → 편집 job `83f6e4c1-4e57-404c-b7a1-4245061523ac` | 편집본이 확정 |
| 오브젝트 | `e4b5e53e-4a8e-404c-a3fa-ab229738370e` | import_object (1방향, 192) |
| idle | group `9469e73b-be01-455d-9897-649d54561e04` | |
| move | group `2a56a038-e57c-468f-9d84-e8e886852029` | |
| dead | group `c31c8b96-a769-4abf-a06b-a3ec26a73e99` | 연기 없는 판 → 확정 |
| attack | 끝 자세 job `a9ec50ef-643a-4f5f-9096-1c19ea0d59c9`, 모션 job `3e6c04a7-bb07-4283-a5fb-d9bef42685c4` | animate_image |
| skill | 끝 자세 job `5cf316f6-980f-47ef-a0e7-5454d62903c0`, 모션 job `778fdbe3-0d58-40ee-a403-49ce5704f4c2` | animate_image |
| 탈락 | attack group `de5b55f5-…`(창 뒤집힘), `4a7af126-…`(찌르기 작음), dead group `ed5c05db-…`(보라 연기) | pixl_test에 `_rejected`로 보관 |
