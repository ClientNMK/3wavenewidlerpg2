using System.Collections.Generic;
using System.Linq;

// 코스튬 (CostumeChart·CostumePointChart, 저장 없이 이번 플레이만). 상점이 생기기 전까지 테스트용으로 차트 코스튬을 전부 보유
// 보유 효과는 가진 것 모두, 착용 효과는 입은 것만, 포인트 단계는 받은 단계 Stat_Value를 모두 더함
public class InGameCostume
{
    readonly HashSet<int> owned = new();
    readonly HashSet<int> claimedLevels = new();
    CostumeData girl = null;        // 입은 소녀 코스튬 (없으면 기본 모습)
    CostumeData hood = null;        // 입은 후드 코스튬 (없으면 HoodChart Resorce 스킨)

    public string HoodSkin { get { return hood?.Skin_Name; } }
    // 코스튬 포인트: 얻은 코스튬의 Costume_Point 합계 (소녀·후드 함께)
    public int Point { get { return DataManager.Instance.CostumeData.GetCostumes().Where(IsOwned).Sum(x => x.Costume_Point); } }
    public int ClaimableCount { get { return DataManager.Instance.CostumeData.GetPoints().Count(CanClaim); } }

    public void Start()
    {
        foreach (CostumeData costume in DataManager.Instance.CostumeData.GetCostumes())
        {
            owned.Add(costume.Data_Id);
            SetStat(costume.Have_Stat_Id, $"costume_have_{costume.Data_Id}", costume.Have_Stat_Value);
        }
        InGameManager.Instance.UIInGameScene.SetCostumeAlert(ClaimableCount > 0);
    }

    public bool IsOwned(CostumeData _costume)
    {
        return owned.Contains(_costume.Data_Id);
    }

    public bool IsEquipped(CostumeData _costume)
    {
        return _costume == girl || _costume == hood;
    }

    // 입던 것의 착용 효과를 빼고 새 것을 더한 뒤 스킨 바꿈
    public void Equip(CostumeData _costume)
    {
        CostumeData prev = _costume.IsHood ? hood : girl;
        if (prev != null)
            SetStat(prev.Equip_Stat_Id, $"costume_equip_{prev.Data_Id}", 0);
        SetStat(_costume.Equip_Stat_Id, $"costume_equip_{_costume.Data_Id}", _costume.Equip_Stat_Value);
        if (_costume.IsHood)
        {
            hood = _costume;
            InGameManager.Instance.MainCharacter.Hood.SetSkin();
        }
        else
        {
            girl = _costume;
            InGameManager.Instance.MainCharacter.SetCostumeSkin(_costume.Skin_Name);
        }
    }

    // 착용 효과만 빼고 기본 모습으로 (보유 효과는 그대로)
    public void Unequip(CostumeData _costume)
    {
        SetStat(_costume.Equip_Stat_Id, $"costume_equip_{_costume.Data_Id}", 0);
        if (_costume.IsHood)
        {
            hood = null;
            InGameManager.Instance.MainCharacter.Hood.SetSkin();
        }
        else
        {
            girl = null;
            InGameManager.Instance.MainCharacter.SetCostumeSkin("default");
        }
    }

    public bool IsClaimed(CostumePointData _level)
    {
        return claimedLevels.Contains(_level.Level);
    }

    public bool CanClaim(CostumePointData _level)
    {
        return !IsClaimed(_level) && Point >= _level.Point;
    }

    public void Claim(CostumePointData _level)
    {
        claimedLevels.Add(_level.Level);
        SetStat(_level.Stat_Id, $"costume_point_{_level.Level}", _level.Stat_Value);
        InGameManager.Instance.UIInGameScene.SetCostumeAlert(ClaimableCount > 0);
    }

    void SetStat(int _statId, string _key, double _value)
    {
        InGameBuffManager.Instance.BuffStatus.SetCostumeStatData((StatType)_statId, _key, _value);
    }
}
