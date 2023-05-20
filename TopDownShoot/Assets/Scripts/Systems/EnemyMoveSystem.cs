using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSTest
{
    
    // 丧尸的移动不能和子弹放在一起，是因为丧尸还需要绕开对象，避免碰撞之类的，子弹不需要
    
    public partial class EnemyMoveSystem:SystemBase
    {
        protected override void OnUpdate()
        {

            float deltaTime = SystemAPI.Time.DeltaTime;
            
            //TODO 改为流场移动
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var playerTsf = SystemAPI.GetComponent<LocalTransform>(player);
            
            float3 targetPos = playerTsf.Position;
            foreach ( MoveToPositionAspect aspect in SystemAPI.Query<MoveToPositionAspect>())
            {                
                aspect.Move(targetPos,deltaTime);                
            }
            
        }


        public readonly partial struct MoveToPositionAspect:IAspect
        {
            //private readonly Entity entity;

            private readonly RefRO<SteerTarget> steer;
            private readonly RefRW<EnemyStatus> status;
            private readonly RefRW<LocalTransform> transform;

            public void Move(float3 targetPos,float deltaTime)
            {
                var dir = math.normalize(targetPos - transform.ValueRW.Position);
                transform.ValueRW.Position += dir * steer.ValueRO.Speed * deltaTime;
                transform.ValueRW.Rotation = quaternion.LookRotation(dir,new float3(0,1,0));
                
                var dis = math.distancesq(targetPos, transform.ValueRW.Position);
                if (dis < 0.1f)
                {
                    status.ValueRW.Hp = 0;
                }
            }
        }
    }
}