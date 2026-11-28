using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : PoolinObject
{
    const float FrameTime = 1f / 30f;

    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Sprite[] sprites = null;
    [SerializeField] Vector2 size = Vector2.one;   // 캐릭터 크기에 곱하는 이펙트 크기 배율 (가로, 세로)
    [SerializeField] bool useUnscaledTime = false; // 히트스톱(게임 정지) 중에도 재생

    float timer = 0f;
    int loopStart = 0;      // 중간 구간(loopStart부터 loopFrames장)을 loopCount번 더 재생 (지속시간이 긴 스킬)
    int loopFrames = 0;
    int loopCount = 0;

    public void Play(Vector3 _position, Vector3 _scale, int _sortingOrder)
    {
        transform.position = _position;
        transform.localScale = new Vector3(_scale.x * size.x, _scale.y * size.y, _scale.z);
        spriteRenderer.sortingOrder = _sortingOrder;
        spriteRenderer.sprite = sprites[0];
        timer = 0f;
        loopCount = 0;
    }

    public void SetLoop(int _startFrame, int _endFrame, int _count)
    {
        loopStart = _startFrame;
        loopFrames = _endFrame - _startFrame + 1;
        loopCount = _count;
    }

    private void Update()
    {
        timer += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        int frame = (int)(timer / FrameTime);
        int loopEnd = loopStart + loopFrames;
        if (loopCount > 0 && frame >= loopEnd)
        {
            int extra = frame - loopEnd;
            frame = extra < loopFrames * loopCount ? loopStart + extra % loopFrames : frame - loopFrames * loopCount;
        }
        if (frame >= sprites.Length)
        {
            ObjectPoolManager.Instance.ReturnObject(key, this.gameObject);
            return;
        }
        spriteRenderer.sprite = sprites[frame];
    }

    public void SetColor(Color _color)
    {
        spriteRenderer.color = _color;
    }
}
