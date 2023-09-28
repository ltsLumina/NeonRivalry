using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class locktothepath : MonoBehaviour
{
    CinemachineVirtualCamera _vCam;
    public CinemachineDollyCart dollyCart;

    private void Start()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
    }

    public void CameraCine(Transform winPlayer)
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            _vCam.LookAt = winPlayer;
            dollyCart.m_Speed = 0;
            dollyCart.m_Position = 0;
            //transform.position = Vector3.Lerp(transform.position,
            //dollyCart.transform.position, 0.01f);

        }
    }
}
