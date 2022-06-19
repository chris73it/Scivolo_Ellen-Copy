using ScriptableObjectArchitecture;
using System.Collections;
using UnityEngine;

public class EnergyPickUpAid : MonoBehaviour
{
    [SerializeField] FloatReference aidAmount = null;
    [SerializeField] FloatReference respawnDelaySeconds = null;
    [SerializeField] FloatReference pistolCurrentEnergy = null;
    [SerializeField] FloatReference pistolMaxEnergy = null;
    [SerializeField] FloatReference rotationSpeed = null;

    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed.Value * Time.deltaTime);
        transform.Rotate(Vector3.right, rotationSpeed.Value  * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pistolCurrentEnergy.Value += aidAmount.Value;
            if (pistolCurrentEnergy.Value > pistolMaxEnergy.Value)
            {
                pistolCurrentEnergy.Value = pistolMaxEnergy.Value;
            }
            StartCoroutine(RespawnEnergyPickUpAid(respawnDelaySeconds.Value));
        }
    }

    float waitTime;
    IEnumerator RespawnEnergyPickUpAid(float respawnDelaySeconds)
    {
        //Debug.Log("respawnDelaySeconds is " + respawnDelaySeconds);
        transform.position -= Vector3.down * 10000f; //Send the Pick Up Aid down there
        waitTime = 0;
        //Debug.Log("[E] Wait time is " + waitTime);
        while (waitTime <= respawnDelaySeconds)
        {
            waitTime += Time.deltaTime;
            //Debug.Log("[I] Wait time is " + waitTime);
            yield return null;
        }
        //Debug.Log("[X] Wait time is " + waitTime);
        transform.position += Vector3.down * 10000f; //Bring the Pick Up Aid back up
    }
}
