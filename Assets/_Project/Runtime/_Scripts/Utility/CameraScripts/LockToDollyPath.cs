#region
using Cinemachine;
using UnityEngine;
#endregion

public class LockToDollyPath : MonoBehaviour
{
    // -- Inspector Variables --
    
    [SerializeField] CinemachineDollyCart dollyCart;
    [SerializeField] int dollyCartSpeed;
    
    // -- Cached References --
    
    CinemachineVirtualCamera virtualCam;

    void Awake() => virtualCam = GetComponent<CinemachineVirtualCamera>();

    public void FollowPlayer(Transform winPlayer)
    {
        dollyCart.m_Speed = dollyCartSpeed;
        dollyCart.m_Position = 0;
        virtualCam.LookAt = winPlayer;
    }
}

