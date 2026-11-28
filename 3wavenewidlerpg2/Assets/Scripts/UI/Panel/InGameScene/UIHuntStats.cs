using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameUtils;

// 사냥 기록 (kit_cocoa LAYOUT_stats.md): 마지막 초기화부터 잰 시간·분당 처치·골드·경험치 (저장 없이 이번 플레이만, 일반 몬스터만)
public class UIHuntStats : MonoBehaviour
{
    const float RefreshTime = 1f;

    [SerializeField] GameObject folded = null;
    [SerializeField] GameObject expanded = null;
    [SerializeField] GameObject confirm = null;
    [SerializeField] Button openButton = null;
    [SerializeField] Button openPillButton = null;
    [SerializeField] Button foldButton = null;
    [SerializeField] Button resetButton = null;
    [SerializeField] Button cancelButton = null;
    [SerializeField] Button confirmButton = null;
    [SerializeField] TMP_Text foldedKillText = null;
    [SerializeField] TMP_Text timeText = null;
    [SerializeField] TMP_Text totalKillText = null;
    [SerializeField] TMP_Text killText = null;
    [SerializeField] TMP_Text goldText = null;
    [SerializeField] TMP_Text expText = null;

    float startTime = 0f;
    int killCount = 0;
    double gold = 0;
    double exp = 0;
    float refreshTimer = 0f;

    private void Start()
    {
        openButton.onClick.AddListener(() => SetExpand(true));
        openPillButton.onClick.AddListener(() => SetExpand(true));
        foldButton.onClick.AddListener(() => SetExpand(false));
        resetButton.onClick.AddListener(() => confirm.SetActive(true));
        cancelButton.onClick.AddListener(() => confirm.SetActive(false));
        confirmButton.onClick.AddListener(ResetStats);
        SetExpand(false);
        ResetStats();
    }

    public void AddKill(double _gold, double _exp)
    {
        killCount++;
        gold += _gold;
        exp += _exp;
    }

    void SetExpand(bool _isOn)
    {
        folded.SetActive(!_isOn);
        expanded.SetActive(_isOn);
        confirm.SetActive(false);
    }

    // 0으로 하고 바로 다시 잼 (초기화 버튼, 스테이지가 바뀔 때)
    public void ResetStats()
    {
        startTime = Time.time;
        killCount = 0;
        gold = 0;
        exp = 0;
        confirm.SetActive(false);
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

    void Refresh()
    {
        float seconds = Time.time - startTime;
        int perMinute = seconds > 0f ? Mathf.RoundToInt(killCount / (seconds / 60f)) : 0;
        int total = (int)seconds;
        foldedKillText.text = string.Format(StringManager.Instance.GetStringText("Msg_PerMinute"), perMinute);
        timeText.text = total >= 3600 ? string.Format("{0}:{1:00}:{2:00}", total / 3600, total / 60 % 60, total % 60) : string.Format("{0}:{1:00}", total / 60, total % 60);
        totalKillText.text = string.Format(StringManager.Instance.GetStringText("Msg_KillCount"), ((double)killCount).ToCurrencyString());
        killText.text = string.Format(StringManager.Instance.GetStringText("Msg_KillCount"), perMinute);
        goldText.text = "+" + gold.ToCurrencyString();
        expText.text = "+" + exp.ToCurrencyString();
    }
}
