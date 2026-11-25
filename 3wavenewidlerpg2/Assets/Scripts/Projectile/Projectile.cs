using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PoolinObject
{
    const float FrameTime = 0.08f;
    const float MoveSpeed = 6f;

    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Sprite[] flySprites = null;
    [SerializeField] Sprite[] hitSprites = null;

    BigDouble damage;
    CharacterBase eventInstigator = null;
    CharacterBase target = null;
    float timer = 0f;
    bool isHit = false;

    public void Fire(BigDouble _damage, CharacterBase _eventInstigator, CharacterBase _target)
    {
        damage = _damage;
        eventInstigator = _eventInstigator;
        target = _target;
        timer = 0f;
        isHit = false;

        // 원본 아트가 오른쪽을 보고 있으므로 왼쪽 발사 시 반전
        spriteRenderer.flipX = target.transform.position.x < transform.position.x;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100) + 1;

        if (!isHit)
        {
            spriteRenderer.sprite = flySprites[(int)(timer / FrameTime) % flySprites.Length];
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, MoveSpeed * Time.deltaTime);
            if (transform.position == target.transform.position)
            {
                target.TakeDamage(damage, eventInstigator, AttackType.Normal);
                isHit = true;
                timer = 0f;
            }
            return;
        }

        int frame = (int)(timer / FrameTime);
        if (frame >= hitSprites.Length)
        {
            ObjectPoolManager.Instance.ReturnObject(key, this.gameObject);
            return;
        }
        spriteRenderer.sprite = hitSprites[frame];
    }
}
