# HANDOFF — 소라게 후드 소녀 메인 캐릭터 (방치형 RPG)

다른 세션(클라우드 포함)이 작업을 이어받기 위한 인수인계 메모입니다. 작성일: 2026-11-25
**이 폴더에는 확정본만 둡니다.** 후보와 이전 버전은 올리지 않으며, 필요하면 아래 PixelLab ID로 다시 받습니다.

## 1. 한 줄 요약
방치형 RPG 메인 캐릭터 "소라게 후드 소녀". 기본형(후드 없음)을 PixelLab MCP로 만들고, 보스를 잡으면 나오는 후드를 장착하는 식으로 단계별 후드 버전(0차, 1차…)을 추가한다.

## 2. 이 폴더 구성
```
ArtSource/girl_base/
├─ HANDOFF.md                         ← 이 문서
├─ concept/shellgirl_concept_sheet.png ← 콘셉트 시트 (확정)
├─ pose/girl_base_se_p2.png           ← 기본 포즈 p2, 96×96 (확정)
├─ pose/girl_base_se_p2_x3.png        ← 3배 확대 미리보기
├─ animations/idle/                   ← idle 확정본 (2026-11-25)
│  ├─ frames/frame_000~007.png        ← 128×128, 바닥선 y=112, 중심 x=64, 투명 배경
│  ├─ idle_strip.png                  ← 가로 스트립 (칸 128px, 8칸)
│  ├─ idle_x1.gif                     ← 원본 크기, 투명, 0.12초/프레임
│  └─ idle_preview_x3.gif             ← 확인용 3배, 베이지 배경 + 빨간 바닥선
├─ animations/run/                    ← run 확정본 (2026-11-25)
│  ├─ frames/frame_000~005.png        ← 128×128, 바닥선 y=112, 중심 x=64, 투명 배경
│  ├─ run_strip.png                   ← 가로 스트립 (칸 128px, 6칸)
│  ├─ run_x1.gif                      ← 원본 크기, 투명, 0.08초/프레임 (약 12fps)
│  └─ run_preview_x3.gif              ← 확인용 3배, 베이지 배경 + 빨간 바닥선
├─ animations/attack/                 ← attack 확정본 (2026-11-26 역동적인 버전으로 교체)
│  ├─ frames/frame_000~010.png        ← 128×128, 바닥선 y=112, 중심 x=64, 투명 배경 (000 = 시작 자세)
│  ├─ attack_strip.png                ← 가로 스트립 (칸 128px, 11칸)
│  ├─ attack_x1.gif                   ← 원본 크기, 투명, 0.07초/프레임
│  └─ attack_preview_x3.gif           ← 확인용 3배, 베이지 배경 + 빨간 바닥선
├─ animations/hurt/                   ← hurt(피격) 확정본 (2026-11-25)
│  ├─ frames/frame_000~008.png        ← 128×128, 바닥선 y=112, 중심 x=64, 투명 배경 (000 = 시작 자세)
│  ├─ hurt_strip.png                  ← 가로 스트립 (칸 128px, 9칸)
│  ├─ hurt_x1.gif                     ← 원본 크기, 투명, 0.08초/프레임
│  └─ hurt_preview_x3.gif             ← 확인용 3배, 베이지 배경 + 빨간 바닥선
├─ animations/dash/                   ← 대시 확정본 (2026-11-26)
│  ├─ frames/frame_000~008.png        ← 128×128, 바닥선 y=112, 투명 배경 (000·008 = 서 있는 자세)
│  ├─ dash_strip.png                  ← 가로 스트립 (칸 128px, 9칸)
│  ├─ dash_x1.gif / dash_preview_x3.gif ← 제자리, 0.06초/프레임
│  └─ dash_move_demo_x2.gif           ← 엔진 이동 + 잔상 예시 (게임에서 보일 모습)
├─ animations/roll/                   ← 구르기 확정본 (2026-11-26)
│  ├─ frames/frame_000~006.png        ← 128×128, 바닥선 y=112, 투명 배경 (000·006 = idle frame_000과 같은 서 있는 자세)
│  ├─ roll_strip.png                  ← 가로 스트립 (칸 128px, 7칸)
│  ├─ roll_x1.gif / roll_preview_x3.gif ← 0.07초/프레임
│  └─ source/roll_ball_pose.png       ← 웅크린 자세 원본 (96×96, 회전 재료)
├─ animations/attack2/                ← 두 번째 공격(콤보) 확정본 (2026-11-26)
│  ├─ frames/frame_000~010.png        ← 128×128, 바닥선 y=112, 투명 배경 (000·010 = 시작 자세)
│  ├─ attack2_strip.png               ← 가로 스트립 (칸 128px, 11칸)
│  └─ attack2_x1.gif / attack2_preview_x3.gif ← 0.07초/프레임
├─ animations/attack3/                ← 세 번째 공격(내려찍기, 오른손 단검만) 확정본 (2026-11-26)
│  ├─ frames/frame_000~010.png        ← 128×128, 바닥선 y=112, 투명 배경 (000·010 = 시작 자세)
│  ├─ attack3_strip.png               ← 가로 스트립 (칸 128px, 11칸)
│  └─ attack3_x1.gif / attack3_preview_x3.gif ← 0.07초/프레임
├─ animations/dead/                   ← 사망 확정본 (2026-11-26)
│  ├─ frames/frame_000~010.png        ← 128×128, 바닥선 y=112, 투명 배경 (000 = 서 있는 자세, 010 = 엎드려 쓰러진 마지막 자세)
│  ├─ dead_strip.png                  ← 가로 스트립 (칸 128px, 11칸)
│  └─ dead_x1.gif / dead_preview_x3.gif ← 0.1초/프레임 (preview는 마지막 자세 1초 유지)
├─ animations/vanish/                 ← 스킬: 점으로 흩어져 사라짐 확정본 (2026-11-26, 스크립트 제작)
│  ├─ frames/frame_000~013.png        ← 128×128 (000 = idle frame_000, 013 = 빈 그림)
│  ├─ vanish_strip.png, vanish_x1.gif, vanish_preview_x3.gif ← 0.06초/프레임
│  ├─ skill_vanish_appear_demo_x2.gif ← 사라짐 → 적 뒤 등장 → 공격 연출 예시
│  └─ source/Dissolve.cs              ← 만든 스크립트 (다른 자세·색으로 다시 만들 때)
├─ animations/appear/                 ← 스킬: 점이 모여 나타남 (= vanish 역순) 확정본 (2026-11-26)
│  └─ frames/frame_000~013.png, appear_strip.png, appear_x1.gif, appear_preview_x3.gif ← 0.06초/프레임, 013 = idle 자세
├─ animations/backstab/               ← 스킬: 적 뒤에서 나타나 적을 바라보고 가로로 휘두르기 확정본 (2026-11-26 최종)
│  ├─ frames/frame_000~010.png        ← 128×128, 000~003 = 점이 모이며 나타남(등장 연출 포함), 004~007 = 가로 휘두르기, 010 = idle 자세
│  ├─ backstab_strip.png, backstab_x1.gif, backstab_preview_x3.gif ← 0.07초/프레임
│  ├─ skill_vanish_backstab_demo_x2.gif ← vanish → 적 뒤 → (flipX로 적 바라봄) backstab 전체 예시
│  └─ source/plain_frames/            ← 나타남 효과 없는 순수 휘두르기 프레임 (다른 용도용)
├─ ENGINE_REQUEST.md                  ← 엔진 적용 요청서 (프로그래머 전달용: pivot·타이밍·이펙트·스킬 순서)
├─ effects/attack2_smear/             ← attack2 흰(크림) 잔상 이펙트 확정본 (2026-11-26, 원본 궤적을 그대로 떼어냄)
│  ├─ frames/frame_000~010.png        ← 128×128, attack2 프레임과 1:1 짝 (잔상은 002~005, 나머지는 빈 프레임)
│  ├─ attack2_smear_strip.png, attack2_smear_x1.gif ← 0.07초/프레임
│  └─ attack2_with_smear_preview_x3.gif ← 잔상 지운 attack2 + 이 이펙트 겹친 확인용
├─ effects/attack_slash/              ← 평타 베기 이펙트 확정본 (2026-11-26 attack2 안 잔상과 같은 크림색 단발 베기로 교체, pixelfx)
│  ├─ dagger-slash.png / .json        ← 스프라이트 시트 (칸 220×140, 4열, 19프레임) + Aseprite식 JSON (meta.pixelfx에 fps·pivot)
│  ├─ frames/frame_000~018.png        ← 같은 프레임 낱장, 투명 배경 (빛 번짐은 반투명 픽셀로 들어 있음)
│  ├─ attack_slash_x1.gif             ← 이펙트만, 원본 크기 (GIF라 반투명 빛은 빠짐)
│  ├─ attack_with_slash_preview_x3.gif ← 공격 애니메이션 + 이펙트 겹친 확인용
│  ├─ attack_impact_demo_x3.gif       ← 평타 전체 연출 예시 (베기 + 슬라임 타격 불꽃 + 히트스톱·번쩍임·밀림·흔들림)
│  └─ source/dagger-slash.js, options.json ← pixelfx 이펙트 코드와 이 버전 옵션 (다시 렌더할 때 사용)
├─ effects/attack3_slash/             ← attack3(내려찍기) 크림 이펙트 확정본 (2026-11-26, pixelfx dagger-slash)
│  ├─ dagger-slash.png / .json        ← 칸 220×140, 4열, 19프레임, 30fps, pivot (40, 128)
│  ├─ frames/frame_000~018.png, attack3_slash_x1.gif
│  ├─ attack3_impact_demo_x3.gif      ← attack3 + 이펙트 + 타격 연출 예시
│  └─ source/dagger-slash.js, options.json
├─ effects/hit_spark/                 ← 타격 불꽃 확정본 (2026-11-26 1.5배로 키움, 주황·금색 그대로, pixelfx)
│  ├─ hit-spark.png / .json           ← 스프라이트 시트 (칸 220×140, 4열, 19프레임, 30fps, pivot (70, 120))
│  ├─ frames/frame_000~018.png, hit_spark_x1.gif
│  └─ source/hit-spark-big.js, options.json ← 크기 조절용 복사본 코드 + 옵션 (size 1.5, hold 1.6, height 32, sparkN 13, chipN 8)
├─ effects/skill_xslash/              ← 스킬용 X자 베기 이펙트 확정본 (2026-11-26, pixelfx double-slash를 키운 복사본, 크림색)
│  ├─ xslash.png / .json              ← 스프라이트 시트 (칸 240×160, 5열, 25프레임, 30fps, pivot (40, 148))
│  ├─ frames/frame_000~024.png, xslash_x1.gif
│  ├─ xslash_impact_demo_x3.gif       ← 평타 동작에 겹친 예시 (어느 스킬 동작에 붙일지는 미정)
│  ├─ source/double-slash-big.js, _lib.js, options.json
│  └─ backstab/                       ← backstab용 (교차점을 캐릭터 가까이 49px로, flipX로 뒤쪽에 사용): xslash_backstab.png/.json, frames/, options.json, backstab_xslash_demo_x3.gif
└─ tools/GifMaker.cs                  ← 확인용 GIF 인코더 (PowerShell Add-Type로 사용)
```
Unity가 자동으로 가져가지 않도록 `Assets` 밖에 둔다. 게임에 넣을 최종 스프라이트는 확정 후 별도로 옮긴다.

