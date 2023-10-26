#region
using Cinemachine;
using UnityEngine;
#endregion

public class LockToDollyPath : MonoBehaviour
{
    CinemachineVirtualCamera virtualCam;
    public CinemachineDollyCart dollyCart;

    [SerializeField] int dollyCartSpeed;

    void Start() => virtualCam = GetComponent<CinemachineVirtualCamera>();

    public void CameraCine(Transform winPlayer)
    {
        virtualCam.LookAt = winPlayer;
        dollyCart.m_Speed = dollyCartSpeed;
        dollyCart.m_Position = 0;
    }
}

