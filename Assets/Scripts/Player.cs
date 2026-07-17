using System.ComponentModel;
using Unity.VisualScripting;
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

    [SerializeField] float jumpSpeed;

    [SerializeField] Animator animator;

    [SerializeField] float groundNormalYMin = 0.7f;

    [SerializeField] float groundDamping = 8f;
    [SerializeField] float airDamping = 0.5f;

    [SerializeField] GameObject firePrefab;

    [SerializeField] float fireSpeed;

    [SerializeField] Vector3 fireOffset;

    [SerializeField] float invincibleTimeMax = 0.5f;

    [SerializeField] float knockbackSpeed;

    float invincibleTime = 0f;


    bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        rb.sleepThreshold = -1;
    }

    private void FixedUpdate()
    {
        // 減衰を地上と空中で変える
        if (isGrounded)
        {
            rb.linearDamping = groundDamping;
        }
        else
        {
            rb.linearDamping = airDamping;
        }

        // 物理計算中に接地判定を行うため、一旦ここで false にしておく
        isGrounded = false;
    }
    // Update is called once per frame
    void Update()
    {
        
        

        var moveVec = playerInput.actions["Move"].ReadValue<Vector2>();
        var moveVec3D = new Vector3(
            moveVec.x * speedMax, 0, moveVec.y * speedMax);
        transform.position = transform.position + moveVec3D * Time.deltaTime;


        var accelVec = playerInput.actions["Move"].ReadValue<Vector2>();

        var cameraDir = playerInput.camera.transform.forward;
        cameraDir.y = 0;
        cameraDir = cameraDir.normalized;

        var cameraRight = playerInput.camera.transform.right;

        var accelVec3D =
            cameraDir * accelVec.y * accel
            + cameraRight * accelVec.x * accel;
        rb.AddForce(accelVec3D, ForceMode.Acceleration);

        // プレイヤーの向きを変える
        if (accelVec3D != Vector3.zero)
        {
            rotateTarget = accelVec3D.normalized;
        }
        transform.forward = rotateTarget;

        // 前方向をコピーしておく
        Vector3 forward = transform.forward;

        // 上方向を固定
        transform.up = Vector3.up;

        // 前方向をターゲットに向かって補間
        transform.forward =
            Vector3.Slerp(forward, rotateTarget, rotateSpeed * Time.deltaTime);

        if (playerInput.actions["Attack"].WasPerformedThisFrame())
        {
            var position = transform.position + transform.TransformVector(fireOffset);
            var fireObj = Object.Instantiate(firePrefab,position,transform.rotation);
            var fireRB = fireObj.GetComponent<Rigidbody>();
            if( fireRB != null )
            {
                fireRB.linearVelocity = transform.forward * fireSpeed;
            }
        }

        // ジャンプ
        if (playerInput.actions["Jump"].WasPressedThisFrame() && isGrounded)
        {
            Vector3 jumpVec = new Vector3(0, jumpSpeed, 0);
            rb.AddForce(jumpVec, ForceMode.VelocityChange);
        }


        // アニメーターのMoveSpeedパラメータに Rigidbody の移動速度の大きさを与える
        Vector3 velocityXZ = rb.linearVelocity;
        velocityXZ.y = 0;
        animator.SetFloat("MoveSpeed", velocityXZ.magnitude);
    }
    private void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if(contact.normal.y>=groundNormalYMin)
            {
                isGrounded = true;
            }
        }
        var attackObj = collision.gameObject.GetComponent<AttackObject>();

        if (attackObj != null && invincibleTime <= 0)
        {
            hp = attackObj.power;
            invincibleTime = invincibleTimeMax;
            if (hp <= 0)
            {
                Destroy(gameObject);
            }

            var dir = transform.position - collision.transform.position;
            dir.y = 0;
            var knockbackVec = dir.normalized * knockbackSpeed;
            rb.linearVelocity = knockbackVec;
        }
    }

    
}
