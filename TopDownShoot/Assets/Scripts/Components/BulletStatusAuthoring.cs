using Unity.Entities;
using UnityEngine;

namespace DOTSTest
{
    public struct BulletStatus:IComponentData
    {
        public float Speed;
        public int Damage;
        public int ImpactCount;//穿透次数
        public float LifeTime;
    }
    public class BulletStatusAuthoring:MonoBehaviour
    {
        public float Speed;
        public int Damage;
        public float LifeTime;
        public int ImpactCount = 1;//穿透次数
    }

    public class BulletStatusBaker : Baker<BulletStatusAuthoring>
    {
        public override void Bake(BulletStatusAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,new BulletStatus()
            {
                Speed = authoring.Speed,
                Damage = authoring.Damage,
                LifeTime = authoring.LifeTime,
                ImpactCount = authoring.ImpactCount
            });
        }
    }
}