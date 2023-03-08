using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;
    int currentPathIndex = 0;
    List<Vector3> pathVectorList;
    Vector3 moveDir;
    public bool isGrabbed { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateRunner()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (pathVectorList != null && pathVectorList.Count != 0)
        {
            //Debug.Log(currentPathIndex + "  " + pathVectorList.Count + "   First: "  + pathVectorList[0]);
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if(Vector3.Distance(transform.position, targetPosition) > 1f)
            {
                 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                //enable animation
                transform.position = transform.position + moveDir * moveSpeed* Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                if(currentPathIndex >= pathVectorList.Count)
                {
                    pathVectorList = null; //Stop moving
                    //Disable animation
                    SetTargetPosition(GameManager.instance.GetRandomTargetPos());
                }
            }
        }
        else
        {
            //Disable animation
            SetTargetPosition(GameManager.instance.GetRandomTargetPos());

        }
    }

    private void HandleRotation()
    {

        if(moveDir != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.instance.FindPath(transform.position, targetPosition);
        //Debug.Log("TargetPos: " + targetPosition +", PathVectorList: " + pathVectorList.ToString());

        if(pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }
    }
}
