using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터의 현재 메시를 복사해 반투명하게 남겼다가 사라지는 잔상
public class Afterimage : PoolinObject
{
    const float FadeTime = 0.3f;
    const float StartAlpha = 0.5f;

    [SerializeField] MeshFilter meshFilter = null;
    [SerializeField] MeshRenderer meshRenderer = null;

    Mesh mesh = null;
    Color32[] colors = null;
    float timer = 0f;

    public void Play(CharacterBase _character)
    {
        if (mesh != null)
            Destroy(mesh);

        mesh = Instantiate(_character.SkeletonRenderer.GetComponent<MeshFilter>().sharedMesh);
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterials = _character.SkeletonRenderer.sharedMaterials;
        meshRenderer.sortingOrder = _character.SkeletonRenderer.sortingOrder - 1;

        transform.SetPositionAndRotation(_character.transform.position, _character.transform.rotation);
        transform.localScale = _character.transform.localScale;

        colors = mesh.colors32;
        timer = 0f;
        SetAlpha(StartAlpha);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= FadeTime)
        {
            ObjectPoolManager.Instance.ReturnObject(key, this.gameObject);
            return;
        }
        SetAlpha(StartAlpha * (1f - timer / FadeTime));
    }

    // Spine 버텍스 컬러는 프리멀티플라이드라 RGB도 같이 줄인다
    void SetAlpha(float _alpha)
    {
        byte alpha = (byte)(255 * _alpha);
        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Color32(alpha, alpha, alpha, alpha);
        mesh.colors32 = colors;
    }
}
