using UnityEngine;

public class Enemy : MonoBehaviour
{

    Rigidbody rb;

    [SerializeField] Enemy[] enemies;

    SerializeField] float rotateSpeed = 20;

    [SerializeField] float moveSpeed;

    public Collider playerCollider { get; set; }

    bool isSeenPlayer;

    void OnEnable()
    {
        foreach (var enemy in enemies)
        {
            enemy.playerCollider = playerCollider;
        }
    }

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

        if (isSeenPlayer)
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


    }


}
