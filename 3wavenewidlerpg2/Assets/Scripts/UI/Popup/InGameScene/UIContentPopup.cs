using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

// 컨텐츠 창: 던전 탭(티켓 던전 카드 목록) / 도전 탭(보스별 단계 오르기) (kit_cocoa LAYOUT_dungeon.md)
// 진행(깬 단계)은 저장 없이 이번 플레이만, 티켓 없음
public class UIContentPopup : UIPopup
{
    const float UnitPixels = 100f;  // Canvas Reference Pixels Per Unit
    const float CardTop = 44f;      // 첫 카드 몬스터가 위로 넘치는 만큼 띄움
    const float CardPitch = 256f;
    const float FaceLeft = 32f;
    const float FacePitch = 142f;
    const float FaceSize = 88f;
    const float StepTop = 16f;
    const float StepPitch = 92f;
    const float StepHeight = 80f;
    public const float CardMonsterHeight = 240f;
    public const float BandMonsterHeight = 320f;
    public static readonly Color CreamColor = new Color32(0xff, 0xf3, 0xdc, 0xff);
    public static readonly Color SubColor = new Color32(0xc2, 0xa8, 0x88, 0xff);
    public static readonly Color HighlightColor = new Color32(0xff, 0xe7, 0xa0, 0xff);
    public static readonly Color DarkColor = new Color32(0x2a, 0x1a, 0x14, 0xff);
    public static readonly Color LockBgColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public static readonly Color SilhouetteColor = new Color(0f, 0f, 0f, 0.5f);

    [SerializeField] Button closeButton = null;
    [SerializeField] Button[] tabButtons = null;            // 0 = 던전, 1 = 도전
    [SerializeField] GameObject[] tabPages = null;
    [SerializeField] Sprite tabOnSprite = null;
    [SerializeField] Sprite tabOffSprite = null;

    [Header("던전")]
    [SerializeField] RectTransform cardContent = null;
    [SerializeField] UIDungeonCard cardTemplate = null;

    [Header("도전")]
    [SerializeField] RectTransform faceContent = null;
    [SerializeField] UIChallengeFace faceTemplate = null;
    [SerializeField] Image bandBg = null;
    [SerializeField] Image bandFrame = null;
    [SerializeField] Sprite frameSprite = null;
    [SerializeField] Sprite frameLockSprite = null;
    [SerializeField] SkeletonGraphic bandBoss = null;
    [SerializeField] TMP_Text bandNameText = null;
    [SerializeField] TMP_Text bandBestText = null;
    [SerializeField] GameObject bandTimeGroup = null;
    [SerializeField] TMP_Text bandTimeText = null;
    [SerializeField] GameObject bandLockGroup = null;
    [SerializeField] TMP_Text bandLockText = null;
    [SerializeField] RectTransform stepViewport = null;
    [SerializeField] RectTransform stepContent = null;
    [SerializeField] UIChallengeStep stepTemplate = null;
    [SerializeField] Button challengeButton = null;
    [SerializeField] TMP_Text challengeButtonText = null;
    [SerializeField] Sprite buttonOnSprite = null;
    [SerializeField] Sprite buttonOffSprite = null;

    readonly List<UIChallengeFace> faces = new();
    List<List<DungeonData>> bosses = null;
    string selectedName = null;         // 도전 버튼이 들어갈 보스·단계
    DungeonData selectedNext = null;

