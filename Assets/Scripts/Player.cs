using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float speedMax;

    [SerializeField] float accel;

    [SerializeField] float rotateSpeed;

    [SerializeField] Animator animator;

    [SerializeField] float jumpSpeed;

    [SerializeField] float groundNormalYMin = 0.7f;

    [SerializeField] float groundDamping = 8f;

    [SerializeField] float airDamping = 0.5f;

    [SerializeField] GameObject firePrefab;

    [SerializeField] float fireSpeed;

    [SerializeField] Vector3 fireOffset;

    [SerializeField] int hp = 10;

    [SerializeField] float invincibleTimeMax = 0.5f;

    [SerializeField] float knockbackSpeed = 5;


    PlayerInput playerInput;

    Rigidbody rb;

    Vector3 rotateTarget;

    bool isGrounded = true;

    float invincibleTime = 0;


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
        if (isGrounded)
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

            // プレイヤーの向きを変える
            if (accelVec3D != Vector3.zero)
            {
                rotateTarget = accelVec3D.normalized;
            }
        }

        // 前方向をコピーしておく
        Vector3 forward = transform.forward;

        // 上方向を固定
        transform.up = Vector3.up;

        // 前方向をターゲットに向かって補間
        var targetForward = Vector3.Slerp(forward, rotateTarget, rotateSpeed * Time.deltaTime);
        if (targetForward != Vector3.zero)
        {
            transform.forward = targetForward;
        }

        // アニメーターのMoveSpeedパラメータに Rigidbody の移動速度の大きさを与える
        Vector3 velocityXZ = rb.linearVelocity;
        velocityXZ.y = 0;
        animator.SetFloat("MoveSpeed", velocityXZ.magnitude);

        // ジャンプ
        if (playerInput.actions["Jump"].WasPressedThisFrame()
            && isGrounded)
        {
            Vector3 jumpVec = new Vector3(0, jumpSpeed, 0);
            rb.AddForce(jumpVec, ForceMode.VelocityChange);
        }

        // 攻撃
        if (playerInput.actions["Attack"].WasPressedThisFrame())
        {
            var position = transform.position + transform.TransformVector(fireOffset);
            var fireObj = Object.Instantiate(firePrefab, position, transform.rotation);
            var fireRB = fireObj.GetComponent<Rigidbody>();
            if (fireRB != null)
            {
                fireRB.linearVelocity = transform.forward * fireSpeed;
            }
        }


        // 無敵時間を減らす
        if (invincibleTime > 0)
        {
            invincibleTime -= Time.deltaTime;
        }
    }

    private void OnCollisionStay(Collision collision)
    {


        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y >= groundNormalYMin)
            {
                isGrounded = true;
            }
        }

        var attackObj = collision.gameObject.GetComponent<AttackObject>();
        if (attackObj != null && invincibleTime <= 0)
        {
            if (collision.gameObject.CompareTag("Fire"))
            {
                return;
            }

            hp -= attackObj.power;
            invincibleTime = invincibleTimeMax;
            if (hp <= 0)
            {
                Destroy(gameObject);
            }

            // ノックバック
            var dir = transform.position - collision.transform.position;
            dir.y = 0;
            var knockbackVec = dir.normalized * knockbackSpeed;
            rb.AddForce(knockbackVec, ForceMode.VelocityChange);
        }
    }
}
