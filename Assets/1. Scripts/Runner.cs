using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance;
    [SerializeField] float activateStunTime = 2f;
    [SerializeField] Animator anim;
    [SerializeField] GameObject destroyParticleSystem;
    [SerializeField] ParticleSystem StunParticleSystem;

    public int ScoreToAdd = 1;

    Rigidbody rb;
    int currentPathIndex = 0;
    List<Vector3> pathVectorList;
    Vector3 moveDir;

    float currentStunTime;
    

    const string ANIM_GROUNDED = "Grounded"; 
    const string ANIM_STUNNED = "Stunned"; 
    const string ANIM_RUNSPEED = "RunSpeed"; 
    public bool isGrabbed { get; set; }
    public bool isStunned { get; set; }
    public bool isGrounded { get; set; }
    bool groundedLastFrame;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void UpdateRunner(bool night)
    {
        if (!isGrabbed && !isStunned)
        {
            HandleMovement(night);
        }
        HandleRotation();

        CheckForGround();
        if (currentStunTime < Time.time)
        {
            anim.SetBool(ANIM_STUNNED, false);
            isStunned = false;
        }
        if (isGrounded != groundedLastFrame)
            StunRunner();
        groundedLastFrame = isGrounded;

        if (transform.position.y < -3)
            DestroyRunner();

        //if (isStunned && isGrounded && !StunParticleSystem.isPlaying && !isGrabbed)
        //    StunParticleSystem.Play();
    }

    private void CheckForGround()
    {
        //RaycastHit hit;
        //Debug.DrawRay(transform.position + Vector3.up, Vector3.down * groundCheckDistance, Color.yellow);
        
        
        if(!Physics.Raycast(transform.position + Vector3.up, Vector3.down, groundCheckDistance, groundLayer))
        {
            isGrounded = false;
            anim.SetBool(ANIM_GROUNDED, false);
        }
        else
        {
            anim.SetBool(ANIM_GROUNDED, true);
            isGrounded = true;
        }

    }

    private void HandleMovement(bool night)
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
                //transform.position = transform.position + moveDir * moveSpeed* Time.deltaTime;
                var nightMoveSpeedBonus = 1f;
                if (night)
                    nightMoveSpeedBonus = 2f;
                anim.SetFloat(ANIM_RUNSPEED, nightMoveSpeedBonus);
                rb.MovePosition(transform.position + moveDir * moveSpeed * (nightMoveSpeedBonus) * Time.deltaTime);
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

    public void RunnerSelectedEntered()
    {
        isGrabbed = true;
        
    }

    public void RunnerSelectedExited()
    {
        isGrabbed = false;
    }

    public void RunnerActivated()
    {
        anim.SetBool(ANIM_STUNNED, true);
        currentStunTime = Time.time + activateStunTime;
        isStunned = true;
        StunParticleSystem.Play();
    }

    public void StunRunner()
    {

        isStunned = true;
        currentStunTime = Time.time + activateStunTime;
    }

    public void DestroyRunner()
    {
        if(destroyParticleSystem != null)
            Instantiate(destroyParticleSystem, transform.position, Quaternion.identity);
        GameManager.instance.RemoveRunner(this);
        Destroy(gameObject);
    }

    public void DestroyRunner(float scoreToAdd)
    {
        ScoreManager.instance.AddScore(scoreToAdd);
        DestroyRunner();
    }
}
