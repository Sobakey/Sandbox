﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public int direction = 0;
    public KeyCode leftKey, rightKey, jumpKey, InventoryKey;
    public float movementSpeed = 3f;
    public float jumpForce = 250f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.30f;
    public SpriteRenderer handSlot;

    private Rigidbody2D rb;
    private Animator anim;
    private GUIManager guiManager;
    private Transform groundCheck;
	
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        guiManager = GameObject.Find("GUI").GetComponent<GUIManager>();
        groundCheck = transform.FindChild("Ground_Checker");
	}
	
	
	void Update () {
        UpdateControls();
        UpdateMovement();
        UpdateBreaking();
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position,Vector2.down,groundCheckDistance,groundLayer);
        return hit.collider != null;
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

        if (Input.GetKeyDown(jumpKey) && IsGrounded())
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

    public void HoldItem(Sprite sprite)
    {
            handSlot.sprite = sprite;                          
    }

    private void UpdateMovement()
    {
        rb.velocity = new Vector2(movementSpeed*direction,rb.velocity.y);
    }

    private void UpdateBreaking()
    { //TODO set destroy radius
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit_left = Physics2D.Raycast(new Vector2(pos.x - 1, pos.y), transform.position - pos, 0.1f);
            RaycastHit2D hit_rigth = Physics2D.Raycast(new Vector2(pos.x + 1, pos.y), transform.position - pos, 0.1f);
            RaycastHit2D hit_up = Physics2D.Raycast(new Vector2(pos.x, pos.y + 1), transform.position - pos, 0.1f);
            RaycastHit2D hit = Physics2D.Raycast(pos, transform.position - pos,0.1f);
            RaycastHit2D hit_down = Physics2D.Raycast(new Vector2 (pos.x,pos.y - 1), transform.position - pos, 0.1f);

            if (hit.collider != null && ((hit_up.collider == null || hit_left.collider == null || hit_rigth.collider == null || hit_down.collider == null)  || hit_up.collider.gameObject.tag == "tall_grass" || (hit_up.collider.gameObject.tag == "player" || hit_left.collider.gameObject.tag == "player" || hit_rigth.collider.gameObject.tag == "player" || hit_down.collider.gameObject.tag == "player")))
            {
                if (hit_down.collider == null)
                {
                    hit_down = hit;
                }

                if (hit.collider.gameObject.tag == "Block" || hit.collider.gameObject.tag == "tall_grass")
                {
                    GameObject.Find("World").GetComponent<WorldGenerator>().DestroyBlock(hit.collider.gameObject, hit_down.collider.gameObject);
                }
            }
        }
    }

    private void Jump()
    {
        rb.AddForce(transform.up*jumpForce);
    }
}