using Unity.Entities;

[GenerateAuthoringComponent]
public struct PartnerData : IComponentData
{
    public Entity partner;
}
