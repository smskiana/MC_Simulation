using UnityEngine;
using _3C.Actors;
using StatSystems.Stats.Items;
using StatSystems;
using Sirenix.OdinInspector;
public class Anmo :MonoBehaviour,IDamageSource
{
    private AnmoInfo stats;
    [SerializeField][ReadOnly]private float timer;
    public bool Firing;
    [SerializeField] private LayerMask ActorMask;
    [SerializeField] private LayerMask GroundMask;
    private void Update() 
    {
        if (!Firing) return;
        timer += Time.deltaTime;
       if(stats==null||timer >= stats.Time)
        {         
            Destroy(this.gameObject);
            return;
        }
        float moveDistance = stats.Speed * Time.deltaTime;
        Vector3 startPos = transform.position;
        Vector3 moveDir = transform.forward;
        if (Physics.Raycast(startPos, moveDir, out var hitInfo, moveDistance,ActorMask|GroundMask))
        {
            int layer = hitInfo.collider.gameObject.layer;
            int mask = 1 << layer;
            if((ActorMask.value & mask) == mask)
            {
                if (hitInfo.collider.TryGetComponent<IDamageTarget>(out var source))
                {
                    var package = GetDamage();
                    package.state = PackageState.Sending;
                    source.SetDamage(package);
                }
            }
            if((GroundMask.value & mask) == mask)
            {
                Deleteblock(hitInfo.point);
            }
            Destroy(this.gameObject);
        }
        transform.position += moveDir * moveDistance;
    }
    public void OnEnable()
    {
        timer = 0;
    }

    public void Init(AnmoInfo info) { stats = info; Firing = true; }
    public bool Deleteblock(Vector3 poi)
    {
        Vector3 dir = transform.forward;  
        Vector3Int dir2 = Tool.Align(poi + 0.01f * (dir.normalized));
        Debug.Log("»÷ÖÐ£º" + dir2);
        WorldManager manager = WorldManager.Instance;
        if (manager != null) 
        {
            manager.RemoveBlock(dir2);
        }
        return false;
    }
    public void SetInfo(AnmoInfo val)
    {
        stats = val;
    }
    public DamagePackage GetDamage() => new(stats.Damage)
    {
        state = PackageState.Preparing,
        Source = this,
    };
}
