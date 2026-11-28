using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 스테이지 이동 (kit_cocoa LAYOUT_stage.md, '사냥터'). 칸 고르기 → 정보 → 이동, 깬 스테이지와 최고 스테이지로만, 못 간 스테이지·월드는 잠김
public class UIStageMovePopup : UIPopup
{
    const float CellPitch = 116f;           // 칸 가로 간격 58 ×2, 한 줄로 늘어놓고 가로 스크롤
    const float CellSize = 96f;
    const float CellPadding = 12f;          // 고른 칸 테두리·체크가 칸 밖으로 나와 잘리지 않게 영역 안쪽 여백
    const float TabAreaWidth = 560f;
    const float TabSpacing = 8f;
    const float ScrollTabWidth = 176f;      // 월드 4개 이상이면 탭 폭 고정 + 가로 스크롤
    const float LabelY = 24f;
    const float LockLabelY = 60f;
    const float QuickOffAlpha = 0.45f;      // 사냥터·최고 버튼 꺼짐
    static readonly Color CreamColor = new Color32(0xff, 0xf3, 0xdc, 0xff);
    static readonly Color SubColor = new Color32(0xc2, 0xa8, 0x88, 0xff);
    static readonly Color LockLabelColor = new Color32(0xc2, 0xa8, 0x88, 0x80);
    static readonly Color HighlightColor = new Color32(0xff, 0xe7, 0xa0, 0xff);
    static readonly Color DarkColor = new Color32(0x2a, 0x1a, 0x14, 0xff);
    static readonly Color BossClearColor = new Color32(0x9b, 0xd6, 0x5a, 0xff);

    [SerializeField] Button closeButton = null;

    [Header("World")]
    [SerializeField] RectTransform tabContent = null;
    [SerializeField] Button tabTemplate = null;
    [SerializeField] Sprite tabOnSprite = null;
    [SerializeField] Sprite tabOffSprite = null;
    [SerializeField] Image worldBg = null;
    [SerializeField] TMP_Text worldNameText = null;
    [SerializeField] TMP_Text worldClearText = null;
    [SerializeField] Image worldFill = null;

    [Header("Cell")]
    [SerializeField] RectTransform cellViewport = null;
    [SerializeField] RectTransform cellContent = null;
    [SerializeField] GameObject cellFade = null;            // 좌우 흐림 (스크롤할 때만)
    [SerializeField] UIStageCell cellTemplate = null;
    [SerializeField] Sprite cellSprite = null;
    [SerializeField] Sprite cellBestSprite = null;
    [SerializeField] Sprite cellLockSprite = null;
    [SerializeField] TMP_Text lockedHintText = null;

    [Header("Info")]
    [SerializeField] TMP_Text stageNameText = null;
    [SerializeField] TMP_Text currentText = null;
    [SerializeField] GameObject infoGroup = null;
    [SerializeField] TMP_Text goldText = null;
    [SerializeField] TMP_Text expText = null;
    [SerializeField] TMP_Text statusText = null;
    [SerializeField] GameObject lockedInfoText = null;

    [Header("Hunt")]
    [SerializeField] TMP_Text huntText = null;              // "지금 사냥터: 1-3"
    [SerializeField] Button huntSaveButton = null;          // 고른 칸이 최고보다 낮은 깬 곳일 때
    [SerializeField] GameObject huntSaved = null;           // 고른 칸이 이미 사냥터

    [Header("Button")]
    [SerializeField] Button huntButton = null;              // 사냥터로 바로
    [SerializeField] TMP_Text huntButtonText = null;
    [SerializeField] Button bestButton = null;              // 최고로 바로
    [SerializeField] TMP_Text bestButtonText = null;
    [SerializeField] Button moveButton = null;              // 고른 칸으로
    [SerializeField] Sprite moveOnSprite = null;
    [SerializeField] Sprite moveOffSprite = null;

    readonly List<List<StageData>> worlds = new();          // 월드 순, 각 월드는 Stage_Number 순
    readonly List<Button> tabs = new();
    readonly List<UIStageCell> cells = new();
    readonly List<int> cellStageIds = new();
    int curStageId = 0;
    int maxStageId = 0;
    int huntStageId = 0;
    int selectedStageId = 0;

    public override void Init()
    {
        if (bInitialized)
            return;

        PopupType = PopupType.Modal;
        touchPanel.onClick.AddListener(OnClickTouchPanel);
        closeButton.onClick.AddListener(OnClickCloseButton);
        huntButton.onClick.AddListener(() => Move(huntStageId));
        bestButton.onClick.AddListener(() => Move(maxStageId));
        moveButton.onClick.AddListener(() => Move(selectedStageId));
        huntSaveButton.onClick.AddListener(SaveHunt);
        bInitialized = true;
    }

