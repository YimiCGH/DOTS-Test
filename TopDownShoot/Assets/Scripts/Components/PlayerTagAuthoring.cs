using Unity.Entities;
using UnityEngine;

namespace DOTSTest
{
    public struct PlayerTag:IComponentData
    {
        
    }

    public class PlayerTagAuthoring : MonoBehaviour
    {
    }

    public class PlayerTagBaker : Baker<PlayerTagAuthoring>
    {
        public override void Bake(PlayerTagAuthoring authoring)
        {     
            var entity = GetEntity(TransformUsageFlags.None);
            //var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerTag>(entity);
        }
    }
}