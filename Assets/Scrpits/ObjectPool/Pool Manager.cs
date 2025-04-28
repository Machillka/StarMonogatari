using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;
    private List<ObjectPool<GameObject>> _poolEffectList = new List<ObjectPool<GameObject>>();

    private void Start()
    {
        CreatePool();
    }

    private void OnEnable()
    {
        EventHandler.ParticalEffectEvent += OnParticalEffectEvent;
    }

    private void OnDisable()
    {
        EventHandler.ParticalEffectEvent -= OnParticalEffectEvent;
    }

    private void CreatePool()
    {
        foreach (GameObject prefab in poolPrefabs)
        {
            Transform parent = new GameObject(prefab.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(prefab, parent),
                e =>
                {
                    e.SetActive(true);
                },
                e =>
                {
                    e.SetActive(false);
                },
                e =>
                {
                    Destroy(e);
                }
            );

            _poolEffectList.Add(newPool);
        }
    }

    private void OnParticalEffectEvent(ParticalEffetcTypes effectType, Vector3 position)
    {
        //WORKFLOW 补齐其他需要的特效
        var objectPool = effectType switch
        {
            ParticalEffetcTypes.LeaveFalling01 => _poolEffectList[0],
            ParticalEffetcTypes.LeaveFalling02 => _poolEffectList[1],
            ParticalEffetcTypes.Rock => _poolEffectList[2],
            ParticalEffetcTypes.ReapableScenery => _poolEffectList[3],
            _ => null
        };

        GameObject effect = objectPool.Get();

        effect.transform.position = position;

        StartCoroutine(ReleaseRoutine(objectPool, effect));
    }

    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> objectPool, GameObject effect)
    {
        yield return new WaitForSeconds(1.5f);
        objectPool.Release(effect);
    }
}
