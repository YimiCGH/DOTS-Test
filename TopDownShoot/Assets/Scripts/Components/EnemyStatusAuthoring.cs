using Unity.Entities;
using UnityEngine;

namespace DOTSTest
{
    public struct EnemyStatus:IComponentData
    {
        public int Hp;
        public float MoveSpeed;
        public float AttackSpeed;
        public int AttackDamage;
    }

    public class EnemyStatusAuthoring:MonoBehaviour
    {
        public int Hp;
        public float MoveSpeed;
        public float AttackSpeed;
        public int AttackDamage;
    }

    public class EnemyStatusBaker : Baker<EnemyStatusAuthoring>
    {
        public override void Bake(EnemyStatusAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,new EnemyStatus()
            {
                Hp = authoring.Hp,
                MoveSpeed = authoring.MoveSpeed,
                AttackSpeed = authoring.AttackSpeed,
                AttackDamage = authoring.AttackDamage,
            });
        }
    }
}