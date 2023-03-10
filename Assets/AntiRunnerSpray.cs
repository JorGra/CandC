using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRunnerSpray : PowerUp
{

    bool isActivated;
    [SerializeField] float sprayDistance;
    [SerializeField] LayerMask hitLayerMask;
    [SerializeField] Transform[] rayOrigins;
    [SerializeField] ParticleSystem spraySystem;
    Grid<GridPiece> runnerGrid;

    protected override void Start()
    {
        base.Start();

        runnerGrid = GameManager.instance.GetRunnerGrid();
    }
    protected override void Update()
    {
        base.Update();

        if (isActivated)
        {
            foreach (var rayTrans in rayOrigins)
            {
                if(Physics.Raycast(rayTrans.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, sprayDistance, hitLayerMask))
                {
                    if(hit.collider.tag == "Runner")
                    {
                        hit.collider.GetComponentInParent<Runner>().DestroyRunner(0.5f);
                    }
                    if(hit.collider.tag == "Grid")
                    {
                        runnerGrid?.GetValue(hit.point)?.SubtractValue(1);
                    }

                }
                Debug.DrawRay(rayTrans.position, transform.TransformDirection(Vector3.forward) * sprayDistance, Color.yellow);

            }
            spraySystem.Play();
        }
    }
    public override void GrabbedPowerUp()
    {
        base.GrabbedPowerUp();
    }
    public override void ActivatePowerUp()
    {
        base.ActivatePowerUp();
        isActivated = true;
    }
    public override void DeactivatePowerUp()
    {
        base.DeactivatePowerUp();
        isActivated = false;
    }
}
