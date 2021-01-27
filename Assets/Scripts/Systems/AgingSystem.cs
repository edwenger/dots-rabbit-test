using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class AgingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref AgeData ageData) => {
            ageData.age += deltaTime;
        }).ScheduleParallel();
    }
}
