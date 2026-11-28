using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITitleScene : UIScene
{
    [SerializeField] Image bgImage = null;

    [Header("SafeArea")]
    [SerializeField] TMP_Text versionText = null;

    [SerializeField] TMP_Text loadingText = null;
    [SerializeField] ColorTweener loadingTextColorTweener = null;
    [SerializeField] RectTransform gaugePanel = null;
    [SerializeField] Image gaugeBarImage = null;
    [SerializeField] GameObject touchText = null;       // 로딩이 끝나면 "화면을 터치하세요"

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        bgImage.sprite = ResourceManager.Instance.GetSprite(AtlasType.Bg, "title");

        bool isDev = GameManager.Instance.IsDevelopBuild;
        versionText.text = string.Format("ver.{0} ({1})", Application.version, isDev ? "dev" : "live");

        SetLoadingText(string.Empty);
        SetGuageBar(0f);
    }
    public void SetLoadingText(string _context)
    {
        if (loadingText != null) loadingText.text = _context;
    }
    public void SetGuageBar(float _fillAmount)
    {
        if (gaugeBarImage != null)
        {
            if (_fillAmount < 0.01f)
            {
                StopAllCoroutines();
                gaugeBarImage.fillAmount = 0f;
                return;
            }

            StopAllCoroutines();
            StartCoroutine(CoSetGuageBar(_fillAmount));
        }
    }
    public void SetActiveTouchText(bool _bActive)
    {
        touchText.SetActive(_bActive);
    }
    public void SetActiveGaugeBar(bool _bActive)
    {
        if (gaugePanel != null)
            gaugePanel.gameObject.SetActive(_bActive);
    }
    public void SetActiveColorTweener(bool _bActive)
    {
        if (_bActive)
        {
            if (loadingText.gameObject.activeSelf)
                loadingTextColorTweener.StartTween();
        }
    }

    IEnumerator CoSetGuageBar(float _fillAmount)
    {
        float fTime = 0f;
        while (gaugeBarImage.fillAmount < _fillAmount)
        {
            gaugeBarImage.fillAmount = Mathf.Lerp(gaugeBarImage.fillAmount, _fillAmount, fTime);
            fTime += Time.deltaTime;

            if (gaugeBarImage.fillAmount > _fillAmount)
                break;

            yield return null;
        }

        gaugeBarImage.fillAmount = _fillAmount;
        yield return null;
    }
}
