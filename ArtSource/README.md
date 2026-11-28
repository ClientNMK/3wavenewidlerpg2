# ArtSource — 아트 작업 안내 (다른 PC에서 이어서 작업할 때 먼저 읽기)

작성: 2026-11-26. 방치형 RPG "소라게 후드 소녀" 아트 원본 폴더.
**확정본만 여기에 둔다.** Unity가 자동으로 가져가지 않도록 `Assets` 밖에 있음. 게임에 넣을 때는 프로그래머가 `Assets`로 옮김.

---

## 0. 다른 PC에서 시작하는 법

1. Fork에서 저장소 `ClientNMK/3wavenewidlerpg2`의 **`art/girl-base` 브랜치**를 받는다 (Pull)
2. Claude Code를 열고 이렇게 말하면 된다:
   > `ArtSource\README.md`랑 `ArtSource\girl_base\HANDOFF.md` 읽고 이어서 작업하자
3. Claude의 "기억(메모리)"은 PC마다 따로라서 다른 PC에는 없음 → **아래 1장 규칙을 그대로 따르게** 하면 된다
4. 도트 생성은 PixelLab MCP(같은 계정)를 쓴다. 캐릭터 ID는 3장 참고

## 1. 작업 규칙 (꼭 지킬 것)

| 규칙 | 내용 |
|---|---|
| 각도 | 모든 캐릭터·몬스터는 **오른쪽 반측면(south-east) 한 방향만**. 왼쪽은 엔진에서 좌우 반전 |
| 무기 | 메인 캐릭터의 단검은 **오른손에만** (그림에서 몸 왼쪽·뒤쪽에 낮게 쥔 손). 쌍수 아님, 왼손은 빈손 |
| 캔버스 | 모든 애니메이션 128×128, 발바닥 바닥선 y=112(발은 111행), 가로 중심 x=64, pivot (64,112) |
| 순서 | **기본형(girl_base) 모션을 먼저 다 정하고, 후드는 나중에 한 번에** 추가 (기본형 바꿀 때 hood_0 다시 합성하지 않음) |
| 확인 | 단계마다 결과를 **GIF로 보여주고** 확인받은 뒤 다음으로 |
| 생성 레시피 | 동작별로 확정된 방법이 있음(`girl_base/HANDOFF.md` "만든 방법"). 싼 방법으로 즉흥적으로 바꾸지 않기. 예상 생성 비용을 미리 말하기 |
| 커밋 | **Claude는 커밋·푸시하지 않는다.** ArtSource에 복사 + HANDOFF 수정까지만. 커밋은 사람이 Fork에서 |
| 저장 경로 | 파일을 저장하면 **전체 경로**를 알려준다 |
| 안내 방식 | 한국어, 짧게, 다음 행동 먼저, 단계는 번호로 |
| 이펙트 색 | 타격 불꽃(hit_spark)은 주황·금색 그대로 (색 변경 금지 지시 있었음) |

## 2. 폴더 지도

```
ArtSource/
├─ README.md                ← 이 문서
├─ tools/                   ← 작업 스크립트 모음 (PowerShell에서 Add-Type로 사용, 아래 4장)
├─ girl_base/               ← 메인 캐릭터 기본형 (후드 없음)  ★ 주 작업 대상
│  ├─ HANDOFF.md            ← 모든 결정·만든 방법·PixelLab ID·다음 할 일 (가장 중요)
│  ├─ ENGINE_REQUEST.md     ← 프로그래머 전달용: pivot·속도·이펙트 타이밍·스킬 순서
│  ├─ concept/              ← 콘셉트 시트
│  ├─ pose/                 ← 기본 포즈 p2
│  ├─ animations/           ← idle run attack attack2 attack3 hurt dead roll dash vanish appear backstab
│  ├─ effects/              ← attack_slash attack2_smear attack3_slash hit_spark skill_xslash (pixelfx로 제작)
│  └─ tools/GifMaker.cs
├─ costum_10001/   ← 메인 캐릭터 겨울 코스튬 (빨간 옷 + 귀마개, 게임에서 쓰는 11개 동작). HANDOFF
├─ hood_0/                  ← 0차 후드(깨진 알껍데기) — 보류 중, 기본형 확정 후 다시 합성
│  ├─ HANDOFF.md            ← 후드 씌우기 방법 (후드만 잘라 기본형 위에 합성)
│  ├─ layer/                ← 후드 레이어, 후드 디자인 원본, 프레임별 머리 좌표(csv)
│  └─ animations/           ← idle run attack attack2 attack3 hurt dead (2026-11-26 기본형 기준)
├─ monsters/                ← mob_slime(초록) + blue/pink/gold 색 바꾼 판, monster_stage_002(가을 숲 버섯) + red/purple/blue, monster_dungeon_2001(보스, 192 캔버스). 각 폴더 HANDOFF
├─ backgrounds/bg_world_2/  ← 가을 숲 배경 1152×2048 (9:16 세로). HANDOFF
├─ backgrounds/bg_dungeon_2001/ ← 악몽 아이 방 (보스 던전) 1152×2048. HANDOFF
├─ items/item_gold_coin/    ← 골드 코인. HANDOFF
└─ ui/                      ← hud_kit, portrait_girl. 각 폴더 HANDOFF
```

