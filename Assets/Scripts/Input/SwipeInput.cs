using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 SwipeDirection { get; private set; }

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    public bool SwipeDetected { get; private set; }

    [SerializeField] private float minimumSwipeDistance = 50f;

    public void OnPointerDown(PointerEventData eventData)
    {
        startTouchPosition = eventData.position;
        SwipeDetected = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        endTouchPosition = eventData.position;

        Vector2 swipe = endTouchPosition - startTouchPosition;

        if (swipe.magnitude >= minimumSwipeDistance)
        {
            SwipeDirection = swipe.normalized;
            SwipeDetected = true;
        }
    }

    public void ResetSwipe()
    {
        SwipeDetected = false;
    }
}