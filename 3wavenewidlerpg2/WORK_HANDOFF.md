# 작업 인계 (2026-11-26 기준)

집이나 다른 PC에서 이어서 작업할 때 먼저 읽는 문서. 규칙은 같은 폴더 `CLAUDE.md`를 따른다.

## 1. 지금 상태 한눈에

- Unity 2022.3.62f3, 씬 흐름: **TitleScene → LoadingScene → InGameScene** (차트를 타이틀에서 불러오므로 항상 TitleScene에서 Play)
- 뒤끝(BACKND) 로그인·차트 로드 동작. 처음 접속(닉네임 없음)이면 차트 로드 전에 닉네임 입력 팝업(`UINicknamePopup`, 2~8자, 필터·중복 검사) — 이 시점 글자는 로컬 `LocalStringChart.csv`. 차트는 뒤끝 콘솔에 `_Dev` 이름으로 올라가 있음
- 자동 전투 방치형 루프 동작: 캐릭터 자동 추적·공격 → 몬스터 처치·골드 드롭 → 10마리 처치 후 보스 도전 → 다음 스테이지
- **저장은 최소만**: 뒤끝 게임 정보 테이블 `UserProfile`(내 데이터 한 줄)에 `LastAccessTime`(서버 시각 Unix 초)·`MaxStageId`만 저장 (`UserManager`, 타이틀에서 차트 뒤 불러옴, 접속 중 60초마다·백그라운드·종료 시 저장). 골드·레벨·강화 등은 아직 저장 안 됨. 게임을 켜면 저장된 스테이지(접속을 끊었던 스테이지)부터, 처치 수는 0부터

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
| StatChart | 1 Attack, 101 Hp, 201 Defence, 301 Penetrate, 401 Cri, 501 Move_Speed, 502 Move_Speed_Per, 503 Attack_Speed, 504 Attack_Speed_Per, 505 Attack_Range, 506 Attack_Number, 507 Damage_Min_Per, 508 Damage_Max_Per (타격마다 공격력 × 최소~최대 랜덤), 601 Predation_Per, 602 Recovery_Hp_Per. **1001~ 후드 스탯**(1001 Hood_Predation_Hp_Per)은 메인 캐릭터 스탯에 넣지 않음. **Stat_Id = StatType 번호** (Stat_Type 칸 없음) |
| SystemChart | Roll_Cooltime 5, Roll_Distance 2, Boss_Challenge_Time 15, Boss_Challenge_Count 10, Attack_Splash_Range 1.5, Skill_Slot_Max 5 |
| MonsterChart | Resource(프리팹 이름), Skin_Name(1~4 = 기본·파랑·분홍·금색 슬라임), 이동속도·감지·공격 범위·공격 속도·크기 |
| StageChart | Monster_Id / Monster_Spawn_Count는 "1, 2, 3"처럼 짝 배열. 보스 Id·체력·공격력, Gold_Value, Exp_Value |
| SkillChart | 20000번 고정 장착 (Cooltime, Damage, Max_Hits 사용) |
| WorldChart | World_Resource = 배경 이름 (스테이지의 World_Id로 찾음, `Assets/Resource/Textures/Bg`) |
| GoodsChart | 골드 Icon `icon_goods_gold` = 드롭 프리팹 이름 |
| StringChart | 문자열 (String_Id, Kr, En, Ja, TW). 원본 `Chart/StringChart_Dev.xlsx`. 차트 로드가 끝나면 로컬 CSV 대신 이 값을 씀 (`TitleManager.CompleteTitleLoading`). 고정 글자는 `UILocalizeText`에 키 입력 |
| CharLevelChart | 레벨 구간마다 필요 경험치 = LevelUp_Goods_Value + Increase × (레벨 − Level_Start), 레벨업마다 LevelUp_Reward_Goods_Value만큼 SP(재화 6). 몬스터 처치 경험치(StageChart Exp_Value)로 자동 연속 레벨업 (`InGameManager.AddExp`, 시작 Lv.1) |
| StatPointChart | StatPoint_Id마다 한 줄, 성장 창 "레벨" 탭. SP Upgrade_Goods_Value 소모, 스탯 = Upgrade_Stat_Value + Increase × (레벨 − Level_Start), Open_CharLevel 미만이면 잠김, Max_Level에서 MAX. 레벨은 `InGameBuffManager.StatPointLevels` |
| HoodChart | 후드 레벨 구간마다 먹일 재화(Goods 5) = Upgrade_Goods_Value + Increase × (레벨 − Start_Level), 포식 가능 체력(1001) = 기본값 + Upgrade_Stat_Value + Increase × (레벨 − Start_Level), Resorce = 스킨. 재화는 StageChart DropItem(Id·Value·Rate)으로 처치 시 확률 획득, 성장 창 "후드" 탭(`UIHoodPage`): 가진 만큼 한 번에 먹여 여러 레벨 상승, 누르면 후드 앞에 레벨업 이펙트(`Textures/Effect/hood_levelup`, 30fps) 1회, 후드는 idle 유지, 한마디 = StringChart `Hood_Talk_1~` 무작위. 드롭 시 처치 기록 + 떨어지는 그림(`icon_goods_hood_exp`) |
| GoldUpgradeChart | Gold_Id별 레벨 구간. 비용 = Goods_Value + Goods_Increase × (레벨 − Level_Start), 스탯 보너스 = Stat_Value + Stat_Increase × (레벨 − Level_Start) (NightmareIdle과 같은 계산). 차트의 Gold_Id마다 한 줄(`UIGoldUpgradeSlot`)로 `UIGoldUpgradePopup`(하단 탭 "성장")에 표시, 레벨은 `InGameBuffManager.GoldUpgradeLevels`(저장 없음) |

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
- 스킬 20000: vanish(약 0.09초, 3배속, 2026-11-27부터 hurt frame_005 맞은 자세에서 흩어짐) → 0.2초 숨음 → 목표 몬스터 뒤(평타 사거리 80%)로 순간이동, **몬스터를 바라봄** → backstab(공격 속도 따라감) → 0.21초 기본 X베기(xslash), 0.28초 타격 1번 (공격력 × Damage, 최대 Max_Hits마리)
- 포식: 평타(스킬 제외) 타격마다 Predation_Per 확률로 포식 상태이상 → 이번 타격 후 일반 몬스터 체력이 최대 체력 × 후드 1001 이하가 되면(처치될 타격 포함) 데미지 대신 후드가 그 몬스터 옆으로 날아가(0.16초) devour로 삼킴(입으로 끌려가며 작아지다 0.4초에 처치, 물어뜯는 동안 Tint Black 청록 번쩍임, 끝나면 캐릭터 뒤로 복귀, 보스전 제외, 삼키는 중엔 다음 포식 없음). Hood_Exp 보상은 아직 없음

