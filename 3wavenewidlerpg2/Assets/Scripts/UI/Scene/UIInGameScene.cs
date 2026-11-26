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
    [SerializeField] TMP_Text killCountText = null;
    [SerializeField] Image stageProgressFill = null;
    [SerializeField] Button bossButton = null;
    [SerializeField] TMP_Text bossButtonText = null;

    [Header("Profile")]
    [SerializeField] TMP_Text nicknameText = null;
    [SerializeField] TMP_Text goldText = null;

    [Header("Camera")]
    [SerializeField] Button cameraZoomButton = null;
    [SerializeField] TMP_Text cameraZoomText = null;

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
        cameraZoomButton.onClick.AddListener(InGameManager.Instance.ToggleCameraZoom);
        nicknameText.text = BackEnd.Backend.UserNickName;
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
        killLogs.Add((string.Format(StringManager.Instance.GetStringText("Msg_KillReward"), _gold, _exp), Time.time));
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
        stageText.text = _stage;
        killCountText.text = $"{_killCount}/{_maxCount}";
        stageProgressFill.fillAmount = (float)_killCount / _maxCount;
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

    public void SetCameraZoomText(string _text)
    {
        cameraZoomText.text = _text;
    }

    public void SetGold(double _gold)
    {
        goldText.text = _gold.ToString("N0");
    }

    public void SetBossButton(bool _interactable, string _text)
    {
        bossButton.interactable = _interactable;
        bossButtonText.text = _text;
    }
}
