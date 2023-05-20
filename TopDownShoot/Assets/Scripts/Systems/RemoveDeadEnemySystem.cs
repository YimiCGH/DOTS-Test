using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DOTSTest
{
    [UpdateAfter(typeof(BulletHitCheck))]
    public partial class RemoveDeadEnemySystem:SystemBase
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

            private readonly RefRO<EnemyStatus> status;
            private readonly RefRO<LocalTransform> transform;

            public void Check(EntityCommandBuffer commandBuffer)
            {
                if (status.ValueRO.Hp <= 0)
                {
                    commandBuffer.DestroyEntity(entity);
                    //播放死亡效果
                }
            }
        }
    }
}