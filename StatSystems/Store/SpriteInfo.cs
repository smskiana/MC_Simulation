using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace StatSystems.Store
{
    public class SpriteInfo : Info
    {
        [SerializeField]private AssetReferenceSprite spriteReference;
        private Sprite spritePrefab;
        private Task<Sprite> loadingTask;
        public Sprite AppearancePrefab
        {
            get => spritePrefab;
        }
        public async Task<Sprite> GetAppearancePrefabAsync()
        {
#if UNITY_EDITOR
            if (!spriteReference.RuntimeKeyIsValid())
            {
                Debug.LogError($"{name} Appearance reference invalid");
            }
#endif 
            if (spritePrefab != null)
                return spritePrefab;
            loadingTask ??= spriteReference.LoadAssetAsync<Sprite>().Task;
            spritePrefab = await loadingTask;
            loadingTask = null;
            return spritePrefab;
        }
        public async Task PreloadAsync()
        {
#if UNITY_EDITOR
            if (!spriteReference.RuntimeKeyIsValid())
            {
                Debug.LogError($"{name} Appearance reference invalid");
            }
#endif

            if (spritePrefab != null)
                return; // 已加载直接返回
            loadingTask ??= spriteReference.LoadAssetAsync<Sprite>().Task;
            spritePrefab = await loadingTask;
            loadingTask = null;
        }
        public void Release()
        {
#if UNITY_EDITOR
            if (!spriteReference.RuntimeKeyIsValid())
            {
                Debug.LogError($"{name} Appearance reference invalid");
            }
#endif 
            if (spritePrefab != null)
            {
                spriteReference.ReleaseAsset();
                spritePrefab = null;
            }
        }
    }
}