## 3. 사용자 작업 방식 (꼭 지킬 것)
- **단계마다 결과 이미지를 보여주고 확인을 받은 뒤 다음으로 넘어간다.**
- 결과물은 **GIF로 보여준다.** (HTML 미리보기는 환경에 따라 스크립트가 막혀 안 보일 수 있음) `tools/GifMaker.cs` 사용 예:
  `Add-Type -Path tools/GifMaker.cs -ReferencedAssemblies System.Drawing; [GifMaker]::Make($framePaths, 'out.gif', 3, 12, '#ebe4d6', 104)` (배율 3, 0.12초, 배경색, 바닥선 행 / 바닥선 없으면 -1)
- 파일을 저장하면 **전체 경로를 항상 알려준다.**
- 한국어로 짧게, 다음 행동을 먼저, 단계는 번호로 (사용자는 ADHD).
- **확정된 것만 저장소에 커밋한다.**

## 4. 확정된 결정 사항
| 항목 | 결정 |
|---|---|
| 카메라 | 탑뷰 (PixelLab view: low top-down) |
| 사용 방향 | **오른쪽 반측면(south-east) 한 방향만.** 왼쪽은 좌우 반전. 정면·뒷모습 불필요 (팀 피드백: 리소스량 절감). **모든 몬스터·캐릭터도 같은 각도만** 만든다 (몬스터: `ArtSource/monsters/mob_slime/` 초록 원본 + 색만 바꾼 `mob_slime_blue`·`mob_slime_pink`·`mob_slime_gold`. 만드는 방법은 각 폴더 HANDOFF 참고). 아이템: `ArtSource/items/item_gold_coin/` (골드 동전, 위에서 보는 한 방향) |
| 이동 | 화면 가로로 일직선 이동. **바닥은 평지(수평선)**. 발 배치가 대각선이면 안 됨 |
| 기본 포즈 | **p2**: 반측면 상체 + 두 다리 평행, 두 발이 같은 수평선 → `pose/girl_base_se_p2.png` |
| 디자인 | 웨이브 애쉬 크림 머리, 금빛 눈, 진갈색 원피스+금단추, 금방울 달린 큰 빨간 리본, 크림 프릴, 갈색 부츠 |
| 단검 | 낮게 역수로 쥐고 칼날은 아래·뒤로, 앞쪽 손 |
| idle 규칙 | **다리는 절대 움직이지 않고 상체·옷·머리카락만 흔들림** |
| run 규칙 | 몸·다리는 옆모습(east) 달리기, **얼굴은 idle과 같은 반측면 각도로 고정**(고개 좌우로 돌리지 않음), 머리카락은 흔들림 |
| attack 규칙 | 반측면 그대로 유지(정면으로 돌지 않음), 역수 단검 한 자루, 끝 프레임은 시작 자세로 복귀 |
| hurt 규칙 | 앞에서 맞고 상체·머리가 뒤로 젖혀졌다 복귀. 반측면 유지, 발은 바닥에 붙음, 역수 단검 유지, 끝 프레임은 시작 자세. 빨간 피격 플래시는 엔진에서 처리 |
| 애니메이션 속도 | idle 0.12초/프레임, run 0.08초/프레임, attack 0.07초/프레임, attack2 0.07초/프레임, attack3 0.07초/프레임, hurt 0.08초/프레임, dead 0.1초/프레임, vanish·appear 0.06초/프레임 (엔진에서 조절). 전체 정리는 `ENGINE_REQUEST.md` |
| 캔버스 정렬 | 모든 애니메이션을 128×128에 놓고 발바닥 바닥선 y=112, 가로 중심 x=64. 엔진 pivot (64,112) |

