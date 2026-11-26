using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        UIInGameScene.SetCameraZoomText(CameraZoomNames[cameraZoomIndex]);
        playerStartPosition = MainCharacter.transform.position;
        InGameBuffManager.Instance.Setup();
        MainCharacter.OnDeadComplete = HandleMainCharacterDeadComplete;
        MainCharacter.Setup();
        UIInGameScene.SetupSkillSlots(MainCharacter.SkillSlots);
        SpawnMonsters();
        RefreshStageUI();
    }

    // 스테이지 진행 저장 전까지 1스테이지부터 시작
    int stageId = 1;
    int killCount = 0;
    bool isBossChallenge = false;
    float bossTimer = 0f;
    public bool IsBossChallenge { get { return isBossChallenge; } }

    StageData CurStageData { get { return DataManager.Instance.StageData.GetStageData(stageId); } }
    int BossChallengeCount { get { return (int)DataManager.Instance.SystemData.GetFloat("Boss_Challenge_Count"); } }

    void SpawnMonsters()
    {
        StageData stageData = CurStageData;
        // 배경은 스테이지가 속한 월드의 World_Resource, 배경 오브젝트는 Bg 아래 같은 이름 묶음만 켬
        string worldResource = DataManager.Instance.WorldData.GetWorldData(stageData.World_Id).World_Resource;
        BgRenderer.sprite = ResourceManager.Instance.GetSprite(AtlasType.Bg, worldResource);
        obstacles.Clear();
        foreach (Transform props in BgRenderer.transform)
        {
            props.gameObject.SetActive(props.name == worldResource);
            if (props.name == worldResource)
                obstacles.AddRange(props.GetComponentsInChildren<SpriteRenderer>().Select(x => x.bounds));
        }
        // Monster_Id[i]를 Monster_Spawn_Count[i]마리 등장
        for (int idx = 0; idx < stageData.Monster_Id.Length; idx++)
        {
            MonsterData monsterData = DataManager.Instance.MonsterData.GetMonsterData(stageData.Monster_Id[idx]);
            for (int i = 0; i < stageData.Monster_Spawn_Count[idx]; i++)
            {
                // 화면 가운데 기준 위·왼쪽·오른쪽 반원 (너무 멀면 배경 밖으로 나감)
                float angle = Random.Range(0f, Mathf.PI);
                float radius = Random.Range(SpawnRadiusMin, SpawnRadiusMax);
                SpawnMonster(monsterData, stageData.Monster_Attack, stageData.Monster_Hp, stageData.Monster_Penetrate, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius);
            }
        }
    }

    // 배경 오브젝트 = 벽. 그림 아래쪽 원 안으로 캐릭터·몬스터가 들어오면 밖으로 밀어냄
    const float ObstacleRadiusRate = 0.55f;     // 그림 가로 절반 대비 (투명 여백 제외)
    const float ObstacleCenterDown = 0.3f;      // 그림 세로 절반 대비 아래로 (바닥 쪽)
    readonly List<Bounds> obstacles = new();

    void PushOutOfObstacles(Transform _tr)
    {
        foreach (Bounds obstacle in obstacles)
        {
            Vector2 center = obstacle.center + Vector3.down * obstacle.extents.y * ObstacleCenterDown;
            float radius = obstacle.extents.x * ObstacleRadiusRate;
            Vector2 diff = (Vector2)_tr.position - center;
            if (diff.sqrMagnitude < radius * radius)
                _tr.position = center + (diff.sqrMagnitude > 0f ? diff.normalized : Vector2.up) * radius;
        }
    }

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

    void ClearMonsters()
    {
        foreach (Monster monster in Monsters)
            ObjectPoolManager.Instance.ReturnObject(monster.MonsterData.Resource, monster.gameObject);
        Monsters.Clear();
    }

    static readonly Vector3 BossPlayerPosition = new Vector3(0f, -3f, 0f);
    static readonly Vector3 BossPosition = new Vector3(0f, 3f, 0f);

    const float BossFadeTime = 0.2f;
    const float BossReadyTime = 1f;     // 보스전 시작·끝 검은 화면 유지 시간
    Vector3 playerStartPosition = Vector3.zero;

    // 일반 몬스터를 Boss_Challenge_Count마리 잡으면 도전 가능
    public void StartBossChallenge()
    {
        if (isBossChallenge || killCount < BossChallengeCount || MainCharacter.State == State.Dead)
            return;

        StartCoroutine(CoStartBossChallenge());
    }

    // 검은 화면 동안 일반 몬스터를 치우고 캐릭터는 아래, 보스는 위에 거리를 두고 다시 세운 뒤 시작
    IEnumerator CoStartBossChallenge()
    {
        StageData stageData = CurStageData;
        yield return UIInGameScene.CoFadeBlack(0f, 1f, BossFadeTime);

        ClearMonsters();
        MainCharacter.SetPosition(BossPlayerPosition);
        MainCharacter.ChangeState(State.Idle);
        yield return new WaitForSeconds(BossReadyTime);

        MonsterData bossData = DataManager.Instance.MonsterData.GetMonsterData(stageData.Boss_Monster_Id);
        SpawnMonster(bossData, stageData.Boss_Monster_Attack, stageData.Boss_Monster_Hp, stageData.Boss_Monster_Penetrate, BossPosition);
        isBossChallenge = true;
        bossTimer = DataManager.Instance.SystemData.GetFloat("Boss_Challenge_Time");
        RefreshStageUI();
        yield return UIInGameScene.CoFadeBlack(1f, 0f, BossFadeTime);
    }

    // 성공하면 다음 스테이지, 실패하면 같은 스테이지 일반 몬스터로 복귀
    void EndBossChallenge(bool _isClear)
    {
        isBossChallenge = false;
        if (_isClear)
        {
            if (DataManager.Instance.StageData.HasStageData(stageId + 1))
                stageId++;
            killCount = 0;
        }
        StartCoroutine(CoEndBossChallenge());
    }

    // 검은 화면 동안 보스를 치우고 캐릭터를 처음 자리에 세운 뒤 일반 몬스터 등장
    IEnumerator CoEndBossChallenge()
    {
        yield return UIInGameScene.CoFadeBlack(0f, 1f, BossFadeTime);

        ClearMonsters();
        MainCharacter.SetPosition(playerStartPosition);
        MainCharacter.ChangeState(State.Idle);
        yield return new WaitForSeconds(BossReadyTime);

        SpawnMonsters();
        RefreshStageUI();
        yield return UIInGameScene.CoFadeBlack(1f, 0f, BossFadeTime);
    }

    // 히트스톱 도중 씬이 바뀌거나 플레이를 멈춰도 게임 속도가 0으로 남지 않게
    private void OnDestroy()
    {
        Time.timeScale = 1f;
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
        UIInGameScene.SetStageText($"Stage {stageData.World_Id}-{stageData.Stage_Number}", count, BossChallengeCount);
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
    Vector3 shakeOffset = Vector3.zero;

    // 카메라 멀리(기본)·중간·가까이. 멀리는 화면 가운데 고정, 나머지는 캐릭터를 따라가되 배경 밖은 안 보이게
    static readonly float[] CameraSizes = { 8f, 6f, 4.5f };
    static readonly string[] CameraZoomNames = { "멀리", "중간", "가까이" };
    int cameraZoomIndex = 0;
    public Rect BattleArea { get; private set; }   // 멀리 볼 때 화면 = 캐릭터가 몬스터를 찾는 범위 (확대해도 전투는 그대로)

    public void ToggleCameraZoom()
    {
        cameraZoomIndex = (cameraZoomIndex + 1) % CameraSizes.Length;
        MainCamera.orthographicSize = CameraSizes[cameraZoomIndex];
        UIInGameScene.SetCameraZoomText(CameraZoomNames[cameraZoomIndex]);
    }

    private void LateUpdate()
    {
        // 후드가 삼키는 중(Pause)인 몬스터는 입으로 끌려가야 하므로 제외
        PushOutOfObstacles(MainCharacter.transform);
        foreach (Monster monster in Monsters)
            if (monster.State != State.Pause)
                PushOutOfObstacles(monster.transform);

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

            shakeOffset = (Vector3)Random.insideUnitCircle * _shakePower * (1f - timer / totalTime);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 1f;
        shakeOffset = Vector3.zero;
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
