 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BGMScripableResource
{
    public BGMType BgmSound = BGMType.Title;
    public AudioClip AudioClip = null;
}

[System.Serializable]
public class SFXScripableResource
{
    public SFXType SoundType = SFXType.Click;
    public AudioClip AudioClip = null;
}

[System.Serializable]
public class SkillSoundScripableResource
{
    public string key = string.Empty;
    public AudioClip AudioClip = null;
}

[CreateAssetMenu(fileName = "SoundScriptableData", menuName = "Scriptable Object/SoundScriptableData", order = int.MaxValue)]
public class SoundScriptableData : ScriptableObject
{
    [Header("BGM")]
    public List<BGMScripableResource> bgmResources = new List<BGMScripableResource>();

    [Header("SFX")]
    [SerializeField] List<SFXScripableResource> effectSoundResources = new List<SFXScripableResource>();
    Dictionary<SFXType, SFXScripableResource> dicSFXResources = new();
    

    [Header("SkillSound")]
    public List<SkillSoundScripableResource> skillSoundResources = new List<SkillSoundScripableResource>();

    public SFXScripableResource GetSFXResource(SFXType _sfxType)
    {
        if (dicSFXResources.ContainsKey(_sfxType))
            return dicSFXResources[_sfxType];

        SFXScripableResource resource = effectSoundResources.Find(x => x.SoundType == _sfxType);
        dicSFXResources.Add(_sfxType, resource);
        return dicSFXResources[_sfxType];
    }
}
