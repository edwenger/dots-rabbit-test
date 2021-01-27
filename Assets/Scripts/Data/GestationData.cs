using Unity.Entities;

[GenerateAuthoringComponent]
public struct GestationData : IComponentData
{
    public float timeUntilDelivery;
}
