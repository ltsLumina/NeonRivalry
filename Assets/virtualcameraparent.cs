using Cinemachine;
using UnityEngine;

public class Virtualcameraparent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<CinemachineVirtualCamera>().enabled = true;
    }
}
