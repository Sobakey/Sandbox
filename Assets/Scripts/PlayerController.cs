using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public int direction = 0;
	public KeyCode leftKey, rightKey, jumpKey, InventoryKey, RunKey;
	public float movementSpeed = 7f;
	public float jumpForce = 250f;
	public LayerMask groundLayer;
	public float groundCheckDistance = 0.30f;
	public SpriteRenderer handSlot;

	public int breakDistance = 1;

	private Rigidbody2D rb;
	private Animator anim;
	private GUIManager guiManager;
	private Transform groundCheck;
	private Hotbar hotbar;
	private WorldGenerator worldGen;
	private BlockManager blockManager;
	private Vector2 bounds;
	private bool isRunning;


	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		guiManager = GameObject.Find("GUI").GetComponent<GUIManager>();
		groundCheck = transform.FindChild("Ground_Checker");
		hotbar = GameObject.Find("HUD").GetComponent<Hotbar>();
		worldGen = GameObject.Find("World").GetComponent<WorldGenerator>();
		blockManager = GameObject.Find("GameManager").GetComponent<BlockManager>();
		bounds = GetComponent<BoxCollider2D>().size;
	}

	void Update()
	{
		UpdateControls();
		UpdateMovement();
		UpdateBreaking();
		UpdatePlacing();
	}

	private bool IsGrounded()
	{
		RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
		return hit.collider != null;
	}

	private void SetDirection(int dir)
	{
		direction = dir;
		transform.localScale = new Vector3(dir, transform.localScale.y);
		anim.SetBool("isWalking", true);
	}

	private void UpdateControls()
	{
		if (Input.GetKey(leftKey) || Input.GetKey(KeyCode.LeftArrow))
		{
			SetDirection(-1);
		}
		else if (Input.GetKey(rightKey) || Input.GetKey(KeyCode.RightArrow))
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            if (guiManager.isOptionPanelOpen)
            {
                guiManager.ShowOptionPanel(false);
                Time.timeScale = 1;
            }
            else
            {
                guiManager.ShowOptionPanel(true);
                Time.timeScale = 0;
            }
        }

		// клавиша ускорения -->
		if (Input.GetKey(leftKey) && Input.GetKey(RunKey))
		{
			isRunning = true;
			movementSpeed = 14f;
		}
		if (Input.GetKey(rightKey) && Input.GetKey(RunKey))
		{
			isRunning = true;
			movementSpeed = 14f;
		}
		if (!Input.GetKey(RunKey) && isRunning)
		{
			isRunning = false;
			movementSpeed = 7f;
		}
		//<--
	}

	public void HoldItem(Sprite sprite, float size)
	{
		handSlot.sprite = sprite;
		handSlot.transform.localScale = new Vector3(size, size, handSlot.transform.localScale.z);
	}

	private void UpdateMovement()
	{
		rb.velocity = new Vector2(movementSpeed * direction, rb.velocity.y);
	}

	private void UpdateBreaking()
	{
		//TODO set destroy radius
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var tile = blockManager.GetTileAtPos(pos);
			if (tile != null && isCanBreak(pos))
			{
				Vector2Int chunkPos = ChunkManager.Instance.GetChunkCoordAtPos(tile.transform.position);

				Chunk chunk;
				if (ChunkManager.Instance.TryGetChunk(chunkPos, out chunk))
				{
					chunk.DestroyTile(tile);
				}
			}
		}
	}

	private bool isCanBreak(Vector3 pos)
	{
		var xDiff = Mathf.Abs(transform.position.x - pos.x);
		var yDiff = Math.Abs(transform.position.y - pos.y);
		return (xDiff - bounds.x) < breakDistance && (yDiff - bounds.y) < breakDistance;
	}
	//TODO:(dx) заменить рейкастинг тут
	private void UpdatePlacing()
	{
		if (Input.GetMouseButtonDown(1))
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit_left = Physics2D.Raycast(new Vector2(pos.x - 1, pos.y), transform.position - pos, 0.1f);
			RaycastHit2D hit_rigth = Physics2D.Raycast(new Vector2(pos.x + 1, pos.y), transform.position - pos, 0.1f);
			RaycastHit2D hit_up = Physics2D.Raycast(new Vector2(pos.x, pos.y + 1), transform.position - pos, 0.1f);
			RaycastHit2D hit = Physics2D.Raycast(new Vector3(pos.x, pos.y, 0), transform.position - pos, 0.1f);
			RaycastHit2D hit_down = Physics2D.Raycast(new Vector2(pos.x, pos.y - 1), transform.position - pos, 0.1f);

			if ((hit.collider == null || hit.collider.gameObject.layer == 8)
			    && (hit_up.collider != null && hit_up.collider.gameObject.tag != "player" || hit_left.collider != null &&
			        hit_left.collider.gameObject.tag != "player"
			        || hit_rigth.collider != null && hit_rigth.collider.gameObject.tag != "player"
			        || hit_down.collider != null && (hit_down.collider.gameObject.tag != "player"
			                                         && hit_down.collider.gameObject.tag != "tall_grass")))
			{
				int i = 1;
				while (hit_down.collider == null || hit_down.collider.gameObject.tag == "tall_grass")
				{
					hit_down = Physics2D.Raycast(new Vector2(pos.x, pos.y - i), transform.position - pos, 0.1f);
					i++;
				}


				Item item = hotbar.GetHeldItem();

				if (item == null)
				{
					return;
				}

				if (item.type == Item.Type.Block)
				{
					blockManager.CreateTileAtPos(item.block_id, pos);
				}
			}
		}
	}

	private void Jump()
	{
		rb.AddForce(transform.up * jumpForce);
	}
}