## 5. PixelLab ID
| 이름 | character_id | 비고 |
|---|---|---|
| **girl_base_se_p2 (현재)** | `91ce750a-fd6e-4cbd-9898-4e0fe19fc089` | south-east = p2 포즈. 8방향 중 south-east만 사용 |
| └ idle_a | group `3bc7cb70-8841-4595-9a13-09113d618925` | v3 custom 8프레임 (124×124). 다리 고정 후처리 + 정렬(dx=4, dy=8) → **확정** (`animations/idle/`) |
| └ idle_b | group `96a5b540-90ec-40f2-ab8d-e576ff1ee375` | 다리 더 많이 움직임 (탈락) |
| └ run | group `2be876c6-2c45-4e03-9b76-e1d3c3bff920` | skeleton-v3 `running-6-frames`. south-east는 다리가 대각선이라 탈락 → **east 방향** 사용 → 얼굴 수정 + 얼굴 고정 후처리 → **확정** (`animations/run/`) |
| └ attack_dynamic_lunge | group `67be4926-2e7a-4bd9-a1a3-b0430730b952` | v3 custom 10프레임(+시작 1) south-east, end_frame = south-east 회전 이미지, 크게 당기기→내딛기→휘두르기 → 흩날린 조각 제거 + 정렬 → **확정** (`animations/attack/`, 2026-11-26) |
| └ attack_dynamic_leap | group `04cc30ec-f0e8-466c-a2d3-b3566cacc958` | 뛰어올라 내려찍기, 중간에 몸이 정면으로 돎 (탈락) |
| └ attack_slash_v2 | group `aa3ac889-d310-46d1-893b-e0fd12b3afd4` | 이전 확정본(동작이 작음) → 2026-11-26 attack_dynamic_lunge로 교체 |
| └ attack_slash (v1) | group `288f51cd-2880-4145-9699-48e9f041c15c` | 후반에 몸이 정면으로 돎 (탈락) |
| └ attack_slash_east | group `263ce08b-d01f-41a8-86e7-56ceed5fcaa5` | 칼 두 자루로 보임 (탈락) |
| └ hurt | group `1857509b-8246-48b1-b085-faa697b8e49d` | v3 custom 8프레임(+시작 1) south-east, end_frame = south-east 회전 이미지 → 정렬만 → **확정** (`animations/hurt/`) |

다운로드: `https://api.pixellab.ai/mcp/characters/<character_id>/download` (ZIP), GIF: `export_asset(format="gif", animation_group_id=..., direction="south-east")`

## 6. 현재 상태와 다음 할 일
**완료:** idle 확정 (2026-11-25) → `animations/idle/`
- 만든 방법: idle_a 프레임의 다리·부츠 영역(124×124 기준 y=93~103행)을 frame 0으로 덮어써 다리를 고정 → 128×128 캔버스에 (dx=4, dy=8)로 배치해 발바닥이 111행(바닥선 112 바로 위)에 오게 함
- 다른 애니메이션도 같은 규칙으로 정렬해야 idle과 전환할 때 캐릭터가 튀지 않음

**완료:** run 확정 (2026-11-25) → `animations/run/`
- 만든 방법:
  1. run 그룹의 **east** 방향 6프레임 사용 (south-east는 다리가 대각선)
  2. `edit_animation`(seed 4242)으로 얼굴을 idle 반측면 각도로 수정
  3. 기준 프레임(2번째)에서 피부·눈·입·볼 색만 골라 얼굴 마스크(244px)를 만들고, 6프레임 모두 그 얼굴로 덮음 → 고개가 돌아가지 않고 머리카락만 흔들림
  4. 128×128에 (dx=−3, dy=6)로 배치, 발바닥 111행
- 알려진 작은 흠: 마지막 2프레임에 얼굴 왼쪽 아래 원래 볼 픽셀 1~2개가 남음 (원래 크기에선 거의 안 보임)

