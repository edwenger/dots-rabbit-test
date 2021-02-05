using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BirthSystem : SystemBase
{
    EndInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    Entity templateEntity = Entity.Null;
    TemplateData templateData;

    public float spawnRadius = 5.0f;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float spawnRadius_ = spawnRadius;  // TODO: find a good pattern for all system configuration variables

        var randomArray = World.GetExistingSystem<RandomSystem>().RandomArray;

        if (templateEntity == Entity.Null)
        {
            EntityQuery templateQuery = GetEntityQuery(ComponentType.ReadOnly<TemplateData>());

            if (!templateQuery.IsEmpty)
            {
                templateEntity = templateQuery.GetSingletonEntity();
                //Debug.Log("templateEntity = " + templateEntity.Index);
                templateData = EntityManager.GetComponentData<TemplateData>(templateEntity);
            }
        }

        Entity cinnamonPrefab = templateData.cinnamonPrefab;
        Entity oreoPrefab = templateData.oreoPrefab;

        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        float deltaTime = Time.DeltaTime;

        Entities
            .WithNativeDisableParallelForRestriction(randomArray)
            .ForEach((Entity entity, int entityInQueryIndex, int nativeThreadIndex, ref GestationData gestationData, in Translation translation) =>
            {
                gestationData.timeUntilDelivery -= deltaTime;

                if (gestationData.timeUntilDelivery <= 0)
                {
                    var random = randomArray[nativeThreadIndex];
                    Entity instance = Entity.Null;

                    for (int i = 0; i < gestationData.litterSize; i++)
                    {
                        if (random.NextFloat() > 0.5f)
                        {
                            //Debug.Log("Time to birth a baby Cinnamon!");
                            instance = commandBuffer.Instantiate(entityInQueryIndex, cinnamonPrefab);
                        }
                        else
                        {
                            //Debug.Log("Time to birth a baby Oreo!");
                            instance = commandBuffer.Instantiate(entityInQueryIndex, oreoPrefab);
                        }

                        //float3 offset = new float3(randomPoint.x, 0, randomPoint.y);  // TODO: find threadsafe Random pattern for inside Job
                        float3 offset = new float3(i + 1, 0, 0) * spawnRadius_;

                        commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation
                        {
                            Value = translation.Value + offset
                        });
                    }

                    commandBuffer.RemoveComponent<GestationData>(entityInQueryIndex, entity);

                    randomArray[nativeThreadIndex] = random; // This is NECESSARY.
                }

            }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}