### 후드 (`Hood`, `ArtSource/pets/pet_hood`)
- 캐릭터 뒤쪽(0.6칸) 위에 떠서 따라다니는 정령 펫. Spine `Assets/Resource/Spines/Character/Hood` (스킨 hood_001 = lv1_a, hood_002 = lv1_b / idle, devour)
- 스킨 = HoodChart Resorce 이름: hood_001(lv1 크림 망토), hood_002(lv2 뿔), hood_001_skin_shell(조개 모자 코스튬). 후드 레벨 구간의 HoodChart Resorce를 따름 (`Hood.Level`, 시작 1)
- 포식 표식: `ArtSource/ui/status_icons/status_devour.png`(16×16 정지) 몬스터 머리 위, 끌려가는 몬스터를 따라가다 덮치는 순간 꺼짐
- 포식 반짝 링(`devour_burst` 이펙트 프리팹, 30fps): 덮치는 순간 후드 pivot에서 오른쪽 6·아래 19px, 후드 앞
- 이동 꼬리(`move_trail`, 30fps 반복): 상시가 아니라 후드가 따라갈 자리에서 1칸 넘게 떨어져 돌아오는 동안만(포식 후 복귀 등), pivot 왼쪽 4px 뒤에, 가까워지면 0.3초 페이드
- 피격(hurt): 공격할 몬스터가 없이 서 있을 때만 재생
- 사망: dead 모션 후 체력 가득 채워 그 자리에서 부활. 보스전 중이면 보스 실패
- 체력 회복: 1초마다 최대 체력 × Recovery_Hp_Per

### 몬스터 (`Monster`, `FSM_Monster`)
- 슬라임 `monster_stage_001`(스킨 1~4), 단풍 버섯 `monster_stage_002`(스킨 1 주황·2 빨강·3 보라·4 파랑, attack 없음). 차트 `Resource`·`Skin_Name`으로 고름
- 일반 몬스터는 공격 없음(2026-11-27): 감지 범위 안이면 추적, 공격 범위에서 멈춰 섬. 보스만 attack 모션 + 투사체(스킨별 색) → 보스 Resource는 attack이 있는 몬스터여야 함
- 피격: 검은 실루엣 번쩍임 + 밀림, 데미지 숫자, 타격 불꽃
- 사망: dead 모션 후 풀 반환, 금화 드롭(통통 튀고 캐릭터에게 날아감)

