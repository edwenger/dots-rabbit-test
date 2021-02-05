using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class GrowthSystem : SystemBase
{
    public float newbornScale = 0.25f;  // TODO: find a good pattern for all system configuration variables
    public float adultScale = 1.0f;
    public float femaleScale = 0.8f;
    public float adultAge = 30f;

    protected override void OnUpdate()
    {
        float _newbornScale = newbornScale;  // else WithoutBurst + Run warning
        float _adultScale = adultScale;
        float _femaleScale = femaleScale;
        float _adultAge = adultAge;

        Entities.WithAll<RabbitTag>().ForEach((ref NonUniformScale scaleData, in AgeData ageData, in SexData sexData) =>
        {
            float scale = _adultScale;
            float age = ageData.age;

            if (age < _adultAge)
            {
                scale = _newbornScale + (age / _adultAge) * (_adultScale - _newbornScale);
            }

            if (sexData.sex == SexEnum.Female)
            {
                scale *= _femaleScale;
            }

            scaleData.Value = new float3(scale, scale, scale);

        }).ScheduleParallel();
    }
}
