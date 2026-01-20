using Manager;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{
    public WorldManager manager;
    public Transform Idol;
    public Transform eye;
    [SerializeField] private float sensitivityX =1f;   // 水平旋转灵敏度
    [SerializeField] private float sensitivityY =1f;   // 垂直旋转灵敏度
    [SerializeField] private float minY = -60f;          // 最小俯仰角（向下）
    [SerializeField] private float maxY = 60f;           // 最大俯仰角（向上）
    [SerializeField] private float moveSpeed = 5f;        // 移动速度 
    [SerializeField] private Vector2 lookInput;
    [SerializeField] private float rotationY = 0f;
    [SerializeField] private float rotationX = 0f;         

    public float SensitivityX { get => sensitivityX;}
    public float SensitivityY { get => sensitivityY;}

    public void Update()
    {
        Look();
    }
    public void Start()
    {
        var inputaction = InputManager.Instance.InputActions;
        inputaction["Look"].performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputaction["Look"].canceled += ctx => lookInput = ctx.ReadValue<Vector2>();
    }

    public void OnEnable()
    {
        Look();
    }

    private void Look()
    {
        float mouseY = lookInput.y * sensitivityY;
        float mouseX = lookInput.x * sensitivityX;
        rotationY -= mouseY;   // 上正下负
        rotationX += mouseX;
        rotationY = Mathf.Clamp(rotationY, minY, maxY);
        // 上下看
        transform.localRotation = Quaternion.Euler(rotationY, 0f, 0f);   
        Idol.localRotation = Quaternion.Euler(0,rotationX, 0f);
    }   
    public void SetEye(Transform mod,Transform eyepos)
    {
        transform.position = eyepos.position;
        transform.localPosition = Vector3.zero;
        Idol = mod;
    }
}
