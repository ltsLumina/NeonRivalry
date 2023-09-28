using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class locktothepath : MonoBehaviour
{
    CinemachineVirtualCamera _vCam;
    public CinemachineDollyCart dollyCart;

    [SerializeField] private int dollycartSpeed;

    private void Start()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
    }

    public void CameraCine(Transform winPlayer)
    {
            _vCam.LookAt = winPlayer;
            dollyCart.m_Speed = dollycartSpeed;
            dollyCart.m_Position = 0;
    }
}