### 스테이지·보스 (`InGameManager`)
- 스테이지 차트대로 등장, 다 잡으면 다시 등장
- 10마리 처치 → 보스 도전 버튼 활성. 누르면 검은 화면(0.2초 어두워짐 → 1초 유지 → 0.2초 밝아짐, 그동안 터치 막힘) 동안 몬스터 정리, 캐릭터 (0,-3)·보스 (0,3) 배치, 밝아지면서 15초 제한 시작
- 성공: 다음 스테이지 / 실패(시간 초과·사망): 같은 스테이지 일반 몬스터로. 끝날 때도 검은 화면 동안 보스 정리, 캐릭터를 씬 시작 자리로, 일반 몬스터 등장

### 공용 보상 획득 연출 (`UIRewardPopup`, kit_cocoa `LAYOUT_reward.md`)
- `PopupManager.Instance.ShowPopup<UIRewardPopup>().Setup(new List<UIRewardPopup.Reward> { new(GoodsId, 값), ... })` 로 모든 보상 받기에 사용
- 칸이 0.12초 간격으로 튀어나옴 → 누르면 아이콘이 곡선으로 재화 칸(골드·보석)/하단 성장 탭(그 외)으로 날아가고, 도착할 때 `InGameManager.AddGoods`로 실제 지급 + 숫자 세며 올림 + "+N"
- 보스 클리어는 이 팝업 대신 `UIBossClear`(kit_cocoa `LAYOUT_clear.md`): VICTORY 문장 + 보상(처음 클리어면 StageChart First_Clear, "첫 클리어" 꼬리표)을 한 장면에서, 2초 뒤(또는 누르면) 보상이 날아가며 닫힘 → 검은 화면 → 보스 도전 버튼 아래 "Stage N / 새 스테이지 시작" 1초. 날아가기는 `UIGoodsFly` 공용
- 보상 칸은 임시로 hud_slot (등급별 칸 부품 없음), 아이콘은 골드·보석·Hood_Exp만

