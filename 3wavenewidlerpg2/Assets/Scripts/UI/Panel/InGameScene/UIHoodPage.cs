using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

// 성장 창 후드 탭: Hood_Exp를 먹여 후드 레벨업 (Plan/requests/Dev_Hood_LevelUp.md, UI_Hood_Window.md)
public class UIHoodPage : MonoBehaviour
{
    const float FeedAnimTime = 0.8f;    // 오른 레벨 수와 상관없이 1초 안쪽
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

    public void Setup()
    {
        if (!bInitialized)
        {
            feedButton.onClick.AddListener(OnClickFeedButton);
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

        int addLevel = GetFeedLevel(out _);
        feedText.text = string.Format("{0} (+{1})", StringManager.Instance.GetStringText("Name_HoodFeed"), addLevel);
        feedButton.interactable = addLevel > 0;
    }

    // 가진 Hood_Exp로 오를 수 있는 레벨 수 (구간이 바뀌면 다음 구간 비용으로 이어서)
    int GetFeedLevel(out double _totalCost)
    {
        double owned = InGameManager.Instance.HoodGoods;
        int maxLevel = DataManager.Instance.HoodData.MaxLevel;
        int level = Hood.Level;
        _totalCost = 0;
        while (level < maxLevel)
        {
            double cost = DataManager.Instance.HoodData.GetHoodData(level).GetCost(level);
            if (_totalCost + cost > owned)
                break;
            _totalCost += cost;
            level++;
        }
        return level - Hood.Level;
    }

    void OnClickFeedButton()
    {
        int startLevel = Hood.Level;
        int addLevel = GetFeedLevel(out double totalCost);
        InGameManager.Instance.TryUseHoodGoods(totalCost);
        for (int i = 0; i < addLevel; i++)
            Hood.LevelUp();

        // 후드는 idle 그대로, 레벨업 이펙트 1회
        ChangeTalk();
        StopAllCoroutines();
        StartCoroutine(CoLevelUp(startLevel, addLevel));
        StartCoroutine(CoLevelUpEffect());
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

    // 경험치 바가 오른 레벨 수만큼 차오르며 레벨 숫자가 올라감
    IEnumerator CoLevelUp(int _startLevel, int _addLevel)
    {
        feedButton.interactable = false;
        float segmentTime = FeedAnimTime / _addLevel;
        for (int i = 0; i < _addLevel; i++)
        {
            for (float t = 0f; t < segmentTime; t += Time.unscaledDeltaTime)
            {
                expFill.fillAmount = t / segmentTime;
                yield return null;
            }
            levelText.text = string.Format("Lv.{0}", _startLevel + i + 1);
        }
        Refresh();
    }
}
