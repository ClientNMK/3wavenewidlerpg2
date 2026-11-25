using UnityEngine;

// 캐릭터 머리 위 체력바. fill은 왼쪽 기준 스프라이트라 가로 크기로 비율을 표시한다.
public class HpBar : MonoBehaviour
{
    [SerializeField] Transform fill = null;
    [SerializeField] float width = 1f;

    public void SetRatio(float _ratio)
    {
        Vector3 scale = fill.localScale;
        scale.x = width * Mathf.Clamp01(_ratio);
        fill.localScale = scale;
    }
}
