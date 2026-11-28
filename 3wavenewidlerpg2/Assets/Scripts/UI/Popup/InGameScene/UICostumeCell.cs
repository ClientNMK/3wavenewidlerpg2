using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

// 의상 창 의상 칸 한 개 (LAYOUT_costume.md '의상 칸'). 그림은 idle 첫 프레임에 멈춤(timeScale 0), 칸 밖은 RectMask2D로 잘라냄
public class UICostumeCell : MonoBehaviour
{
    const float BodyHeight = 92f;
    static readonly Vector2 Foot = new(58f, -100f);         // 칸 왼쪽 위 기준 몸 아래 가운데
    static readonly Color LockColor = new Color32(0x6a, 0x5a, 0x4a, 0xff);
    static readonly Color SubColor = new Color32(0xc2, 0xa8, 0x88, 0xff);

    [SerializeField] Button button = null;
    [SerializeField] Image floor = null;
    [SerializeField] Image line = null;
    [SerializeField] SkeletonGraphic graphic = null;
    [SerializeField] TMP_Text gradeText = null;
    [SerializeField] GameObject select = null;          // 고른 칸
    [SerializeField] GameObject equipped = null;        // '착용' 꼬리표
    [SerializeField] GameObject lockIcon = null;

    public void Setup(CostumeData _costume, SkeletonDataAsset _asset, bool _isOwned, bool _isEquipped, bool _isSelected, Action _onClick)
    {
        Color grade = UICostumePopup.GradeColors[_costume.Grade - 1];
        floor.color = line.color = _isOwned ? grade : LockColor;
        UICostumePopup.SetSkeleton(graphic, _asset, _costume.Skin_Name, false);
        graphic.rectTransform.anchoredPosition = Foot + GameUtils.GameUtils.FitSkeletonBody(graphic, BodyHeight, false);
        graphic.color = _isOwned ? Color.white : UICostumePopup.ShadowColor;
        gradeText.text = _isOwned ? StringManager.Instance.GetStringText($"Name_Grade_{_costume.Grade}") : "???";
        gradeText.color = _isOwned ? grade : SubColor;
        select.SetActive(_isSelected);
        equipped.SetActive(_isEquipped);
        lockIcon.SetActive(!_isOwned);
        button.onClick.AddListener(() => _onClick());
    }
}
