using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float powerUpUseTime = 5f;
    float currentPowerUpUseTime;

    [SerializeField] GameObject despawnParticle;


    bool wasGrabbed = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (wasGrabbed && currentPowerUpUseTime < Time.time)
        {
            if (despawnParticle != null)
                Instantiate(despawnParticle, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    public virtual void GrabbedPowerUp()
    {
        wasGrabbed = true;
        currentPowerUpUseTime = Time.time + powerUpUseTime;
    }

    public virtual void ActivatePowerUp()
    {

    }

    public virtual void DeactivatePowerUp()
    {

    }

}
