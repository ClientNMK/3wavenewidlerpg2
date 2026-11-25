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
├─ animations/attack/                 ← attack 확정본 (2026-11-25)
│  ├─ frames/frame_000~008.png        ← 128×128, 바닥선 y=112, 중심 x=64, 투명 배경 (000 = 시작 자세)
│  ├─ attack_strip.png                ← 가로 스트립 (칸 128px, 9칸)
│  ├─ attack_x1.gif                   ← 원본 크기, 투명, 0.08초/프레임
│  └─ attack_preview_x3.gif           ← 확인용 3배, 베이지 배경 + 빨간 바닥선
├─ animations/hurt/                   ← hurt(피격) 확정본 (2026-11-25)
│  ├─ frames/frame_000~008.png        ← 128×128, 바닥선 y=112, 중심 x=64, 투명 배경 (000 = 시작 자세)
│  ├─ hurt_strip.png                  ← 가로 스트립 (칸 128px, 9칸)
│  ├─ hurt_x1.gif                     ← 원본 크기, 투명, 0.08초/프레임
│  └─ hurt_preview_x3.gif             ← 확인용 3배, 베이지 배경 + 빨간 바닥선
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
| 사용 방향 | **오른쪽 반측면(south-east) 한 방향만.** 왼쪽은 좌우 반전. 정면·뒷모습 불필요 (팀 피드백: 리소스량 절감). **모든 몬스터·캐릭터도 같은 각도만** 만든다 (몬스터: `ArtSource/monsters/mob_slime/` 초록 원본 + 색만 바꾼 `mob_slime_blue`·`mob_slime_pink`·`mob_slime_gold`. 만드는 방법은 각 폴더 HANDOFF 참고) |
| 이동 | 화면 가로로 일직선 이동. **바닥은 평지(수평선)**. 발 배치가 대각선이면 안 됨 |
| 기본 포즈 | **p2**: 반측면 상체 + 두 다리 평행, 두 발이 같은 수평선 → `pose/girl_base_se_p2.png` |
| 디자인 | 웨이브 애쉬 크림 머리, 금빛 눈, 진갈색 원피스+금단추, 금방울 달린 큰 빨간 리본, 크림 프릴, 갈색 부츠 |
| 단검 | 낮게 역수로 쥐고 칼날은 아래·뒤로, 앞쪽 손 |
| idle 규칙 | **다리는 절대 움직이지 않고 상체·옷·머리카락만 흔들림** |
| run 규칙 | 몸·다리는 옆모습(east) 달리기, **얼굴은 idle과 같은 반측면 각도로 고정**(고개 좌우로 돌리지 않음), 머리카락은 흔들림 |
| attack 규칙 | 반측면 그대로 유지(정면으로 돌지 않음), 역수 단검 한 자루, 끝 프레임은 시작 자세로 복귀 |
| hurt 규칙 | 앞에서 맞고 상체·머리가 뒤로 젖혀졌다 복귀. 반측면 유지, 발은 바닥에 붙음, 역수 단검 유지, 끝 프레임은 시작 자세. 빨간 피격 플래시는 엔진에서 처리 |
| 애니메이션 속도 | idle 0.12초/프레임, run 0.08초/프레임, attack 0.08초/프레임, hurt 0.08초/프레임 (엔진에서 조절) |
| 캔버스 정렬 | 모든 애니메이션을 128×128에 놓고 발바닥 바닥선 y=112, 가로 중심 x=64. 엔진 pivot (64,112) |

