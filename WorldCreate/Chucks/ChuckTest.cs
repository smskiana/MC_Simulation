using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using WorldCreate.TerrainInfos;

namespace WorldCreate.Chucks
{
    public class ChuckTest:MonoBehaviour,IChunksCommunicate
    {
        public Dictionary<Vector2Int,Chuck> chucks;
        [SerializeField] private string filename = "/chucks";
        [SerializeField] private int viewDis = 5;
        [SerializeField] private int loadDis = 6;
        [SerializeField] private int unloadDis = 8;
        public Transform FollowPos;
        public ChunkView Prefab;
        public int seed =121222;
        public Vector2Int CurrentLoadPos;
        private TerrainInfo info;
        private bool HaveInit = false;
        private Task LoadTask;
        public void Update()
        {
            var pos = Chuck.GetChuckId(Vector3Int.CeilToInt(FollowPos.transform.position));
            if (LoadTask == null&&HaveInit&&CurrentLoadPos!=pos)
                _ = UpdateChucks(pos);
        }
        [Button]
        public async Task Begin()
        {
            info = new TerrainInfo(seed);
            chucks = new ();

            if (FollowPos != null)
            {
                var pos = Vector3Int.CeilToInt(FollowPos.position);
                CurrentLoadPos = Chuck.GetChuckId(pos);  
            }
            for (int i = CurrentLoadPos.x - loadDis; i <= CurrentLoadPos.x + loadDis; i++)
            {
                for (int j = CurrentLoadPos.y - loadDis; j <= CurrentLoadPos.y + loadDis; j++)
                {
                    Vector2Int id = new(i,j);
                    info.LoadFile(id);
                    var chuck = new Chuck(id,
                        Instantiate(this.Prefab.gameObject,this.transform).GetComponent<ChunkView>())
                    {
                       world = this,
                    };
                    chucks.Add(id,chuck);
                }
            }
            Task task = Task.Run(() => 
            {
                foreach (var chuck in chucks)
                    chuck.Value.AddAllBlocks(info);

                for (int i = CurrentLoadPos.x - viewDis; i <= CurrentLoadPos.x + viewDis; i++)
                {
                    for (int j = CurrentLoadPos.y - viewDis; j <= CurrentLoadPos.y + viewDis; j++)
                    {
                        chucks[new(i,j)].Initblock();
                        chucks[new(i,j)].InitMeshInfo();
                    }
                }
            });
            try
            {
                await task;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }
            Debug.Log("完成");
            for (int i = CurrentLoadPos.x - viewDis; i <= CurrentLoadPos.x + viewDis; i++)
            {
                for (int j = CurrentLoadPos.y - viewDis; j <= CurrentLoadPos.y + viewDis; j++)
                {
                    chucks[new(i, j)].CreateMesh();
                }
            }
            info.UnloadAll();
            HaveInit = true;
        }
        public async Task UpdateChucks(Vector2Int newPos)
        {
            List<Vector2Int> load = new();
            HashSet<Vector2Int> show = new();
            List<Chuck> unload = new();
            List<Vector2Int> enter = new();
            List<Vector2Int> leave = new();
            CalcEnter(CurrentLoadPos, newPos, loadDis, enter);
            foreach (var id in enter)
            {
                if (!chucks.TryGetValue(id, out var chuck))
                {
                    load.Add(id);
                    info.LoadFile(id);
                    chuck = new Chuck(id,
                    Instantiate(this.Prefab.gameObject, this.transform).GetComponent<ChunkView>())
                    {
                        world = this
                    };
                    chucks.Add(id, chuck);
                }
            }
            UpdateVision(CurrentLoadPos, newPos, viewDis, leave, enter);
            foreach (var id in leave)
            {
                if (chucks.TryGetValue(id, out var chuck))
                {
                    chuck.view.gameObject.SetActive(false);
                }
            }
            foreach (var id in enter)
            {
                if (chucks.TryGetValue(id, out var chuck))
                {
                    if (chuck.State == ChuckState.HaveCreateMesh)
                    {
                        chuck.view.gameObject.SetActive(true);
                    }
                    else
                    {
                        show.Add(id);
                    }
                }
            }
          
            this.LoadTask = Task.Run(() =>
            {
                foreach (var id in load)
                {
                    chucks[id].AddAllBlocks(info);
                }
                foreach (var id in show)
                {
                    chucks[id].Initblock();
                    chucks[id].InitMeshInfo();
                }
            });
            await this.LoadTask;
            CalcLeave(CurrentLoadPos, newPos,unloadDis,leave);
            foreach (var id in leave)
            {
                if(chucks.TryGetValue(id,out var chuck))
                {
                    chucks.Remove(id);
                    Destroy(chuck.view.gameObject);
                    SaveChuck(chuck);
                }
            }

            CurrentLoadPos = newPos;
            foreach (var id in show)
            {
                chucks[id].CreateMesh();
            }
            info.UnloadAll();
            LoadTask = null;
        }
        public static void UpdateVision(Vector2Int oldC,Vector2Int newC,int area,
            List<Vector2Int> leave, List<Vector2Int> enter)   
        {
            leave.Clear();
            enter.Clear();

            int dx = newC.x - oldC.x;
            int dy = newC.y - oldC.y;

            int size = 2 * area + 1;

            // ---------- X movement ----------
            if (dx != 0)
            {
                int step = dx > 0 ? 1 : -1;

                for (int i = 0; i < Mathf.Abs(dx); i++)
                {
                    int leaveX = oldC.x - area + i * step;
                    int enterX = newC.x + area - i * step;

                    if (step < 0)
                    {
                        leaveX = oldC.x + area - i * step;
                        enterX = newC.x - area + i * step;
                    }

                    for (int y = -area; y <= area; y++)
                    {
                        leave.Add(new Vector2Int(leaveX, oldC.y + y));
                        enter.Add(new Vector2Int(enterX, newC.y + y));
                    }
                }
            }

            // ---------- Y movement ----------
            if (dy != 0)
            {
                int step = dy > 0 ? 1 : -1;

                for (int i = 0; i < Mathf.Abs(dy); i++)
                {
                    int leaveY = oldC.y - area + i * step;
                    int enterY = newC.y + area - i * step;

                    if (step < 0)
                    {
                        leaveY = oldC.y + area - i * step;
                        enterY = newC.y - area + i * step;
                    }

                    for (int x = -area; x <= area; x++)
                    {
                        leave.Add(new Vector2Int(oldC.x + x, leaveY));
                        enter.Add(new Vector2Int(newC.x + x, enterY));
                    }
                }
            }
        }
        public static void CalcEnter(Vector2Int oldC,Vector2Int newC,int area,
            List<Vector2Int> enter)
        {
            enter.Clear();

            int dx = newC.x - oldC.x;
            int dy = newC.y - oldC.y;

            // X movement
            if (dx > 0)
            {
                int x = newC.x + area;
                for (int y = -area; y <= area; y++)
                    enter.Add(new Vector2Int(x, newC.y + y));
            }
            else if (dx < 0)
            {
                int x = newC.x - area;
                for (int y = -area; y <= area; y++)
                    enter.Add(new Vector2Int(x, newC.y + y));
            }

            // Y movement
            if (dy > 0)
            {
                int y = newC.y + area;
                for (int x = -area; x <= area; x++)
                    enter.Add(new Vector2Int(newC.x + x, y));
            }
            else if (dy < 0)
            {
                int y = newC.y - area;
                for (int x = -area; x <= area; x++)
                    enter.Add(new Vector2Int(newC.x + x, y));
            }
        }
        public static void CalcLeave(Vector2Int oldC,Vector2Int newC,int area,
            List<Vector2Int> leave)
        {
            leave.Clear();

            int dx = newC.x - oldC.x;
            int dy = newC.y - oldC.y;

            // X movement
            if (dx > 0)
            {
                int x = oldC.x - area;
                for (int y = -area; y <= area; y++)
                    leave.Add(new Vector2Int(x, oldC.y + y));
            }
            else if (dx < 0)
            {
                int x = oldC.x + area;
                for (int y = -area; y <= area; y++)
                    leave.Add(new Vector2Int(x, oldC.y + y));
            }

            // Y movement
            if (dy > 0)
            {
                int y = oldC.y - area;
                for (int x = -area; x <= area; x++)
                    leave.Add(new Vector2Int(oldC.x + x, y));
            }
            else if (dy < 0)
            {
                int y = oldC.y + area;
                for (int x = -area; x <= area; x++)
                    leave.Add(new Vector2Int(oldC.x + x, y));
            }
        }

