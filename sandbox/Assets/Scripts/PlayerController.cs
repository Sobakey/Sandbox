using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public int direction = 0;
    public KeyCode leftKey, rightKey, jumpKey, InventoryKey;
    public float movementSpeed = 3f;
    public float jumpForce = 250f;

    private Rigidbody2D rb;
    private Animator anim;
    private GUIManager guiManager;
	
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        guiManager = GameObject.Find("GUI").GetComponent<GUIManager>();
	}
	
	
	void Update () {
        UpdateControls();
        UpdateMovement();
        UpdateBreaking();

    }

    private void SetDirection(int dir)
    {
        direction = dir;
        transform.localScale = new Vector3 (dir, transform.localScale.y);
        anim.SetBool("isWalking", true);
    }

    private void UpdateControls()
    {
        if (Input.GetKey(leftKey))
        {
            SetDirection(-1);
        }
        else if (Input.GetKey(rightKey))
        {
            SetDirection(1);
        }
        else
        {
            direction = 0;
            anim.SetBool("isWalking", false);
        }

        if (Input.GetKeyDown(jumpKey))
        {
            Jump();
        }

        if (Input.GetKeyDown(InventoryKey))
        {
            if (guiManager.isInventoryOpen)
            {
                guiManager.ShowInventory(false);
            }
            else
            {
                guiManager.ShowInventory(true);
            }
        }
    }

    private void UpdateMovement()
    {
        rb.velocity = new Vector2(movementSpeed*direction,rb.velocity.y);
    }

    private void UpdateBreaking()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, transform.position - pos,0.1f); 
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "Block" || hit.collider.gameObject.tag == "tall_grass")
                {
                    GameObject.Find("World").GetComponent<WorldGenerator>().DestroyBlock(hit.collider.gameObject);
                }
            }
        }
    }

    private void Jump()
    {
        rb.AddForce(transform.up*jumpForce);
    }
}
