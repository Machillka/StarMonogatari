using UnityEngine;
using Unity.Cinemachine;
public class SelfDebuger : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("当前 Cinemachine Brain 绑定的相机: " + CinemachineCore.Instance.GetActiveBrain(0)?.ActiveVirtualCamera);
    }
}
