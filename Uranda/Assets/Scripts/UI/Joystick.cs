using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image bg;
    public Image handle;
    private Vector2 input;
    private Vector2 pointerOffset;

    public float Horizontal => input.x;
    public float Vertical => input.y;
    public Vector2 Direction => new Vector2(Horizontal, Vertical);

    public bool IsHolding { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bg.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pointerOffset
        );

        IsHolding = true;
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.rectTransform.anchoredPosition = Vector2.zero;
        IsHolding = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bg.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 pos
        );

        pos -= pointerOffset;
        pos /= bg.rectTransform.sizeDelta / 2f;
        input = Vector2.ClampMagnitude(pos, 1f);
        handle.rectTransform.anchoredPosition = input * (bg.rectTransform.sizeDelta / 2f);
    }
}
