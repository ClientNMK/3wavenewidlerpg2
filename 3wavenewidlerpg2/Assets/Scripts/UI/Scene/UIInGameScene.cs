using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameUtils;

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
    [SerializeField] Sprite goBestSprite = null;            // 아래 스테이지: 최고 스테이지로 (위 화살표 2개)
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
    const float BossIconSpace = 40f;    // 보스 버튼 아이콘 32 + 글자까지 8

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
    [SerializeField] Button contentButton = null;       // 하단 컨텐츠 탭
    [SerializeField] Button missionButton = null;       // 하단 미션 탭
    [SerializeField] Button costumeButton = null;       // 오른쪽 사이드 의상 버튼
    [SerializeField] GameObject costumeDot = null;      // 받을 코스튬 포인트 보상이 있을 때
    [SerializeField] RectTransform navOn = null;        // 하단 탭 켜짐 표시 (열린 창의 탭으로 옮김)
    [SerializeField] GameObject[] hideOnWindow = null;  // 창이 열린 동안 숨김 (창 뒤로 비쳐 보이므로)

    // 일반 스테이지 보스 등장 연출 (kit_cocoa LAYOUT_battle.md '보스 등장 연출: 일반 스테이지 보스', 0.8초)
    [Header("Boss Intro")]
    [SerializeField] CanvasGroup bossIntroGroup = null;     // 띠·빛줄기·문장 묶음 (검은 화면 위, 끝에 흐려짐)
    [SerializeField] RectTransform bossIntroBand = null;    // 가운데에서 양옆으로 펼쳐짐
    [SerializeField] RectTransform bossIntroRays = null;    // 문장과 함께 등장, 천천히 회전
    [SerializeField] RectTransform bossIntroEmblem = null;  // 0 → 1.15 → 1.0
    [SerializeField] Image bossIntroFlash = null;
    const float BossIntroTime = 0.8f;
    const float BandOpenTime = 0.12f;
    const float EmblemStart = 0.12f;
    const float EmblemPopTime = 0.11f;      // 1.15까지
    const float EmblemSettleTime = 0.07f;   // 1.0으로
    const float IntroFlashStart = 0.32f;    // 흰빛 0.08초 1번
    const float IntroFlashTime = 0.08f;
    const float RaysDegreePerSec = 90f;     // 1바퀴 4초
    const float FadeOutStart = 0.62f;

    // 던전·도전 입장 이름 컷인 (kit_cocoa LAYOUT_battle.md '컨텐츠 보스 등장 연출: 이름 컷인', 시간은 문서 시간표, 거리는 ×2)
    [Header("Boss Cut-in")]
    [SerializeField] CanvasGroup bossCutIn = null;          // 검은 화면 위, 끝에 흐려짐
    [SerializeField] RectTransform cutInBand = null;        // 가운데 줄에서 위아래로 펼쳐짐 (띠 밖은 잘림)
    [SerializeField] RectTransform[] cutInLines = null;     // 속도선 (오른쪽 밖 → 왼쪽 밖)
    [SerializeField] Spine.Unity.SkeletonGraphic cutInBoss = null;
    [SerializeField] RectTransform[] cutInRows = null;      // 0 "BOSS", 1 이름, 2 밑줄·스테이지 (왼쪽 밖에서 들어옴)
    [SerializeField] TMP_Text cutInNameText = null;
    [SerializeField] TMP_Text cutInStageText = null;
    [SerializeField] Image cutInFlash = null;
    [SerializeField] Image cutInDim = null;                 // 띠 밖 화면 전체 어둡게
    const float CutInTime = 1.35f;
    const float CutInOpenTime = 0.15f;
    const float CutInCloseStart = 1.15f;
    const float CutInDim = 0.72f;
    const float CutInBodyHeight = 190f;         // 보이는 몸 높이를 이만큼(문서 px)에 맞춤, 배율은 0.5 단위
    static readonly Vector2 CutInBodyFoot = new(524f, -336f);  // 보이는 몸 아래 가운데 = 띠 안 (262, 168) ×2
    const float CutInBossEnter = 520f;          // 오른쪽 밖에서 미끄러져 들어옴
    const float CutInBossDrift = 16f;           // 멈춘 동안 왼쪽으로 천천히
    const float CutInTextEnter = 400f;
    const float CutInTextTime = 0.18f;
    const float CutInExitDistance = 120f;       // 닫힐 때 왼쪽으로 빠짐
    const float CutInLinePassTime = 0.5f;
    const float CutInBandWidth = 720f;

    // 보스 클리어 연출 (kit_cocoa LAYOUT_clear.md) + 다음 스테이지 알림
    [Header("Clear")]
    public UIBossClear BossClear = null;
    [UnityEngine.Serialization.FormerlySerializedAs("huntStats")] public UIHuntStats HuntStats = null;
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
        bossButton.onClick.AddListener(InGameManager.Instance.OnClickBossButton);
        stageMoveButton.onClick.AddListener(InGameManager.Instance.ShowStageMove);
        giveUpButton.onClick.AddListener(InGameManager.Instance.GiveUpBossChallenge);
        autoBossButton.onClick.AddListener(InGameManager.Instance.ToggleAutoBoss);
        SetAutoBoss(false);
        cameraZoomButton.onClick.AddListener(InGameManager.Instance.ToggleCameraZoom);
        goldUpgradeButton.onClick.AddListener(() => PopupManager.Instance.ShowPopup<UIGoldUpgradePopup>());
        contentButton.onClick.AddListener(() => PopupManager.Instance.ShowPopup<UIContentPopup>());
        missionButton.onClick.AddListener(() => PopupManager.Instance.ShowPopup<UIMissionPopup>());
        costumeButton.onClick.AddListener(() => PopupManager.Instance.ShowPopup<UICostumePopup>());
        navOn.gameObject.SetActive(false);
        nicknameText.text = BackEnd.Backend.UserNickName;
    }

    // 하단 탭 창이 열리면 전투 HUD를 숨기고 그 탭에 켜짐 표시, 닫히면(null) 되돌림. 의상 창은 하단 탭이 없어 HUD만 숨김
    public void SetTabWindow(UIPopup _window)
    {
        foreach (GameObject obj in hideOnWindow)
            obj.SetActive(_window == null);
        navOn.gameObject.SetActive(_window != null && !(_window is UICostumePopup));
        if (!navOn.gameObject.activeSelf)
            return;
        navOn.SetParent((_window is UIContentPopup ? contentButton : _window is UIMissionPopup ? missionButton : goldUpgradeButton).transform, false);
        navOn.SetAsFirstSibling();
    }

    public void SetCostumeAlert(bool _isOn)
    {
        costumeDot.SetActive(_isOn);
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
        HuntStats.AddKill(_gold, _exp);
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

    // 아래(깬) 스테이지면 처치 수 대신 "반복 사냥", 진행바는 흐린 색으로 꽉 채움 (LAYOUT_battle '아래 스테이지에서 방치 중')
    public void SetStageText(string _stage, int _killCount, int _maxCount, bool _isCleared)
    {
        // 누르면 스테이지 이동 창이 열린다는 표시
        stageText.text = $"{_stage}<space=8><size=14><color=#E6C27A>▼";
        killCountText.text = _isCleared ? StringManager.Instance.GetStringText("Msg_RepeatHunt") : $"{_killCount}/{_maxCount}";
        killCountText.color = _isCleared ? BossOffTextColor : CreamTextColor;
        stageProgressFill.fillAmount = _isCleared ? 1f : (float)_killCount / _maxCount;
        stageProgressFill.color = _isCleared ? BossOffTextColor : StageFillColor;
    }

    // 스킬 4 화면 가르기 (ENGINE_REQUEST 4-4): 게임 화면(메인 카메라)을 그림으로 받아 화면 가운데를 지나는 선(가로 4 : 세로 1)을 경계로
    // 위쪽은 선 방향(오른쪽 위), 아래쪽은 반대로 밀었다가 원래대로. HUD는 그대로
    const float SplitAngle = 14.036f;           // atan(1/4)
    const float SplitShift = 26f / 900f;        // 화면 폭 900px 중 26px (0.16초에 빠르게)
    const float SplitExtraShift = 6f / 900f;    // 이후 끝까지 천천히
    const float SplitFastTime = 0.16f;
    const float SplitHalfSize = 4000f;          // 선 한쪽을 다 덮는 판
    static readonly Color SplitGapColor = new Color32(0x08, 0x06, 0x0c, 0xff);
    GameObject screenSplit = null;
    readonly RectTransform[] splitScreens = new RectTransform[2];     // 0 위, 1 아래 (회전한 판 안이라 x가 선 방향)
    RenderTexture splitTexture = null;

    public IEnumerator CoScreenSplit(float _time)
    {
        RectTransform canvasRect = (RectTransform)transform;
        if (screenSplit == null)
        {
            splitTexture = new RenderTexture(Screen.width, Screen.height, 24);
            screenSplit = new GameObject("ScreenSplit", typeof(RectTransform), typeof(Image));
            screenSplit.layer = 5;
            RectTransform root = (RectTransform)screenSplit.transform;
            root.SetParent(transform, false);
            root.SetAsFirstSibling();   // HUD 뒤
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = root.offsetMax = Vector2.zero;
            Image gap = screenSplit.GetComponent<Image>();
            gap.color = SplitGapColor;  // 벌어진 틈과 가장자리
            gap.raycastTarget = false;
            for (int i = 0; i < splitScreens.Length; i++)
            {
                // 아래 변(위쪽 판)·위 변(아래쪽 판)이 선이 되도록 화면 가운데에 피벗
                var half = new GameObject("Half", typeof(RectTransform), typeof(Image), typeof(Mask));
                half.layer = 5;
                RectTransform halfRect = (RectTransform)half.transform;
                halfRect.SetParent(root, false);
                halfRect.pivot = new Vector2(0.5f, i == 0 ? 0f : 1f);
                halfRect.sizeDelta = Vector2.one * SplitHalfSize;
                halfRect.localEulerAngles = new Vector3(0f, 0f, SplitAngle);
                half.GetComponent<Image>().raycastTarget = false;
                half.GetComponent<Mask>().showMaskGraphic = false;
                var screen = new GameObject("Screen", typeof(RectTransform), typeof(RawImage));
                screen.layer = 5;
                splitScreens[i] = (RectTransform)screen.transform;
                splitScreens[i].SetParent(halfRect, false);
                splitScreens[i].anchorMin = splitScreens[i].anchorMax = halfRect.pivot;
                splitScreens[i].sizeDelta = canvasRect.rect.size;
                splitScreens[i].localEulerAngles = new Vector3(0f, 0f, -SplitAngle);
                RawImage raw = screen.GetComponent<RawImage>();
                raw.texture = splitTexture;
                raw.raycastTarget = false;
            }
        }

        Camera mainCamera = InGameManager.Instance.MainCamera;
        mainCamera.targetTexture = splitTexture;
        screenSplit.SetActive(true);
        float width = canvasRect.rect.width;
        for (float t = 0f; t < _time; t += Time.unscaledDeltaTime)
        {
            float fast = Mathf.Clamp01(t / SplitFastTime);
            float shift = (SplitShift * (1f - (1f - fast) * (1f - fast)) + SplitExtraShift * Mathf.Clamp01((t - SplitFastTime) / (_time - SplitFastTime))) * width;
            splitScreens[0].anchoredPosition = new Vector2(shift, 0f);
            splitScreens[1].anchoredPosition = new Vector2(-shift, 0f);
            yield return null;
        }
        mainCamera.targetTexture = null;
        screenSplit.SetActive(false);
    }

    // 검은 화면 위에서 BOSS 문장 (게임 시간이 멈춰 있어도 돌아가게 unscaled 시간)
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
            bossIntroFlash.color = new Color(1f, 1f, 1f, t >= IntroFlashStart && t < IntroFlashStart + IntroFlashTime ? 0.8f : 0f);
            bossIntroGroup.alpha = 1f - Mathf.Clamp01((t - FadeOutStart) / (BossIntroTime - FadeOutStart));
            yield return null;
        }
        bossIntroGroup.gameObject.SetActive(false);
    }

    // 검은 화면을 0.72로 낮추고 이름 컷인, 끝에 어둠까지 걷음 (게임 시간이 멈춰 있어도 돌아가게 unscaled 시간)
    public IEnumerator CoBossCutIn(MonsterData _boss, string _stageText)
    {
        UIContentPopup.SetupMonster(cutInBoss, _boss.Data_Id, 1f);
        FitCutInBoss();
        cutInNameText.text = StringManager.Instance.GetStringText(_boss.TextCode_Name);
        cutInStageText.text = _stageText;
        bossCutIn.gameObject.SetActive(true);
        blackScreen.color = Color.clear;    // 어둡게는 컷인 Dim이 맡고, 검은 화면은 연출 동안 터치만 막음
        for (float t = 0f; t < CutInTime; t += Time.unscaledDeltaTime)
        {
            float close = Mathf.Clamp01((t - CutInCloseStart) / (CutInTime - CutInCloseStart));
            // 검은 화면에서 0.72로 옅어지고, 닫힐 때는 묶음과 함께 걷힘
            cutInDim.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, CutInDim, t / CutInOpenTime));
            cutInBand.localScale = new Vector3(1f, Mathf.Clamp01(t / CutInOpenTime) * (1f - close), 1f);
            for (int i = 0; i < cutInLines.Length; i++)
            {
                float pass = (t / CutInLinePassTime + (float)i / cutInLines.Length) % 1f;
                cutInLines[i].anchoredPosition = new Vector2(Mathf.Lerp(CutInBandWidth, -cutInLines[i].sizeDelta.x, pass), cutInLines[i].anchoredPosition.y);
            }
            // 0.15초 오른쪽 밖 → 0.38초 -10 → 0.46초 0 → 1.15초까지 왼쪽으로 8
            float bossX = t < 0.15f ? CutInBossEnter
                : t < 0.38f ? Mathf.Lerp(CutInBossEnter, -20f, (t - 0.15f) / 0.23f)
                : t < 0.46f ? Mathf.Lerp(-20f, 0f, (t - 0.38f) / 0.08f)
                : -CutInBossDrift * Mathf.Clamp01((t - 0.46f) / (CutInCloseStart - 0.46f));
            cutInBoss.rectTransform.anchoredPosition = new Vector2(bossX - close * CutInExitDistance, 0f);
            // "BOSS" 0.28초, 이름 0.08초 뒤, 밑줄·스테이지 0.16초 뒤
            for (int i = 0; i < cutInRows.Length; i++)
            {
                float enter = Mathf.Clamp01((t - 0.28f - 0.08f * i) / CutInTextTime);
                cutInRows[i].anchoredPosition = new Vector2(-CutInTextEnter * (1f - enter) * (1f - enter) - close * CutInExitDistance, 0f);
            }
            // 보스가 멈추는 순간 흰빛 번쩍
            cutInFlash.color = new Color(1f, 1f, 1f, t >= 0.42f && t < 0.52f ? 0.8f * (1f - (t - 0.42f) / 0.1f) : 0f);
            bossCutIn.alpha = 1f - close;
            yield return null;
        }
        bossCutIn.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(false);
    }

    // 보이는 몸(첫 idle 프레임의 불투명 영역, 몬스터 텍스처 Read/Write 필요)으로 배율·위치 (LAYOUT_battle '보스 맞추기')
    void FitCutInBoss()
    {
        // 문서 좌표는 ×2
        ((RectTransform)cutInBoss.rectTransform.parent).anchoredPosition = CutInBodyFoot + GameUtils.GameUtils.FitSkeletonBody(cutInBoss, CutInBodyHeight * 2f, true);
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
        goldText.text = _gold.ToCurrencyString();
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
            _text.text = (_from + (_to - _from) * (t / GoodsCountTime)).ToCurrencyString();
            yield return null;
        }
        _text.text = _to.ToCurrencyString();
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
    // _bestStage가 있으면 아래 스테이지: 같은 자리에 "1-9로 이동" 버튼(위 화살표 아이콘), 자동 스위치 숨김, 안내 한 줄
    public void SetBossButton(bool _canChallenge, int _challengeCount, bool _isAuto, string _bestStage)
    {
        bool isCleared = _bestStage != null;
        bool isOn = _canChallenge || isCleared;
        bossButton.interactable = isOn;
        bossButton.image.sprite = isOn ? bossButtonOnSprite : bossButtonOffSprite;
        bossButtonIcon.sprite = isCleared ? goBestSprite : _canChallenge ? skullSprite : skullLightSprite;
        bossButtonText.text = isCleared ? string.Format(StringManager.Instance.GetStringText("Msg_GoBestMove"), _bestStage) : StringManager.Instance.GetStringText("Name_BossChallenge");
        bossButtonText.color = isOn ? BossOnTextColor : BossOffTextColor;
        // 아이콘 + 글자 묶음을 버튼 가운데로 (글자 길이가 상태마다 다름)
        float iconX = (((RectTransform)bossButton.transform).rect.width - BossIconSpace - bossButtonText.preferredWidth) * 0.5f;
        bossButtonIcon.rectTransform.anchoredPosition = new Vector2(iconX, bossButtonIcon.rectTransform.anchoredPosition.y);
        bossButtonText.rectTransform.offsetMin = new Vector2(iconX + BossIconSpace, 0f);
        bossAlert.SetActive(_canChallenge && !isCleared);
        autoBossButton.gameObject.SetActive(!isCleared);
        bossHintText.gameObject.SetActive(isCleared || !_canChallenge || _isAuto);
        bossHintText.text = isCleared ? StringManager.Instance.GetStringText("Msg_GoBestHint")
            : _canChallenge ? StringManager.Instance.GetStringText("Msg_AutoBossSoon") : string.Format(StringManager.Instance.GetStringText("Msg_BossChallengeHint"), _challengeCount);
        bossHintText.color = isCleared || _canChallenge ? AutoSoonTextColor : CreamTextColor;
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

    public void SetBossStage(string _title, float _hpRate, float _remainTime)
    {
        stageText.text = _title;
        killCountText.text = string.Format("{0:0}%", _hpRate * 100f);
        stageProgressFill.fillAmount = _hpRate;
        int seconds = Mathf.CeilToInt(_remainTime);
        bossTimerText.text = string.Format("{0}:{1:00}", seconds / 60, seconds % 60);
    }
}
