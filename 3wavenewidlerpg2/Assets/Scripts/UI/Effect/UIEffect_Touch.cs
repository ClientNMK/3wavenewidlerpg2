using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class UIEffect_Touch : MonoBehaviour
{
    [SerializeField] SkeletonGraphic[] spines = null;
    int idx = 0;

    bool bInitialized = false;
    System.Action OnInitialized = null;
    private void Start()
    {
        Init();
    }

    void Init()
    {
        if (bInitialized)
            return;

        foreach (var spine in spines)
        {
            spine.AnimationState.Complete += (Spine.TrackEntry trackEntry) =>
            {
                this.HandleAnimComplete(spine, trackEntry);
            };
            spine.gameObject.SetActive(false);
        }
        idx = 0;

        bInitialized = true;

        if (OnInitialized != null)
            OnInitialized();
        OnInitialized = null;
    }
    public void ShowEffect(Vector2 mousePosition)
    {
        if (!bInitialized)
        {
            if (this.gameObject.activeSelf == false)
                this.gameObject.SetActive(true);
            OnInitialized += () => { this.ShowEffect(mousePosition); };
            return;
        }

        SkeletonGraphic spine = spines[idx++];
        if (idx >= spines.Length) idx = 0;
        if (spine == null)
            return;

        if (spine.gameObject.activeSelf == false)
            spine.gameObject.SetActive(true);
        spine.AnimationState.SetAnimation(0, "common", false);
        spine.timeScale = 2.0f;
        spine.transform.position = mousePosition;
    }

    void HandleAnimComplete(SkeletonGraphic spine, TrackEntry trackEntry)
    {
        if (spine == null)
            return;

        if (trackEntry.Animation.Name.Equals("common"))
        {
            spine.gameObject.SetActive(false);
        }
    }
}
