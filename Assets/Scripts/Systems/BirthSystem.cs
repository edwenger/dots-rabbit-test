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

    public float spawnRadius = 3.0f;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float spawnRadius_ = spawnRadius;  // TODO: find a good pattern for all system configuration variables

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

        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        float deltaTime = Time.DeltaTime;

        Entities
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, ref GestationData gestationData, in Translation translation) =>
            {
                gestationData.timeUntilDelivery -= deltaTime;

                if (gestationData.timeUntilDelivery <= 0)
                {
                    Debug.Log("Time to birth a baby Cinnamon!");

                    //float3 offset = new float3(randomPoint.x, 0, randomPoint.y);  // TODO: find threadsafe Random pattern for inside Job
                    float3 offset = new float3(spawnRadius_, 0, 0);

                    var instance = commandBuffer.Instantiate(entityInQueryIndex, cinnamonPrefab);
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation
                    {
                        Value = translation.Value + offset
                    });

                    commandBuffer.RemoveComponent<GestationData>(entityInQueryIndex, entity);
                }

            }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}