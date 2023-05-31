using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxFallingSpeed = 2f;
    [SerializeField] private float maxMovementSpeed = 2f;
    [SerializeField] private float jumpHeight = 2f;

    private Rigidbody2D rb2d;
    private Collider2D col;

    private int sideMove;
    private bool jump;
    private float distToGround;
    
    
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        distToGround = col.bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        sideMove = 0;
        if (Input.GetKey("a"))
        {
            sideMove += -1;
        }

        if (Input.GetKey("d"))
        {
            sideMove += 1;
        }

        
        if (Input.GetKeyDown("w"))
        {
            jump = true;
        }
        
    }

    bool IsGrounded()
    {
        RaycastHit2D hit =  Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x-0.02f, 0.02f), 0, Vector2.down, distToGround + 0.05f);
        if (hit)
        {
            if (hit.collider.CompareTag("ground"))
            {
                return true;
            }
        }

        return false;
    }
    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(maxMovementSpeed * sideMove, rb2d.velocity.y);

        if (jump && IsGrounded())
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            rb2d.AddForce(new Vector2(0, jumpHeight));
            jump = false;
        }

        Vector2 horizontalMove = new Vector2(rb2d.velocity.x, 0);
        float horizontalDist = horizontalMove.magnitude * Time.deltaTime;
        Vector2 verticalMove = new Vector2(0, rb2d.velocity.y);
        float verticalDist = verticalMove.magnitude * Time.deltaTime;
        

        RaycastHit2D hitSides = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, transform.localScale.y), 0, horizontalMove, horizontalDist);

        if (hitSides)
        {
            if (hitSides.distance > 0.02f)
            {
                float newSpeed = Mathf.Min(maxMovementSpeed, hitSides.distance / Time.deltaTime);
                rb2d.velocity = horizontalMove.normalized * newSpeed + verticalMove;
            }
            else
            {
                rb2d.velocity = verticalMove;
            }
        }
        
        RaycastHit2D hitVertical = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x-0.01f, transform.localScale.y), 0, verticalMove, verticalDist);
        
        if (hitVertical)
        {
            if (hitVertical.distance > 0.02f)
            {
                float newSpeed = Mathf.Min(maxFallingSpeed, hitVertical.distance / Time.deltaTime);
                rb2d.velocity = verticalMove.normalized * newSpeed + new Vector2(rb2d.velocity.x, 0);
            }
            else
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            }
        }

        jump = false;
    }
}
