using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static string nextScene;
    public static Action OnComplete = null;

    [SerializeField] UILoadingScene uiLoadingScene = null;
    private void Start()
    {
        StopAllCoroutines();
        StartCoroutine(CoLoadScene());
    }

    public static void LoadScene(string _sceneName, Action _OnComplete = null)
    {
        nextScene = _sceneName;
        OnComplete = _OnComplete;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator CoLoadScene()
    {
        PopupManager.Instance.ReleaseAllPopup();

        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            // allowSceneActivation이 false면 progress는 0.9에서 멈춤
            uiLoadingScene.SetGaugeBar(Mathf.Min(op.progress / 0.9f, timer));
            if (op.progress >= 0.9f && timer >= 1.0f)
                break;
        }

        ObjectPoolManager.Instance.ResourcesLoadAllAndAddOriginPoolingObject();
        ObjectPoolManager.Instance.Release();
        Resources.UnloadUnusedAssets();
        GC.Collect();

        yield return new WaitForSeconds(1f);

        if (OnComplete != null)
            OnComplete();
        OnComplete = null;
        op.allowSceneActivation = true;
        yield return null;
    }
}
