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
        BgRenderer.sprite = ResourceManager.Instance.GetSprite(AtlasType.Bg, "bg_world_1");
        InGameBuffManager.Instance.Setup();
        MainCharacter.OnDeadComplete = HandleMainCharacterDeadComplete;
        MainCharacter.Setup();
        SpawnMonsters();
        RefreshStageUI();
    }

    // 스테이지 진행 저장 전까지 1스테이지부터 시작
    int stageId = 1;
    int killCount = 0;
    bool isBossChallenge = false;
    float bossTimer = 0f;

    StageData CurStageData { get { return DataManager.Instance.StageData.GetStageData(stageId); } }
    int BossChallengeCount { get { return (int)DataManager.Instance.SystemData.GetFloat("Boss_Challenge_Count"); } }

    void SpawnMonsters()
    {
        StageData stageData = CurStageData;
        // Monster_Id[i]를 Monster_Spawn_Count[i]마리 등장
        for (int idx = 0; idx < stageData.Monster_Id.Length; idx++)
        {
            MonsterData monsterData = DataManager.Instance.MonsterData.GetMonsterData(stageData.Monster_Id[idx]);
            for (int i = 0; i < stageData.Monster_Spawn_Count[idx]; i++)
                SpawnMonster(monsterData, stageData.Monster_Attack, stageData.Monster_Hp, stageData.Monster_Penetrate, new Vector3(Random.Range(-3.5f, 3.5f), Random.Range(1f, 6f), 0f));
        }
    }

    void SpawnMonster(MonsterData _monsterData, double _attack, double _hp, double _penetrate, Vector3 _position)
    {
        Monster monster = ObjectPoolManager.Instance.GetObject<Monster>("Monster", _monsterData.Resource);
        monster.transform.position = _position;
        monster.Setup(_monsterData, _attack, _hp, _penetrate);
        monster.OnDeadEnter = HandleMonsterDead;
        Monsters.Add(monster);
    }

    void ClearMonsters()
    {
        foreach (Monster monster in Monsters)
            ObjectPoolManager.Instance.ReturnObject(monster.MonsterData.Resource, monster.gameObject);
        Monsters.Clear();
    }

    static readonly Vector3 BossPlayerPosition = new Vector3(0f, -3f, 0f);
    static readonly Vector3 BossPosition = new Vector3(0f, 3f, 0f);

    // 일반 몬스터를 Boss_Challenge_Count마리 잡으면 도전 가능
    public void StartBossChallenge()
    {
        if (isBossChallenge || killCount < BossChallengeCount || MainCharacter.State == State.Dead)
            return;

        // 일반 몬스터를 치우고 캐릭터는 아래, 보스는 위에 거리를 두고 다시 세움
        StageData stageData = CurStageData;
        ClearMonsters();
        MainCharacter.SetPosition(BossPlayerPosition);
        MainCharacter.ChangeState(State.Idle);
        MonsterData bossData = DataManager.Instance.MonsterData.GetMonsterData(stageData.Boss_Monster_Id);
        SpawnMonster(bossData, stageData.Boss_Monster_Attack, stageData.Boss_Monster_Hp, stageData.Boss_Monster_Penetrate, BossPosition);

        isBossChallenge = true;
        bossTimer = DataManager.Instance.SystemData.GetFloat("Boss_Challenge_Time");
        RefreshStageUI();
    }

    // 성공하면 다음 스테이지, 실패하면 같은 스테이지 일반 몬스터로 복귀
    void EndBossChallenge(bool _isClear)
    {
        isBossChallenge = false;
        ClearMonsters();
        if (_isClear)
        {
            if (DataManager.Instance.StageData.HasStageData(stageId + 1))
                stageId++;
            killCount = 0;
        }
        SpawnMonsters();
        RefreshStageUI();
    }

    private void Update()
    {
        if (!isBossChallenge)
            return;

        bossTimer -= Time.deltaTime;
        if (bossTimer <= 0f)
            EndBossChallenge(false);
        else
            UIInGameScene.SetBossButton(false, string.Format(StringManager.Instance.GetStringText("Msg_BossChallengeTime"), bossTimer.ToString("0.0")));
    }

    void RefreshStageUI()
    {
        StageData stageData = CurStageData;
        int count = Mathf.Min(killCount, BossChallengeCount);
        UIInGameScene.SetStageText($"Stage {stageData.World_Id}-{stageData.Stage_Number}", $"{count}/{BossChallengeCount}");
        if (isBossChallenge)
            return;

        UIInGameScene.SetBossButton(count >= BossChallengeCount, StringManager.Instance.GetStringText("Name_BossChallenge"));
    }

    // 사망 모션이 끝나면 부활 (보스전 중이었으면 실패)
    void HandleMainCharacterDeadComplete(CharacterBase _character)
    {
        if (isBossChallenge)
            EndBossChallenge(false);
        MainCharacter.Setup();
    }

    const float ShakeTime = 0.15f;
    Vector3 cameraPosition = Vector3.zero;

    // 타격 순간 전체 정지 후 화면 흔들림 (흔들림은 px)
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
            if (timer >= _hitStopTime)
                Time.timeScale = 1f;

            MainCamera.transform.position = cameraPosition + (Vector3)Random.insideUnitCircle * _shakePower * (1f - timer / totalTime);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 1f;
        MainCamera.transform.position = cameraPosition;
    }

    const string GoldDropKey = "icon_goods_gold";   // 굿즈 차트 골드 Icon
    double gold = 0;    // 재화 저장 전까지 이번 플레이에서 모은 골드만 표시

    // 일반 몬스터는 모두 잡으면 다시 등장, 보스를 잡으면 도전 성공
    void HandleMonsterDead(CharacterBase _monster)
    {
        Monsters.Remove((Monster)_monster);
        StageData stageData = CurStageData;
        UIInGameScene.AddKillLog(stageData.Gold_Value, stageData.Exp_Value);
        gold += stageData.Gold_Value;
        UIInGameScene.SetGold(gold);

        GoodsDrop goldDrop = ObjectPoolManager.Instance.GetObject<GoodsDrop>("Goods", GoldDropKey);
        goldDrop.key = GoldDropKey;
        goldDrop.Play(_monster.transform.position, MainCharacter.transform.localScale);

        if (isBossChallenge)
        {
            EndBossChallenge(true);
            return;
        }

        killCount++;
        RefreshStageUI();
        if (Monsters.Count == 0)
            SpawnMonsters();
    }
}
