using Unity.Entities;
using Unity.Transforms;

namespace DOTSTest
{
    public partial class RemoveBulletSystem:SystemBase
    {
        protected override void OnUpdate()
        {
            //通过 GetSingleton 的方式 创建 的commandBuffer，可以自动playback,和dispose
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);
            

            foreach (HealthAspect aspect in SystemAPI.Query<HealthAspect>())
            {
                aspect.Check(ecb);
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