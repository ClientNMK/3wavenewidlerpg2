using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 보스 실패 연출 + 결과 판 (kit_cocoa LAYOUT_fail.md A). 게임이 멈춘 동안 unscaled 시간으로 재생
public class UIBossDefeat : MonoBehaviour
{
    public enum Reason
    {
        TimeOver,
        Dead,
        GiveUp,
    }
    static readonly string[] ReasonKeys = { "Msg_DefeatTimeOver", "Msg_DefeatDead", "Msg_DefeatGiveUp" };

    const float BandStart = 0.1f;
    const float BandOpenTime = 0.2f;
    const float EmblemDropTime = 0.3f;      // 0.1초부터 위에서 떨어짐
    const float EmblemDropHeight = 240f;
    const float ShakeTime = 0.3f;           // 떨어진 뒤 좌우 3도로 두 번
    const float ShakeAngle = 3f;
    const float ReasonStart = 0.5f;
    const float ResultStart = 1.2f;
    const float ResultMoveTime = 0.3f;      // 문장이 결과 판 위로 올라가 작아짐
    const float ResultEmblemScale = 0.8f;
    const float AutoConfirmTime = 3f;       // 결과 판이 뜬 뒤 누르지 않아도 확인처럼 스테이지로 돌아감
    public const int GoldUpgradeTab = 0;    // 바로가기 → 성장 창 탭
    public const int HoodTab = 2;

    [SerializeField] RectTransform band = null;
    [SerializeField] RectTransform emblem = null;
    [SerializeField] TMP_Text reasonText = null;
    [SerializeField] CanvasGroup resultGroup = null;
    [SerializeField] TMP_Text bossNameText = null;
    [SerializeField] TMP_Text resultReasonText = null;
    [SerializeField] Image bossHpFill = null;
    [SerializeField] TMP_Text goldSubText = null;
    [SerializeField] GameObject goldAlert = null;
    [SerializeField] Button goldMoveButton = null;
    [SerializeField] TMP_Text hoodSubText = null;
    [SerializeField] GameObject hoodAlert = null;
    [SerializeField] Button hoodMoveButton = null;
    [SerializeField] Button confirmButton = null;
    [SerializeField] float resultEmblemY = -281f;   // 결과 판 위 문장 가운데 (화면 위 기준)

    int closeTab = int.MinValue;    // 확인 = -1, 바로가기 = 성장 창 탭

    private void Awake()
    {
        goldMoveButton.onClick.AddListener(() => closeTab = GoldUpgradeTab);
        hoodMoveButton.onClick.AddListener(() => closeTab = HoodTab);
        confirmButton.onClick.AddListener(() => closeTab = -1);
    }

    // 확인 또는 바로가기를 누를 때까지. _onClose(-1 = 확인, 0 이상 = 열 성장 창 탭)
    public IEnumerator CoPlay(Reason _reason, string _bossName, float _hpRate, Action<int> _onClose)
    {
        gameObject.SetActive(true);
        closeTab = int.MinValue;
        string reason = StringManager.Instance.GetStringText(ReasonKeys[(int)_reason]);
        reasonText.text = reason;
        bossNameText.text = string.Format(StringManager.Instance.GetStringText("Msg_BossName"), _bossName);
        resultReasonText.text = string.Format(StringManager.Instance.GetStringText("Msg_DefeatHpLeft"), reason, Mathf.CeilToInt(_hpRate * 100f));
        bossHpFill.fillAmount = _hpRate;
        SetupShortcuts();

        Vector2 emblemPos = emblem.anchoredPosition;
        float total = ResultStart + ResultMoveTime;
        for (float t = 0f; t < total; t += Time.unscaledDeltaTime)
        {
            band.localScale = new Vector3(Mathf.Clamp01((t - BandStart) / BandOpenTime), 1f, 1f);
            float drop = Mathf.Clamp01((t - BandStart) / EmblemDropTime);
            float shake = Mathf.Clamp01((t - BandStart - EmblemDropTime) / ShakeTime);
            float move = Mathf.Clamp01((t - ResultStart) / ResultMoveTime);
            emblem.gameObject.SetActive(t >= BandStart);
            emblem.anchoredPosition = new Vector2(emblemPos.x, Mathf.Lerp(emblemPos.y + EmblemDropHeight * (1f - drop * drop), resultEmblemY, move));
            emblem.localEulerAngles = new Vector3(0f, 0f, shake < 1f ? Mathf.Sin(shake * Mathf.PI * 4f) * ShakeAngle : 0f);
            emblem.localScale = Vector3.one * Mathf.Lerp(1f, ResultEmblemScale, move);
            reasonText.alpha = t < ReasonStart ? 0f : 1f - move;
            band.GetComponent<Graphic>().color = new Color(1f, 1f, 1f, 1f - move);
            resultGroup.alpha = move;
            yield return null;
        }
        resultGroup.interactable = resultGroup.blocksRaycasts = true;

        for (float t = 0f; closeTab == int.MinValue; t += Time.unscaledDeltaTime)
        {
            if (t >= AutoConfirmTime)
                closeTab = -1;
            yield return null;
        }

        resultGroup.interactable = resultGroup.blocksRaycasts = false;
        emblem.anchoredPosition = emblemPos;
        emblem.localScale = Vector3.one;
        gameObject.SetActive(false);
        _onClose(closeTab);
    }

    // 지금 할 수 있는 성장: 강화할 수 있으면 알림 점
    void SetupShortcuts()
    {
        double gold = InGameManager.Instance.Gold;
        goldSubText.text = string.Format(StringManager.Instance.GetStringText("Msg_DefeatGoldSub"), gold.ToString("N0"));
        goldAlert.SetActive(DataManager.Instance.GoldUpgradeData.GetGoldIds().Any(id =>
        {
            int level = InGameBuffManager.Instance.GoldUpgradeLevels.GetValueOrDefault(id);
            GoldUpgradeData data = DataManager.Instance.GoldUpgradeData.GetGoldUpgradeData(id, level);
            return level < data.Level_End && gold >= data.Upgrade_Goods_Value + data.Upgrade_Goods_Increase_Value * (level - data.Level_Start);
        }));

        Hood hood = InGameManager.Instance.MainCharacter.Hood;
        double hoodGoods = InGameManager.Instance.HoodGoods;
        hoodSubText.text = string.Format(StringManager.Instance.GetStringText("Msg_DefeatHoodSub"), hoodGoods.ToString("N0"));
        hoodAlert.SetActive(hood.Level < DataManager.Instance.HoodData.MaxLevel && hoodGoods >= DataManager.Instance.HoodData.GetHoodData(hood.Level).GetCost(hood.Level));
    }
}
