using Unity.Entities;
using UnityEngine;

namespace DOTSTest
{
    public struct ShootCtl : IComponentData
    {
        public Entity BulletPrefab;
        public float ShootTimeScale;      
        public float ShootDistance;//射程
        public int SplitNum;
        public int SplitAngle;
        
        public float shootCD;
    }

    public class ShootCtrlAuthoring:MonoBehaviour
    {
        public GameObject BulletPrefab;
        public float ShootTimeScale;
        public float ShootDistance;
        public int SplitNum = 1;//分叉数
        public int SplitAngle = 30;//散射角度
    }

    public class ShootCtlBaker : Baker<ShootCtrlAuthoring>
    {
        public override void Bake(ShootCtrlAuthoring authoring)
        {
            var entityPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic);
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,new ShootCtl()
            {
                BulletPrefab = entityPrefab,
                ShootTimeScale = authoring.ShootTimeScale,
                ShootDistance = authoring.ShootDistance,
                SplitNum = authoring.SplitNum,
                SplitAngle = authoring.SplitAngle
            });
        }
    }
}