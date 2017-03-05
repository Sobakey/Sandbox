using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Drop  {

    public string itemName;
    [Range(0.0f,1.0f)]
    public float dropChance;

    public bool DropChanceSucess()
    {
        return Random.value <= dropChance;
    }

	public void Instantiate(Vector2 pos)
	{
		GameObject dropObject = new GameObject();
		dropObject.transform.position = pos;
		dropObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		dropObject.AddComponent<SpriteRenderer>().sprite = ItemDatabase.Instance.FindItem(itemName).sprite;
		dropObject.AddComponent<PolygonCollider2D>();
		dropObject.AddComponent<Rigidbody2D>();
		dropObject.layer = LayerMask.NameToLayer("drop");
		dropObject.AddComponent<Magnetism>().target = GameObject.FindWithTag("player").transform;
		dropObject.name = itemName;
	}
}
