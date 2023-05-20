using Unity.Entities;
using UnityEngine;

namespace DOTSTest
{
    public struct BulletTag:IComponentData
    {
        
    }

    public class BulletTagAuthoring : MonoBehaviour
    {
    }

    public class BulletTagBaker : Baker<BulletTagAuthoring>
    {
        public override void Bake(BulletTagAuthoring authoring)
        {     
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent<BulletTag>(entity);
        }
    }
}