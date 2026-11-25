using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

public class UISystemSmallLoadingPanel : UIPanel
{
    [SerializeField] SkeletonGraphic[] spines = null;
    public override void ShowPanel()
    {
        //var weaponData = DataManager.Instance.EquipmentData.Datas.Find(x => x.EquipItemType == EquipItemType.Weapon && x.Data_Id == 1001);

        //var armorData = DataManager.Instance.EquipmentData.Datas.Find(x => x.EquipItemType == EquipItemType.Armor && x.Data_Id == 2001);

        //var helmetData = DataManager.Instance.EquipmentData.Datas.Find(x => x.EquipItemType == EquipItemType.Helmet && x.Data_Id == 3001);

        //var skeleton = spines[0].Skeleton;
        //var skin = new Spine.Skin("new-character");
        //if (skeleton.Data.FindSkin(weaponData.Skin_Name) != null)
        //    skin.AddSkin(skeleton.Data.FindSkin(weaponData.Skin_Name));
        //if (skeleton.Data.FindSkin(armorData.Skin_Name) != null)
        //    skin.AddSkin(skeleton.Data.FindSkin(armorData.Skin_Name));
        //if (skeleton.Data.FindSkin(helmetData.Skin_Name) != null)
        //    skin.AddSkin(skeleton.Data.FindSkin(helmetData.Skin_Name));

        //spines[0].Skeleton.SetSkin(skin);
        //spines[0].Skeleton.SetSlotsToSetupPose();

        //int randIdx = GameUtils.GameUtils.GetRandomRange(0, tipKeys.Length);
        //if (randIdx < 0 || randIdx >= tipKeys.Length)
        //    return;

        //if (tipTexts != null)
        //    tipTexts[0].text = StringManager.Instance.GetStringText(tipKeys[randIdx]);
        base.ShowPanel();
    }
}