## 5. PixelLab ID
| 이름 | character_id | 비고 |
|---|---|---|
| **girl_base_se_p2 (현재)** | `91ce750a-fd6e-4cbd-9898-4e0fe19fc089` | south-east = p2 포즈. 8방향 중 south-east만 사용 |
| └ idle_a | group `3bc7cb70-8841-4595-9a13-09113d618925` | v3 custom 8프레임 (124×124). 다리 고정 후처리 + 정렬(dx=4, dy=8) → **확정** (`animations/idle/`) |
| └ idle_b | group `96a5b540-90ec-40f2-ab8d-e576ff1ee375` | 다리 더 많이 움직임 (탈락) |
| └ run | group `2be876c6-2c45-4e03-9b76-e1d3c3bff920` | skeleton-v3 `running-6-frames`. south-east는 다리가 대각선이라 탈락 → **east 방향** 사용 → 얼굴 수정 + 얼굴 고정 후처리 → **확정** (`animations/run/`) |
| └ attack_slash_v2 | group `aa3ac889-d310-46d1-893b-e0fd12b3afd4` | v3 custom 8프레임(+시작 1) south-east, end_frame = south-east 회전 이미지 → 남는 칼날 지움 + 정렬 → **확정** (`animations/attack/`) |
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

**완료:** attack 확정 (2026-11-25) → `animations/attack/`
- 만든 방법:
  1. v3 custom, south-east, 8프레임. `end_frame_url`에 south-east 회전 이미지를 넣어 끝 자세가 시작 자세로 돌아오게 함. 설명에 "stays facing right in three-quarter view, body does not turn to the front" 명시
  2. 4~8번째 프레임(frame_003~007)에 손 위로 겹친 잔상 칼날을 직접 지움 (손 아래 칼날만 남김)
  3. 124×96 원본을 128×128에 (dx=20, dy=8)로 배치, 발바닥 111행, 머리 중심 x를 idle과 맞춤
- 알려진 점: 베기 동작이 작은 편 (칼이 가슴 높이까지만 올라옴). 필요하면 더 큰 동작으로 재생성

**완료:** hurt(피격) 확정 (2026-11-25) → `animations/hurt/`
- 만든 방법:
  1. v3 custom, south-east, 8프레임. `end_frame_url`에 south-east 회전 이미지. 설명: "getting hit and hurt: flinches backward from an impact in front, head and upper body jerk back, eyes squeeze shut... Feet stay planted on the ground, stays facing right in three-quarter view, body does not turn to the front, still holding the dagger low in reverse grip"
  2. 96×124 원본을 128×128에 (dx=18, dy=8)로 배치, 발바닥 111행 (모든 프레임 발바닥 행 동일). 후처리 없음
- 알려진 점: 젖혀지는 정도가 작은 편. 가운데 프레임(003~005)에서 다리가 살짝 벌어짐(발은 바닥에 붙어 있음)

**현재 단계:** walk(선택) 또는 0차 후드 버전

**다음 순서 (사용자 확인을 받으며):**
1. (선택) walk 생성, GIF로 확인받기
   - walk: `mode="skeleton-v3"` + 템플릿 `walking`. 반측면은 다리가 대각선이 되므로 run처럼 east 방향 + 얼굴 고정 방식 사용
   - attack: v3 custom "베기"가 가장 나았음 (lead-jab 찌르기는 3프레임뿐이라 약함)
2. 128×128, 바닥선 y=112로 정렬 → 확정본만 `animations/<동작>/` 에 커밋 (idle과 같은 파일 구성)
3. 0차 후드 버전: 같은 설명 + "wearing a hood made of a cracked egg shell, one big round eye on top of the hood" → `ArtSource/hood_0/`. 같은 캐릭터에 장비만 바뀐 상태로 만들기 위해 `create_character_state` 사용 검토

## 7. 기술 메모
- `edit_image`에 같은 이미지를 4번 넣으면 같은 비용(10회)으로 변형 4개를 받음 (포즈 수정에 유용)
- 템플릿 애니메이션(template 모드)은 손에 든 무기를 자주 지움 → skeleton-v3 권장
- v3 reference 캐릭터: `reference_image_url`(정면) + `direction_images`로 원하는 방향 그림을 그대로 넣을 수 있음 (south-east를 넣으려면 `n_directions=8`)
- PixelLab `create_image_pro` 작은 크기 결과는 큰 시트를 잘못 자른 것일 수 있음 → 이어 붙여 확인
- PowerShell 변수는 대소문자를 구분하지 않음 ($C와 $c 충돌 주의)
- PixelLab Job 슬롯 최대 10개
