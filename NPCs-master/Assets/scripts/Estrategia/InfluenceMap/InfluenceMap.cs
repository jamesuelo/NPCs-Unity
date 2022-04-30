using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Vector2I
{
	public int x;
	public int y;
	public float d;

	public Vector2I(int nx, int ny)
	{
		x = nx;
		y = ny;
		d = 1;
	}

	public Vector2I(int nx, int ny, float nd)
	{
		x = nx;
		y = ny;
		d = nd;
	}
}

public class InfluenceMap : GridData
{
	List<IPropagator> propagators = new List<IPropagator>();
	float[,] influencesBuffer;
	public float Decay { get; set; }
	public float Momentum { get; set; }
	public int Width { get{ return gridMap.Nodos.GetLength(0); } }
	public int Height { get{ return gridMap.Nodos.GetLength(1); } }
	public Nodo GetValue(int x, int y)
	{
		return gridMap.Nodos[x, y];
	}

	[SerializeField] 
	private Grid gridMap;
	
	public InfluenceMap(int size, float decay, float momentum, Grid grid)
	{
		influencesBuffer = new float[size, size];
		Decay = decay;
		Momentum = momentum;
		gridMap = grid;
	}
	
	public InfluenceMap(int width, int height, float decay, float momentum, Grid grid)
	{
		influencesBuffer = new float[width, height];
		Decay = decay;
		Momentum = momentum;
		gridMap = grid;
	}
	
	public void SetInfluence(int x, int y, float value)
	{
		if (x < Width && y < Height)
		{
			Nodo nodo = gridMap.GetNodoPosicionGlobal(new Vector3(x, 0, y));
			if (nodo.walkable) {
				nodo.influence = value;
			}
			Vector3 indices = gridMap.GetIndicesNodos(new Vector3(x,0,y));
			influencesBuffer[(int)indices.x,(int) indices.z] = value;
		}
	}

	public void SetInfluence(Vector2I pos, float value)
	{
		if (pos.x < Width && pos.y < Height)
		{
			Nodo nodo = gridMap.GetNodoPosicionGlobal(new Vector3(pos.x, 0, pos.y));
			if (nodo.walkable) {
				nodo.influence = value;
			}
			Vector3 indices = gridMap.GetIndicesNodos(new Vector3(pos.x,0,pos.y));
			influencesBuffer[(int)indices.x,(int) indices.z] = value;
		}
	}

	public void RegisterPropagator(IPropagator p)
	{
		propagators.Add(p);
	}

	public void Propagate()
	{
		UpdatePropagators();
		UpdatePropagation();
		UpdateInfluenceBuffer();
	}

	void UpdatePropagators()
	{
		foreach (IPropagator p in propagators)
		{
			SetInfluence(p.GridPosition, p.Value);
		}
	}

	void UpdatePropagation()
	{
		for (int xIdx = 0; xIdx < gridMap.Nodos.GetLength(0); ++xIdx)
		{
			for (int yIdx = 0; yIdx < gridMap.Nodos.GetLength(1); ++yIdx)
			{

				float maxInf = 0.0f;
				float minInf = 0.0f;
				Vector2I[] neighbors = GetNeighbors(xIdx, yIdx);
				foreach (Vector2I n in neighbors)
				{
					float inf = influencesBuffer[n.x, n.y] * Mathf.Exp(-Decay * n.d); //* Decay;
					maxInf = Mathf.Max(inf, maxInf);
					minInf = Mathf.Min(inf, minInf);
				}
				
				Nodo nodo = gridMap.Nodos[xIdx, yIdx];
				if (nodo.walkable) {
					if (Mathf.Abs(minInf) > maxInf)
					{
						nodo.influence = Mathf.Lerp(influencesBuffer[xIdx, yIdx], minInf, Momentum);
					}
					else
					{
						nodo.influence = Mathf.Lerp(influencesBuffer[xIdx, yIdx], maxInf, Momentum);
					}
				}
			}
		}
	}

	void UpdateInfluenceBuffer()
	{
		for (int xIdx = 0; xIdx < gridMap.Nodos.GetLength(0); ++xIdx)
		{
			for (int yIdx = 0; yIdx < gridMap.Nodos.GetLength(1); ++yIdx)
			{
				Nodo nodo = gridMap.Nodos[xIdx, yIdx];
				influencesBuffer[xIdx, yIdx] = nodo.influence;
			}
		}
	}

	Vector2I[] GetNeighbors(int x, int y)
	{
		List<Vector2I> retVal = new List<Vector2I>();
		
		// as long as not in left edge
		if (x > 0)
		{
			retVal.Add(new Vector2I(x-1, y));
		}

		// as long as not in right edge
		if (x < gridMap.Nodos.GetLength(0)-1)
		{
			retVal.Add(new Vector2I(x+1, y));
		}
		
		// as long as not in bottom edge
		if (y > 0)
		{
			retVal.Add(new Vector2I(x, y-1));
		}

		// as long as not in upper edge
		if (y < gridMap.Nodos.GetLength(1)-1)
		{
			retVal.Add(new Vector2I(x, y+1));
		}


		// diagonals

		// as long as not in bottom-left
		if (x > 0 && y > 0)
		{
			retVal.Add(new Vector2I(x-1, y-1, 1.4142f));
		}

		// as long as not in upper-right
		if (x < gridMap.Nodos.GetLength(0)-1 && y < gridMap.Nodos.GetLength(1)-1)
		{
			retVal.Add(new Vector2I(x+1, y+1, 1.4142f));
		}

		// as long as not in upper-left
		if (x > 0 && y < gridMap.Nodos.GetLength(1)-1)
		{
			retVal.Add(new Vector2I(x-1, y+1, 1.4142f));
		}

		// as long as not in bottom-right
		if (x < gridMap.Nodos.GetLength(0)-1 && y > 0)
		{
			retVal.Add(new Vector2I(x+1, y-1, 1.4142f));
		}
		return retVal.ToArray();
	}
}
