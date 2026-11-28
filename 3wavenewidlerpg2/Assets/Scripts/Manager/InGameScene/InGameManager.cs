using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    static InGameManager _instance = null;
    static public InGameManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    [Header("Camera")]
    public Camera MainCamera = null;
    public Camera UICamera = null;

    [Header("UIInGameScene")]
    public UIInGameScene UIInGameScene = null;

    [Header("Character")]
    public MainCharacter MainCharacter = null;
    public List<Monster> Monsters = new();

    [Header("Bg")]
    public SpriteRenderer BgRenderer = null;

    private void Start()
    {
        GameManager.Instance.gameType = GameType.InGame;
        cameraPosition = MainCamera.transform.position;
        float halfHeight = MainCamera.orthographicSize;
        float halfWidth = halfHeight * MainCamera.aspect;
        BattleArea = new Rect(cameraPosition.x - halfWidth, cameraPosition.y - halfHeight, halfWidth * 2f, halfHeight * 2f);
        UIInGameScene.SetCameraZoomStep(cameraZoomIndex);
        playerStartPosition = MainCharacter.transform.position;
        InGameBuffManager.Instance.Setup();
        MainCharacter.OnDeadComplete = HandleMainCharacterDeadComplete;
        MainCharacter.Setup();
        UIInGameScene.SetupSkillSlots(MainCharacter.SkillSlots);
        // 저장된 최고 스테이지부터
        stageId = UserManager.Instance.MaxStageId;
        SpawnMonsters();
        RefreshStageUI();
        ShowIdleReward();
    }

    // 방치(오프라인) 보상: 최고 스테이지 골드·경험치 × 1분당 처치 수 × 비운 분 (최소 미만이면 없음, 최대에서 멈춤)
    void ShowIdleReward()
    {
        long minutes = (TimeManager.Instance.GetCurrentTimeStamp() - UserManager.Instance.LastAccessTime) / 60;
        UserManager.Instance.SaveAccessTime();
        if (minutes < DataManager.Instance.SystemData.GetFloat("Offline_Time_Min_Min"))
            return;

        int maxMinutes = (int)DataManager.Instance.SystemData.GetFloat("Offline_Time_Max_Min");
        float killPerMinute = DataManager.Instance.SystemData.GetFloat("Offline_Time_Monster_Count_Min");
        StageData stage = DataManager.Instance.StageData.GetStageData(UserManager.Instance.MaxStageId);
        PopupManager.Instance.ShowPopup<UIIdleRewardPopup>().Setup((int)Mathf.Min(minutes, maxMinutes), maxMinutes, stage.Gold_Value * killPerMinute, stage.Exp_Value * killPerMinute);
    }

    // 스테이지 진행 저장 전까지 1스테이지부터 시작
    int stageId = 1;
    int killCount = 0;
    bool isBossChallenge = false;
    bool isBossFailing = false;     // 실패 연출 동안 (히트스톱이 멈춤을 풀지 않게)
    bool isStageChanging = false;   // 보스전 끝(VICTORY·검은 화면)·스테이지 이동 동안: 일반 몬스터 채우지 않음
    bool isBossStarting = false;    // 도전 시작(멈춤·검은 화면·등장 연출) 동안: 자동 도전 중복 시작·히트스톱 해제 막음
    bool isAutoBoss = false;        // 자동 보스 도전 (저장 전까지 이번 플레이만)
    float bossTimer = 0f;
    public bool IsBossChallenge { get { return isBossChallenge; } }
    DungeonData dungeon = null;     // 던전·도전 중이면 그 단계 (아니면 스테이지 보스)
    string dungeonName = null;
    readonly Dictionary<int, int> dungeonClearedSteps = new();      // Dungeon_Type별 깬 최고 단계 (저장 없이 이번 플레이만)

    public int GetDungeonClearedStep(int _dungeonType)
    {
        return dungeonClearedSteps.TryGetValue(_dungeonType, out int step) ? step : 0;
    }

    // 던전·도전 입장: 처치 수 조건 없이 보스전과 같은 흐름
    public void StartDungeon(DungeonData _step, string _name)
    {
        if (isBossChallenge || isBossStarting || MainCharacter.State == State.Dead)
            return;

        dungeon = _step;
        dungeonName = _name;
        isBossStarting = true;
        StartCoroutine(CoStartBossChallenge());
    }

    StageData CurStageData { get { return DataManager.Instance.StageData.GetStageData(stageId); } }
    int BossChallengeCount { get { return (int)DataManager.Instance.SystemData.GetFloat("Boss_Challenge_Count"); } }

    void SpawnMonsters()
    {
        StageData stageData = CurStageData;
        // 배경은 스테이지가 속한 월드의 World_Resource
        string worldResource = DataManager.Instance.WorldData.GetWorldData(stageData.World_Id).World_Resource;
        BgRenderer.sprite = ResourceManager.Instance.GetSprite(AtlasType.Bg, worldResource);
        // Monster_Id[i]가 Monster_Spawn_Count[i]마리가 되게 모자란 만큼 등장
        for (int idx = 0; idx < stageData.Monster_Id.Length; idx++)
        {
            MonsterData monsterData = DataManager.Instance.MonsterData.GetMonsterData(stageData.Monster_Id[idx]);
            int aliveCount = Monsters.FindAll(x => x.MonsterData == monsterData).Count;
            for (int i = aliveCount; i < stageData.Monster_Spawn_Count[idx]; i++)
            {
                // 화면 가운데 기준 둘레 전체에서 무작위 (너무 멀면 배경 밖으로 나감)
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float radius = Random.Range(SpawnRadiusMin, SpawnRadiusMax);
                SpawnMonster(monsterData, stageData.Monster_Attack, stageData.Monster_Hp, stageData.Monster_Penetrate, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius);
            }
        }
    }

    const float SpawnInterval = 5f;     // 일반 몬스터는 5초마다 모자란 만큼 채움 (NightmareIdle 일반 스테이지와 같음)
    float spawnTimer = 0f;
    const float SpawnRadiusMin = 2f;
    const float SpawnRadiusMax = 4.5f;  // 화면 가로 절반(약 4.5칸)까지

    void SpawnMonster(MonsterData _monsterData, double _attack, double _hp, double _penetrate, Vector3 _position)
    {
        Monster monster = ObjectPoolManager.Instance.GetObject<Monster>("Monster", _monsterData.Resource);
        monster.transform.position = _position;
        monster.Setup(_monsterData, _attack, _hp, _penetrate);
        monster.OnDeadEnter = HandleMonsterDead;
        Monsters.Add(monster);
    }

    // 죽는 중(Monsters에서 이미 빠짐)이거나 후드가 삼키던 몬스터, 투사체, 떨어진 재화(이미 획득 처리됨)도 남지 않게 필드에서 모두 정리
    void ClearMonsters()
    {
        MainCharacter.Hood.StopDevour();
        foreach (Monster monster in FindObjectsOfType<Monster>())
            ObjectPoolManager.Instance.ReturnObject(monster.MonsterData.Resource, monster.gameObject);
        ReturnAll<Projectile>();
        ReturnAll<GoodsDrop>();
        Monsters.Clear();
    }

    void ReturnAll<T>() where T : PoolinObject
    {
        foreach (T obj in FindObjectsOfType<T>())
            ObjectPoolManager.Instance.ReturnObject(obj.key, obj.gameObject);
    }

    static readonly Vector3 BossPlayerPosition = new Vector3(0f, -3f, 0f);
    static readonly Vector3 BossPosition = new Vector3(0f, 3f, 0f);

    const float BossFadeTime = 0.2f;
    const float BossReadyTime = 0.3f;   // 보스전 끝 검은 화면 유지 시간 (시작은 보스 등장 연출)
    Vector3 playerStartPosition = Vector3.zero;

    // 일반 몬스터를 Boss_Challenge_Count마리 잡으면 도전 가능
    public void StartBossChallenge()
    {
        if (isBossChallenge || isBossStarting || killCount < BossChallengeCount || MainCharacter.State == State.Dead)
            return;

        isBossStarting = true;
        StartCoroutine(CoStartBossChallenge());
    }

    // 누르는 순간 전투를 멈추고, 멈춘 채 검은 화면 동안 일반 몬스터를 치우고 캐릭터는 아래·보스는 위에 세운 뒤 등장 연출 → 검은 화면 걷고 시작
    IEnumerator CoStartBossChallenge()
    {
        StageData stageData = CurStageData;
        Time.timeScale = 0f;
        yield return UIInGameScene.CoFadeBlack(0f, 1f, BossFadeTime);

        ClearMonsters();
        // 던전·도전은 그 던전 배경 (bg_dungeon_{Dungeon_Type}, 끝나면 SpawnMonsters가 월드 배경으로 되돌림)
        if (dungeon != null)
        {
            Sprite dungeonBg = ResourceManager.Instance.GetSprite(AtlasType.Bg, $"bg_dungeon_{dungeon.Dungeon_Type}");
            if (dungeonBg != null)
                BgRenderer.sprite = dungeonBg;
        }
        MainCharacter.SetPosition(BossPlayerPosition);
        MainCharacter.ChangeState(State.Idle);
        if (dungeon != null)
            SpawnMonster(DataManager.Instance.MonsterData.GetMonsterData(dungeon.Monster_Id), dungeon.Monster_Attack, dungeon.Monster_Hp, dungeon.Monster_Penetrate, BossPosition);
        else
            SpawnMonster(DataManager.Instance.MonsterData.GetMonsterData(stageData.Boss_Monster_Id), stageData.Boss_Monster_Attack, stageData.Boss_Monster_Hp, stageData.Boss_Monster_Penetrate, BossPosition);
        // 검은 화면 위에서 등장 연출 → 연출이 끝나면 검은 화면을 걷고 다시 움직이며 제한 시간 시작
        yield return UIInGameScene.CoBossIntro();
        yield return UIInGameScene.CoFadeBlack(1f, 0f, BossFadeTime);
        Time.timeScale = 1f;
        isBossChallenge = true;
        isBossStarting = false;
        bossTimer = dungeon != null ? dungeon.Limit_Time : DataManager.Instance.SystemData.GetFloat("Boss_Challenge_Time");
        UIInGameScene.SetBossBattle(true);
        RefreshStageUI();
    }

    // 성공하면 다음 스테이지, 실패하면 같은 스테이지 일반 몬스터로 복귀
    void EndBossChallenge(bool _isClear)
    {
        isBossChallenge = false;
        // 클리어하면 VICTORY 연출, 첫 클리어 보상(맨 앞, firstCount개)도 그 장면에서
        List<UIRewardPopup.Reward> clearRewards = new();
        int firstCount = 0;
        if (!_isClear)
        {
            // 실패(시간 초과·사망·포기)하면 자동 도전도 꺼짐
            isAutoBoss = false;
            UIInGameScene.SetAutoBoss(false);
        }
        if (dungeon != null)
        {
            // 던전·도전은 깼을 때만 단계가 오름, 처음 깬 단계면 FirstClear 보상 + 단계 보상
            if (_isClear && dungeon.Step > GetDungeonClearedStep(dungeon.Dungeon_Type))
            {
                dungeonClearedSteps[dungeon.Dungeon_Type] = dungeon.Step;
                if (dungeon.FirstClear_Reward_Id != 0)
                    clearRewards.Add(new(dungeon.FirstClear_Reward_Id, dungeon.FirstClear_Reward_Value));
                firstCount = clearRewards.Count;
            }
            if (_isClear && dungeon.Reward_Item_Id != 0)
                clearRewards.Add(new(dungeon.Reward_Item_Id, dungeon.Reward_Item_Value));
            dungeon = null;
        }
        else
        {
            if (_isClear && stageId == UserManager.Instance.MaxStageId && clearedStageIds.Add(stageId))
                clearRewards.Add(new(CurStageData.First_Clear_Goods_Id, CurStageData.First_Clear_Value));
            firstCount = clearRewards.Count;
            if (_isClear)
            {
                if (DataManager.Instance.StageData.HasStageData(stageId + 1))
                    stageId++;
                UserManager.Instance.SetMaxStage(stageId);
            }
            // 성공·실패 모두 처음부터 다시 Boss_Challenge_Count마리 (던전에서 돌아오면 그대로)
            killCount = 0;
        }
        StartCoroutine(CoEndBossChallenge(_isClear, clearRewards, firstCount));
    }

    // 검은 화면 동안 보스를 치우고 캐릭터를 처음 자리에 세운 뒤 일반 몬스터 등장
    IEnumerator CoEndBossChallenge(bool _isClear, List<UIRewardPopup.Reward> _clearRewards, int _firstCount)
    {
        isStageChanging = true;
        if (_isClear)
            yield return UIInGameScene.BossClear.CoPlay(_clearRewards, _firstCount);
        yield return UIInGameScene.CoFadeBlack(0f, 1f, BossFadeTime);

        ClearMonsters();
        // 보스전에서 쓰러졌으면 검은 화면 동안 부활 (그 전까지는 쓰러진 채로)
        if (MainCharacter.State == State.Dead)
            MainCharacter.Setup();
        MainCharacter.SetPosition(playerStartPosition);
        MainCharacter.ChangeState(State.Idle);
        yield return new WaitForSeconds(BossReadyTime);

        SpawnMonsters();
        UIInGameScene.SetBossBattle(false);
        RefreshStageUI();
        yield return UIInGameScene.CoFadeBlack(1f, 0f, BossFadeTime);
        isStageChanging = false;
        // 다음 스테이지 알림은 검은 화면이 걷힌 뒤
        if (_isClear)
            UIInGameScene.ShowStageBanner(StageName);
    }

    // 스테이지 이동 창 (보스전 중에는 스테이지 바가 보스 바라 열지 않음)
    public void ShowStageMove()
    {
        if (isBossChallenge || isBossStarting)
            return;
        PopupManager.Instance.ShowPopup<UIStageMovePopup>().Setup(stageId);
    }

    // 검은 화면 동안 필드를 치우고 고른 스테이지 일반 몬스터부터 다시
    public void MoveStage(int _stageId)
    {
        stageId = _stageId;
        killCount = 0;
        StartCoroutine(CoMoveStage());
    }

    IEnumerator CoMoveStage()
    {
        isStageChanging = true;
        yield return UIInGameScene.CoFadeBlack(0f, 1f, BossFadeTime);
        ClearMonsters();
        MainCharacter.SetPosition(playerStartPosition);
        MainCharacter.ChangeState(State.Idle);
        SpawnMonsters();
        RefreshStageUI();
        yield return UIInGameScene.CoFadeBlack(1f, 0f, BossFadeTime);
        isStageChanging = false;
        UIInGameScene.ShowStageBanner(StageName);
    }

    // 포기 = 시간 초과와 같은 실패 (같은 스테이지 일반 몬스터로)
    public void GiveUpBossChallenge()
    {
        if (isBossChallenge)
            FailBossChallenge(UIBossDefeat.Reason.GiveUp);
    }

    // 실패(시간 초과·쓰러짐·포기): 전투를 멈추고 실패 연출·결과 판 → 확인하면 원래 실패 처리, 바로가기면 성장 창 탭 열기
    void FailBossChallenge(UIBossDefeat.Reason _reason)
    {
        isBossChallenge = false;
        StartCoroutine(CoFailBossChallenge(_reason));
    }

    IEnumerator CoFailBossChallenge(UIBossDefeat.Reason _reason)
    {
        Monster boss = Monsters[0];
        string bossName = StringManager.Instance.GetStringText(boss.MonsterData.TextCode_Name);
        float hpRate = (float)(boss.CurHp.ToDouble() / boss.GetTotalHp().ToDouble());
        // 멈춘 화면에 공격 동작·이펙트가 남지 않게: 서 있는 자세로, 날아가던 것·떠 있던 것은 정리 (쓰러졌으면 쓰러진 채로)
        if (_reason != UIBossDefeat.Reason.Dead)
            MainCharacter.ChangeState(State.Idle);
        boss.ChangeState(State.Idle);
        MainCharacter.Hood.StopDevour();
        ReturnAll<Projectile>();
        ReturnAll<Effect>();
        ReturnAll<DamageText>();
        isBossFailing = true;
        Time.timeScale = 0f;
        int openTab = -1;
        yield return UIInGameScene.BossDefeat.CoPlay(_reason, bossName, hpRate, _tab => openTab = _tab);
        isBossFailing = false;
        Time.timeScale = 1f;
        EndBossChallenge(false);
        if (openTab >= 0)
            PopupManager.Instance.ShowPopup<UIGoldUpgradePopup>().SelectTab(openTab);
    }

    // 히트스톱 도중 씬이 바뀌거나 플레이를 멈춰도 게임 속도가 0으로 남지 않게
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!isBossChallenge && !isBossStarting && !isBossFailing && !isStageChanging)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= SpawnInterval)
            {
                spawnTimer = 0f;
                SpawnMonsters();
            }
            return;
        }
        if (!isBossChallenge)
            return;

        bossTimer -= Time.deltaTime;
        if (bossTimer <= 0f)
        {
            FailBossChallenge(UIBossDefeat.Reason.TimeOver);
            return;
        }
        // 보스전에는 보스 한 마리만 있음
        Monster boss = Monsters[0];
        UIInGameScene.SetBossStage(GetBossTitle(boss), (float)(boss.CurHp.ToDouble() / boss.GetTotalHp().ToDouble()), bossTimer);
    }

    // 던전·도전이면 "던전 1 · 4단계", 아니면 보스 이름
    string GetBossTitle(Monster _boss)
    {
        return dungeon != null ? string.Format(StringManager.Instance.GetStringText("Msg_DungeonTitle"), dungeonName, dungeon.Step)
            : string.Format(StringManager.Instance.GetStringText("Msg_BossName"), StringManager.Instance.GetStringText(_boss.MonsterData.TextCode_Name));
    }

    string StageName { get { return $"Stage {CurStageData.World_Id}-{CurStageData.Stage_Number}"; } }

    void RefreshStageUI()
    {
        if (isBossChallenge)
            return;

        StageData stageData = CurStageData;
        int count = Mathf.Min(killCount, BossChallengeCount);
        UIInGameScene.SetStageText(StageName, count, BossChallengeCount);
        UIInGameScene.SetBossButton(count >= BossChallengeCount, BossChallengeCount, isAutoBoss);
        if (isAutoBoss)
            StartBossChallenge();
    }

    // 켜면 처치 수가 차는 순간 자동으로 도전
    public void ToggleAutoBoss()
    {
        isAutoBoss = !isAutoBoss;
        UIInGameScene.SetAutoBoss(isAutoBoss);
        RefreshStageUI();   // 안내 문구 갱신 + 이미 차 있으면 바로 도전
    }

    // 사망 모션이 끝나면 부활 (보스전 중이었으면 실패)
    // 보스전이면 보스 실패 연출, 일반 스테이지면 가벼운 판을 보여 주고 RestartTime 뒤 그 자리에서 부활
    const float RestartTime = 3f;
    void HandleMainCharacterDeadComplete(CharacterBase _character)
    {
        if (isBossChallenge)
        {
            FailBossChallenge(UIBossDefeat.Reason.Dead);
            return;
        }
        StartCoroutine(CoDeathRestart());
    }

    IEnumerator CoDeathRestart()
    {
        yield return UIInGameScene.DeathPanel.CoPlay(RestartTime);
        MainCharacter.Setup();
    }

    const float ShakeTime = 0.15f;
    // 임팩트 프레임 (스킬 2 지연 일섬, ENGINE_REQUEST 4-2): 5프레임 정지 동안 흰 화면+검은 실루엣 / 검은 화면+흰 실루엣, 풀리면 정해진 흔들림
    const float ImpactFrameTime = 1f / 30f;
    const int ImpactScreenOrder = -9995;        // 배경(-10000) 바로 위, 캐릭터 아래
    static readonly bool[] ImpactWhiteFrames = { true, false, false, true, false };
    static readonly Vector2[] ImpactShakePixels = { new(7f, -3f), new(-6f, 3f), new(5f, -2f), new(-4f, 2f), new(2f, -1f), new(-1f, 1f) };
    static readonly Color ImpactWhiteColor = new Color32(0xff, 0xfa, 0xf4, 0xff);
    static readonly Color ImpactBlackColor = new Color32(0x0b, 0x09, 0x10, 0xff);
    static readonly Color NoTintColor = new Color(0f, 0f, 0f, 0f);
    SpriteRenderer impactScreen = null;
    Vector3 cameraPosition = Vector3.zero;
    Vector3 shakeOffset = Vector3.zero;

    // 카메라 멀리(기본)·중간·가까이. 멀리는 화면 가운데 고정, 나머지는 캐릭터를 따라가되 배경 밖은 안 보이게
    static readonly float[] CameraSizes = { 8f, 6f, 4.5f };
    int cameraZoomIndex = 0;
    public Rect BattleArea { get; private set; }   // 멀리 볼 때 화면 = 캐릭터가 몬스터를 찾는 범위 (확대해도 전투는 그대로)

    public void ToggleCameraZoom()
    {
        cameraZoomIndex = (cameraZoomIndex + 1) % CameraSizes.Length;
        MainCamera.orthographicSize = CameraSizes[cameraZoomIndex];
        UIInGameScene.SetCameraZoomStep(cameraZoomIndex);
    }

    private void LateUpdate()
    {
        Vector3 position = cameraPosition;
        if (cameraZoomIndex > 0)
        {
            Bounds bg = BgRenderer.bounds;
            float halfHeight = MainCamera.orthographicSize;
            float halfWidth = halfHeight * MainCamera.aspect;
            position.x = Mathf.Clamp(MainCharacter.transform.position.x, bg.min.x + halfWidth, bg.max.x - halfWidth);
            position.y = Mathf.Clamp(MainCharacter.transform.position.y, bg.min.y + halfHeight, bg.max.y - halfHeight);
        }
        MainCamera.transform.position = position + shakeOffset;
    }

    // 타격 순간 전체 정지 후 화면 흔들림 (흔들림은 px)
    // 캐릭터·몬스터는 멈추고 이펙트(_effect, 정지 중에도 재생)는 계속, 끝나면 흔들림 시작
    public IEnumerator CoImpactFrame(Effect _effect)
    {
        if (impactScreen == null)
        {
            impactScreen = new GameObject("ImpactScreen").AddComponent<SpriteRenderer>();
            impactScreen.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 4f, 4f), new Vector2(0.5f, 0.5f), 1f);
            impactScreen.transform.localScale = Vector3.one * 20f;     // 80칸, 화면을 다 덮음
            impactScreen.sortingOrder = ImpactScreenOrder;
        }
        impactScreen.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, 0f);
        impactScreen.gameObject.SetActive(true);
        List<CharacterBase> characters = new(Monsters) { MainCharacter };
        Time.timeScale = 0f;
        foreach (bool isWhite in ImpactWhiteFrames)
        {
            impactScreen.color = isWhite ? ImpactWhiteColor : ImpactBlackColor;
            foreach (CharacterBase character in characters)
            {
                character.SetMaterialColor(isWhite ? Color.black : Color.white);
                character.SetMaterialTintColor(isWhite ? NoTintColor : Color.white);
            }
            _effect.SetColor(isWhite ? Color.black : Color.white);
            yield return new WaitForSecondsRealtime(ImpactFrameTime);
        }
        impactScreen.gameObject.SetActive(false);
        foreach (CharacterBase character in characters)
        {
            character.SetMaterialColor(Color.white);
            character.SetMaterialTintColor(NoTintColor);
        }
        _effect.SetColor(Color.white);
        Time.timeScale = 1f;
        PlayImpactShake();
    }

    public void PlayImpactShake()
    {
        StartCoroutine(CoImpactShake());
    }

    IEnumerator CoImpactShake()
    {
        foreach (Vector2 offset in ImpactShakePixels)
        {
            shakeOffset = offset / 64f;
            yield return new WaitForSecondsRealtime(ImpactFrameTime);
        }
        shakeOffset = Vector3.zero;
    }

    public void HitStop(float _hitStopTime, float _shakePixels)
    {
        StartCoroutine(CoHitStop(_hitStopTime, _shakePixels / 64f));
    }

    IEnumerator CoHitStop(float _hitStopTime, float _shakePower)
    {
        float totalTime = _hitStopTime + ShakeTime;
        float timer = 0f;
        Time.timeScale = 0f;
        while (timer < totalTime)
        {
            // 보스 도전 시작으로 멈춘 동안에는 풀지 않음
            if (timer >= _hitStopTime && !isBossStarting && !isBossFailing)
                Time.timeScale = 1f;

            shakeOffset = (Vector3)Random.insideUnitCircle * _shakePower * (1f - timer / totalTime);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        if (!isBossStarting && !isBossFailing)
            Time.timeScale = 1f;
        shakeOffset = Vector3.zero;
    }

    const string GoldDropKey = "icon_goods_gold";   // 굿즈 차트 골드 Icon
    const string HoodExpDropKey = "icon_goods_hood_exp";    // 굿즈 차트 Hood_Exp Icon
    double gold = 0;    // 재화 저장 전까지 이번 플레이에서 모은 골드만 표시
    public double Gold { get { return gold; } }

    public bool TryUseGold(double _cost)
    {
        if (gold < _cost)
            return false;

        gold -= _cost;
        UIInGameScene.SetGold(gold);
        return true;
    }

    // 레벨·경험치·SP도 저장 전까지 이번 플레이만
    public int Level { get; private set; } = 1;
    double exp = 0;     // 현재 레벨에서 모은 경험치
    public double StatPoint { get; private set; } = 0;

    // 필요 경험치를 넘으면 연속 레벨업, 레벨마다 차트 보상(SP)
    void AddExp(double _exp)
    {
        exp += _exp;
        CharLevelData data = DataManager.Instance.CharLevelData.GetCharLevelData(Level);
        while (Level < DataManager.Instance.CharLevelData.MaxLevel && exp >= data.GetNeedExp(Level))
        {
            exp -= data.GetNeedExp(Level);
            StatPoint += data.LevelUp_Reward_Goods_Value;
            Level++;
            data = DataManager.Instance.CharLevelData.GetCharLevelData(Level) ?? data;
        }
        UIInGameScene.SetLevel(Level, Level < DataManager.Instance.CharLevelData.MaxLevel ? (float)(exp / data.GetNeedExp(Level)) : 1f);
    }

    public bool TryUseStatPoint(double _cost)
    {
        if (StatPoint < _cost)
            return false;

        StatPoint -= _cost;
        return true;
    }

    // 후드 레벨업 재화 (스테이지 DropItem, 저장 전까지 이번 플레이만)
    public double HoodGoods { get; private set; } = 0;

    public double Jewel { get; private set; } = 0;     // 보석 (저장 전까지 이번 플레이만)

    // 보상으로 재화 획득 (GoodsChart Data_Id). 화면 숫자는 보상 연출이 도착할 때 올라감
    public const int GoldGoodsId = 1;
    public const int ExpGoodsId = 2;
    public const int JewelGoodsId = 3;
    public const int HoodExpGoodsId = 5;
    public const int StatPointGoodsId = 6;
    public void AddGoods(int _goodsId, double _value)
    {
        switch (_goodsId)
        {
            case GoldGoodsId: gold += _value; break;
            case ExpGoodsId: AddExp(_value); break;
            case JewelGoodsId: Jewel += _value; break;
            case HoodExpGoodsId: HoodGoods += _value; break;
            case StatPointGoodsId: StatPoint += _value; break;
        }
    }

    readonly HashSet<int> clearedStageIds = new();     // 처음 클리어 보상 받은 스테이지 (저장 전까지 이번 플레이만)

    public bool TryUseHoodGoods(double _cost)
    {
        if (HoodGoods < _cost)
            return false;

        HoodGoods -= _cost;
        return true;
    }

    // 일반 몬스터는 5초마다 다시 채움(Update), 보스를 잡으면 도전 성공
    void HandleMonsterDead(CharacterBase _monster)
    {
        Monsters.Remove((Monster)_monster);
        StageData stageData = CurStageData;
        UIInGameScene.AddKillLog(stageData.Gold_Value, stageData.Exp_Value);
        gold += stageData.Gold_Value;
        UIInGameScene.SetGold(gold);
        AddExp(stageData.Exp_Value);
        if (Random.value < stageData.DropItem_Rate)
        {
            HoodGoods += stageData.DropItem_Value;
            UIInGameScene.AddHoodExpLog(stageData.DropItem_Value);
            GoodsDrop hoodExpDrop = ObjectPoolManager.Instance.GetObject<GoodsDrop>("Goods", HoodExpDropKey);
            hoodExpDrop.key = HoodExpDropKey;
            hoodExpDrop.Play(_monster.transform.position, MainCharacter.transform.localScale);
        }

        GoodsDrop goldDrop = ObjectPoolManager.Instance.GetObject<GoodsDrop>("Goods", GoldDropKey);
        goldDrop.key = GoldDropKey;
        goldDrop.Play(_monster.transform.position, MainCharacter.transform.localScale);

        if (isBossChallenge)
        {
            // 남은 체력이 반올림돼 1%로 남아 보이지 않게 잡은 순간 0%로
            UIInGameScene.SetBossStage(GetBossTitle((Monster)_monster), 0f, bossTimer);
            EndBossChallenge(true);
            return;
        }

        killCount++;
        RefreshStageUI();
    }
}
