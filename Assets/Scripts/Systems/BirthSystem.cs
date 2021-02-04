using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BirthSystem : SystemBase
{
    EndInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    Entity templateEntity = Entity.Null;
    TemplateData templateData;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
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
            .ForEach((Entity entity, int entityInQueryIndex, ref GestationData gestationData) =>
            {
                gestationData.timeUntilDelivery -= deltaTime;

                if (gestationData.timeUntilDelivery <= 0)
                {
                    Debug.Log("Time to birth a baby Cinnamon!");

                    var instance = commandBuffer.Instantiate(entityInQueryIndex, cinnamonPrefab);
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { 
                        Value = new float3(0, 2.6f, 0) });

                    commandBuffer.RemoveComponent<GestationData>(entityInQueryIndex, entity);
                }

            }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
