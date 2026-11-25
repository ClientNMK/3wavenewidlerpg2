using UnityEngine;

public class SafeArea : MonoBehaviour
{
    RectTransform _rectTransform;
    Rect _safeArea;

    Vector2 _minAnchor;
    Vector2 _maxAnchor;

    [SerializeField] bool bMatchWidth = true;
    [SerializeField] bool bMatchHeight = true;
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _safeArea = Screen.safeArea;
        _minAnchor = _safeArea.position;
        _maxAnchor = _minAnchor + _safeArea.size;
        _minAnchor.x = bMatchWidth == true ? _minAnchor.x / Screen.width : _rectTransform.anchorMin.x;
        _minAnchor.y = bMatchHeight == true ? _minAnchor.y / Screen.height : _rectTransform.anchorMin.y;
        _maxAnchor.x = bMatchWidth == true ? _maxAnchor.x / Screen.width : _rectTransform.anchorMax.x;
        _maxAnchor.y = bMatchHeight == true ? _maxAnchor.y / Screen.height : _rectTransform.anchorMax.y;

        _rectTransform.anchorMin = _minAnchor;
        _rectTransform.anchorMax = _maxAnchor;
    }
}
