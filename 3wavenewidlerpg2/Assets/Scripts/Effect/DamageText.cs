using UnityEngine;
using TMPro;

// 맞은 몬스터 머리 위로 떠올랐다가 사라지는 데미지 숫자
public class DamageText : PoolinObject
{
    const float RiseDistance = 0.5f;
    const float LifeTime = 0.7f;
    const float FadeTime = 0.3f;
    static readonly Color NormalColor = new(1f, 0.9f, 0.4f);   // 프리팹 기본 색
    static readonly Color CriColor = new(1f, 0.35f, 0.25f);

    [SerializeField] TextMeshPro text = null;

    Vector3 startPosition = Vector3.zero;
    float timer = 0f;

    public void Play(Vector3 _position, BigDouble _damage, bool _isCri)
    {
        startPosition = _position;
        transform.position = _position;
        text.text = _damage.ToDouble().ToString("N0");
        text.color = _isCri ? CriColor : NormalColor;
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

        // 처음에 빠르게 올라가다 느려지고, 끝에서 투명해짐
        float t = timer / LifeTime;
        transform.position = startPosition + Vector3.up * RiseDistance * (1f - (1f - t) * (1f - t));
        text.alpha = Mathf.Clamp01((LifeTime - timer) / FadeTime);
    }
}
