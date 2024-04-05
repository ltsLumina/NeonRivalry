using UnityEngine;

namespace Abiogenesis3d
{
    public class MouseEventsPropagator : MonoBehaviour
    {
        const SendMessageOptions msgOpts = SendMessageOptions.DontRequireReceiver;

        void OnMouseEnter() {transform.parent.SendMessageUpwards("OnMouseEnter", msgOpts);}
        void OnMouseExit() {transform.parent.SendMessageUpwards("OnMouseExit", msgOpts);}
        void OnMouseOver() {transform.parent.SendMessageUpwards("OnMouseOver", msgOpts);}
        void OnMouseDown() {transform.parent.SendMessageUpwards("OnMouseDown", msgOpts);}
        void OnMouseUp() {transform.parent.SendMessageUpwards("OnMouseUp", msgOpts);}
        void OnMouseUpAsButton() {transform.parent.SendMessageUpwards("OnMouseUpAsButton", msgOpts);}

        // NOTE: adding this to enable script checkbox
        void Start() {}
    }
}