    public override void Init()
    {
        if (bInitialized)
            return;

        touchPanel.onClick.AddListener(OnClickTouchPanel);
        closeButton.onClick.AddListener(OnClickCloseButton);
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int tab = i;
            tabButtons[i].onClick.AddListener(() => SelectTab(tab));
        }
        challengeButton.onClick.AddListener(() => Enter(selectedName, selectedNext));
        bInitialized = true;
    }

    public override void Setup()
    {
        base.Setup();
        BuildDungeons();
        BuildChallenges();
        SelectTab(0);
    }

    public override void ShowPopup()
    {
        base.ShowPopup();
        InGameManager.Instance.UIInGameScene.SetTabWindow(this);
    }

    public override void ClosePopup()
    {
        base.ClosePopup();
        InGameManager.Instance.UIInGameScene.SetTabWindow(null);
    }

    void SelectTab(int _tab)
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].image.sprite = i == _tab ? tabOnSprite : tabOffSprite;
            tabButtons[i].GetComponentInChildren<TMP_Text>().color = i == _tab ? DarkColor : SubColor;
            tabPages[i].SetActive(i == _tab);
        }
    }

    void BuildDungeons()
    {
        foreach (Transform child in cardContent)
            Destroy(child.gameObject);
        List<List<DungeonData>> dungeons = DataManager.Instance.DungeonData.GetDungeons(false);
        for (int i = 0; i < dungeons.Count; i++)
        {
            UIDungeonCard card = Instantiate(cardTemplate, cardContent);
            ((RectTransform)card.transform).anchoredPosition = new Vector2(((RectTransform)card.transform).anchoredPosition.x, -(CardTop + i * CardPitch));
            List<DungeonData> steps = dungeons[i];
            int cleared = InGameManager.Instance.GetDungeonClearedStep(steps[0].Dungeon_Type);
            string dungeonName = string.Format(StringManager.Instance.GetStringText("Name_DungeonNumber"), i + 1);
            card.Setup(i, steps, cleared, () => Enter(dungeonName, steps[Mathf.Min(cleared, steps.Count - 1)]), () => Sweep(steps[cleared - 1]));
        }
        cardContent.sizeDelta = new Vector2(cardContent.sizeDelta.x, CardTop + dungeons.Count * CardPitch);
    }

    void BuildChallenges()
    {
        foreach (UIChallengeFace face in faces)
            Destroy(face.gameObject);
        faces.Clear();
        bosses = DataManager.Instance.DungeonData.GetDungeons(true);
        int lastOpen = 0;
        for (int i = 0; i < bosses.Count; i++)
        {
            int boss = i;
            UIChallengeFace face = Instantiate(faceTemplate, faceContent);
            ((RectTransform)face.transform).anchoredPosition = new Vector2(FaceLeft + i * FacePitch, ((RectTransform)face.transform).anchoredPosition.y);
            face.Setup(bosses[i][0].Monster_Id, IsBossLocked(i), GetClearedStep(i));
            face.Button.onClick.AddListener(() => SelectBoss(boss));
            faces.Add(face);
            if (!IsBossLocked(i))
                lastOpen = i;
        }
        faceContent.sizeDelta = new Vector2(FaceLeft * 2f + (bosses.Count - 1) * FacePitch + FaceSize, faceContent.sizeDelta.y);
        SelectBoss(lastOpen);
    }

    // 앞 보스를 마지막 단계까지 깨야 열림
    bool IsBossLocked(int _index)
    {
        return _index > 0 && GetClearedStep(_index - 1) < bosses[_index - 1].Count;
    }

    int GetClearedStep(int _bossIndex)
    {
        return InGameManager.Instance.GetDungeonClearedStep(bosses[_bossIndex][0].Dungeon_Type);
    }

    // 창을 닫고 보스전과 같은 흐름으로 입장
    void Enter(string _name, DungeonData _step)
    {
        PopupManager.Instance.ClosePopup(this);
        InGameManager.Instance.StartDungeon(_step, _name);
    }

    // 깬 최고 단계 보상을 바로 (단계는 그대로)
    void Sweep(DungeonData _step)
    {
        if (_step.Reward_Item_Id != 0)
            PopupManager.Instance.ShowPopup<UIRewardPopup>().Setup(new List<UIRewardPopup.Reward> { new(_step.Reward_Item_Id, _step.Reward_Item_Value) });
    }

    void SelectBoss(int _index)
    {
        for (int i = 0; i < faces.Count; i++)
            faces[i].SetSelect(i == _index);

        List<DungeonData> steps = bosses[_index];
        bool isLocked = IsBossLocked(_index);
        int clearedStep = GetClearedStep(_index);
        DungeonData next = steps[Mathf.Min(clearedStep, steps.Count - 1)];
        selectedName = string.Format(StringManager.Instance.GetStringText("Name_ChallengeBoss"), _index + 1);
        selectedNext = next;
        bandBg.sprite = ResourceManager.Instance.GetSprite(AtlasType.Bg, $"bg_dungeon_{steps[0].Dungeon_Type}");
        bandBg.enabled = bandBg.sprite != null;
        bandBg.color = isLocked ? LockBgColor : Color.white;
        bandFrame.sprite = isLocked ? frameLockSprite : frameSprite;
        SetupMonster(bandBoss, next.Monster_Id, BandMonsterHeight);
        bandBoss.color = isLocked ? SilhouetteColor : Color.white;
        bandNameText.text = string.Format(StringManager.Instance.GetStringText("Name_ChallengeBoss"), _index + 1);
        bandBestText.text = clearedStep > 0 ? string.Format(StringManager.Instance.GetStringText("Msg_ChallengeBest"), clearedStep) : StringManager.Instance.GetStringText("Msg_ChallengeNotCleared");
        bandTimeGroup.SetActive(!isLocked);
        bandLockGroup.SetActive(isLocked);
        bandTimeText.text = string.Format(StringManager.Instance.GetStringText("Msg_ChallengeLimit"), next.Limit_Time);
        if (isLocked)
            bandLockText.text = string.Format(StringManager.Instance.GetStringText("Msg_ChallengeUnlock"), _index, bosses[_index - 1].Count);

        // 위가 높은 단계
        foreach (Transform child in stepContent)
            Destroy(child.gameObject);
        float nextY = 0f;
        for (int i = 0; i < steps.Count; i++)
        {
            DungeonData step = steps[steps.Count - 1 - i];
            UIChallengeStep row = Instantiate(stepTemplate, stepContent);
            float y = StepTop + i * StepPitch;
            ((RectTransform)row.transform).anchoredPosition = new Vector2(((RectTransform)row.transform).anchoredPosition.x, -y);
            UIChallengeStep.StepState state = isLocked || step.Step > clearedStep + 1 ? UIChallengeStep.StepState.Locked
                : step.Step == clearedStep + 1 ? UIChallengeStep.StepState.Next : UIChallengeStep.StepState.Cleared;
            row.Setup(step, state);
            if (state == UIChallengeStep.StepState.Next)
                nextY = y;
        }
        float contentHeight = StepTop + steps.Count * StepPitch;
        stepContent.sizeDelta = new Vector2(stepContent.sizeDelta.x, contentHeight);
        // 다음 도전 단계가 보이게
        stepContent.anchoredPosition = new Vector2(0f, Mathf.Clamp(nextY - (stepViewport.rect.height - StepHeight) * 0.5f, 0f, Mathf.Max(0f, contentHeight - stepViewport.rect.height)));

        challengeButton.image.sprite = isLocked ? buttonOffSprite : buttonOnSprite;
        challengeButton.interactable = !isLocked;
        challengeButtonText.text = isLocked ? StringManager.Instance.GetStringText("Name_Locked") : string.Format(StringManager.Instance.GetStringText("Msg_ChallengeStep"), next.Step);
    }

    // 몬스터 idle을 UI에 (프레임 높이를 _height로, 좌우 반전)
    public static void SetupMonster(SkeletonGraphic _graphic, int _monsterId, float _height)
    {
        MonsterData monster = DataManager.Instance.MonsterData.GetMonsterData(_monsterId);
        SkeletonDataAsset data = Resources.Load<SkeletonAnimation>("Prefabs/Monster/" + monster.Resource).skeletonDataAsset;
        _graphic.skeletonDataAsset = data;
        _graphic.initialSkinName = monster.Skin_Name;
        _graphic.Initialize(true);
        _graphic.AnimationState.SetAnimation(0, "idle", true);
        float scale = _height / (_graphic.Skeleton.Data.Height * data.scale * UnitPixels);
        _graphic.rectTransform.localScale = new Vector3(-scale, scale, 1f);
    }
}
