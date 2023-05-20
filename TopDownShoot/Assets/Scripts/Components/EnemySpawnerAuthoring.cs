using Unity.Entities;
using UnityEngine;

namespace DOTSTest
{
    public struct EnemySpawner:IComponentData
    {
        public Entity Prefab;
        public int MaxNum;
        public float SpawnSpeed;
    }

    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
        public int MaxNum;
        public float SpawnSpeed;
    }

    public class EnemyPrefabBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            var entityPrefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic);
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,new EnemySpawner()
            {
                Prefab = entityPrefab,
                MaxNum = authoring.MaxNum,
                SpawnSpeed = authoring.SpawnSpeed
            });
        }
    }
}