**완료:** attack 확정 (2026-11-26, 역동적인 버전으로 교체) → `animations/attack/`
- 동작: 몸을 낮추며 단검을 뒤로 크게 당김 → 앞발을 크게 내디딤 → 단검을 머리 높이까지 올렸다가 앞으로 휘둘러 내림(frame_007~008) → 시작 자세로 복귀. 11프레임, 0.07초/프레임(0.77초)
- 만든 방법:
  1. v3 custom, south-east, **10프레임**, `end_frame_url` = south-east 회전 이미지. 설명: "dynamic dagger attack: big wind-up pulling the dagger hand far back behind her with the body twisted and crouched low, then an explosive lunge forward with a long step of the front foot, the arm whipping the reverse-grip dagger in a wide fast slash across the front, hair and skirt flying with the motion, strong follow-through, then she pulls back to the starting pose; stays facing right in three-quarter view, body does not turn to the front, only one dagger in reverse grip" (2회)
  2. 칼날 겹침 없음 확인. frame_007~008에 몸과 떨어져 흩날린 크림색 조각(40px 미만 덩어리)을 지움
  3. 136×144 원본을 128×128에 (dx=0, dy=−2)로 배치, 발바닥 111행, 가로 중심을 idle frame_000과 맞춤
- 알려진 점: frame_008에서 단검 끝이 바닥선보다 2~3px 아래로 내려감
- 이전 버전(작은 베기, 9프레임 0.08초)은 git 기록과 PixelLab group `aa3ac889-…`에 남아 있음

**완료:** hurt(피격) 확정 (2026-11-25) → `animations/hurt/`
- 만든 방법:
  1. v3 custom, south-east, 8프레임. `end_frame_url`에 south-east 회전 이미지. 설명: "getting hit and hurt: flinches backward from an impact in front, head and upper body jerk back, eyes squeeze shut... Feet stay planted on the ground, stays facing right in three-quarter view, body does not turn to the front, still holding the dagger low in reverse grip"
  2. 96×124 원본을 128×128에 (dx=18, dy=8)로 배치, 발바닥 111행 (모든 프레임 발바닥 행 동일). 후처리 없음
- 알려진 점: 젖혀지는 정도가 작은 편. 가운데 프레임(003~005)에서 다리가 살짝 벌어짐(발은 바닥에 붙어 있음)

