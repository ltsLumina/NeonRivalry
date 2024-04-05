using UnityEngine;
using UnityEngine.EventSystems;

namespace Abiogenesis3d
{
    public class MouseDragUI : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        KeyCode dragKey = KeyCode.Mouse0;
        Vector2 lastMousePosition;
        RectTransform rectTransform;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!Input.GetKey(dragKey)) return;

            lastMousePosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!Input.GetKey(dragKey)) return;

            Vector2 currentMousePosition = eventData.position;

            currentMousePosition.x = Mathf.Clamp(currentMousePosition.x, 0, Screen.width);
            currentMousePosition.y = Mathf.Clamp(currentMousePosition.y, 0, Screen.height);

            Vector2 diff = currentMousePosition - lastMousePosition;

            rectTransform.position += (Vector3)diff;
            lastMousePosition = currentMousePosition;
        }
    }
}
