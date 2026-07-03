using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private PlayerInput playerInput;
    Rigidbody rb;
    Vector3 rotateTarget;

    [SerializeField]
    private float speedMax;

    [SerializeField] float accel;

    [SerializeField] float rotateSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var accelVec = playerInput.actions["Move"].ReadValue<Vector2>();

        var cameraDir = playerInput.camera.transform.forward;
        cameraDir.y = 0;
        cameraDir = cameraDir.normalized;

        var cameraRight = playerInput.camera.transform.right;

        var accelVec3D =
            cameraDir * accelVec.y * accel
            + cameraRight * accelVec.x * accel;
        rb.AddForce(accelVec3D, ForceMode.Acceleration);

        if(accelVec3D!=Vector3.zero)
        {
            rotateTarget = accelVec3D.normalized;
        }
        // 前方向をコピーしておく
        Vector3 forward = transform.forward;

        // 上方向を固定
        transform.up = Vector3.up;

        // 前方向をターゲットに向かって補間
        transform.forward = Vector3.Slerp(forward, rotateTarget, rotateSpeed * Time.deltaTime);
    }
}
