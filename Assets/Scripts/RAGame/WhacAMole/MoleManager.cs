using System.Collections;
using UnityEngine;

public class MoleManager : MonoBehaviour
{
    private float lifetime;
    private float spawnTime;
    private System.Action onWhackCallback;

    public float RemainingTime
    {
        get
        {
            return Mathf.Max(0, lifetime - (Time.time - spawnTime));
        }
    }

    public void Initialize(float moleLifetime, System.Action onWhackCallback)
    {
        this.lifetime = moleLifetime;
        this.spawnTime = Time.time;
        StartCoroutine(DestroyAfterLifetime());
        this.onWhackCallback = onWhackCallback;
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    // Detecta la colisión con las manos 
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Hand") || collision.gameObject.name.Contains("Hand"))
        {
            if (onWhackCallback != null)
            {
                onWhackCallback.Invoke();
            }

            Destroy(gameObject);
        }
    }
}
