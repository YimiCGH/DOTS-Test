using Unity.Entities;
using Unity.Transforms;

namespace DOTSTest
{
    public partial class RemoveBulletSystem:SystemBase
    {
        protected override void OnUpdate()
        {
            var entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);
            

            foreach (HealthAspect aspect in SystemAPI.Query<HealthAspect>())
            {
                aspect.Check(entityCommandBuffer);
            }
        }
        
        public readonly partial struct HealthAspect:IAspect
        {
            private readonly Entity entity;

            private readonly RefRO<BulletStatus> status;
            private readonly RefRO<LocalTransform> transform;

            public void Check(EntityCommandBuffer commandBuffer)
            {
                if (status.ValueRO.LifeTime <= 0)
                {
                    commandBuffer.DestroyEntity(entity);
                    //播放子弹爆炸特效
                }
            }
        }
    }
}