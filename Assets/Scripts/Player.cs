using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField]
    private float speedMax;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        var moveVec = playerInput.actions["MOVE"].ReadValue<Vector2>();
        

        var cameraDir=playerInput.camera.transform.forward;
        var cameraRight=playerInput.camera.transform.right;

        var moveVec3D = cameraDir * moveVec.y * speedMax + cameraRight * moveVec.x * speedMax;

        transform.position = transform.position + moveVec3D * Time.deltaTime;

    }
}