**완료:** 평타 베기 이펙트 확정 (2026-11-25 흰색 X자 → 금·주황 X자 → **2026-11-26 크림색 단발 베기로 최종 교체**) → `effects/attack_slash/`
- 이유: attack2 애니메이션 안에 그려진 잔상(외곽선 없는 크림색 #FDF2E8 한 색의 반달)과 이질감이 없도록 같은 그림체로 맞춤
- 모양: 캐릭터 앞에서 크림색 반달 하나가 빠르게 지나가고 꼬리부터 가늘어지며 사라짐. 외곽선·불꽃·조각·섬광·링·빛 번짐·점선 자국 없음
- 이 버전 옵션(`source/options.json`): radius 42, width 10, sy 0.75, cx 28, cy −36, a0 −1.9, second 0, sparkN 0, moteN 0, glint 0, flareL 0, ringR 0, trace 0, echo 0, light 0, palette `#e9d8c6 #f2e4d6 #f8ece0 #fdf2e8 ×4` (`trace`·`echo`는 이번에 추가한 옵션)
- 이전 금·주황 큰 X자 버전 옵션(스킬용으로 다시 쓸 수 있음): radius 62, width 27, sy 0.95, cx 46, cy −46, second 1, flareL 23, ringR 21, sparkN 30, moteN 36, glint 17, light 0.15, palette `#4a1712 #c9281f #f2561d #ff8a2a #ffe26b #fff3a6 #ffffff`
- 타격 불꽃(`effects/hit_spark`): 색·모양은 pixelfx `impacts/hit-spark` 기본 주황·금색 그대로(사용자 지시로 색 변경 금지). 2026-11-26 타격감을 위해 **1.5배로 키움**: 원본은 두고 크기 조절 복사본 `impacts/hit-spark-big`을 새로 만들어 size 1.5(십자 섬광·다이아몬드·빛 번짐), hold 1.6(섬광이 화면에 남는 시간, 원본은 30fps에서 1프레임뿐이라 잘 안 보였음), 불꽃 9→13개·조각 6→8개
- **평타 전체 연출 (엔진에서 구현, 수치는 데모 `attack_impact_demo_x3.gif` 기준, 자유롭게 조절):**
  1. 베기 이펙트: attack 시작 후 0.49초에 재생 (아래 엔진 연결 참고)
  2. 타격 불꽃: 베기 시작 후 0.05초(attack 시작 후 0.54초)에 **몬스터 앞면 바닥**에 재생. `hit-spark` 프레임의 pivot **(70, 120)** 을 맞은 지점 바로 아래 바닥에 둔다 (섬광은 pivot 위 32px, 몬스터 몸 가운데쯤). 데모에서는 슬라임 pivot에서 왼쪽 19px
  3. 히트스톱: 맞는 순간 캐릭터·몬스터·이펙트 모두 **0.1초** 정지 (타격 불꽃은 첫 프레임으로 멈춤)
  4. 피격 번쩍임: 정지 동안 + 0.07초 몬스터를 **검은색 실루엣**으로 그림 (사용자 선택. 흰색은 크림색 베기와 묻히고, 빨간 틴트는 후보였음). 유니티에서는 몬스터 스프라이트 색을 검정으로 덮는 셰이더/머티리얼(또는 SpriteRenderer.color = black + 알파 유지)로 구현, 이미지 파일 추가 없음
  5. 밀림: 몬스터를 **5px** 뒤로 밀었다가 0.25초에 걸쳐 원위치 (정지 중엔 ±1px 떨림)
  6. 화면 흔들림: **2px**, 정지 시간 + 0.15초 동안 줄어들며
  7. 크기를 바꾸고 싶으면 유니티 Scale(정수 아닌 배율은 픽셀이 뭉개짐) 대신 pixelfx에서 다시 렌더
- **엔진 연결:**
  1. 이펙트 프레임(220×140)의 pivot **(40, 128)** 을 캐릭터 pivot **(64, 112)** 에 맞춘다 (캐릭터 128 캔버스 기준 이펙트 왼쪽 위 = (24, −16))
  2. attack 시작 후 **0.49초(frame_007 시작, 휘두르는 순간)** 에 이펙트 재생, **30fps**, 19프레임, 1회 (2026-11-26 새 attack 기준. 예전 작은 attack은 0.24초였음)
  3. 타격 순간(히트스톱·데미지 숫자)은 이펙트 시작 후 약 0.05초
  4. 캐릭터 앞쪽(오른쪽)으로 캐릭터 칸 밖까지 나가므로 캐릭터와 별도 스프라이트로 그린다. 왼쪽을 볼 때는 캐릭터와 함께 좌우 반전
  5. 빛 번짐은 반투명이라 어두운 배경에서 더 잘 보인다. 밝은 배경이면 `options.json`의 `light`를 0으로 다시 렌더
- 만든 방법: pixelfx(`C:\Users\HP\Documents\Claude\pixelfx`, github.com/patch-gg/pixelfx)의 `melee/sword-slash`를 바탕으로 `melee/dagger-slash`를 새로 작성 (위치·크기·호 모양·X자 두 번째 베기·섬광·링 옵션 추가). 다시 렌더: pixelfx 폴더에서 `node tools/render.mjs melee/dagger-slash --sheet --opts "<options.json 내용>"` (PowerShell 5.1에서는 따옴표를 `\"`로 이스케이프)

**완료:** 구르기(roll) 확정 (2026-11-26) → `animations/roll/`
- 순서: 서 있기 → 웅크림 → 90° → 180° → 270° → 웅크림 → 서 있기 (오른쪽으로 앞구르기 = 시계 방향), 7프레임, 0.07초/프레임. 제자리 동작이라 앞으로 이동은 엔진에서 처리
- 만든 방법: AI 구르기 생성이 3번 실패(v3 custom은 "ball"을 진짜 공으로 그리거나 뒤로 넘어짐, skeleton-v3 `front-flip`은 머리가 바닥 아래로 가고 픽셀이 뭉개짐) → `edit_image`로 south-east 회전 이미지를 "몸을 동그랗게 웅크린 자세"로 바꿔 4개 뽑고(10회) 그중 하나(`source/roll_ball_pose.png`)를 잘라 90°씩 회전(`RotateFlip`, 픽셀 손실 없음), 공 모양 아래쪽을 바닥선 111행에 맞추고 가로 중심 x=66에 배치. 처음과 끝은 idle frame_000
- 알려진 점: 웅크린 자세에선 단검이 거의 안 보임. 회전하면서 빛 방향도 같이 돎. 서 있기→웅크림 사이가 조금 급함(필요하면 숙이는 프레임 추가)
- PixelLab: 웅크린 자세 edit_image job `20dc94c7-2add-4dd9-8683-1ceafcb611bd` (index 1 사용). 실패한 그룹: roll `9c3104b3-…`, roll_v2 `5b8245a6-…`, roll_frontflip `cecedc67-…`

**완료:** 대시(dash) 확정 (2026-11-26) → `animations/dash/`
- 용도: 일정 쿨타임마다 몬스터 앞까지 빠르게 붙는 이동
- 동작: 몸을 낮추고 양팔을 뒤로 젖혀 단검이 뒤로 끌림, 앞손을 앞으로 뻗은 자세로 돌진 → 멈추며 시작 자세로 복귀. 9프레임, 0.06초/프레임(0.54초). **제자리 동작**
- **엔진 연결:**
  1. frame_000~001은 출발 준비(제자리), **frame_002~006 동안 캐릭터를 목표(몬스터 앞) 쪽으로 빠르게 이동**(예시에서는 0→30→75→120→150→165px, ease-out), frame_007~008은 멈춘 뒤 서 있기
  2. 잔상: 이동 중인 프레임마다 직전 1~2개 위치에 같은 프레임을 반투명(약 38%, 18%)으로 그림 → `dash_move_demo_x2.gif` 참고
  3. 왼쪽을 볼 때는 좌우 반전
- 만든 방법: v3 custom, south-east, 8프레임(+시작 1), `end_frame_url` = south-east 회전 이미지. 설명: "quick assassin dash: she drops into a low forward-leaning stance and shoots forward in a blur, body almost horizontal with arms swept back, the reverse-grip dagger trailing behind, hair flying straight back, then skids to a stop with knees bent and rises back to the starting pose; stays facing right in three-quarter view, body does not turn to the front, only one dagger, stays at the same horizontal spot" (2회). 104×124 원본을 128×128에 (dx=15, dy=8), 발바닥 111행. 후처리 없음
- PixelLab: 확정 group `2651e850-b6f4-42eb-a706-c46cc7253006` (dash_b). 탈락: dash `18ae9d4b-…` (걷는 느낌)
- 알려진 점: "몸을 거의 수평으로"까지는 숙이지 않음

**완료:** 0차 후드 확정 (2026-11-26) → `ArtSource/hood_0/` (자세한 내용은 그 폴더 HANDOFF)
- 동작을 새로 생성하지 않고, 후드 그림만 잘라 기본형 idle·run·attack·hurt 위에 씌우고 뒷머리를 지움 (장비 레이어 방식, 생성 비용 0)
- **기본형 동작을 바꾸면 hood_0도 다시 합성해야 함** (스크립트 `pixl_test\assets\character\hood_0\work\tools\HoodTool.cs`, 머리 위치 `ArtSource/hood_0/layer/head_anchor.csv`)
- roll, dash는 아직 씌우지 않음

**완료:** attack2(두 번째 공격, 콤보) 확정 (2026-11-26) → `animations/attack2/`
- 동작: 빠르게 두 번 찌름 → 살짝 뛰어올라 단검을 돌리며 위로 올려 베기 → 착지 후 시작 자세. 11프레임, 0.07초/프레임(0.77초)
- **흰 궤적 제거 (2026-11-26):** 원본 2~5번 프레임에 그려져 있던 흰 베기 궤적을 지움. 4~5번은 스크립트로 지움(세로 10px 이상·바깥에 닿은 흰색 덩어리 + 주변 밝은 테두리). 2~3번은 궤적이 치마·다리를 덮고 있어 `edit_image`(10회, "Remove the white curved slash trail effect completely. Redraw the parts it was covering: the skirt with its cream frill hem, the legs and the boots...")로 다시 그린 뒤 머리 매칭으로 원래 위치에 맞춤(frame 2: (−3, −2)). 궤적 있는 원본은 `pixl_test\assets\character\girl_base\work\attack2_with_trail\`
- 만든 방법: v3 custom, south-east, 10프레임(+시작 1), `end_frame_url` = south-east 회전 이미지. 설명: "flashy dagger combo: two lightning-fast stabs forward with the reverse-grip dagger, then she hops up and spins the dagger in a rising backhand slash that ends with the blade raised high, skirt and hair flaring, then lands and returns to the starting pose; stays facing right in three-quarter view, body does not turn to the front, only one dagger in reverse grip" (2회). 108×124 원본을 128×128에 (dx=12, dy=8), 발바닥 111행. 후처리 없음
- PixelLab group `75747229-ce58-427a-9798-0805f87a158c`
- 알려진 점: 3번 프레임에서 회전하며 몸이 잠깐 뒤로 돎
- **attack2 잔상 이펙트 확정 (2026-11-26) → `effects/attack2_smear/`:** 지운 흰 궤적을 별도 이펙트로 되살림. 궤적 있는 원본(`work\attack2_aligned`)에서 잔상 색 #FDF2E8 픽셀 중 15px 이상 덩어리만 뽑아 같은 128×128 위치에 저장(모양·위치가 원본과 픽셀 단위로 같음, 캐릭터 그림의 같은 색 작은 점은 제외)
  - 엔진 연결: 캐릭터와 같은 pivot (64, 112), attack2와 **같은 프레임 번호를 같은 속도(0.07초)로** 캐릭터 위에 그림 (잔상은 002~005번, 0.14~0.35초)
  - 003번 잔상의 파인 곳은 원본에서 손이 가리던 부분이라 그대로 둠

**완료:** dead(사망) 확정 (2026-11-26) → `animations/dead/`
- 동작: 맞고 휘청 → 단검을 떨어뜨림 → 무릎이 꺾이며 **앞으로 엎드려** 쓰러짐 → 눈 감고 엎드린 채 끝. 11프레임, 0.1초/프레임. 마지막 프레임(frame_010)을 유지
- 만든 방법: v3 custom, south-east, 10프레임(+시작 1), end_frame 없음. 설명: "death: struck by a fatal blow from the front, she staggers backward, the dagger slips from her hand, her knees buckle and she collapses onto the ground, ending lying down still on her side with eyes closed; stays on the same spot, facing right in three-quarter view" (3회). 124×124 원본을 128×128에 (dx=4, dy=8), 발바닥 111행. 후처리 없음
- PixelLab group `f5758625-8062-4486-91f1-9630b0970812`
- 알려진 점: 설명은 뒤로 넘어지기였지만 앞으로 엎어짐(사용자 확인 후 확정). 떨어진 단검은 캐릭터 왼쪽 바닥에 남음

**완료:** hood_0에도 attack2·dead 씌움 (2026-11-26, `ArtSource/hood_0/animations/`)

**무기 규칙: 단검은 오른손에만 든다** (그림에서 몸 왼쪽·뒤쪽에 낮게 쥔 손). 쌍수 아님. 왼손은 항상 빈손
- 프롬프트 팁: "her right hand (the hand on the left side of the image)", "her left hand stays empty", "only one dagger in the whole animation"을 넣어야 쌍수가 덜 나옴

**완료:** attack3 = 점프 내려찍기(오른손 단검만) 확정 (2026-11-26) → `animations/attack3/`
- 처음엔 스킬용(skill_slam)으로 확정했다가 같은 날 attack3로 바꿈 (skill_slam 폴더는 없앰). 아래 올려베기는 attack3에서 빠짐
- 원본: 내려찍기 후보 A (PixelLab group `68b1dcbc-f565-47b9-9d9e-b6a14e48328f`, v3 custom 10프레임, 100×136 원본을 (dx=16, dy=2)로 배치). 몸을 낮췄다가 아래로 찍는 자세(3~7번), 8번에서 들어 올림, 시작 자세 복귀. 11프레임, 0.07초/프레임
- 원본은 왼손에도 단검이 생겨 쌍수였음 → 3~8번 프레임의 **왼손 단검과 칼끝 흰 충격 표시를 지움** (`edit_image` 2회, 20회 비용: "Remove only the dagger held in the hand on the RIGHT side of the image... Keep the small dagger in the hand on the LEFT side..."). 편집 후 밀린 위치는 머리 템플릿 매칭으로 원래 자리에 다시 맞춤 (frame 4~6은 (−2~−3, −7), frame 7은 (−3, −3))
- 직접 지우기는 실패: 칼이 치마 앞에 겹쳐 있어 치마·앞 부츠까지 잘림
- **엔진 연결:** 땅을 찍는 무기가 보이지 않으므로 타격 이펙트를 캐릭터 오른쪽 아래(앞발 앞, 128 캔버스 기준 대략 x=80~90, y=105~112)에 5번 프레임(0.35초) 근처에서 재생하면 자연스러움
- hood_0판: `ArtSource/hood_0/animations/attack3/` (같은 프레임 기준으로 다시 합성)
- **attack3 이펙트 확정 (2026-11-26) → `effects/attack3_slash/`:** 평타와 같은 크림색(#FDF2E8) 외곽선 없는 반달이지만, 내려찍기에 맞춰 **머리 위에서 시작해 몸 앞쪽 아래로 내려오는 호**. 옵션: radius 46, width 13, sy 0.95, cx 20, cy −40, a0 −2.5, 불꽃·섬광·링·빛·점선·안쪽 반달 모두 0
  - 엔진 연결: 이펙트 pivot (40, 128)을 캐릭터 pivot (64, 112)에 맞춤, attack3 시작 후 **0.21초(frame_003, 내리치기 시작)** 에 30fps 1회
  - 타격 연출(데모 기준): 타격 불꽃 0.28초에 몬스터 앞면, 히트스톱 0.12초, 검은 피격 번쩍임, 밀림 6px, 흔들림 3px (평타보다 조금 세게). 위 줄의 "5번 프레임(0.35초) 앞발 앞" 위치에 땅 타격 효과를 추가로 넣어도 됨

**보류(사용 안 함):** 올려베기(오른손) — 한때 attack3로 확정했던 것. 프레임은 `pixl_test\assets\character\girl_base\work\attack3_uppercut_r_aligned\`
- 동작: 웅크림 → 오른팔이 몸 앞으로 돌아 나오며 단검을 바닥 근처에서 머리 위까지 크게 올려 벰(3~5번 프레임, 몸이 살짝 비틀려 등 쪽이 조금 보임) → 시작 자세. 11프레임, 0.07초/프레임
- 만든 방법: v3 custom, south-east, 10프레임(+시작 1), `end_frame_url` = south-east 회전 이미지. 설명: "rising uppercut slash with her right hand only: she crouches low, then springs up and swings the reverse-grip dagger that is already in her right hand (the hand on the left side of the image, behind her body) in a big arc from near the ground up to high above her head, arm fully extended upward at the top, then settles back to the starting pose; her left hand stays empty in a loose fist and never holds a weapon; stays facing right in three-quarter view, body never turns to the front or back, only one dagger in the whole animation" (2회). 96×128 원본을 128×128에 (dx=18, dy=6). 후처리 없음
- PixelLab group `b9313e58-43bc-409d-9757-df6e6a8430ad`
- 탈락: 서서 내려치기(`79da502f-…` — 단검이 머리 위로 안 올라감), 올려베기 1차 2종(`388b4932-…`, `b784a6e4-…` — 단검 두 자루), 런처 오른손판(`21a99ff9-…` — 여전히 두 자루)

**완료:** vanish(점으로 사라짐)·appear(점이 모여 등장) 확정 (2026-11-26) → `animations/vanish/`, `animations/appear/`
- 용도: 스킬로 사라졌다가 적 뒤에서 등장. 순서·엔진 처리는 `ENGINE_REQUEST.md` 4장
- 만든 방법 (AI 생성 아님, 비용 0): `source/Dissolve.cs`로 idle frame_000을 분해. 14프레임: 0 = 원래 자세, 1~2 = 밝아지며 기 모음, 3~9 = 머리→발 순서로 부서짐(부서지기 직전 픽셀이 밝게 빛남), 부서진 픽셀의 26%가 밝은 점이 되어 위·앞뒤로 흩어지며 3~6프레임 뒤 사라짐, 13 = 빈 그림. 호출: `[Dissolve]::Make(<시작 png>, <출력 폴더>, 14, 3, 9, 4242)` (프레임 수, 부서짐 시작·끝 프레임, 랜덤 시드)
- appear는 vanish 프레임을 역순으로 저장한 것
- 다른 자세에서 사라지기, 점 색 바꾸기 등은 스크립트로 바로 다시 만들 수 있음

**완료:** 스킬용 X자 베기 이펙트 확정 (2026-11-26) → `effects/skill_xslash/`
- 모양: pixelfx `melee/double-slash`(사용자가 고른 스타일)를 메인 캐릭터 크기로 키움. 위에서 아래로 베고 0.14초 뒤 아래에서 위로 올려 베어 X자가 닫힘 → 닫히는 순간 두 획이 하얗게 번쩍이고 교차점에서 섬광·빛 조각·불꽃, 점선 X 자국이 잠깐 남음. 색은 크림·흰색 팔레트 `#a89480 #cdb9a4 #e6d6c4 #f3e6d8 #fdf2e8 #fffaf4 #ffffff`
- 사용자 요청으로 **획을 얇게(원래의 40%), 남는 시간을 짧게(45%)** 함 (애니메이션이 길어서 너무 커 보였음). 그래서 X가 닫힐 때 첫 획은 거의 사라져 빠른 두 번 베기처럼 보임
- 옵션(`source/options.json`): size 1.9, cx 50, cy −22(크기 1 기준, 교차점은 발 기준 (95, −42)), thin 0.4, life 0.45. 원본은 두고 크기·위치·굵기(thin)·지속시간(life) 옵션을 넣은 복사본 `melee/double-slash-big` 작성
- 엔진 연결 (2026-11-26 실측으로 다시 확인):
  1. 이펙트 pivot (40, 148)을 캐릭터 pivot (64, 112)에 맞춤 (JSON `meta.pixelfx.pivot`과 일치 확인), 30fps 1회
  2. **2타 타이밍:** 첫 획이 끝까지 그어지는 순간 = 이펙트 시작 후 0.06초 → 화면에는 **frame_002(0.067초)** 부터 보임 → 1타(약한 타격: 히트스톱 0.06초, 번쩍임). X가 닫히는 순간 = 0.152초 → 하얀 번쩍임·교차 섬광은 **frame_005(0.167초)** 부터 보임 → 2타(본 타격: 타격 불꽃·히트스톱 0.12초·검은 번쩍임·밀림·흔들림). 하얀 X가 멈춘 채로 보여 타격감이 큼
  3. 이 기본 시트(`xslash.png`)는 **캐릭터 앞(오른쪽)** 을 베도록 그려져 있고 교차점은 발 기준 (95, −42) 
- ⚠️ **2026-11-26 backstab이 "적을 바라보고 가로로 휘두르기"로 바뀜 (아래 backstab 완료 항목).** 아래 "뒤로 찌르기용 X베기(xslash_backstab, flipX)" 설정은 **예전 동작 기준**이라 다시 맞춰야 함. 새 동작은 다른 공격처럼 캐릭터 앞(오른쪽)을 베므로 기본 시트(`xslash.png`)나 평타 베기를 캐릭터와 같은 방향으로 쓰는 쪽이 맞을 가능성이 큼. 휘두르는 순간 = frame_004 시작(0.28초)
- (예전 기록) **backstab(뒤로 찌르기 버전)에 붙일 때 (확인 결과 기본 시트 그대로는 안 맞음):** 이 동작은 캐릭터가 몬스터 오른쪽에 나타나 **오른쪽을 본 채 단검을 왼쪽(뒤)으로 찌름**(frame_005~006). 기본 시트를 그대로 쓰면 X가 몬스터 반대쪽(앞)에 그려짐
  - 그래서 몬스터에 닿는 거리로 옮긴 **`backstab/xslash_backstab.png`** 를 추가 (교차점 발 기준 **(36, −30)** = ENGINE_REQUEST의 "몬스터 중심~캐릭터 pivot 약 36px"에 맞춤, 옵션 `backstab/options.json`: cx 19, cy −16, 나머지 같음)
  - 이펙트 SpriteRenderer에 **flipX = true** (pivot 기준으로 좌우 반전되므로 pivot은 그대로 캐릭터 pivot (64,112)에 맞춤) → X가 캐릭터 뒤 36px, 몬스터 위에 생김
  - 타이밍(backstab 0.07초/프레임): 단검을 당기는 **frame_004 시작(0.28초)에 이펙트 시작** → 1타 0.347초(frame_005, 찌르는 순간 = 기존 타격 판정 0.35초와 같음), 2타 0.447초(frame_006, 단검이 뻗어 있는 동안)
  - 데모: `backstab/backstab_xslash_demo_x3.gif` (최종 `animations/backstab` 프레임 사용, 몬스터는 캐릭터 pivot에서 왼쪽 36px, 1타 히트스톱 0.06초·2타 0.12초)
  - 캐릭터가 왼쪽을 보는(flipX) 상황이면 이펙트 flipX도 함께 뒤집어 반대로 둔다 (항상 "찌르는 쪽"에 X가 오게)

**완료:** backstab(후방 기습 가로 베기) 확정 (2026-11-26 최종) → `animations/backstab/`
- 스킬 흐름: vanish → 적 뒤로 순간이동 → **appear 없이 바로 backstab** (첫 4프레임에 점이 모이며 나타나는 효과가 들어 있음). **적을 바라보게 재생** (다른 공격과 같음: 몬스터가 왼쪽이면 flipX)
- 동작: 나타나며 단검을 어깨까지 올림 → 3번에서 단검을 뒤로 낮게 당김 → **4~7번 오른팔을 앞으로 쭉 뻗고 단검을 수평으로 가로 휘두름(자세 유지)** → 시작 자세. 11프레임, 0.07초/프레임. 휘두르는 순간 = frame_004 (0.28초)
- 변경 이력 (같은 날): ① 뒤로 역수 찌르기(적에게 등 돌림) → ② 사용자 "적을 바라보고 찔러야" → 칼날만 앞으로 뒤집은 찌르기(무료) → ③ 사용자 "찌르지 말고 가로 각도에 맞춰 팔 모양 잡고 휘두르게" → 4~7번을 `edit_image`(10회, "Redraw only her right arm and the single reverse-grip dagger so she is swinging it in a fast flat horizontal slash toward the RIGHT side of the image at chest height... Keep her body, head, face, hair, skirt and legs exactly the same.")로 다시 그림, 머리 매칭으로 위치 복원 (4: (0,−1), 5: (2,0), 6: (−1,0), 7: (6,0))
- 알려진 점: 4~7번이 거의 같은 자세라 칼이 지나가는 중간 프레임 없이 3→4에서 한 번에 넘어감 (사용자 확인 후 그대로 확정). 궤적은 엔진에서 베기 이펙트로 보완 가능
- 예전 버전 프레임: `pixl_test\...\girl_base\work\backstab_plunge_fixed`(뒤로 찌르기), `backstab_fwd_final`(앞으로 찌르기)
- 아래는 공통 원본 기록:
- 만든 방법: v3 custom, south-east, 10프레임(+시작 1), end_frame = south-east 회전 이미지. 설명 "sneak attack from behind with her right hand only: she raises the reverse-grip dagger in her right hand (the hand on the left side of the image) up to her shoulder, then plunges it forward and down in one fast heavy stab..." (2회, PixelLab group `58f70db6-19d2-4be8-a806-a535c2fa5a71`). 96×124 원본을 (dx=18, dy=8)
- 단검 수정: 3·7번에 단검이 없어서 `edit_image`(10회)로 오른손에 단검을 그려 넣고 머리 매칭으로 위치 복원(3: (−7,0), 7: (−9,−1)). 5번 앞쪽에 튀어나온 작은 칼끝은 직접 지움(32px). 같은 편집에서 5·6번은 찌르는 자세가 바뀌어버려 원본 사용 → **4~6번(찌르는 구간)은 수정 전과 같아서 위 X베기 이펙트 타이밍은 그대로 맞음**
- 나타남 효과: `Dissolve.cs`의 `MaterializeInto(프레임들, 4, 출력, 4242)` — 4프레임 동안 발부터 위로 픽셀이 채워지고(채워지는 순간 밝게), 아직 안 채워진 곳은 위에서 내려오며 모이는 점으로 표시
- 탈락: 내디뎌 찌르기(`9095ffa5-…`, 동작이 작음)

**현재 단계:** 기본형 모션 계속 제작 (2026-11-26)
- **작업 순서 규칙 (사용자 결정, 2026-11-26): 기본형(girl_base) 모션을 먼저 다 정한 뒤에 후드를 한 번에 추가한다.** 기본형 모션을 만들거나 바꿀 때 hood_0을 다시 합성하지 않는다
- 그래서 `ArtSource/hood_0/`은 2026-11-26 시점 기본형 기준이며, 기본형 모션이 확정되면 전부 다시 합성할 예정 (방법은 hood_0 HANDOFF)

**다음 순서 (사용자 확인을 받으며):**
1. (선택) walk 생성, GIF로 확인받기
   - walk: `mode="skeleton-v3"` + 템플릿 `walking`. 반측면은 다리가 대각선이 되므로 run처럼 east 방향 + 얼굴 고정 방식 사용
   - attack: v3 custom "베기"가 가장 나았음 (lead-jab 찌르기는 3프레임뿐이라 약함)
2. 128×128, 바닥선 y=112로 정렬 → 확정본만 `animations/<동작>/` 에 커밋 (idle과 같은 파일 구성)
3. 기본형 모션이 모두 확정되면 → hood_0 전체 다시 합성 (roll·dash 포함)
4. 1차 이후 후드: hood_0과 같은 방식 (`create_character_state`로 디자인 → 후드만 잘라 씌우기)

## 7. 기술 메모
- `edit_image`에 같은 이미지를 4번 넣으면 같은 비용(10회)으로 변형 4개를 받음 (포즈 수정에 유용)
- 템플릿 애니메이션(template 모드)은 손에 든 무기를 자주 지움 → skeleton-v3 권장
- v3 reference 캐릭터: `reference_image_url`(정면) + `direction_images`로 원하는 방향 그림을 그대로 넣을 수 있음 (south-east를 넣으려면 `n_directions=8`)
- PixelLab `create_image_pro` 작은 크기 결과는 큰 시트를 잘못 자른 것일 수 있음 → 이어 붙여 확인
- PowerShell 변수는 대소문자를 구분하지 않음 ($C와 $c 충돌 주의)
- PixelLab Job 슬롯 최대 10개
