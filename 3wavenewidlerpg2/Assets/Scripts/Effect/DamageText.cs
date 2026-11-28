using UnityEngine;
using TMPro;

// 맞은 몬스터 머리 위로 떠올랐다가 사라지는 데미지 숫자 (메인 캐릭터가 받은 피해는 아래로 내려감)
public class DamageText : PoolinObject
{
    const float RiseDistance = 0.5f;
    const float LifeTime = 0.7f;
    const float FadeTime = 0.3f;
    static readonly Color NormalColor = new(1f, 0.9f, 0.4f);   // 프리팹 기본 색
    static readonly Color CriColor = new(1f, 0.35f, 0.25f);
    static readonly Color TakenColor = new(1f, 0.55f, 0.55f);

    [SerializeField] TextMeshPro text = null;

    Vector3 startPosition = Vector3.zero;
    float timer = 0f;
    float direction = 1f;

    public void Play(Vector3 _position, BigDouble _damage, bool _isCri, bool _isTaken = false)
    {
        startPosition = _position;
        transform.position = _position;
        text.text = _damage.ToDouble().ToString("N0");
        text.color = _isTaken ? TakenColor : _isCri ? CriColor : NormalColor;
        direction = _isTaken ? -1f : 1f;
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= LifeTime)
        {
            ObjectPoolManager.Instance.ReturnObject(key, this.gameObject);
            return;
        }

        // 처음에 빠르게 움직이다 느려지고, 끝에서 투명해짐
        float t = timer / LifeTime;
        transform.position = startPosition + Vector3.up * direction * RiseDistance * (1f - (1f - t) * (1f - t));
        text.alpha = Mathf.Clamp01((LifeTime - timer) / FadeTime);
    }
}
