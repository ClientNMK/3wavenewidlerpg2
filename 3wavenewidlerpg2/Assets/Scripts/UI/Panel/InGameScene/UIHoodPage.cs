using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Spine.Unity;

// 성장 창 후드 탭: Hood_Exp를 먹여 후드 레벨업 (Plan/requests/Dev_Hood_LevelUp.md, UI_Hood_Window.md)
// 누를 때마다 1레벨, 꾹 누르면 반복 (강화 버튼과 같음)
public class UIHoodPage : MonoBehaviour
{
    const float HoldDelay = 0.4f;
    const float HoldInterval = 0.08f;
    const string TalkKeyFormat = "Hood_Talk_{0}";

    [SerializeField] SkeletonGraphic hoodSpine = null;
    [SerializeField] TMP_Text talkText = null;
    [SerializeField] TMP_Text levelText = null;
    [SerializeField] TMP_Text expText = null;
    [SerializeField] Image expFill = null;
    [SerializeField] TMP_Text nameText = null;
    [SerializeField] TMP_Text valueText = null;
    [SerializeField] Button feedButton = null;
    [SerializeField] TMP_Text feedText = null;
    [SerializeField] Image levelUpEffect = null;        // 후드 pivot에 맞춘 레벨업 이펙트 (후드 앞)
    [SerializeField] Sprite[] levelUpFrames = null;     // ArtSource/pets/pet_hood/effects/levelup, 30fps
    const float LevelUpFrameTime = 1f / 30f;

    Hood Hood { get { return InGameManager.Instance.MainCharacter.Hood; } }
    bool bInitialized = false;
    Coroutine holdCoroutine = null;
    Coroutine effectCoroutine = null;

    public void Setup()
    {
        if (!bInitialized)
        {
            // 누르는 순간 1번, 계속 누르고 있으면 HoldDelay 뒤부터 HoldInterval마다 반복
            var trigger = feedButton.gameObject.AddComponent<EventTrigger>();
            AddTrigger(trigger, EventTriggerType.PointerDown, () =>
            {
                if (!feedButton.interactable)
                    return;
                ChangeTalk();
                Feed();
                holdCoroutine = StartCoroutine(CoHoldFeed());
            });
            AddTrigger(trigger, EventTriggerType.PointerUp, StopHold);
            AddTrigger(trigger, EventTriggerType.PointerExit, StopHold);
            bInitialized = true;
        }
        levelUpEffect.enabled = false;  // 재생 중에 창을 닫았으면 멈춘 그림이 남아 있음
        ChangeTalk();
        Refresh();
    }

    // 한마디: StringChart Hood_Talk_1부터 있는 만큼 중 무작위 (없으면 키 이름이 보임)
    void ChangeTalk()
    {
        int count = 0;
        while (DataManager.Instance.StringData.GetStringData(string.Format(TalkKeyFormat, count + 1)) != null)
            count++;
        talkText.text = StringManager.Instance.GetStringText(string.Format(TalkKeyFormat, Random.Range(1, count + 1)));
    }

    void Refresh()
    {
        int level = Hood.Level;
        HoodData data = DataManager.Instance.HoodData.GetHoodData(level);
        StatData statData = DataManager.Instance.StatData.GetStatData(data.Upgrade_Stat_Id);
        hoodSpine.Skeleton.SetSkin(data.Resorce);
        hoodSpine.Skeleton.SetSlotsToSetupPose();
        levelText.text = string.Format("Lv.{0}", level);
        nameText.text = StringManager.Instance.GetStringText(statData.TextCode_Name);

        // 포식 가능 체력 비율 (0.01 = 1%)
        double owned = InGameManager.Instance.HoodGoods;
        if (level >= DataManager.Instance.HoodData.MaxLevel)
        {
            expText.text = "MAX";
            expFill.fillAmount = 1f;
            valueText.text = string.Format("{0:0.##}%", Hood.PredationHpPer * 100);
            feedText.text = "MAX";
            feedButton.interactable = false;
            return;
        }

        double cost = data.GetCost(level);
        // 모자라면 보유 숫자만 #E2765E
        expText.text = string.Format(owned < cost ? "<color=#E2765E>{0:N0}</color> / {1:N0}" : "{0:N0} / {1:N0}", owned, cost);
        expFill.fillAmount = Mathf.Clamp01((float)(owned / cost));
        double nextValue = statData.Stat_Default_Value + data.GetStatValue(level + 1);
        valueText.text = string.Format("{0:0.##}% > {1:0.##}%", Hood.PredationHpPer * 100, nextValue * 100);

        feedText.text = StringManager.Instance.GetStringText("Name_HoodFeed");
        feedButton.interactable = owned >= cost;
    }

    void AddTrigger(EventTrigger _trigger, EventTriggerType _type, System.Action _action)
    {
        var entry = new EventTrigger.Entry { eventID = _type };
        entry.callback.AddListener(_ => _action());
        _trigger.triggers.Add(entry);
    }

    IEnumerator CoHoldFeed()
    {
        yield return new WaitForSecondsRealtime(HoldDelay);
        while (feedButton.interactable)
        {
            Feed();
            yield return new WaitForSecondsRealtime(HoldInterval);
        }
    }

    void StopHold()
    {
        if (holdCoroutine != null)
            StopCoroutine(holdCoroutine);
        holdCoroutine = null;
    }

    // 1레벨: 비용을 쓰고 레벨업, 후드는 idle 그대로 레벨업 이펙트를 처음부터
    void Feed()
    {
        InGameManager.Instance.TryUseHoodGoods(DataManager.Instance.HoodData.GetHoodData(Hood.Level).GetCost(Hood.Level));
        Hood.LevelUp();
        if (effectCoroutine != null)
            StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(CoLevelUpEffect());
        Refresh();
    }

    IEnumerator CoLevelUpEffect()
    {
        levelUpEffect.enabled = true;
        for (int i = 0; i < levelUpFrames.Length; i++)
        {
            levelUpEffect.sprite = levelUpFrames[i];
            yield return new WaitForSecondsRealtime(LevelUpFrameTime);
        }
        levelUpEffect.enabled = false;
    }
}
