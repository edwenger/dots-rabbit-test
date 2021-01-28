using Unity.Entities;
using Unity.Jobs;

public class AgingSystem : SystemBase
{
    public float newbornScale = 0.25f;
    public float adultScale = 1.0f;
    public float adultAge = 30f;

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref AgeData ageData) => {
            ageData.age += deltaTime;
        }).ScheduleParallel();
    }
}
