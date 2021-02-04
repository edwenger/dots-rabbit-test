using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[AddComponentMenu("ECS Conversion/Template")]
public class TemplateAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject oreoPrefab;
    public GameObject cinnamonPrefab;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(oreoPrefab);
        referencedPrefabs.Add(cinnamonPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var templateData = new TemplateData
        {
            oreoPrefab = conversionSystem.GetPrimaryEntity(oreoPrefab),
            cinnamonPrefab = conversionSystem.GetPrimaryEntity(cinnamonPrefab)
        };
        dstManager.AddComponentData(entity, templateData);
    }
}
