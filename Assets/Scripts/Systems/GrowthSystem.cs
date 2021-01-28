using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class GrowthSystem : SystemBase
{
    public float newbornScale = 0.25f;
    public float adultScale = 1.0f;
    public float femaleScale = 0.8f;
    public float adultAge = 30f;

    protected override void OnUpdate()
    {
        float _newbornScale = newbornScale;  // else WithoutBurst + Run warning
        float _adultScale = adultScale;
        float _femaleScale = femaleScale;
        float _adultAge = adultAge;

        Entities.WithAll<Rabbit>().ForEach((ref LocalToParent local, in AgeData ageData, in SexData sexData) =>
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

            local.Value = new float4x4(
                scale, 0, 0, 0,
                0, scale, 0, local.Value.c3.y,  // TODO: only currently preserving y-offset
                0, 0, scale, 0,
                0, 0, 0, 1.0f);

        }).ScheduleParallel();
    }
}
