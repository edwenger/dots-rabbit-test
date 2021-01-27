using Unity.Entities;

public enum SexEnum
{
    Male,
    Female
}

[GenerateAuthoringComponent]
public struct SexData : IComponentData
{
    public SexEnum sex;
}