using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 미션 창: 일일 / 주간 / 반복 탭 (kit_cocoa LAYOUT_mission.md). 정렬은 받기 → 진행 중 → 받음, 같은 상태는 차트 순서
public class UIMissionPopup : UIPopup
{
    const float RowTop = 12f;       // 받기 강조 테두리가 위로 넘치는 만큼 띄움
    const float RowPitch = 124f;
    const float RefreshTime = 1f;   // 진행·초기화 시간 갱신
    static readonly Color TabOnTextColor = new Color32(0x2a, 0x1a, 0x14, 0xff);
    static readonly Color TabOffTextColor = new Color32(0xc2, 0xa8, 0x88, 0xff);

    [SerializeField] Button closeButton = null;
    [SerializeField] Button[] tabButtons = null;            // 0 일일, 1 주간, 2 반복 (= Mission_Type - 1)
    [SerializeField] GameObject[] tabDots = null;
    [SerializeField] Sprite tabOnSprite = null;
    [SerializeField] Sprite tabOffSprite = null;
    [SerializeField] GameObject resetGroup = null;          // 일일·주간: 초기화까지 남은 시간
    [SerializeField] TMP_Text resetText = null;
    [SerializeField] GameObject repeatInfo = null;
    [SerializeField] RectTransform rowContent = null;
    [SerializeField] UIMissionRow rowTemplate = null;
    [SerializeField] Sprite[] kindIcons = null;             // Progress_Type - 1 순서
    [SerializeField] Button claimAllButton = null;
    [SerializeField] Sprite buttonOnSprite = null;
    [SerializeField] Sprite buttonOffSprite = null;

    int tab = 0;
    float refreshTimer = 0f;

    InGameMission Mission { get { return InGameManager.Instance.Mission; } }

    public override void Init()
    {
        if (bInitialized)
            return;

        touchPanel.onClick.AddListener(OnClickTouchPanel);
        closeButton.onClick.AddListener(OnClickCloseButton);
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => SelectTab(index));
        }
        claimAllButton.onClick.AddListener(ClaimAll);
        bInitialized = true;
    }

    public override void Setup()
    {
        base.Setup();
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
        tab = _tab;
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].image.sprite = i == tab ? tabOnSprite : tabOffSprite;
            tabButtons[i].GetComponentInChildren<TMP_Text>().color = i == tab ? TabOnTextColor : TabOffTextColor;
        }
        Refresh();
    }

    private void Update()
    {
        refreshTimer += Time.unscaledDeltaTime;
        if (refreshTimer < RefreshTime)
            return;
        refreshTimer = 0f;
        Refresh();
    }

    List<MissionData> GetTabMissions(int _tab)
    {
        return DataManager.Instance.MissionData.GetMissions().FindAll(x => x.Mission_Type == _tab + 1);
    }

    void Refresh()
    {
        List<MissionData> missions = GetTabMissions(tab);
        bool isRepeat = missions.Count > 0 && missions[0].Reset_Type == MissionResetType.Repeat;
        resetGroup.SetActive(!isRepeat);
        repeatInfo.SetActive(isRepeat);
        if (!isRepeat)
        {
            int seconds = (int)((missions[0].Reset_Type == MissionResetType.Daily ? Mission.DailyResetTime : Mission.WeeklyResetTime) - TimeManager.Instance.GetCurrentTimeStamp());
            string remain = missions[0].Reset_Type == MissionResetType.Daily
                ? string.Format("{0:00}:{1:00}:{2:00}", seconds / 3600, seconds / 60 % 60, seconds % 60)
                : string.Format(StringManager.Instance.GetStringText("Msg_DaysHours"), seconds / 86400, seconds / 3600 % 24);
            resetText.text = string.Format(StringManager.Instance.GetStringText("Msg_MissionReset"), remain);
        }

        foreach (Transform child in rowContent)
            Destroy(child.gameObject);
        // 받기 가능 → 진행 중 → 받음 (OrderBy는 같은 값끼리 차트 순서 유지)
        List<MissionData> sorted = missions.OrderBy(x => Mission.GetClaimCount(x) > 0 ? 0 : Mission.IsClaimed(x) ? 2 : 1).ToList();
        for (int i = 0; i < sorted.Count; i++)
        {
            MissionData mission = sorted[i];
            UIMissionRow row = Instantiate(rowTemplate, rowContent);
            ((RectTransform)row.transform).anchoredPosition = new Vector2(((RectTransform)row.transform).anchoredPosition.x, -(RowTop + i * RowPitch));
            row.Setup(mission, Mission.GetProgress(mission), Mission.GetClaimCount(mission), Mission.IsClaimed(mission),
                kindIcons[(int)mission.Progress_Type - 1], () => Claim(new List<MissionData> { mission }), () => Move(mission));
        }
        rowContent.sizeDelta = new Vector2(rowContent.sizeDelta.x, RowTop + sorted.Count * RowPitch);

        for (int i = 0; i < tabDots.Length; i++)
            tabDots[i].SetActive(GetTabMissions(i).Exists(x => Mission.GetClaimCount(x) > 0));
        bool canClaim = missions.Exists(x => Mission.GetClaimCount(x) > 0);
        claimAllButton.interactable = canClaim;
        claimAllButton.image.sprite = canClaim ? buttonOnSprite : buttonOffSprite;
    }

    void ClaimAll()
    {
        Claim(GetTabMissions(tab).FindAll(x => Mission.GetClaimCount(x) > 0));
    }

    // 받은 몫만큼 재화별로 합쳐 공용 보상 연출로 지급
    void Claim(List<MissionData> _missions)
    {
        List<UIRewardPopup.Reward> rewards = new();
        foreach (MissionData mission in _missions)
        {
            double value = mission.Reward_Value * Mission.Claim(mission);
            int index = rewards.FindIndex(x => x.GoodsId == mission.Reward_Id);
            if (index >= 0)
                rewards[index] = new(mission.Reward_Id, rewards[index].Value + value);
            else
                rewards.Add(new(mission.Reward_Id, value));
        }
        PopupManager.Instance.ShowPopup<UIRewardPopup>().Setup(rewards);
        Refresh();
    }

    // 진행 중 '이동': 던전은 컨텐츠 창, 나머지는 전투 화면으로
    void Move(MissionData _mission)
    {
        PopupManager.Instance.ClosePopup(this);
        if (_mission.Progress_Type == MissionProgressType.DungeonPlay)
            PopupManager.Instance.ShowPopup<UIContentPopup>();
    }
}
