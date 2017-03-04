using System;
using UnityEngine;

public struct Vector2Int
{
	public int x;
	public int y;
	public Vector2Int(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Vector2Int(Vector2 vector)
	{
		this.x = Mathf.RoundToInt(vector.x);
		this.y = Mathf.RoundToInt(vector.y);
	}

	public override string ToString()
	{
		return string.Format("{0};{1}", x, y);
	}
}

public static class Vector2Extensions
{
	static public Vector2Int ToInt(this Vector2 vector)
	{
		return new Vector2Int(vector);
	}
	static public Vector2Int ToInt(this Vector3 vector)
	{
		return new Vector2Int(vector);
	}
}