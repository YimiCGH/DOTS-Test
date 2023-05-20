using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTSTest
{
    public partial class BulletMoveSystem:SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var bulletMoveAspect in SystemAPI.Query<BulletMoveAspect>())
            {
                bulletMoveAspect.Move(deltaTime);
            }
        }

        public readonly partial struct BulletMoveAspect : IAspect
        {
            private readonly RefRW<BulletStatus> status;
            private readonly RefRW<LocalTransform> transform;

            public void Move(float deltaTime)
            {
                var dir = transform.ValueRO.Forward();
                transform.ValueRW.Position += dir * status.ValueRO.Speed * deltaTime;
                transform.ValueRW.Rotation = quaternion.LookRotation(dir,new float3(0,1,0));

                status.ValueRW.LifeTime -= deltaTime;                
            }
        }
    }
}