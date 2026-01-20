using MyGUI;
using Manager;
using UnityEngine;
using WorldCreatation;
using _3C.Actors.player;
using StatSystems.Store.Items;
using System.Collections.Generic;
using StatSystems.Store;

public class WorldManager : SingletonMono<WorldManager>
{
    private readonly Dictionary<Vector3Int, int> hitTimes = new();
    public Player Player;
    public Camera Camera;
    private bool worldCreateOver = false;
    public bool WorldCreateOver { get => worldCreateOver; set => worldCreateOver = value; }
    public void Start()
    {
        if (World.Instance != null)
        {        
            if (World.Instance.HaveInit)
            {
                SetPlaypos();
            }
            else
            {
                if(Player != null)
                {
                    Player.gameObject.SetActive(false);
                    World.Instance.InitFinish += SetPlaypos;
                }              
            }
        }
    }
    public void SetPlaypos()
    {
        int y = World.Instance.GetInitHeight(0, 0);
        Player.transform.position = new Vector3(.5f, y, .5f);
        Player.SetCamera(Camera.transform);
        if(UIManager.Instance) UIManager.Instance.BlindPlayer(Player);
        Player.gameObject.SetActive(true);
    }
    public bool RemoveBlock(Vector3Int pos)
    {

        int id = 0;
        bool work = false;
        if(World.Instance != null)
        {
            id = World.Instance.GetBlockID(pos);
            work = World.Instance.RemoveBlock(pos);
        }
        if(!work) return false;
        if(id != 0 && InfoStorer.Instance.TryFindBlock(id, out var block))
        {
            if(DropperManager.Instance != null)
                DropperManager.Instance.CreateDropprer(pos, block);
        }
        return true;
    }
    public bool AddBlock(Vector3Int pos,BlocksInfo Id)
    {
        Vector3Int vector3Int = Player.PostionInt;
        if(vector3Int==pos||vector3Int==pos+Vector3Int.up) return false;
        World.Instance.AddBlock(Id, pos);
        return true;
    } 
    protected override void SingletonAwake()
    {
       
    }
}