using Tools;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

namespace DOTSTest
{
    public partial class SpawnEnemySystem:SystemBase
    {
        private float _Timer = 0;
        private CameraViewRange _cameraViewRange;
        protected override void OnCreate()
        {
           
        }

        protected override void OnUpdate()
        {
            if (_cameraViewRange == null)
            {
                _cameraViewRange = new CameraViewRange();
            }

            var spawnerComponent = SystemAPI.GetSingleton<EnemySpawner>();
            if (_Timer > 0)
            {
                _Timer -= SystemAPI.Time.DeltaTime;
                return;
            }

            _Timer = spawnerComponent.SpawnSpeed;
            var query = EntityManager.CreateEntityQuery(typeof(EnemyTag));            
            

            var entityCommandBuffer = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);
            
            if (query.CalculateEntityCount() < spawnerComponent.MaxNum)
            {
                var entity = entityCommandBuffer.Instantiate(spawnerComponent.Prefab);
               
                var pos = _cameraViewRange.GetRandomSpawnPoint();
              
                entityCommandBuffer.SetComponent(entity,new LocalTransform
                {
                    Position = pos,
                    Scale = 1,
                    Rotation = quaternion.identity
                });
                
            }
        }
    }
}