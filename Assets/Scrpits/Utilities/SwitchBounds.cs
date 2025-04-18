using UnityEngine;
using Unity.Cinemachine;

public class SwitchBounds : MonoBehaviour
{
    //TODO[x]: 根据场景切换修改边界
    // private void Start()
    // {
    //     SwitchConfinerShape();
    // }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += SwitchConfinerShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= SwitchConfinerShape;
    }

    private void SwitchConfinerShape()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundConfiner").GetComponent<PolygonCollider2D>();

        CinemachineConfiner2D confiner = GetComponent<CinemachineConfiner2D>();

        confiner.BoundingShape2D = confinerShape;
    }
}