        public bool GetChuck(Vector3Int worldPos,out Chuck chuck)
        {
            Vector2Int n = Chuck.GetChuckId(worldPos);  
            chucks.TryGetValue(n, out chuck);
            return chuck != null;
        }
        public bool DeleteFace(Vector3Int worldPos, Face face)
        {
            if(GetChuck(worldPos,out var chuck))
            {
               return chuck.RemoveFace(chuck.ToChuckPos(worldPos),face);
            }
            return false;
        }
        public bool AddFace(Vector3Int worldPos, Face face)
        {
            if (GetChuck(worldPos, out var chuck))
            {
                return chuck.AddFace(chuck.ToChuckPos(worldPos),face);
            }
            return false;
        }
        public bool IsBlockExist(Vector3Int worldPos)
        {
            if (GetChuck(worldPos, out Chuck chuck))
            {
                return chuck.IsBlockExit(worldPos);
            }
            return false;
        }
        [Button]
        public void RemoveBlock(Vector3Int worldPos)
        {
            if(GetChuck(worldPos, out Chuck chuck))
            {
                chuck.RemoveBlock(chuck.ToChuckPos(worldPos));
                foreach (var f_chuck in chucks.Values)
                {
                    if (f_chuck.Excute())
                        f_chuck.CreateMesh();
                }
            }
           
        }
        [Button]
        public void AddBlock(Vector3Int worldPos)
        {
            if(GetChuck(worldPos, out Chuck chuck))
            {
                Block block = new(chuck.ToChuckPos(worldPos))
                {
                    ID = 1
                };
                chuck.AddBlock(block);
                foreach (var f_chuck in chucks.Values)
                {
                    if (f_chuck.Excute())
                        f_chuck.CreateMesh();
                }
            }
           
        }
        [Button]
        public void Save()
        {
            string path = Application.persistentDataPath + filename;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var value = GetChucksJson();
            foreach (var item in value)
            {
                string json = JsonUtility.ToJson(item);
                string pathname = item.id.ToString()+".json";
                File.WriteAllText(Path.Combine(path,pathname), json);
            }
            
        }
        public void SaveChuck(Chuck chuck)
        {
            string path = Application.persistentDataPath + filename;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var json = chuck.GetChuckJson();
            string name = json.id.ToString() + ".json";
            string context = JsonUtility.ToJson(json);
            File.WriteAllText(Path.Combine(path,name), context);    
        }
        public List<ChuckJson> GetChucksJson()
        {
            var chucksJson = new List<ChuckJson>();
            foreach (var chuck in chucks.Values)
            {
                chucksJson.Add(chuck.GetChuckJson());
            }
            return chucksJson;
        }
    }
}
