using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 몬스터가 떨어뜨리는 재화. 통통 튀며 떨어진 뒤 잠깐 머물다 캐릭터에게 날아가 사라진다.
public class GoodsDrop : PoolinObject
{
    enum Phase
    {
        Bounce,
        Stay,
        Fly,
    }

    const float FrameTime = 0.08f;
    const float JumpSpeed = 8f;
    const float Gravity = 30f;
    const float BounceRate = 0.4f;     // 튈 때마다 남는 속도 비율
    const int BounceCount = 3;
    const float ScatterSpeed = 1f;
    const float StayTime = 0.5f;
    const float FlyTime = 0.3f;

    [SerializeField] SpriteRenderer spriteRenderer = null;     // 튀는 높이만큼 위로 올리는 자식
    [SerializeField] Sprite[] sprites = null;

    Phase phase = Phase.Bounce;
    Vector3 groundPosition = Vector3.zero;
    Vector3 scatter = Vector3.zero;
    Vector3 flyStartPosition = Vector3.zero;
    float height = 0f;
    float velocity = 0f;
    int bounce = 0;
    float timer = 0f;
    float animTimer = 0f;

    public void Play(Vector3 _position, Vector3 _scale)
    {
        transform.localScale = _scale;
        groundPosition = _position;
        scatter = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f), 0f) * ScatterSpeed;
        height = 0f;
        velocity = JumpSpeed;
        bounce = 0;
        timer = 0f;
        phase = Phase.Bounce;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        animTimer += deltaTime;
        spriteRenderer.sprite = sprites[(int)(animTimer / FrameTime) % sprites.Length];

        switch (phase)
        {
            case Phase.Bounce:
                groundPosition += scatter * deltaTime;
                velocity -= Gravity * deltaTime;
                height += velocity * deltaTime;
                if (height <= 0f)
                {
                    height = 0f;
                    velocity = -velocity * BounceRate;
                    if (++bounce >= BounceCount)
                        phase = Phase.Stay;
                }
                break;

            case Phase.Stay:
                timer += deltaTime;
                if (timer >= StayTime)
                {
                    timer = 0f;
                    flyStartPosition = groundPosition;
                    phase = Phase.Fly;
                }
                break;

            case Phase.Fly:
                timer += deltaTime;
                if (timer >= FlyTime)
                {
                    ObjectPoolManager.Instance.ReturnObject(key, this.gameObject);
                    return;
                }
                groundPosition = Vector3.Lerp(flyStartPosition, InGameManager.Instance.MainCharacter.transform.position, timer / FlyTime);
                break;
        }

        transform.position = groundPosition;
        spriteRenderer.transform.localPosition = new Vector3(0f, height, 0f);
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-groundPosition.y * 100) + 1;
    }
}
