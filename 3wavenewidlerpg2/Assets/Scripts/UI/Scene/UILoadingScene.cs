using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILoadingScene : MonoBehaviour
{
    [SerializeField] Image bgImage = null;
    [SerializeField] TMP_Text loadingText = null;
    [SerializeField] Image gaugeBarImage = null;

    private void Start()
    {
        bgImage.sprite = ResourceManager.Instance.GetSprite(AtlasType.Bg, "title");
        loadingText.text = StringManager.Instance.GetStringText("Msg_LoadGameDatas");
        gaugeBarImage.fillAmount = 0f;
    }

    public void SetGaugeBar(float _fillAmount)
    {
        gaugeBarImage.fillAmount = _fillAmount;
    }
}
