using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 일반 스테이지에서 쓰러짐: 가벼운 판 + 다시 시작까지 바 (kit_cocoa LAYOUT_fail.md B)
public class UIDeathPanel : MonoBehaviour
{
    [SerializeField] GameObject overlay = null;     // 어두운 판 + 빨간 비네트 (HUD 뒤, HUD는 그대로 보임)
    [SerializeField] Image restartFill = null;
    [SerializeField] TMP_Text restartText = null;

    public IEnumerator CoPlay(float _restartTime)
    {
        gameObject.SetActive(true);
        overlay.SetActive(true);
        for (float t = 0f; t < _restartTime; t += Time.deltaTime)
        {
            restartFill.fillAmount = t / _restartTime;
            restartText.text = string.Format(StringManager.Instance.GetStringText("Msg_RestartIn"), Mathf.CeilToInt(_restartTime - t));
            yield return null;
        }
        gameObject.SetActive(false);
        overlay.SetActive(false);
    }
}
