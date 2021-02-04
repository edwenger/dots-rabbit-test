using Unity.Entities;

[GenerateAuthoringComponent]
public struct TemplateData : IComponentData
{
    public Entity oreoPrefab;
    public Entity cinnamonPrefab;
}
