using UnityEngine;

public class Enemy : MonoBehaviour
{

    Rigidbody rb;

    

    [SerializeField] float rotateSpeed = 20;

    [SerializeField] float moveSpeed;

    [SerializeField] int hp = 2;

    [SerializeField] float invincibleTimeMax = 0.5f;

    [SerializeField] float knockbackSpeed;

    float invincibleTime = 0f;

    public Collider playerCollider { get; set; }

    bool isSeenPlayer;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        var subVec = playerCollider.bounds.center - rb.position;
        subVec.y = 0;
        rb.linearVelocity = subVec.normalized * moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
        var direction = playerCollider.bounds.center - rb.position;

        isSeenPlayer = true;
        if (Physics.Raycast(rb.position, direction.normalized,
            out var hitInfo))
        {
            if (hitInfo.collider != playerCollider)
            {
                // プレイヤー以外の障害物に当たった場合は見えない1
                isSeenPlayer = false;
            }
        }

        if (isSeenPlayer && invincibleTime <= 0)
        {
            var subVec = playerCollider.bounds.center - rb.position;
            subVec.y = 0;
            rb.linearVelocity = subVec.normalized * moveSpeed;

            // プレイヤーの方向を向く
            var rotateTarget = subVec.normalized;
            Vector3 forward = transform.forward;
            transform.forward = Vector3.Slerp(forward, rotateTarget,
                rotateSpeed * Time.deltaTime);
        }

        if (invincibleTime > 0)
        {
            invincibleTime-= Time.deltaTime;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        var attackObj = collision.gameObject.GetComponent<AttackObject>();

        if (attackObj != null && invincibleTime <= 0) 
        {
            hp = attackObj.power;
            invincibleTime=invincibleTimeMax;
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