    // 지금 있는 칸이 골라진 상태로 연다
    public void Setup(int _curStageId)
    {
        curStageId = _curStageId;
        maxStageId = UserManager.Instance.MaxStageId;
        huntStageId = UserManager.Instance.HuntStageId;
        worlds.Clear();
        for (int id = 1; DataManager.Instance.StageData.HasStageData(id); id++)
        {
            StageData stage = DataManager.Instance.StageData.GetStageData(id);
            if (worlds.Count == 0 || worlds[worlds.Count - 1][0].World_Id != stage.World_Id)
                worlds.Add(new());
            worlds[worlds.Count - 1].Add(stage);
        }

        StageData cur = DataManager.Instance.StageData.GetStageData(curStageId);
        currentText.text = string.Format(StringManager.Instance.GetStringText("Msg_StageCurrent"), GetStageNumber(cur));
        bestButtonText.text = string.Format(StringManager.Instance.GetStringText("Msg_BestStage"), GetStageNumber(DataManager.Instance.StageData.GetStageData(maxStageId)));
        SetQuickButton(bestButton, bestButtonText, maxStageId != curStageId);

        foreach (Button tab in tabs)
            Destroy(tab.gameObject);
        tabs.Clear();
        float tabWidth = worlds.Count < 4 ? (TabAreaWidth - TabSpacing * (worlds.Count - 1)) / worlds.Count : ScrollTabWidth;
        for (int i = 0; i < worlds.Count; i++)
        {
            List<StageData> stages = worlds[i];
            Button tab = Instantiate(tabTemplate, tabContent);
            RectTransform rect = (RectTransform)tab.transform;
            rect.anchoredPosition = new Vector2(i * (tabWidth + TabSpacing), 0f);
            rect.sizeDelta = new Vector2(tabWidth, rect.sizeDelta.y);
            tab.GetComponentInChildren<TMP_Text>().text = string.Format(StringManager.Instance.GetStringText("Name_World"), stages[0].World_Id);
            tab.transform.Find("Group/Lock").gameObject.SetActive(stages[0].Stage_Id > maxStageId);
            tab.onClick.AddListener(() => ShowWorld(stages));
            tabs.Add(tab);
        }
        tabContent.sizeDelta = new Vector2(worlds.Count * (tabWidth + TabSpacing) - TabSpacing, tabContent.sizeDelta.y);

        ShowWorld(worlds.Find(x => x[0].World_Id == cur.World_Id));
    }

    void ShowWorld(List<StageData> _stages)
    {
        int worldId = _stages[0].World_Id;
        bool isLocked = _stages[0].Stage_Id > maxStageId;
        for (int i = 0; i < tabs.Count; i++)
        {
            bool isOn = worlds[i] == _stages;
            tabs[i].image.sprite = isOn ? tabOnSprite : tabOffSprite;
            tabs[i].GetComponentInChildren<TMP_Text>().color = isOn ? DarkColor : SubColor;
        }

        worldBg.sprite = ResourceManager.Instance.GetSprite(AtlasType.Bg, DataManager.Instance.WorldData.GetWorldData(worldId).World_Resource);
        float bgWidth = worldBg.rectTransform.sizeDelta.x;
        worldBg.rectTransform.sizeDelta = new Vector2(bgWidth, bgWidth * worldBg.sprite.rect.height / worldBg.sprite.rect.width);
        worldNameText.text = string.Format(StringManager.Instance.GetStringText("Name_World"), worldId);
        int clearCount = _stages.Count(x => x.Stage_Id < maxStageId);
        worldClearText.text = string.Format(StringManager.Instance.GetStringText("Msg_WorldClearCount"), clearCount, _stages.Count);
        worldFill.fillAmount = (float)clearCount / _stages.Count;

        foreach (Transform child in cellContent)
            Destroy(child.gameObject);
        cells.Clear();
        cellStageIds.Clear();
        for (int i = 0; i < _stages.Count; i++)
        {
            StageData stage = _stages[i];
            UIStageCell cell = Instantiate(cellTemplate, cellContent);
            ((RectTransform)cell.transform).anchoredPosition = new Vector2(CellPadding + i * CellPitch, -CellPadding);
            bool isCellLocked = isLocked || stage.Stage_Id > maxStageId;
            bool isBest = !isCellLocked && stage.Stage_Id == maxStageId;
            cell.Bg.sprite = isCellLocked ? cellLockSprite : isBest ? cellBestSprite : cellSprite;
            cell.Check.SetActive(!isCellLocked && !isBest);
            cell.BestTag.SetActive(isBest);
            // 사냥터가 최고가 되면 '최고' 꼬리표 우선
            cell.HuntTag.SetActive(!isCellLocked && !isBest && stage.Stage_Id == huntStageId);
            cell.LockIcon.SetActive(isCellLocked);
            cell.Face.SetActive(stage.Stage_Id == curStageId);
            cell.Label.text = GetStageNumber(stage);
            cell.Label.fontSize = isCellLocked ? 18f : 26f;
            cell.Label.color = isCellLocked ? LockLabelColor : isBest ? HighlightColor : CreamColor;
            cell.Label.rectTransform.anchoredPosition = new Vector2(0f, -(isCellLocked ? LockLabelY : LabelY));
            // 잠긴 칸은 눌러도 고르지 않음
            if (!isCellLocked)
                cell.Button.onClick.AddListener(() => Select(stage.Stage_Id));
            cells.Add(cell);
            cellStageIds.Add(stage.Stage_Id);
        }
        float contentWidth = CellPadding * 2f + (_stages.Count - 1) * CellPitch + CellSize;
        cellContent.sizeDelta = new Vector2(contentWidth, cellContent.sizeDelta.y);
        cellFade.SetActive(contentWidth > cellViewport.rect.width);

        // 잠긴 월드: 칸은 모두 잠김, 안내 문구, 이동 버튼 꺼짐
        lockedHintText.gameObject.SetActive(isLocked);
        stageNameText.gameObject.SetActive(!isLocked);
        infoGroup.SetActive(!isLocked);
        lockedInfoText.SetActive(isLocked);
        if (isLocked)
        {
            List<StageData> prevWorld = worlds[worlds.IndexOf(_stages) - 1];
            lockedHintText.text = string.Format(StringManager.Instance.GetStringText("Msg_WorldUnlockHint"), prevWorld[0].World_Id, prevWorld[prevWorld.Count - 1].Stage_Number);
            selectedStageId = 0;
            SetMoveButton(false);
            RefreshHunt();
            ScrollTo(0f);
            return;
        }

        // 지금 있는 월드면 지금 칸, 아니면 그 월드에서 갈 수 있는 마지막 칸
        Select(_stages.Exists(x => x.Stage_Id == curStageId) ? curStageId : Mathf.Min(maxStageId, _stages[_stages.Count - 1].Stage_Id));
        int index = _stages.FindIndex(x => x.Stage_Id == selectedStageId);
        ScrollTo(CellPadding + index * CellPitch - (cellViewport.rect.width - CellSize) * 0.5f);
    }

