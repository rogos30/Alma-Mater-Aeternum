using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFollowerController : Interactable
{
    [SerializeField] private Transform target;
    [Range(0.5f, 10f)][SerializeField] public float moveSpeed;
    public float distanceFromTarget;
    public float delay = 0.5f;

    Animator animator;
    bool isFacingRight = false;
    float timeWithNoMovement;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        transform.position = target.position;
        timeWithNoMovement = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(MoveDelayed(target))
        if (Vector2.Distance(transform.position, target.position) >= distanceFromTarget)
        {
            timeWithNoMovement = 0f;
            Vector2 difference = target.position - transform.position;
            double angle = Math.Abs(Math.Atan(difference.x / difference.y) * 180 / Math.PI);
            if (angle > 45)
            {
                MoveHorizontal(difference.x >= 0);
            }
            else
            {
                MoveVertical(difference.y >= 0);
            }
            if (Vector2.Distance(transform.position, target.position) > 5)
            {
                transform.position = target.position;
            }
            else
            {
                Vector3 newPos = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                newPos.z = newPos.y;
                transform.position = newPos;
            }
        }
        else
        {
            timeWithNoMovement += Time.deltaTime;
            if (timeWithNoMovement >= 0.15f)
            {
                animator.SetInteger("isWalking", 0);
            }
        }
    }

    IEnumerator MoveDelayed(Transform target)
    {
        yield return new WaitForSeconds(delay);
        if (Vector2.Distance(transform.position, target.position) >= distanceFromTarget)
        {
            timeWithNoMovement = 0f;
            Vector2 difference = target.position - transform.position;
            double angle = Math.Abs(Math.Atan(difference.x / difference.y) * 180/Math.PI);
            if (angle > 45)
            {
                MoveHorizontal(difference.x >= 0);
            }
            else
            {
                MoveVertical(difference.y >= 0);
            }
            if (Vector2.Distance(transform.position, target.position) > 5)
            {
                transform.position = target.position;
            }
            else
            {
                Vector3 newPos = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                newPos.z = newPos.y;
                transform.position = newPos;
            }
        }
        else
        {
            timeWithNoMovement += Time.deltaTime;
            if (timeWithNoMovement >= 0.15f)
            {
                animator.SetInteger("isWalking", 0);
            }
        }
    }
    void MoveHorizontal(bool right)
    {
        if (right)
        {
            if (!isFacingRight)
            {
                Flip();
            }
        }
        else
        {

            if (isFacingRight)
            {
                Flip();
            }
        }
        animator.SetInteger("isWalking", 3);
    }
    void MoveVertical(bool up)
    {
        if (up)
        {
            animator.SetInteger("isWalking", 1);
        }
        else
        {
            animator.SetInteger("isWalking", 2);
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public override void Interact()
    {
        return;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("VisionCone"))
        {
            //Debug.Log("AAAAAAAAA");
            other.GetComponentInParent<PatrolNPCController>().CheckIfPlayerInSight();
        }
        else if (other.CompareTag("PursuitKillZone"))
        {
            other.GetComponentInParent<PatrolNPCController>().killZoneEngaged = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("VisionCone"))
        {
            //Debug.Log("BBBBBBBBB");
            other.GetComponentInParent<PatrolNPCController>().PlayableLeaveSight();
        }
    }
}
