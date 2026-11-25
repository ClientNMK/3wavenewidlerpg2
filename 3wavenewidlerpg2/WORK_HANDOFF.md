# 작업 인계 (2026-11-26 기준)

집이나 다른 PC에서 이어서 작업할 때 먼저 읽는 문서. 규칙은 같은 폴더 `CLAUDE.md`를 따른다.

## 1. 지금 상태 한눈에

- Unity 2022.3.62f3, 씬 흐름: **TitleScene → LoadingScene → InGameScene** (차트를 타이틀에서 불러오므로 항상 TitleScene에서 Play)
- 뒤끝(BACKND) 로그인·차트 로드 동작. 차트는 뒤끝 콘솔에 `_Dev` 이름으로 올라가 있음
- 자동 전투 방치형 루프 동작: 캐릭터 자동 추적·공격 → 몬스터 처치·골드 드롭 → 10마리 처치 후 보스 도전 → 다음 스테이지
- **아직 저장 없음**: 스테이지·골드는 게임을 다시 켜면 처음부터

## 2. 작업 방식 (꼭 알아야 할 것)

- 씬·프리팹·임포트 설정은 `Assets/Editor/*Once.cs` **일회용 에디터 스크립트**로 만든다. Unity 창에서 **Ctrl+R**(Play 꺼진 상태) → 한 번 실행되고 스스로 삭제됨. 결과는 `Editor.log`에서 `[...Once] 완료`로 확인
- 일회용 스크립트 여러 개를 한 번에 두면 하나가 실행 안 되고 넘어갈 수 있다. 가능하면 하나로 합치기
- 캐릭터·몬스터 Spine 데이터(json/atlas/png)는 아트 스트립을 이어 붙여 **직접 생성**한다 (`Assets/Resource/Spines/...`). json을 문자열 치환으로 고칠 때 같은 패턴이 스킨과 애니메이션에 여러 번 나오니 한 곳만 바뀌는지 확인 (CLAUDE.md에도 기록)
- 아트 원본: 상위 폴더 `ArtSource/` (각 폴더 `HANDOFF.md`, 메인 캐릭터는 `ArtSource/girl_base/ENGINE_REQUEST.md`가 엔진 연결 기준)
- 차트 원본: 상위 폴더 `Chart/*.xlsx`
- 아트 1px = 1/64 유닛. 메인 캐릭터 Transform Scale 0.5, 몬스터는 차트 `Monster_Scale`

## 3. 차트와 쓰는 값

| 차트 | 쓰는 내용 |
|---|---|
| StatChart | 1 Attack, 101 Hp, 201 Defence, 301 Penetrate, 401 Cri, 501 Move_Speed, 502 Move_Speed_Per, 503 Attack_Speed, 504 Attack_Speed_Per, 505 Attack_Range, 506 Attack_Number, 602 Recovery_Hp_Per. **Stat_Id = StatType 번호** (Stat_Type 칸 없음) |
| SystemChart | Roll_Cooltime 5, Roll_Distance 2, Boss_Challenge_Time 15, Boss_Challenge_Count 10, Attack_Splash_Range 1.5 |
| MonsterChart | Resource(프리팹 이름), Skin_Name(1~4 = 기본·파랑·분홍·금색 슬라임), 이동속도·감지·공격 범위·공격 속도·크기 |
| StageChart | Monster_Id / Monster_Spawn_Count는 "1, 2, 3"처럼 짝 배열. 보스 Id·체력·공격력, Gold_Value, Exp_Value |
| SkillChart | 20000번 고정 장착 (Cooltime, Damage, Max_Hits 사용) |
| GoodsChart | 골드 Icon `icon_goods_gold` = 드롭 프리팹 이름 |

- 속도 계산: 기본값 × (1 + 증가율). 증가율(502, 504)은 0이면 그대로
- 퍼센트 스탯은 0.01 = 1%

## 4. 만든 기능

