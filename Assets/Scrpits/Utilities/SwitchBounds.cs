using UnityEngine;
using Unity.Cinemachine;

public class SwitchBounds : MonoBehaviour
{
    //TODO: 根据场景切换修改边界
    private void Start()
    {
        SwitchConfinerShape();
    }

    private void SwitchConfinerShape()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundConfiner").GetComponent<PolygonCollider2D>();

        CinemachineConfiner2D confiner = GetComponent<CinemachineConfiner2D>();

        confiner.BoundingShape2D = confinerShape;
    }
}
