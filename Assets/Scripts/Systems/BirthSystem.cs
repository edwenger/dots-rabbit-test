using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

public class BirthSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref GestationData gestationData) => {
            gestationData.timeUntilDelivery -= deltaTime;
        }).ScheduleParallel();

        Entities.ForEach((ref GestationData gestationData) =>
        {
            if (gestationData.timeUntilDelivery <= 0)
            {
                Debug.Log("We made a baby!");
                // TODO: dispose of GestationData component?
            }

        }).WithStructuralChanges().Run();
    }
}
