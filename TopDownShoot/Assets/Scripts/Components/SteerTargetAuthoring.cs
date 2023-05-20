using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSTest
{
    public struct SteerTarget:IComponentData
    {
        public float Speed;
    }

    public class SteerTargetAuthoring : MonoBehaviour
    {
        public float Speed;
    }

    public class SteerTargetBaker : Baker<SteerTargetAuthoring>
    {
        public override void Bake(SteerTargetAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity,new SteerTarget()
            {
                Speed = authoring.Speed
            });
        }
    }
}