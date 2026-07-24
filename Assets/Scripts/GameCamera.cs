using UnityEngine;
using UnityEngine.InputSystem;

public class GameCamera : MonoBehaviour
{

    [SerializeField] PlayerInput playerInput;

    [SerializeField] Transform lookTarget;
    [SerializeField] Vector3 offset;
    [SerializeField] float targetDistance;
    [SerializeField] float rotateSpeed;


    float pitch;
    float yaw;

    public float minPitch = -30f; // 下方向（見下ろし）の制限
    public float maxPitch = 60f;  // 上方向（見上げ）の制限


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        yaw = 90;
        pitch = 0;
    }

    


    // Update is called once per frame
    void Update()
    {
        var lookVec = playerInput.actions["Look"].ReadValue<Vector2>();
        yaw += lookVec.x * rotateSpeed * Time.deltaTime;
        pitch -= lookVec.y * rotateSpeed * Time.deltaTime;

        //kokototyuu
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        var target = lookTarget.position + offset;
        var rotation = Quaternion.Euler(pitch, yaw, 0);
        var position = rotation * new Vector3(0, 0, -targetDistance) + target;



        transform.rotation = rotation;
        transform.position = position;
    }

    
}