    void Select(int _stageId)
    {
        selectedStageId = _stageId;
        for (int i = 0; i < cells.Count; i++)
            cells[i].Select.SetActive(cellStageIds[i] == _stageId);

        StageData stage = DataManager.Instance.StageData.GetStageData(_stageId);
        stageNameText.text = $"Stage {GetStageNumber(stage)}";
        goldText.text = string.Format("+{0:N0}", stage.Gold_Value);
        expText.text = string.Format("+{0:N0}", stage.Exp_Value);
        bool isCleared = _stageId < maxStageId;
        statusText.text = StringManager.Instance.GetStringText(isCleared ? "Msg_BossCleared" : "Msg_Challenging");
        statusText.color = isCleared ? BossClearColor : HighlightColor;
        SetMoveButton(_stageId != curStageId);
        RefreshHunt();
    }

    // 고른 칸을 사냥터로 저장 (확인 창 없음), 칸 꼬리표도 옮김
    void SaveHunt()
    {
        huntStageId = selectedStageId;
        UserManager.Instance.SetHuntStage(huntStageId, true);
        for (int i = 0; i < cells.Count; i++)
            cells[i].HuntTag.SetActive(cellStageIds[i] == huntStageId);
        RefreshHunt();
    }

    // 사냥터 줄 + 사냥터 버튼: 고른 칸이 최고보다 낮은 깬 곳이면 저장 버튼, 이미 사냥터면 저장됨, 최고면 비움
    void RefreshHunt()
    {
        bool hasHunt = huntStageId > 0;
        string hunt = hasHunt ? GetStageNumber(DataManager.Instance.StageData.GetStageData(huntStageId)) : null;
        huntText.text = hasHunt ? string.Format(StringManager.Instance.GetStringText("Msg_HuntCurrent"), hunt) : StringManager.Instance.GetStringText("Msg_HuntNone");
        bool isCleared = selectedStageId > 0 && selectedStageId < maxStageId;
        huntSaveButton.gameObject.SetActive(isCleared && selectedStageId != huntStageId);
        huntSaved.SetActive(isCleared && selectedStageId == huntStageId);
        huntButtonText.text = hasHunt ? string.Format(StringManager.Instance.GetStringText("Msg_HuntStage"), hunt) : StringManager.Instance.GetStringText("Msg_HuntNone");
        SetQuickButton(huntButton, huntButtonText, hasHunt && huntStageId != curStageId);
    }

    void SetMoveButton(bool _isOn)
    {
        moveButton.interactable = _isOn;
        moveButton.image.sprite = _isOn ? moveOnSprite : moveOffSprite;
    }

    // 꺼짐: 판 알파 0.45 + 흐린 글
    void SetQuickButton(Button _button, TMP_Text _text, bool _isOn)
    {
        _button.interactable = _isOn;
        _button.image.color = new Color(1f, 1f, 1f, _isOn ? 1f : QuickOffAlpha);
        _text.color = _isOn ? CreamColor : SubColor;
    }

    void ScrollTo(float _x)
    {
        cellContent.anchoredPosition = new Vector2(-Mathf.Clamp(_x, 0f, Mathf.Max(0f, cellContent.sizeDelta.x - cellViewport.rect.width)), 0f);
    }

    void Move(int _stageId)
    {
        PopupManager.Instance.ClosePopup(this);
        InGameManager.Instance.MoveStage(_stageId);
    }

    string GetStageNumber(StageData _stage)
    {
        return $"{_stage.World_Id}-{_stage.Stage_Number}";
    }
}
