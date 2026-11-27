using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInGameScene : MonoBehaviour
{
    public JoystickController JoystickController = null;

    [Header("Roll")]
    [SerializeField] Image rollCoolImage = null;
    [SerializeField] TMP_Text rollCoolText = null;

    [Header("Skill")]
    [SerializeField] UISkillSlot[] skillSlots = null;

    [Header("Stage")]
    [SerializeField] TMP_Text stageText = null;
    [SerializeField] Button stageMoveButton = null;      // 스테이지 바 전체
    [SerializeField] TMP_Text killCountText = null;
    [SerializeField] Image stageProgressFill = null;
    [SerializeField] Button bossButton = null;
    [SerializeField] TMP_Text bossButtonText = null;

    // 보스 도전 (시안 Plan/mockups/review/boss_challenge_states.png, kit_cocoa NOTES '보스 도전')
    [Header("Boss")]
    [SerializeField] GameObject bossChallengeGroup = null;  // 도전 버튼·장식·안내 (보스전 중 숨김)
    [SerializeField] Image bossButtonIcon = null;
    [SerializeField] GameObject bossAlert = null;
    [SerializeField] TMP_Text bossHintText = null;
    [SerializeField] Sprite bossButtonOnSprite = null;      // btn_main
    [SerializeField] Sprite bossButtonOffSprite = null;     // hud_pill
    [SerializeField] Sprite skullSprite = null;             // 금색 버튼 위
    [SerializeField] Sprite skullLightSprite = null;        // 어두운 판 위
    [SerializeField] GameObject bossBattleGroup = null;     // 남은 시간·포기 (보스전 중만)
    [SerializeField] TMP_Text bossTimerText = null;
    [SerializeField] Button giveUpButton = null;
    [SerializeField] Image stageSkullIcon = null;           // 보스전 중 스테이지 바 앞 해골
    [SerializeField] CanvasGroup[] topInfoGroups = null;    // 이름·경험치, 재화 (보스전 중 흐리게)
    [SerializeField] Button autoBossButton = null;          // 자동 보스 도전 판 (보스 버튼 왼쪽)
    [SerializeField] TMP_Text autoBossText = null;
    [SerializeField] Image autoBossSwitch = null;
    [SerializeField] Sprite switchOnSprite = null;
    [SerializeField] Sprite switchOffSprite = null;
    static readonly Color CreamTextColor = new Color32(0xff, 0xf3, 0xdc, 0xff);
    static readonly Color AutoSoonTextColor = new Color32(0xff, 0xe7, 0xa0, 0xff);
    static readonly Color BossOffTextColor = new Color32(0xc2, 0xa8, 0x88, 0xff);
    static readonly Color BossOnTextColor = new Color32(0x2a, 0x1a, 0x14, 0xff);
    static readonly Color StageFillColor = new Color32(0xee, 0x7a, 0x5c, 0xff);
    static readonly Color BossFillColor = new Color32(0xc8, 0x4b, 0x3a, 0xff);
    const float TopInfoBossAlpha = 0.5f;
    const float StageSkullSpace = 34f;  // 보스 이름 앞 해골 자리

    [Header("Profile")]
    [SerializeField] TMP_Text nicknameText = null;
    [SerializeField] TMP_Text goldText = null;
    [SerializeField] TMP_Text gemText = null;
    [SerializeField] TMP_Text goodsFloatText = null;    // 보상 도착 시 재화 칸 왼쪽에 "+5,000" (복제해서 씀)
    [System.Serializable]
    public struct GoodsIcon
    {
        public int GoodsId;
        public Sprite Icon;
    }
    [SerializeField] GoodsIcon[] goodsIcons = null;     // 보상 칸·날아가는 아이콘 (GoodsChart Data_Id별)
    const float GoodsCountTime = 0.3f;
    const float GoodsFloatTime = 0.6f;
    const float GoodsFloatRise = 30f;
    [SerializeField] TMP_Text levelText = null;
    [SerializeField] Image expFill = null;

    [Header("Camera")]
    [SerializeField] Button cameraZoomButton = null;
    [SerializeField] Image[] cameraZoomDots = null;     // 버튼 아래 단계 점 (왼쪽부터 1·2·3단계)
    [SerializeField] Sprite dotStepOnSprite = null;
    [SerializeField] Sprite dotStepOffSprite = null;
    [SerializeField] Button goldUpgradeButton = null;
    [SerializeField] GameObject[] hideOnWindow = null;  // 창이 열린 동안 숨김 (창 뒤로 비쳐 보이므로)

    // 보스 등장 연출 (kit_cocoa LAYOUT_battle.md '보스 등장 연출', 0.9초로 줄임)
    [Header("Boss Intro")]
    [SerializeField] CanvasGroup bossIntroGroup = null;     // 띠·빛줄기·문장 묶음 (검은 화면 위, 끝에 흐려짐)
    [SerializeField] RectTransform bossIntroBand = null;    // 가운데에서 양옆으로 펼쳐짐
    [SerializeField] RectTransform bossIntroRays = null;    // 문장과 함께 등장, 천천히 회전
    [SerializeField] RectTransform bossIntroEmblem = null;  // 0 → 1.15 → 1.0
    const float BossIntroTime = 0.9f;       // 문서 1.2초를 0.75배로 줄임 (아래 시간 모두 같은 비율)
    const float BandOpenTime = 0.15f;
    const float EmblemStart = 0.19f;
    const float EmblemPopTime = 0.11f;      // 1.15까지
    const float EmblemSettleTime = 0.08f;   // 1.0으로
    const float RaysDegreePerSec = 90f;     // 1바퀴 4초
    const float FadeOutStart = 0.75f;

    // 보스 클리어 연출 (kit_cocoa LAYOUT_clear.md) + 다음 스테이지 알림
    [Header("Clear")]
    public UIBossClear BossClear = null;
    [SerializeField] CanvasGroup stageBanner = null;        // 보스 도전 버튼 아래 "Stage 1-8 / 새 스테이지 시작"
    [SerializeField] TMP_Text stageBannerText = null;
    const float StageBannerTime = 1f;
    const float StageBannerFadeTime = 0.2f;

    // 실패 연출 (kit_cocoa LAYOUT_fail.md)
    [Header("Fail")]
    public UIBossDefeat BossDefeat = null;
    public UIDeathPanel DeathPanel = null;

    [Header("Black Screen")]
    [SerializeField] Image blackScreen = null;  // 켜져 있는 동안 터치도 막음

    [Header("Kill Log")]
    [SerializeField] TMP_Text killLogText = null;
    const int KillLogMax = 5;
    const float KillLogShowTime = 1f;
    const float KillLogFadeTime = 0.5f;
    readonly List<(string text, float startTime)> killLogs = new();

    private void Start()
    {
        bossButton.onClick.AddListener(InGameManager.Instance.StartBossChallenge);
        stageMoveButton.onClick.AddListener(InGameManager.Instance.ShowStageMove);
        giveUpButton.onClick.AddListener(InGameManager.Instance.GiveUpBossChallenge);
        autoBossButton.onClick.AddListener(InGameManager.Instance.ToggleAutoBoss);
        SetAutoBoss(false);
        cameraZoomButton.onClick.AddListener(InGameManager.Instance.ToggleCameraZoom);
        goldUpgradeButton.onClick.AddListener(() => PopupManager.Instance.ShowPopup<UIGoldUpgradePopup>());
        nicknameText.text = BackEnd.Backend.UserNickName;
    }

    public void SetActiveBattleHud(bool _active)
    {
        foreach (GameObject obj in hideOnWindow)
            obj.SetActive(_active);
    }

    // 캐릭터 슬롯이 만들어진 뒤 호출
    public void SetupSkillSlots(List<MainCharacter.SkillSlot> _slots)
    {
        for (int i = 0; i < skillSlots.Length; i++)
            skillSlots[i].Setup(_slots[i]);
    }

    private void Update()
    {
        float remainTime = InGameManager.Instance.MainCharacter.RollRemainTime;
        rollCoolImage.fillAmount = remainTime / DataManager.Instance.SystemData.GetFloat("Roll_Cooltime");
        rollCoolText.text = remainTime > 0f ? remainTime.ToString("0.0") : string.Empty;

        if (killLogs.Count > 0)
            RefreshKillLog();
    }

    // 처치할 때마다 한 줄 추가, 최근 KillLogMax줄만 표시
    public void AddKillLog(double _gold, double _exp)
    {
        AddLog(string.Format(StringManager.Instance.GetStringText("Msg_KillReward"), _gold, _exp));
    }

    public void AddHoodExpLog(double _value)
    {
        AddLog(string.Format(StringManager.Instance.GetStringText("Msg_HoodExpReward"), _value));
    }

    void AddLog(string _text)
    {
        killLogs.Add((_text, Time.time));
        if (killLogs.Count > KillLogMax)
            killLogs.RemoveAt(0);
        RefreshKillLog();
    }

    // 각 줄은 KillLogShowTime초 뒤 KillLogFadeTime초에 걸쳐 투명해지며 사라짐
    void RefreshKillLog()
    {
        killLogs.RemoveAll(x => Time.time - x.startTime >= KillLogShowTime + KillLogFadeTime);
        killLogText.text = string.Join("\n", killLogs.Select(x =>
        {
            float alpha = 1f - Mathf.Clamp01((Time.time - x.startTime - KillLogShowTime) / KillLogFadeTime);
            return $"<alpha=#{(int)(alpha * 255):X2}>{x.text}";
        }));
    }

    public void SetStageText(string _stage, int _killCount, int _maxCount)
    {
        // 누르면 스테이지 이동 창이 열린다는 표시
        stageText.text = $"{_stage}<space=8><size=14><color=#E6C27A>▼";
        killCountText.text = $"{_killCount}/{_maxCount}";
        stageProgressFill.fillAmount = (float)_killCount / _maxCount;
    }

    // 게임 시간이 멈춰 있어도 돌아가게 unscaled 시간 사용
    public IEnumerator CoBossIntro()
    {
        bossIntroGroup.gameObject.SetActive(true);
        for (float t = 0f; t < BossIntroTime; t += Time.unscaledDeltaTime)
        {
            float emblem = t < EmblemStart ? 0f
                : t < EmblemStart + EmblemPopTime ? Mathf.Lerp(0f, 1.15f, (t - EmblemStart) / EmblemPopTime)
                : Mathf.Lerp(1.15f, 1f, (t - EmblemStart - EmblemPopTime) / EmblemSettleTime);
            bossIntroBand.localScale = new Vector3(Mathf.Clamp01(t / BandOpenTime), 1f, 1f);
            bossIntroEmblem.localScale = Vector3.one * emblem;
            bossIntroRays.localScale = Vector3.one * Mathf.Min(emblem, 1f);
            bossIntroRays.localEulerAngles = new Vector3(0f, 0f, -t * RaysDegreePerSec);
            bossIntroGroup.alpha = 1f - Mathf.Clamp01((t - FadeOutStart) / (BossIntroTime - FadeOutStart));
            yield return null;
        }
        bossIntroGroup.gameObject.SetActive(false);
    }

    // 검은 화면 투명도 _from → _to
    public IEnumerator CoFadeBlack(float _from, float _to, float _time)
    {
        blackScreen.gameObject.SetActive(true);
        for (float timer = 0f; timer < _time; timer += Time.unscaledDeltaTime)
        {
            blackScreen.color = new Color(0f, 0f, 0f, Mathf.Lerp(_from, _to, timer / _time));
            yield return null;
        }
        blackScreen.color = new Color(0f, 0f, 0f, _to);
        blackScreen.gameObject.SetActive(_to > 0f);
    }

    // 지금 단계 점만 켬
    public void SetCameraZoomStep(int _index)
    {
        for (int i = 0; i < cameraZoomDots.Length; i++)
            cameraZoomDots[i].sprite = i == _index ? dotStepOnSprite : dotStepOffSprite;
    }

    public void SetGold(double _gold)
    {
        goldText.text = _gold.ToString("N0");
    }

    public Sprite GetGoodsIcon(int _goodsId)
    {
        return System.Array.Find(goodsIcons, x => x.GoodsId == _goodsId).Icon;
    }

    // 새 스테이지 이름을 1초 띄웠다 사라짐
    public void ShowStageBanner(string _stage)
    {
        stageBannerText.text = _stage;
        StopCoroutine(nameof(CoStageBanner));
        StartCoroutine(nameof(CoStageBanner));
    }

    IEnumerator CoStageBanner()
    {
        stageBanner.gameObject.SetActive(true);
        float total = StageBannerTime + StageBannerFadeTime * 2f;
        for (float t = 0f; t < total; t += Time.deltaTime)
        {
            stageBanner.alpha = Mathf.Min(t / StageBannerFadeTime, (total - t) / StageBannerFadeTime, 1f);
            yield return null;
        }
        stageBanner.gameObject.SetActive(false);
    }

    // 보상 아이콘이 날아갈 곳: 골드·보석은 위쪽 재화 칸, 그 외는 하단 성장 탭
    public RectTransform GetGoodsTarget(int _goodsId)
    {
        if (_goodsId == InGameManager.GoldGoodsId)
            return goldText.rectTransform;
        if (_goodsId == InGameManager.JewelGoodsId)
            return gemText.rectTransform;
        return (RectTransform)goldUpgradeButton.transform;
    }

    // 보상 도착: 칸이 잠깐 커졌다 돌아오고, 골드·보석은 숫자를 세며 올림 + 왼쪽에 "+N"
    public void PlayGoodsArrive(int _goodsId, double _value)
    {
        RectTransform target = GetGoodsTarget(_goodsId);
        StartCoroutine(CoPulse(target));
        if (_goodsId == InGameManager.GoldGoodsId)
            StartCoroutine(CoCountUp(goldText, InGameManager.Instance.Gold - _value, InGameManager.Instance.Gold));
        else if (_goodsId == InGameManager.JewelGoodsId)
            StartCoroutine(CoCountUp(gemText, InGameManager.Instance.Jewel - _value, InGameManager.Instance.Jewel));
        else
            return;

        var floatText = Instantiate(goodsFloatText, goodsFloatText.transform.parent);
        // 재화 숫자의 왼쪽 위 기준점에서 왼쪽, 세로 가운데 (글자는 오른쪽 정렬·피벗 오른쪽)
        floatText.rectTransform.position = target.position;
        floatText.rectTransform.anchoredPosition += new Vector2(-40f, -target.rect.height * 0.5f);
        floatText.text = string.Format("+{0:N0}", _value);
        floatText.gameObject.SetActive(true);
        StartCoroutine(CoFloat(floatText));
    }

    IEnumerator CoPulse(RectTransform _target)
    {
        for (float t = 0f; t < GoodsCountTime; t += Time.unscaledDeltaTime)
        {
            _target.localScale = Vector3.one * (1f + 0.15f * Mathf.Sin(t / GoodsCountTime * Mathf.PI));
            yield return null;
        }
        _target.localScale = Vector3.one;
    }

    IEnumerator CoCountUp(TMP_Text _text, double _from, double _to)
    {
        for (float t = 0f; t < GoodsCountTime; t += Time.unscaledDeltaTime)
        {
            _text.text = (_from + (_to - _from) * (t / GoodsCountTime)).ToString("N0");
            yield return null;
        }
        _text.text = _to.ToString("N0");
    }

    // 재화 칸 왼쪽에서 떠오르며 사라짐
    IEnumerator CoFloat(TMP_Text _text)
    {
        Vector2 start = _text.rectTransform.anchoredPosition;
        for (float t = 0f; t < GoodsFloatTime; t += Time.unscaledDeltaTime)
        {
            _text.rectTransform.anchoredPosition = start + Vector2.up * GoodsFloatRise * (t / GoodsFloatTime);
            _text.alpha = 1f - t / GoodsFloatTime;
            yield return null;
        }
        Destroy(_text.gameObject);
    }

    public void SetLevel(int _level, float _expRate)
    {
        levelText.text = string.Format("Lv.{0}", _level);
        expFill.fillAmount = _expRate;
    }

    // 진행 중: 어두운 판 + 흐린 해골·글자 + 아래 안내 / 도전 가능: 금색 버튼 + 해골 + 알림 점
    // 안내: 진행 중 "N마리를 잡으면…" / 도전 가능 + 자동 켜짐 "자동 도전: 곧 보스전을…"
    public void SetBossButton(bool _canChallenge, int _challengeCount, bool _isAuto)
    {
        bossButton.interactable = _canChallenge;
        bossButton.image.sprite = _canChallenge ? bossButtonOnSprite : bossButtonOffSprite;
        bossButtonIcon.sprite = _canChallenge ? skullSprite : skullLightSprite;
        bossButtonText.text = StringManager.Instance.GetStringText("Name_BossChallenge");
        bossButtonText.color = _canChallenge ? BossOnTextColor : BossOffTextColor;
        bossAlert.SetActive(_canChallenge);
        bossHintText.gameObject.SetActive(!_canChallenge || _isAuto);
        bossHintText.text = _canChallenge ? StringManager.Instance.GetStringText("Msg_AutoBossSoon") : string.Format(StringManager.Instance.GetStringText("Msg_BossChallengeHint"), _challengeCount);
        bossHintText.color = _canChallenge ? AutoSoonTextColor : CreamTextColor;
    }

    // 켜짐 = 크림 글자, 꺼짐 = 흐린 글자
    public void SetAutoBoss(bool _on)
    {
        autoBossSwitch.sprite = _on ? switchOnSprite : switchOffSprite;
        autoBossText.color = _on ? CreamTextColor : BossOffTextColor;
    }


    // 보스전: 스테이지 바를 보스 바로, 도전 버튼 대신 남은 시간·포기, 이름·재화는 흐리게
    public void SetBossBattle(bool _active)
    {
        bossChallengeGroup.SetActive(!_active);
        bossBattleGroup.SetActive(_active);
        stageSkullIcon.gameObject.SetActive(_active);
        stageText.margin = new Vector4(_active ? StageSkullSpace : 0f, 0f, 0f, 0f);
        stageProgressFill.color = _active ? BossFillColor : StageFillColor;
        foreach (CanvasGroup group in topInfoGroups)
            group.alpha = _active ? TopInfoBossAlpha : 1f;
    }

    public void SetBossStage(string _bossName, float _hpRate, float _remainTime)
    {
        stageText.text = string.Format(StringManager.Instance.GetStringText("Msg_BossName"), _bossName);
        killCountText.text = string.Format("{0:0}%", _hpRate * 100f);
        stageProgressFill.fillAmount = _hpRate;
        int seconds = Mathf.CeilToInt(_remainTime);
        bossTimerText.text = string.Format("{0}:{1:00}", seconds / 60, seconds % 60);
    }
}
