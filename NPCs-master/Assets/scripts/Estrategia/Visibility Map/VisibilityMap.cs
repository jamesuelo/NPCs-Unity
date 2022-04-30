using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityMap : GridData
{
	public int Width { get{ return gridMap.Nodos.GetLength(0); } }			//establecemos los valores de la clase que hereda 
	public int Height { get{ return gridMap.Nodos.GetLength(1); } }
	public Nodo GetValue(int x, int y)
	{
		return gridMap.Nodos[x, y];
	}

	[SerializeField] 
	private Grid gridMap;

	
	public VisibilityMap(Grid grid)		//establecemos como constructor solamente el grid
	{
		gridMap = grid;
	}



}
