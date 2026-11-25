using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RectTransform rect = null;
    [SerializeField] Image joystickBgImage;
    [SerializeField] Image joystickAngleImage;
    [SerializeField] Image joystickImage;

    public Vector3 InputDirection;

    public System.Action onDrag = null;
    public System.Action onDragEnd = null;

    private bool isActive = false;
    public bool IsDragController { get { return isActive; } }

    // Start is called before the first frame update
    void Start()
    {
        if (rect == null) rect = this.GetComponent<RectTransform>();

        if (joystickBgImage == null)
            joystickBgImage = this.transform.Find("background").GetComponent<Image>();

        if (joystickAngleImage == null)
            joystickAngleImage = this.transform.Find("background/angleImage").GetComponent<Image>();

        if (joystickImage == null)
            joystickImage = this.transform.Find("background/stick").GetComponent<Image>();

        joystickBgImage.gameObject.SetActive(false);
        joystickAngleImage.gameObject.SetActive(joystickAngleImage);
        joystickImage.gameObject.SetActive(false);

        joystickImage.rectTransform.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (isActive && onDrag != null)
        {
            onDrag();
        }
    }

    public void Drag(Vector2 positon)
    {
        Vector2 normalizePos = new Vector3((positon.x / (joystickBgImage.rectTransform.sizeDelta.x / 2)), (positon.y / (joystickBgImage.rectTransform.sizeDelta.y / 2)));
        // pos.x = (pos.x / joystickBgImage.rectTransform.sizeDelta.x);
        // pos.y = (pos.y / joystickBgImage.rectTransform.sizeDelta.y);

        if (normalizePos.magnitude > 1) normalizePos.Normalize();

        joystickImage.rectTransform.anchoredPosition = new Vector2(joystickBgImage.rectTransform.sizeDelta.x / 2 * normalizePos.x, joystickBgImage.rectTransform.sizeDelta.y / 2 * normalizePos.y);
        InputDirection = normalizePos;

        float angle = Mathf.Atan2(InputDirection.y, InputDirection.x) * Mathf.Rad2Deg;
        joystickAngleImage.rectTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //pos.x = (pos.x / joystickBgImage.rectTransform.sizeDelta.x);
        //pos.y = (pos.y / joystickBgImage.rectTransform.sizeDelta.y);

        //float x = (joystickBgImage.rectTransform.pivot.x == 1f) ? pos.x * 2 + 1 : pos.x * 2 - 1;
        //float y = (joystickBgImage.rectTransform.pivot.y == 1f) ? pos.y * 2 + 1 : pos.y * 2 - 1;

        //InputDirection = new Vector3(x, 0, y);
        //InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

        //joystickImage.rectTransform.anchoredPosition = new Vector3(InputDirection.x * (joystickBgImage.rectTransform.sizeDelta.x / 2), InputDirection.z * (joystickBgImage.rectTransform.sizeDelta.y / 2));

        isActive = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBgImage.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            Drag(pos);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        joystickBgImage.gameObject.SetActive(true);
        joystickImage.gameObject.SetActive(true);

        Vector2 pos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out pos);
        joystickBgImage.transform.localPosition = pos;
        joystickAngleImage.rectTransform.rotation = Quaternion.Euler(0, 0, 0);

        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickBgImage.gameObject.SetActive(false);
        joystickImage.gameObject.SetActive(false);

        InputDirection = Vector3.zero;
        joystickImage.rectTransform.anchoredPosition = Vector2.zero;

        isActive = false;
        if (onDragEnd != null)
            onDragEnd();
    }


    public void SetAnchoredPosition(Vector3 position)
    {
        // Debug.Log(position);
        Vector3  Pos = Camera.main.ScreenToViewportPoint(position);
        if (rect != null)
            rect.anchoredPosition = Pos;
    }

    public void SetActive(bool bActive)
    {
        if (this.gameObject.activeSelf != bActive)
            this.gameObject.SetActive(bActive);
    }

}
