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
    [SerializeField] Image skillCoolImage = null;
    [SerializeField] TMP_Text skillCoolText = null;

    [Header("Stage")]
    [SerializeField] TMP_Text stageText = null;
    [SerializeField] TMP_Text killCountText = null;
    [SerializeField] Button bossButton = null;
    [SerializeField] TMP_Text bossButtonText = null;

    [Header("Profile")]
    [SerializeField] TMP_Text nicknameText = null;
    [SerializeField] TMP_Text goldText = null;

    [Header("Kill Log")]
    [SerializeField] TMP_Text killLogText = null;
    const int KillLogMax = 5;
    const float KillLogShowTime = 1f;
    const float KillLogFadeTime = 0.5f;
    readonly List<(string text, float startTime)> killLogs = new();

    private void Start()
    {
        bossButton.onClick.AddListener(InGameManager.Instance.StartBossChallenge);
        nicknameText.text = BackEnd.Backend.UserNickName;
    }

    private void Update()
    {
        MainCharacter mainCharacter = InGameManager.Instance.MainCharacter;
        SetCoolBox(rollCoolImage, rollCoolText, mainCharacter.RollRemainTime, DataManager.Instance.SystemData.GetFloat("Roll_Cooltime"));
        SetCoolBox(skillCoolImage, skillCoolText, mainCharacter.SkillRemainTime, mainCharacter.SkillData.Cooltime);

        if (killLogs.Count > 0)
            RefreshKillLog();
    }

    void SetCoolBox(Image _coolImage, TMP_Text _coolText, float _remainTime, float _coolTime)
    {
        _coolImage.fillAmount = _remainTime / _coolTime;
        _coolText.text = _remainTime > 0f ? _remainTime.ToString("0.0") : string.Empty;
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

    public void SetStageText(string _stage, string _killCount)
    {
        stageText.text = _stage;
        killCountText.text = _killCount;
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
