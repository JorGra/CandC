using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    int currentPathIndex = 0;
    List<Vector3> pathVectorList;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (pathVectorList != null && pathVectorList.Count != 0)
        {
            Debug.Log(currentPathIndex + "  " + pathVectorList.Count);
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if(Vector3.Distance(transform.position, targetPosition) > 1f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                //enable animation
                transform.position = transform.position + moveDir * moveSpeed* Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                if(currentPathIndex >= pathVectorList.Count)
                {
                    pathVectorList = null;
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

    public void SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.instance.FindPath(transform.position, targetPosition);

        if(pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }
    }
}
