using UnityEngine;
using TMPro;

// 맞은 몬스터 위로 떠오르는 데미지 숫자 (kit_cocoa TEXT_STYLE '데미지 숫자', 360 기준 ×2.5 = 900 화면 px → ÷100칸)
// 메인 캐릭터가 받은 피해는 아래로 내려감
public class DamageText : PoolinObject
{
    const float RiseTime = 0.5f;            // 처음 빠르고 끝에 느림: 1 − 2^(−10k)
    const float LifeTime = 0.95f;
    const float FadeTime = 0.2f;
    const float ScreenMargin = 0.06f;       // 화면 가장자리 6px 안쪽
    const float NormalSize = 2.5f;          // 10pt
    const float CriSize = 4f;               // 16pt
    const float NormalRise = 0.35f;         // 14
    const float CriRise = 0.5f;             // 20
    static readonly Color NormalColor = new Color32(0xff, 0xe2, 0x6b, 0xff);
    static readonly Color CriColor = new Color32(0xff, 0xf4, 0xc8, 0xff);
    static readonly Color TakenColor = new(1f, 0.55f, 0.55f);

    [SerializeField] TextMeshPro text = null;
    [SerializeField] Material normalMaterial = null;    // 테두리 (#2A1A14)
    [SerializeField] Material criMaterial = null;       // 치명타 테두리 (#961E14)

    Vector3 startPosition = Vector3.zero;
    float timer = 0f;
    float rise = 0f;

    public void Play(Vector3 _position, BigDouble _damage, bool _isCri, bool _isTaken = false)
    {
        text.text = _damage.ToDouble().ToString("N0");
        text.fontSize = _isCri ? CriSize : NormalSize;
        text.color = _isTaken ? TakenColor : _isCri ? CriColor : NormalColor;
        text.fontSharedMaterial = _isCri ? criMaterial : normalMaterial;
        rise = (_isCri ? CriRise : NormalRise) * (_isTaken ? -1f : 1f);

        // 가로 가운데 정렬이라 글자 폭 절반만큼 더 안쪽
        Camera camera = InGameManager.Instance.MainCamera;
        float halfHeight = camera.orthographicSize;
        float halfWidth = halfHeight * camera.aspect;
        Vector2 size = text.GetPreferredValues();
        _position.x = Mathf.Clamp(_position.x, camera.transform.position.x - halfWidth + ScreenMargin + size.x * 0.5f, camera.transform.position.x + halfWidth - ScreenMargin - size.x * 0.5f);
        _position.y = Mathf.Clamp(_position.y, camera.transform.position.y - halfHeight + ScreenMargin + size.y * 0.5f, camera.transform.position.y + halfHeight - ScreenMargin - size.y * 0.5f - Mathf.Max(rise, 0f));
        startPosition = _position;
        transform.position = _position;
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

        float k = Mathf.Min(timer / RiseTime, 1f);
        transform.position = startPosition + Vector3.up * rise * (1f - Mathf.Pow(2f, -10f * k));
        text.alpha = Mathf.Clamp01((LifeTime - timer) / FadeTime);
    }
}
