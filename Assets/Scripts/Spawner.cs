using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    private EntityManager entityManager;

    [SerializeField] private GameObject[] spawnPrefabs = new GameObject[2];
    [SerializeField] private float spawnRadius = 5f;

    private readonly Entity[] spawnEntityPrefabs = new Entity[2];

    public int nSpawned;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var blob = new BlobAssetStore();

        var settings = GameObjectConversionSettings.FromWorld(
            World.DefaultGameObjectInjectionWorld, blob);

        for (int i = 0; i < spawnPrefabs.Length; i++)
        {
            spawnEntityPrefabs[i] = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                spawnPrefabs[i], settings);
        }

        Spawn();

        blob.Dispose();
    }

    private void Spawn()
    {
        NativeArray<Entity> spawnArray = new NativeArray<Entity>(nSpawned, Allocator.Temp);

        for (int i = 0; i < spawnArray.Length; i++)
        {
            int randomIndex = Random.Range(0, spawnEntityPrefabs.Length);
            spawnArray[i] = entityManager.Instantiate(spawnEntityPrefabs[randomIndex]);

            entityManager.SetComponentData(spawnArray[i], new Translation {
                Value = RandomPointInCircle(spawnRadius) });
        }

        spawnArray.Dispose();
    }

    private float3 RandomPointInCircle(float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle * radius;

        return new float3(randomPoint.x, 0, randomPoint.y);
    }
}
