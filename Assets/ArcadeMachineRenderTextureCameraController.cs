#region
using Lumina.Essentials.Modules;
using UnityEngine;
#endregion

/// <summary>
///     I felt like having a really long name for this class.
/// </summary>
public class ArcadeMachineRenderTextureCameraController : MonoBehaviour
{
    void LateUpdate() => transform.position = Helpers.CameraMain.transform.position;
}
