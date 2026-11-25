using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : PoolinObject
{
    const float FrameTime = 1f / 30f;

    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Sprite[] sprites = null;
    [SerializeField] Vector2 size = Vector2.one;   // 캐릭터 크기에 곱하는 이펙트 크기 배율 (가로, 세로)

    float timer = 0f;

    public void Play(Vector3 _position, Vector3 _scale, int _sortingOrder)
    {
        transform.position = _position;
        transform.localScale = new Vector3(_scale.x * size.x, _scale.y * size.y, _scale.z);
        spriteRenderer.sortingOrder = _sortingOrder;
        spriteRenderer.sprite = sprites[0];
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        int frame = (int)(timer / FrameTime);
        if (frame >= sprites.Length)
        {
            ObjectPoolManager.Instance.ReturnObject(key, this.gameObject);
            return;
        }
        spriteRenderer.sprite = sprites[frame];
    }
}