### 메인 캐릭터 (`MainCharacter`, `FSM_MainCharacter`)
- 상태: Idle / Move / Attack / Hit / Dash / Skill / Dead
- 자동 전투: 화면 안(또는 공격 범위 안) 가장 가까운 몬스터로 이동 → 평타 사거리(505) 안이면 공격
- 평타: attack / attack2 / attack3 랜덤. 타격 시 목표 몬스터 중심 `Attack_Splash_Range` 안에서 `Attack_Number`마리
  - attack: 0.49초 베기 이펙트, 0.54초 타격 (히트스톱 0.1, 밀림 5px, 흔들림 2px)
  - attack2: 잔상 이펙트는 Spine 안 `smear` 슬롯으로 같은 프레임 재생, 0.21초 타격
  - attack3: 0.21초 베기, 0.28초 타격 (히트스톱 0.12, 밀림 6px, 흔들림 3px)
  - 재생 속도 = 공격 속도
- 대시(State.Dash, 코드 이름은 Roll): 자동 전투 중, 쿨타임이 차고 적이 `평타 사거리 + Roll_Distance × 0.5`보다 멀 때. frame_002~006 동안 ease-out 이동, 평타 사거리 앞에서 멈춤, 잔상(Afterimage)
- 스킬 20000: vanish(0.28초, 빠르게) → 0.2초 숨음 → 목표 몬스터 뒤(평타 사거리 80%)로 순간이동, **몬스터를 바라봄** → backstab(공격 속도 따라감) → 0.21초 기본 X베기(xslash), 0.28초 타격 1번 (공격력 × Damage, 최대 Max_Hits마리)
- 피격(hurt): 공격할 몬스터가 없이 서 있을 때만 재생
- 사망: dead 모션 후 체력 가득 채워 그 자리에서 부활. 보스전 중이면 보스 실패
- 체력 회복: 1초마다 최대 체력 × Recovery_Hp_Per

### 몬스터 (`Monster`, `FSM_Monster`)
- 슬라임 프리팹 하나(`monster_stage_001`) + 스킨 1~4. 감지 범위 안이면 추적, 공격 범위에서 젤리 투사체 발사(스킨별 색)
- 피격: 검은 실루엣 번쩍임 + 밀림, 데미지 숫자, 타격 불꽃
- 사망: dead 모션 후 풀 반환, 금화 드롭(통통 튀고 캐릭터에게 날아감)

### 스테이지·보스 (`InGameManager`)
- 스테이지 차트대로 등장, 다 잡으면 다시 등장
- 10마리 처치 → 보스 도전 버튼 활성. 누르면 몬스터 정리, 캐릭터 (0,-3)·보스 (0,3) 배치, 15초 제한
- 성공: 다음 스테이지 / 실패(시간 초과·사망): 같은 스테이지 일반 몬스터로

### UI
- 상단 HUD(hud_kit 부품 0.75배): 초상화+레벨, 닉네임, 골드·보석, 스테이지 바(Stage 1-1, 처치 수), 보스 도전 버튼, 자동 토글
- 오른쪽 아래: 대시·스킬 쿨타임 박스 / 왼쪽 아래: 처치 기록(1초 후 0.5초 페이드)
- 체력바: 캐릭터·몬스터 머리 위, 화면에서 같은 크기
- 폰트: DNFBitBitv2 SDF (TMP 기본 폰트)
- 타이틀·로딩 화면: 같은 배경(title), 로딩 게이지

## 5. 아직 안 정한 것 / 다음 할 일

- [ ] **무적 여부**: 대시 중, 스킬로 사라진 동안 무적으로 할지 (사용자 결정 대기)
- [ ] 저장: 스테이지 진행, 골드·경험치(지금은 표시만), 레벨
- [ ] 모양만 있는 UI: 레벨(Lv.1 고정), 보석, + 버튼, 자동 토글
- [ ] 월드 차트 `World_Resource`로 배경 바꾸기 (지금은 bg_world_1 고정)
- [ ] 스킬 차트 미사용 칸: Duration, Skill_Scale, Skill_Range, Resource 등
- [ ] 스킬 아이콘 정식 그림 (`Assets/Resource/Textures/UI/icon_skill_001.png`에 덮어쓰기)
- [ ] backstab 전용 X베기 이펙트 (지금은 기본 xslash 사용)
- [ ] 몬스터 감지 방식 2번(도망) 미구현
- [ ] 타이틀·로딩·팝업 글자 크기는 아직 InGame처럼 줄이지 않음

## 6. 커밋 상태

- 이 문서를 쓸 때까지 **코드·에셋 변경은 커밋하지 않았다**. 다른 PC에서 받으려면 먼저 커밋·푸시가 필요하다.