## 3. PixelLab ID (자세한 그룹 ID는 각 HANDOFF)

| 대상 | character_id |
|---|---|
| 메인 캐릭터 기본형 (p2 포즈) | `91ce750a-fd6e-4cbd-9898-4e0fe19fc089` |
| 0차 후드 디자인 (기본형의 state) | `90228c57-36df-455c-8980-ee79e9b01721` |
| 몬스터·아이템·UI | 각 폴더 `HANDOFF.md` |

다운로드: `https://api.pixellab.ai/mcp/characters/<character_id>/download` (ZIP, 모든 애니메이션 원본 프레임)

## 4. tools/ 스크립트

| 파일 | 하는 일 |
|---|---|
| `GifMaker.cs` | 프레임 PNG들 → 확인용 GIF. `[GifMaker]::Make($프레임경로들, 'out.gif', 배율, 1/100초, '#ebe4d6', 바닥선행)` (배경 $null = 투명, 바닥선 없으면 −1) |
| `Dissolve.cs` | 점으로 흩어지기(vanish) `Make(...)`, 처음 몇 프레임 동안 점이 모이며 나타나기 `MaterializeInto(...)` |
| `HoodTool.cs` | 후드 씌우기: 머리 위치 찾기(`Register`), 뒷머리 지우고 합성(`CompositeClean`·`HairBelow`·`DarkStrands`), 얼굴 앞 단검 살리기(`RestoreFront`), 회전(`Rotate`·`CompositeCleanRot`), 떠 있는 조각 지우기(`RemoveSpecks`). 사용법은 `hood_0/HANDOFF.md` |
| `Fx.cs` | 애니메이션에 구워진 흰 궤적 지우기(`RemoveStreaks`) |
| `SlimeRecolor.cs` | 슬라임 색 바꾸기 |
| `SplitParts.cs` | UI 키트 조각 나누기 |

사용 예: `Add-Type -AssemblyName System.Drawing; Add-Type -Path ArtSource\tools\GifMaker.cs -ReferencedAssemblies System.Drawing`
주의: PowerShell 변수는 대소문자를 구분하지 않음 (`$c`와 `$C`, `$p`와 `$P`, `$f`와 `$F`가 같은 변수 → 덮어써서 오류 자주 남)

## 5. 저장소에 없는 것 (작업 PC에만 있음)

- **`pixl_test/` (작업 폴더, 약 62MB)**: 후보, 탈락작, 확인용 GIF, 중간 프레임(work/). git에 올리지 않는 폴더라 **다른 PC에는 없다**
  - 확정본은 모두 ArtSource에 있으므로 이어서 작업하는 데는 없어도 된다
  - 탈락 후보를 다시 보고 싶으면 PixelLab ID(각 HANDOFF)로 다시 받으면 된다
  - 꼭 필요하면 USB·드라이브로 따로 옮긴다 (팀 저장소에는 올리지 않기로 함)
- 원래 작업 PC에서는 Fork 내장 git을 사용 (PATH에 git 없음)

## 6. 현재 상태 (2026-11-26)

**메인 캐릭터 기본형 확정 모션:** idle, run, attack(평타), attack2(콤보), attack3(점프 내려찍기), hurt, dead, roll, dash, vanish(점으로 사라짐), appear(점이 모여 나타남), backstab(적 뒤에서 나타나 가로 베기)
**이펙트:** 평타 베기, attack2 잔상, attack3 베기, 타격 불꽃, 스킬 X베기
**몬스터:** 슬라임 4색 (idle·move·attack·dead + 투사체)

**다음에 할 만한 것:**
1. (선택) walk
2. backstab용 베기 이펙트를 새 동작(적을 바라보고 가로 베기)에 맞추기 — 예전 `effects/skill_xslash/backstab/`은 뒤로 찌르기 기준이라 방향이 안 맞음
3. 기본형 모션이 다 정해지면 → hood_0 전체 다시 합성 (roll·dash·vanish·backstab 포함), 그다음 1차 후드
4. 상세 목록은 `girl_base/HANDOFF.md` "다음 순서"
