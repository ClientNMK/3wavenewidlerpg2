using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 수정하지 말 것
public enum SFXPlayType
{
    InGame = 0,
    UI,
}

public enum SFXType
{
    Click = 1,
    // Summon,
    // Popup,
    // LevelUp,
    // Gain,
    Gotcha,
    UI_Promo,
    UI_Eat,
    UI_Reward,
    UI_Guide,
    UI_LevelUp,
    UI_ContentsOpen,
    UI_Enhance,
    UI_Inject_S,
    UI_Inject_F,


    Attack = 101,
    HeavyAttack,

    Skill_001 = 201,
    Skill_002,
    Skill_003,
    Skill_004,
    Skill_005,
    Skill_006,
    Skill_007,
    Skill_008,
    Skill_009,
    Skill_010,
    Skill_011,
    Skill_012,
    Skill_013,
    Skill_014_1,
    Skill_014_2,
    Skill_015,
    Skill_016,
    Skill_017,
    Skill_018,
    Skill_019,
    Skill_020,

    Dash,
    Hit,
}

public enum BGMType
{
    Title = 1,

    Stage_Normal_1 = 11,
    Stage_Normal_2 = 12,
    Stage_Normal_3 = 13,
    Stage_Normal_4 = 14,
    Stage_Normal_5 = 15,
    Stage_Boss,

    Dungeon_Gold = 21,
    Dungeon_Equip,
    Dungeon_Skill,
    Dungeon_Heart,
    Dungeon_Material,
    Dungeon_Core,

    Challenge_1 = 31,
    Challenge_2,

    Weekly = 41,

    Guild = 51,

}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] SoundScriptableData soundScriptableData = null;

    [SerializeField] AudioSource bgmSource = null;
    [SerializeField] AudioSource[] effectAudioSource = null;

    public int bgmVolume = 1;
    public int effectVolume = 1;
    public float effectVolumeMultifly = 0.3f;
    public bool[] isEffectVolumeActive = new bool[(int)SFXPlayType.UI + 1];
    void Start()
    {
        Init();

        for (int i = 0; i < (int)SFXPlayType.UI + 1; i++)
            isEffectVolumeActive[i] = true;
    }

    public void Init()
    {
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        for (int i = 0; i < effectAudioSource.Length; i++)
        {
            effectAudioSource[i].loop = false;
            effectAudioSource[i].playOnAwake = false;
        }

        SetBgmVolume(GetBgmVolume());
        SetSFXVolume(GetSFXVolume());
    }

    public int GetBgmVolume()
    {
        bgmVolume = PlayerPrefs.GetInt("BGM", 1);
        return bgmVolume;
    }
    public int GetSFXVolume()
    {
        effectVolume = PlayerPrefs.GetInt("SFX", 1);
        return effectVolume;
    }

    public void SetBgmVolume(int value)
    {
        PlayerPrefs.SetInt("BGM", value);
        bgmVolume = GetBgmVolume();
        if (value < 1)
            Pause(true);
        else
            Pause(false);
    }
    public void SetSFXVolume(int value)
    {
        PlayerPrefs.SetInt("SFX", value);
        effectVolume = GetSFXVolume();
        for (int i = 0; i < effectAudioSource.Length; i++)
        {
            // effectAudioSource[i].volume = (effectVolume * 0.01f) * effectVolumeMultifly;
            effectAudioSource[i].volume = effectVolume;
        }
    }

    public void PlaySFX(SFXPlayType playType, SFXType soundType)
    {
        float volume = GetSFXVolume();
        if (volume <= 0)
            return;

        if ((int)playType < 0 || (int)playType >= effectAudioSource.Length)
            return;

        AudioSource audioSource = effectAudioSource[(int)playType];
        SFXScripableResource resource = soundScriptableData.GetSFXResource(soundType);
        if (resource == null)
            return;

        AudioClip clip = resource != null ? resource.AudioClip : null;
        if (audioSource != null && clip != null)
        {
            if (isEffectVolumeActive[(int)playType])
                audioSource.PlayOneShot(clip);
        }
    }
    public void PlaySkillEffectSound(string keyName)
    {
        float volume = GetSFXVolume();
        if (volume <= 0)
            return;

        SFXPlayType playType = SFXPlayType.InGame;
        if ((int)playType < 0 || (int)playType >= effectAudioSource.Length)
            return;

        AudioSource audioSource = effectAudioSource[(int)playType];
        SkillSoundScripableResource resource = soundScriptableData.skillSoundResources.Find(x => x.key.Equals(keyName));
        AudioClip clip = resource != null ? resource.AudioClip : null;
        if (audioSource != null && clip != null)
        {
            if (isEffectVolumeActive[(int)playType])
                audioSource.PlayOneShot(clip);
        }
    }
    public void SetSFX(SFXPlayType playType, bool bActive)
    {
        if ((int)playType < 0 || (int)playType >= effectAudioSource.Length)
            return;

        AudioSource audioSource = effectAudioSource[(int)playType];
        if (audioSource != null)
        {
            isEffectVolumeActive[(int)playType] = bActive;
            if (bActive == false) audioSource.Stop();
        }
    }
    public void PlayBgm(BGMType bgmType)
    {
        float volume = GetBgmVolume();
        if (volume <= 0)
            return;

        if (soundScriptableData.bgmResources.Count < 1)
            return;

        BGMScripableResource resource = soundScriptableData.bgmResources.Find(x => x.BgmSound == bgmType);
        AudioClip clip = resource != null ? resource.AudioClip : null;
        if (clip != null)
        {
            if (bgmSource.clip == clip)
                return;

            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }
    public void Pause(bool bPause)
    {
        //float volume = GetBgmVolume();
        //if (volume <= 0)
        //    return;
        if (bgmSource != null)
        {
            if (bPause) bgmSource.Pause();
            else bgmSource.Play();

        }
    }
}
