using _3C.Actors.Monsters;
using Sirenix.OdinInspector;
using StatSystems.Store;
using System.Collections.Generic;
using UnityEngine;
using WorldCreatation;
namespace Manager
{
    public class MonsterManager : SingletonMono<MonsterManager>
    {
        [System.Serializable]
        private class IdPool
        {
            public readonly Stack<int> pool = new Stack<int>();
            public int counter=0;
        }
        public LayerMask CanStand;
        [LabelText("距离限制")]public float distaceLimited = 3;
        private readonly Dictionary<int, Dictionary<int, Monster>> monsters = new();
        [ShowInInspector]private readonly Dictionary<int,IdPool> monsterEmptyIds = new();
        private const int MonsterIdoffset = 4;
        public  const int MaxMonsterCount = 1<<4;
        public IEnumerable<Monster> Monsters
        {
            get
            {
                foreach (var dic in monsters.Values)
                {
                    foreach (var monster in dic.Values)
                    {
                        yield return monster;
                    }
                }
            }
        }
        protected override void SingletonAwake()
        {
            
        }
        [Button("生成怪物（测试）")]
        public void CreateMonster(Monster monster,Vector2 pos)
        {
            int y = World.maxYpos + 10;
            Ray ray = new(new Vector3(pos.x, y, pos.y), Vector3.down);
            bool flowControl = Standtest(y, ray, out var info);
            if (!flowControl)
                return;
            var @object = Object.Instantiate(monster.gameObject);
            var mo = @object.GetComponent<Monster>();
            mo.Target = WorldManager.Instance.Player.transform;
            mo.transform.position = info.point;
            Add(mo,0);
        }
        [Button("生成怪物")]
        public void CreateMonster(ActorInfo monsterinfo,Vector2 pos)
        {
            float y = World.maxYpos + 10;
            var monster = monsterinfo.Prefab;
            Ray ray = new(new Vector3(pos.x, y, pos.y), Vector3.down);
            bool flowControl = Standtest(y, ray, out var info);
            if (!flowControl)
                return;
            var @object = Object.Instantiate(monster);
            var mo = @object.GetComponent<Monster>();
            mo.Target = WorldManager.Instance.Player.transform;
            mo.transform.position = info.point;
            Add(mo, monsterinfo.ID);
        }
        private bool Standtest(float y, Ray ray, out RaycastHit info)
        {
            if (!Physics.Raycast(ray, out info, y - .5f, CanStand, QueryTriggerInteraction.Ignore)||info.point==Vector3.zero)
            {
                return false;
            }
            Vector3 point = info.point;
            foreach (var mst in Monsters)
            {
                if ((mst.transform.position-point).magnitude <= distaceLimited) return false;
            }

            return true;
        }
        public void RycycleMonster(Monster monster)
        {   
            int id = monster.ID;
            int head = id >> MonsterIdoffset;
            int tail = id - (head << MonsterIdoffset);

            if(monsterEmptyIds.TryGetValue(head, out var pair))
            {
                pair.pool.Push(tail); 
                if(pair.pool.Count == pair.counter)
                {
                    monsterEmptyIds.Remove(head);
                } 
            }
            if (monsters.TryGetValue(head, out var m))
            {
                m.Remove(tail);
            }
            if(m.Count == 0) monsters.Remove(head);
            Destroy(monster.gameObject);
        }
        private void Add(Monster monster,int head)
        {
            int firstPart = head << MonsterIdoffset;
            int tail;
            if (!monsterEmptyIds.TryGetValue(head, out var pair))
            {              
                pair = new IdPool();
                monsterEmptyIds.Add(head, pair);
            }
            if (pair.pool.Count > 0)
                tail = pair.pool.Pop();
            else
                tail = pair.counter++;
            if(tail>=MaxMonsterCount)
            {
                Debug.LogError("满了");
                return;
            }
            int id = firstPart + tail;
            monster.ID = id;
            if (!monsters.TryGetValue(head, out var m))
            {
                m = new();
                monsters.Add(head, m);
            }
            m.Add(tail, monster);
        }
        [Button("重建")]
        public void Rebuilt()
        {
            monsterEmptyIds.Clear();
            List<Monster> All = new(); 
            List<int> emptyKey = new();
            foreach(var (head,dic) in monsters)
            {
                foreach(var monster in dic.Values)
                {
                    if (monster != null)
                        All.Add(monster);
                }
                dic.Clear();
                if (All.Count == 0)
                {
                    emptyKey.Add(head);
                    return;
                }
                var pool = new IdPool();
                foreach(var monster in All)
                {
                    int tail = pool.counter++;
                    monster.ID = head<<MonsterIdoffset+tail;
                    dic.Add(tail, monster);
                }
                monsterEmptyIds.Add(head, pool);
                All.Clear();
            }
            foreach(var key in emptyKey)
            {
                monsters.Remove(key);
            }
        }

    }
}
