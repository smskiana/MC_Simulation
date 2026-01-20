using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Store
{
    [CreateAssetMenu(fileName = "NewPrefabInfo", menuName = "Game/PrefabInfo")]
    public class PrefabInfo : Info
    {
        [SerializeField]private AssetReferenceGameObject prefabReference;
        private GameObject prefab;
        private Task<GameObject> loadingTask;
        public GameObject Prefab
        {
            get =>prefab;
        }
        public async Task<GameObject> GetPrefabAsync()
        {
#if UNITY_EDITOR
            if (!prefabReference.RuntimeKeyIsValid())
            {
                Debug.LogError($"{name} Appearance reference invalid");
                return null;
            }
#endif 
            if (prefab != null)
                return prefab;
            loadingTask ??= prefabReference.LoadAssetAsync<GameObject>().Task;
            try
            {
                prefab = await loadingTask;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load prefab {name}: {e}");
                prefab = null;
            }
            finally
            {
                loadingTask = null;
            }
            return prefab;
        }
        public async Task PreloadAsync()
        {
            if (!prefabReference.RuntimeKeyIsValid())
            {
                Debug.LogError($"{name} Appearance reference invalid");
                return;
            }
            if (prefab != null)
                return; // 已加载直接返回
            loadingTask ??= prefabReference.LoadAssetAsync<GameObject>().Task;
            try
            {
                prefab = await loadingTask;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load prefab {name}: {e}");
                prefab = null;
            }
            finally
            {
                loadingTask = null;
            }
        }
        public void Release()
        {
            if (prefab != null)
            {
                prefabReference.ReleaseAsset();
                prefab = null;
            }
        }
    }
}
