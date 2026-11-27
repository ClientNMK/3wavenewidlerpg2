using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// 골드 강화 한 줄 (Gold_Id 하나)
public class UIGoldUpgradeSlot : MonoBehaviour
{
    [SerializeField] Image badgeFill = null;
    [SerializeField] Image icon = null;
    [SerializeField] TMP_Text nameText = null;
    [SerializeField] TMP_Text valueText = null;
    [SerializeField] TMP_Text costText = null;
    [SerializeField] Button upgradeButton = null;

    int goldId = 0;
    public int Amount { get; set; } = 1;    // 성장 창 x1·x10·x100
    int CurLevel { get { return InGameBuffManager.Instance.GoldUpgradeLevels.GetValueOrDefault(goldId); } }

    public void Setup(int _goldId, Color _badgeColor, Sprite _icon)
    {
        goldId = _goldId;
        badgeFill.color = _badgeColor;
        icon.sprite = _icon;
        // 누르는 순간 1번, 계속 누르고 있으면 HoldDelay 뒤부터 HoldInterval마다 반복
        var trigger = upgradeButton.gameObject.AddComponent<EventTrigger>();
        AddTrigger(trigger, EventTriggerType.PointerDown, () =>
        {
            if (!upgradeButton.interactable)
                return;
            OnClickUpgradeButton();
            holdCoroutine = StartCoroutine(CoHoldUpgrade());
        });
        AddTrigger(trigger, EventTriggerType.PointerUp, StopHold);
        AddTrigger(trigger, EventTriggerType.PointerExit, StopHold);
        Refresh();
    }

    const float HoldDelay = 0.4f;
    const float HoldInterval = 0.08f;
    Coroutine holdCoroutine = null;

    void AddTrigger(EventTrigger _trigger, EventTriggerType _type, System.Action _action)
    {
        var entry = new EventTrigger.Entry { eventID = _type };
        entry.callback.AddListener(_ => _action());
        _trigger.triggers.Add(entry);
    }

    IEnumerator CoHoldUpgrade()
    {
        yield return new WaitForSecondsRealtime(HoldDelay);
        while (upgradeButton.interactable)
        {
            OnClickUpgradeButton();
            yield return new WaitForSecondsRealtime(HoldInterval);
        }
    }

    void StopHold()
    {
        if (holdCoroutine != null)
            StopCoroutine(holdCoroutine);
        holdCoroutine = null;
    }

    public void Refresh()
    {
        int level = CurLevel;
        GoldUpgradeData data = DataManager.Instance.GoldUpgradeData.GetGoldUpgradeData(goldId, level);
        StatData statData = DataManager.Instance.StatData.GetStatData(data.Upgrade_Stat_Id);
        nameText.text = string.Format("{0}  <color=#C2A888><size=80%>Lv.{1}</size></color>", StringManager.Instance.GetStringText(statData.TextCode_Name), level);

        double curValue = InGameBuffManager.Instance.BuffStatus.GetStatValue((StatType)data.Upgrade_Stat_Id);
        int addLevel = GetAddLevel(level);
        if (addLevel == 0)
        {
            upgradeButton.interactable = false;
            valueText.text = curValue.ToString("N0");
            costText.text = "MAX";
            return;
        }

        int nextLevel = level + addLevel;
        GoldUpgradeData nextData = DataManager.Instance.GoldUpgradeData.GetGoldUpgradeData(goldId, nextLevel);
        double nextValue = statData.Stat_Default_Value + nextData.Upgrade_Stat_Value + nextData.Upgrade_Stat_Increase_Value * (nextLevel - nextData.Level_Start);
        valueText.text = string.Format("{0:N0} > {1:N0}", curValue, nextValue);
        // 골드가 모자라면 레벨 탭처럼 꺼진 버튼
        double cost = GetCost(level, addLevel);
        costText.text = cost.ToString("N0");
        upgradeButton.interactable = InGameManager.Instance.Gold >= cost;
    }

    // 한 번에 올릴 레벨 수 (x1·x10·x100, 최대 레벨까지만)
    int GetAddLevel(int _level)
    {
        int maxLevel = DataManager.Instance.GoldUpgradeData.GetGoldUpgradeData(goldId, int.MaxValue).Level_End;
        return Mathf.Min(Amount, maxLevel - _level);
    }

    // 레벨마다 구간 비용을 더함 (구간이 바뀌면 다음 구간 비용)
    double GetCost(int _level, int _addLevel)
    {
        double cost = 0;
        for (int lv = _level; lv < _level + _addLevel; lv++)
        {
            GoldUpgradeData data = DataManager.Instance.GoldUpgradeData.GetGoldUpgradeData(goldId, lv);
            cost += data.Upgrade_Goods_Value + data.Upgrade_Goods_Increase_Value * (lv - data.Level_Start);
        }
        return cost;
    }

    void OnClickUpgradeButton()
    {
        int level = CurLevel;
        int addLevel = GetAddLevel(level);
        if (!InGameManager.Instance.TryUseGold(GetCost(level, addLevel)))
            return;

        InGameBuffManager.Instance.SetGoldUpgradeLevel(goldId, level + addLevel);
        Refresh();
    }
}
