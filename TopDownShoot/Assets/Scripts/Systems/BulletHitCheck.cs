using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSTest
{
    [UpdateAfter(typeof(EnemyMoveSystem))]
    [UpdateAfter(typeof(BulletMoveSystem))]
    public partial class BulletHitCheck:SystemBase
    {
        EntityQuery enemyGroup;        
        EntityQuery bulletGroup;
        
        protected override void OnCreate()
        {
            bulletGroup = GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(), ComponentType.ReadOnly<BulletTag>(),ComponentType.ReadWrite<BulletStatus>());
            enemyGroup = GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(), ComponentType.ReadOnly<EnemyTag>(),ComponentType.ReadWrite<EnemyStatus>());
        }
        
        protected override void OnUpdate()
        {
            var translationType = GetComponentTypeHandle<LocalTransform>(true);
            var enemyStatusType = GetComponentTypeHandle<EnemyStatus>(false);

            var bulletEntities = bulletGroup.ToEntityArray(Allocator.TempJob);
            var bulletTransforms = bulletGroup.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
            var bulletStatus = bulletGroup.ToComponentDataArray<BulletStatus>(Allocator.TempJob);
            var entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);

            BulletCollisionJob job = new BulletCollisionJob()
            {
                EnemyStatusType = enemyStatusType,
                TransformType = translationType,
                BulletTransforms = bulletTransforms,
                BulletStatus = bulletStatus,
                    BulletEntities = bulletEntities,
                    CommandBuffer = entityCommandBuffer
            };

            var handle = job.Schedule(enemyGroup,this.Dependency);
            handle.Complete();

            bulletTransforms.Dispose();
            bulletStatus.Dispose();
            bulletEntities.Dispose();
        }

        [BurstCompile]
        struct BulletCollisionJob : IJobChunk
        {
            public ComponentTypeHandle<EnemyStatus> EnemyStatusType;
            [ReadOnly] public ComponentTypeHandle<LocalTransform> TransformType;

            public EntityCommandBuffer CommandBuffer;
            public NativeArray<Entity> BulletEntities;//子弹实体
            public NativeArray<LocalTransform> BulletTransforms;//子弹位置
            public NativeArray<BulletStatus> BulletStatus;//子弹状态
            
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                
                var enemyStatus = chunk.GetNativeArray(ref EnemyStatusType);
                var enemyTransforms = chunk.GetNativeArray(ref TransformType);

                
                for (int i = 0; i < chunk.Count; i++)
                {
                    var status = enemyStatus[i];
                    var pos = enemyTransforms[i].Position;
                   
                    for (int j = 0; j < BulletTransforms.Length; j++)
                    {
                        var pos2 = BulletTransforms[j].Position;
                        var bulletStatus = BulletStatus[j];
                        if (bulletStatus.LifeTime <= 0)
                        {
                            continue;//不用再检测
                        }

                        int hitNum = bulletStatus.ImpactCount;//允许穿透数

                        if (CheckCollision(pos, pos2, 0.5f))
                        {
                            //如果会穿透就特殊处理
                            hitNum--;
                            
                            //收到伤害处理
                            status.Hp = 0;
                            enemyStatus[i] = status;
                
                            if (hitNum <= 0)
                            {
                                //销毁子弹
                                bulletStatus.LifeTime = 0;
                                BulletStatus[j] = bulletStatus;
                                
                                //不知道有没有更好的方法，目前先这样子用，可能有点曲线救国的样子
                                CommandBuffer.SetComponent(BulletEntities[j],bulletStatus);
                                break;
                            }
                        }                      
                        
                    }

                    
                }
            }
            
            
            static bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
            {
                float3 delta = posA - posB;
                float distanceSquare = delta.x * delta.x + delta.z * delta.z;

                return distanceSquare <= radiusSqr;
            }
        }
    }
}