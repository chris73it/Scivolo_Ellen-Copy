using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Target : MonoBehaviour
{
    [SerializeField] float healthDuration;

    public void Hit(float damageSpeed)
    {
        healthDuration -= damageSpeed * Time.deltaTime;

        if (healthDuration <= 0)
        {
            Die(0); //Die with no delays
        }
    }

    public void Die(float delay)
    {
        Destroy(gameObject, delay);
    }
}
