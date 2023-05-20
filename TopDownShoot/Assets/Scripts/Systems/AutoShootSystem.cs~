using Drawing;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSTest
{
    [UpdateAfter(typeof(EnemyMoveSystem))]
    public partial class AutoShootSystem:SystemBase
    {
        EntityQuery enemyGroup;        
        //EntityQuery playerGroup;
        
        protected override void OnCreate()
        {
            //playerGroup = GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(), ComponentType.ReadOnly<PlayerTag>());
            enemyGroup = GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(), ComponentType.ReadOnly<EnemyTag>());
        }
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);

            foreach (CharacterShootAspect shootAspect in SystemAPI.Query<CharacterShootAspect>())
            {
                var translationType = GetComponentTypeHandle<LocalTransform>(true);
                shootAspect.GetTarget(ecb,enemyGroup,translationType,deltaTime,this.Dependency);
            }
        }

        public readonly partial struct CharacterShootAspect : IAspect
        {            
            private readonly RefRW<LocalTransform> _transform;
            private readonly RefRW<ShootCtl> _shootCtl;

            public void GetTarget(EntityCommandBuffer commandBuffer,EntityQuery enemyGroup,ComponentTypeHandle<LocalTransform> componentTypeHandle,float deltaTime,JobHandle dependency)
            {
                Draw.CircleXZ(_transform.ValueRO.Position,_shootCtl.ValueRO.ShootDistance,Color.green);
                if (_shootCtl.ValueRW.shootCD > 0)
                {
                    _shootCtl.ValueRW.shootCD -= deltaTime;
                    return;
                }

                _shootCtl.ValueRW.shootCD = _shootCtl.ValueRW.ShootTimeScale;
                NativeList<float3> result = new NativeList<float3>(10,Allocator.TempJob);

                var selectTargetJob = new SelectTargetChunkJob()
                {
                    TransformType = componentTypeHandle,
                    Center = _transform.ValueRO.Position,
                    ShootDistanceSQ = math.pow(_shootCtl.ValueRO.ShootDistance,2),
                    result = result.AsParallelWriter()
                };
                var handle = selectTargetJob.ScheduleParallel(enemyGroup,dependency);
                
                handle.Complete();

                //朝向敌人，进行射击
           
                if (result.Length > 0)
                {
                    var min = float.MaxValue;
                    var pos = _transform.ValueRW.Position;
                    var nearestTarget = float3.zero;
                    foreach (var target in result)
                    {
                        var dis = math.distancesq(target, pos);
                        if (dis < min)
                        {
                            min = dis;
                            nearestTarget = target;
                        }
                    }
              
                    var dir = math.normalize(nearestTarget - pos);
                    _transform.ValueRW.Rotation = quaternion.LookRotation(dir,new float3(0,1,0));
                
                    SplitShoot(commandBuffer,_shootCtl.ValueRO.BulletPrefab,
                        _transform.ValueRO.Position + new float3(0,1,0) * 0.5f,
                        dir,
                        _shootCtl.ValueRO.SplitAngle,
                        _shootCtl.ValueRO.SplitNum);
                }

                result.Dispose();
            }
        }

        static void SplitShoot(EntityCommandBuffer commandBuffer,Entity bulletPrefab,float3 position,Vector3 forward,float splitAngle,float splitNum)
        {            
            if (splitNum == 1)
            {
                Shoot(commandBuffer,bulletPrefab,position,forward);
                return;
            }

            float beginAngle = - splitAngle * 0.5f;
            float step = splitAngle / (splitNum - 1);
            for (int i = 0; i < splitNum; i++)
            {
                float angle = beginAngle + i * step;
                var dir = Quaternion.AngleAxis(angle, Vector3.up) * forward;
                Shoot(commandBuffer,bulletPrefab,position,dir);
            }
        }

        static void Shoot(EntityCommandBuffer commandBuffer,Entity bulletPrefab,float3 position,float3 forward)
        {
            var bullet = commandBuffer.Instantiate(bulletPrefab);
            commandBuffer.SetComponent(bullet,new LocalTransform
            {
                Position = position,
                Scale = 1,
                Rotation =quaternion.LookRotation(forward,new float3(0,1,0))
            });
        }


        [BurstCompile]
        struct SelectTargetChunkJob:IJobChunk
        {
            public float3 Center;
            public float ShootDistanceSQ;
            
            [ReadOnly] public ComponentTypeHandle<LocalTransform> TransformType;
            public NativeList<float3>.ParallelWriter result;//输出结果
            
            
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var enemyTransforms = chunk.GetNativeArray(ref TransformType);
                
                float minDis = float.MaxValue;
                bool hasTarget = false;
                float3 targetPos = float3.zero;
                for (int i = 0; i < chunk.Count; i++)
                {
                    var pos = enemyTransforms[i].Position;
                    //在玩家射程内
                    var dis = math.distancesq(pos, Center);
                    if (dis < ShootDistanceSQ && dis < minDis)
                    {
                        
                        minDis = dis;
                        hasTarget = true;
                        targetPos = pos;
                    }
                }
                
                if (hasTarget)
                {                    
                    result.AddNoResize(targetPos);
                }
            }
        }
        
    }
}