using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    private EntityManager entityManager;

    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private float spawnRadius = 5f;

    private Entity spawnEntityPrefab;

    public int nSpawned;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var blob = new BlobAssetStore();

        var settings = GameObjectConversionSettings.FromWorld(
            World.DefaultGameObjectInjectionWorld, blob);

        spawnEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
            spawnPrefab, settings);

        Spawn();

        blob.Dispose();
    }

    private void Spawn()
    {
        NativeArray<Entity> spawnArray = new NativeArray<Entity>(nSpawned, Allocator.Temp);

        for (int i = 0; i < spawnArray.Length; i++)
        {
            spawnArray[i] = entityManager.Instantiate(spawnEntityPrefab);

            entityManager.SetComponentData(spawnArray[i], new Translation {
                Value = RandomPointOnCircle(spawnRadius) });
        }

        spawnArray.Dispose();
    }

    private float3 RandomPointOnCircle(float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle.normalized * radius;

        return new float3(randomPoint.x, 2.5f, randomPoint.y);
    }
}