### 실패 연출 (kit_cocoa `LAYOUT_fail.md`)
- 보스 실패(시간 초과·쓰러짐·포기): 전투 멈춤 + 화면 덮개(어두운 판 #1A1620 0.6 + `fail_vignette`) → 띠·DEFEAT 문장·이유 → 결과 판(보스 체력, 바로가기 골드 강화·후드 먹이기, 확인) → 확인(또는 3초 뒤 자동) 시 실패 처리(검은 화면, 0/10, 자동 꺼짐, 알림 판 없음). 바로가기는 성장 창 해당 탭을 엶
- 일반 스테이지 쓰러짐: HUD 뒤 화면 덮개(`SafeArea/DeathOverlay`) + "쓰러졌어요" 판, 3초 뒤 그 자리에서 부활(처치 수 그대로, 기획 확인 필요)
- 장비 바로가기 줄은 장비 기능이 없어서 뺌

### 방치(오프라인) 보상 (`UIIdleRewardPopup`, kit_cocoa `LAYOUT_idle.md`)
- InGame 시작 때 (지금 서버 시각 − LastAccessTime)분이 Offline_Time_Min_Min(10) 이상이면 팝업, Offline_Time_Max_Min(720)에서 멈춤
- 보상 = 최고 스테이지(`MaxStageId`) Gold_Value·Exp_Value × Offline_Time_Monster_Count_Min × 분. Hood_Exp 없음
- 받기 / 2배 받기(광고 연결 전이라 지금은 바로 2배) / 닫기 = 받기. 받으면 골드·경험치가 위로 날아가 도착할 때 지급

### UI
- 상단 HUD(hud_kit 부품 0.75배): 초상화+레벨, 닉네임, 골드·보석, 스테이지 바(Stage 1-1, 처치 수), 보스 도전 버튼, 자동 토글
- 청록 유리 UI(`pixl_test/shellgirl-ui-glass` → `Assets/Resource/Textures/UI/Glass`, Bilinear): 초상화 자개 프레임, 명패(닉네임), 남색 백판 = 스테이지 정보판 + 처치 진행바(Filled), 금빛 버튼 = 보스 도전(아래 가운데, 스킬 슬롯 위)
- 오른쪽 아래: 대시 쿨타임 박스(항상 자동) + 그 왼쪽 스킬 슬롯 5칸(`UISkillSlot`, 누르면 자동 사용 ON/OFF, 1번 = 20000 고정, 2~5번 빈 칸) / 처치 기록은 슬롯 줄 위(1초 후 0.5초 페이드)
- 카메라 버튼(사이드 버튼 아래, hud_round + icon_zoom, 아래 점 3개 = 지금 단계): 멀리(크기 8, 가운데 고정) → 중간(6) → 가까이(4.5). 중간·가까이는 캐릭터를 따라가고 배경 밖은 안 보임. 몬스터 찾는 범위는 멀리 화면(`BattleArea`) 그대로
- UI 테스트(코코아+금선 키트, `Textures/UI/Cocoa`, Bilinear, 1080 기준 부품이라 Image PPU Multiplier 1.5): 상단 HUD(`HUD/Cocoa`: 초상화·이름·재화·스테이지 바·사이드 버튼) + 하단 탭 6개(성장·인벤토리·스킬·미션·컨텐츠·상점, 성장만 동작 = 성장 창 강화·레벨·후드·전직·어빌리티 탭) + 강화 팝업(성장 창). 배치는 `pixl_test/assets/ui/kit_cocoa/preview/assembled_from_parts_x2.png`, 기준 `Plan/UI_Style_Guide.md`. 예전 HUD 부품은 꺼 둠. 강화 탭 x1/x10/x100 = 한 번에 올릴 레벨 수(구간 비용 합산), 레벨 탭 목록은 5줄 보이는 세로 스크롤, 보스 도전(시안 boss_challenge_states): 스테이지 바 아래 버튼 — 진행 중 hud_pill+흐린 해골+안내(Msg_BossChallengeHint), 도전 가능 btn_main+해골+알림 점 / 보스전 중 스테이지 바 = 해골+보스 이름+체력 %(빨간 바), 아래 남은 시간+포기(= 시간 초과 실패), 이름·재화 흐리게. 실패(시간 초과·사망·포기) 시 처치 수 0으로 초기화. 자동 보스 도전 = 보스 버튼 왼쪽 "자동" 스위치(켜면 처치 수가 차는 순간 도전, 실패하면 꺼짐, 저장 없음). 보스 도전을 누르면 검은 화면 동안 보스 배치 → 보스 등장 연출 1.2초(게임 정지, `UIInGameScene.CoBossIntro`) → 제한 시간 시작.  배치 기준 `pixl_test/assets/ui/kit_cocoa/LAYOUT_battle.md`(360×640 ×2). 상단 "자동 전투" 판은 모양만(항상 켜짐). 상단 왼쪽(`Cocoa/TopLeft`: 초상화·레벨·이름·경험치)과 가운데(`Cocoa/TopCenter`: 스테이지 바·보스 도전·보스전)만 Scale 0.85, 나머지 HUD는 100%. 버튼 빛·비네트 효과는 아직 없음. 사이드 버튼은 모양만
- 체력바: 캐릭터·몬스터 머리 위, 화면에서 같은 크기
- 배경 오브젝트(바위·유목·소라, 벽 처리)는 2026-11-27 삭제. 그림은 `Assets/Resource/Textures/Props`에 남아 있음
- 후드처럼 Spine 재질이 Tint Black 셰이더면 SkeletonAnimation의 Tint Black도 켜야 함 (안 켜면 초록 반투명으로 깨짐)
- 그림자: `ArtSource/common/shadow` (메인 캐릭터 4px 오른쪽, 반전 시 왼쪽), 모든 캐릭터 아래
- 조이스틱: 화면 전체 터치 / 몬스터 등장: 화면 가운데 기준 위·좌·우 반원, 반지름 2~4.5칸
- 폰트: DNFBitBitv2 SDF (TMP 기본 폰트)
- 타이틀·로딩 화면: 같은 배경(title), 로딩 게이지

## 5. 아직 안 정한 것 / 다음 할 일

- [ ] **무적 여부**: 대시 중, 스킬로 사라진 동안 무적으로 할지 (사용자 결정 대기)
- [ ] 저장: 스테이지 진행, 골드·경험치(지금은 표시만), 레벨
- [ ] 모양만 있는 UI: 레벨(Lv.1 고정), 보석, + 버튼, 자동 토글
- [ ] 배경 bg_world_4, 5 그림 (월드 차트에는 있음)
- [ ] 스킬 차트 미사용 칸: Duration, Skill_Scale, Skill_Range, Resource 등
- [ ] 스킬 아이콘 정식 그림 (`Assets/Resource/Textures/UI/icon_skill_001.png`에 덮어쓰기)
- [ ] backstab 전용 X베기 이펙트 (지금은 기본 xslash 사용)
- [ ] 몬스터 감지 방식 2번(도망) 미구현
- [ ] 타이틀·로딩·팝업 글자 크기는 아직 InGame처럼 줄이지 않음

## 6. 커밋 상태

- 이 문서를 쓸 때까지 **코드·에셋 변경은 커밋하지 않았다**. 다른 PC에서 받으려면 먼저 커밋·푸시가 필요하다.
