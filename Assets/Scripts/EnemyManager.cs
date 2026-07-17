using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] Enemy[] enemies;

    [SerializeField] Collider playerCollider;

    void OnEnable()
    {
        foreach (var enemy in enemies)
        {
            enemy.playerCollider = playerCollider;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
