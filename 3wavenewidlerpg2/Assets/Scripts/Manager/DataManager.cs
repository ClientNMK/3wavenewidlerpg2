using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using BackEnd;

public class DataManager : Singleton<DataManager>
{
    [Header("String Data")]
    public SerializableStringData StringData = new();

    [Header("System Data")]
    public SerializableSystemData SystemData = new();

    [Header("Character Data")]
    public SerializableStatData StatData = new();
    public SerializableMonsterData MonsterData = new();
    public SerializableSkillData SkillData = new();
    public SerializableGoldUpgradeData GoldUpgradeData = new();
    public SerializableCharLevelData CharLevelData = new();
    public SerializableStatPointData StatPointData = new();
    public SerializableHoodData HoodData = new();

    [Header("Stage Data")]
    public SerializableWorldData WorldData = new();
    public SerializableStageData StageData = new();
}
