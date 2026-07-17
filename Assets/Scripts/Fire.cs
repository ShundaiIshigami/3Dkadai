using UnityEngine;
using UnityEngine.VFX;

public class Fire : MonoBehaviour
{
    [SerializeField] float lifespan;

    void Update()
    {
        lifespan -= Time.deltaTime;        
        if(lifespan < 0 )
        {
            Destroy(gameObject);
        }
        else if(lifespan < 0.5f)
        {
            GetComponentInChildren<VisualEffect>().Stop();
        }
    }
}
