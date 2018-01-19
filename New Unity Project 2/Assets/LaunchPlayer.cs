using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPlayer : MonoBehaviour {
    public Rigidbody2D rb;
    Vector2 dragStart;
    Vector2 dragEnd;
    [SerializeField]
    private float forceScale = 1;

    public float dashSpeed = 200f;
    // Use this for initialization
    [SerializeField]
    private Vector2 normal;
    private float jumpTimer = 0;
    public int contacts;
    public GameObject stuckObject = null;
    private HingeJoint2D joint;

    public float dashTimeSeconds = 0.3f;

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private LineRenderer Line;
    [SerializeField]
    private Rigidbody2D playerBouncerObject;
    [SerializeField]
    private GameObject arrowObject;

    public float jumpLimit = 50;

    private bool renderLine;

    private bool dashAllowed = false;
    private bool jumpingAllowed = false;

    private float dashTimer = 0f;
    
    public enum State { normal, dashing, stunned, jumping };
    public State state = State.normal;

	void Start () {

        joint = GetComponent<HingeJoint2D>();
        mainCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        if (contacts < 1) jumpTimer += Time.deltaTime; else jumpTimer = 0;
		if (Input.GetMouseButtonDown(0))
        {
            renderLine = true;
            dragStart = Input.mousePosition;
            Line.SetPosition(0, mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane)));
        }
        dragEnd = Input.mousePosition;
        Vector2 f = dragEnd - dragStart;
        if (f.magnitude > jumpLimit)
        {
            f = f.normalized * jumpLimit;
            dragEnd = dragStart + f;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (state == State.jumping && dashAllowed)
            {
                f *= forceScale;
                Dash(f);
            }
            if (jumpTimer < 0.1f && jumpingAllowed)
            {
                f *= forceScale;
                applyLaunch(f);
                joint.enabled = false;
                state = State.jumping;
            }
            renderLine = false;
        }
        Line.SetPosition(1, mainCamera.ScreenToWorldPoint(new Vector3(dragEnd.x, dragEnd.y, mainCamera.nearClipPlane)));
        Line.gameObject.SetActive(renderLine);

        SetArrow();
	}

    private void FixedUpdate()
    {
        if (state == State.dashing)
        {
            rb.gravityScale = 0f;
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0)
            {
                state = State.jumping;
                rb.gravityScale = 1f;
                rb.velocity = rb.velocity/2.5f;
                playerBouncerObject.velocity = playerBouncerObject.velocity/2.5f;
            }
        } else
        {
            rb.gravityScale = 1f;
        }
        string[] mask = { "Ground" };
        if (Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.2f), 1.2f, LayerMask.GetMask(mask)))
        {
            jumpingAllowed = true;
            dashAllowed = true;
            if (state != State.stunned & state != State.dashing) state = State.normal;
        }
        else
        {
            if (state == State.normal) state = State.jumping;
            jumpingAllowed = false;
        }
    }

    private void Dash(Vector2 forceVector)
    {
        rb.velocity = Vector2.zero;
        playerBouncerObject.velocity = Vector2.zero;
        rb.AddForce(forceVector.normalized * dashSpeed);
        dashAllowed = false;
        state = State.dashing;
        dashTimer = dashTimeSeconds;
    }

  

    public void applyLaunch(Vector2 forceVector)
    {
        rb.velocity = rb.velocity - new Vector2(rb.velocity.x * 0.2f, rb.velocity.y * 0.8f);
        rb.AddForce(forceVector);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        foreach (ContactPoint2D contact in coll.contacts) {
            normal = contact.normal;
            if (normal.y >= 0)
            {
                contacts += 1;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D coll)
    {
        foreach(ContactPoint2D contact in coll.contacts)
        {
            if (normal.y >= 0)
            {
                contacts -= 1;
            }
        }
    }

    private void SetArrow()
    {
        if (rb.velocity.magnitude > 10)
        {
            arrowObject.SetActive(true);
            float radians = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
            arrowObject.transform.eulerAngles = new Vector3(0,0,radians * Mathf.Rad2Deg - 90);
        }
        else
        {
            arrowObject.SetActive(false);
        }        
    }
}