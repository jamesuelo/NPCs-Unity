using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridDisplayV : MonoBehaviour
{
	MeshRenderer meshRenderer;
	MeshFilter meshFilter;
	Mesh mesh;

	GridData data;

	[SerializeField]
	Material material;
	
	[SerializeField]
	Color neutralColor = Color.black;
	[SerializeField]
	Color positiveColor = Color.red;
	[SerializeField]
	Color negativeColor = Color.blue;
	[SerializeField]
	Color negative2Color = Color.blue;
	[SerializeField]
	Color negative3Color = Color.blue;
	[SerializeField]
	Color negative4Color = Color.blue;
	
	Color[] colorsArray;

	public void SetGridData(GridData m)
	{
		data = m;
	}

	public void CreateMesh(Vector3 bottomLeftPos, float gridSize)
	{
		mesh = new Mesh();
		mesh.name = name;
		meshFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		meshFilter.mesh = mesh;
		meshRenderer.material = material;
		
		float objectHeight = transform.position.y;
		float staX = 0;
		float staZ = 0;
		
		List<Vector3> verts = new List<Vector3>();
		for (int y = 0; y < data.Height; ++y)
		{
			for (int x = 0; x < data.Width; ++x)
			{
				
				Vector3 bl = new Vector3(staX + (x * gridSize), objectHeight, staZ + (y * gridSize));
				Vector3 br = new Vector3(staX + ((x+1) * gridSize), objectHeight, staZ + (y * gridSize));
				Vector3 tl = new Vector3(staX + (x * gridSize), objectHeight, staZ + ((y+1) * gridSize));
				Vector3 tr = new Vector3(staX + ((x+1) * gridSize), objectHeight, staZ + ((y+1) * gridSize));
				
				
				verts.Add(bl);
				verts.Add(br);
				verts.Add(tl);
				verts.Add(tr);
			}
		}
		
		List<Color> colors = new List<Color>();
		for (int y = 0; y < data.Height; ++y)
		{
			for (int x = 0; x < data.Width; ++x)
			{
				colors.Add(Color.white);
				colors.Add(Color.white);
				colors.Add(Color.white);
				colors.Add(Color.white);
			}
		}
		colorsArray = colors.ToArray();
		
		List<Vector3> norms = new List<Vector3>();
		for (int y = 0; y < data.Height; ++y)
		{
			for (int x = 0; x < data.Width; ++x)
			{
				norms.Add(Vector3.up);
				norms.Add(Vector3.up);
				norms.Add(Vector3.up);
				norms.Add(Vector3.up);
			}
		}
		
		List<Vector2> uvs = new List<Vector2>();
		for (int y = 0; y < data.Height; ++y)
		{
			for (int x = 0; x < data.Width; ++x)
			{
				uvs.Add(new Vector2(0, 0));
				uvs.Add(new Vector2(1, 0));
				uvs.Add(new Vector2(0, 1));
				uvs.Add(new Vector2(1, 1));
			}
		}
		
		List<int> tris = new List<int>();
		for (int idx = 0; idx < verts.Count; idx+=4) {

			int bl = idx;
			int br = idx+1;
			int tl = idx+2;
			int tr = idx+3;

			
			tris.Add(bl);
			tris.Add(tl);
			tris.Add(br);
			
			tris.Add(tl);
			tris.Add(tr);
			tris.Add(br);
		}

		mesh.vertices = verts.ToArray();
		mesh.normals = norms.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.colors = colorsArray;
		mesh.triangles = tris.ToArray();
	}
	
	void SetColor(int x, int y, Color c)
	{
		int i = ((y * data.Width) + x) * 4;
		colorsArray[i] = c;
		colorsArray[i+1] = c;
		colorsArray[i+2] = c;
		colorsArray[i+3] = c;
	}

	void Update()
	{
	if(data == null){

	}
	else{
		for (int y = 0; y < data.Height; ++y)
		{
			for (int x = 0; x < data.Width; ++x)
			{
				Nodo nodo = data.GetValue(x, y);
				Color c = neutralColor;
                if(nodo.visibilidad == 0)				//totalmente visible
					c = Color.Lerp(negative4Color, negative4Color,0.5f);			
				else if (nodo.visibilidad > 0 && nodo.visibilidad <= 0.19)			//muy visible
                    c = Color.Lerp(negative3Color, negative3Color, 0.5f);		
				else if (nodo.visibilidad > 0.19 && nodo.visibilidad <= 0.39)		//bien visible
                    c = Color.Lerp(negative2Color, negative2Color,0.5f);			
				else if (nodo.visibilidad > 0.39 && nodo.visibilidad <= 0.59)		//mal visible
                    c = Color.Lerp(negativeColor, negativeColor,0.5f);		
				else if (nodo.visibilidad > 0.59 && nodo.visibilidad <= 0.79)		//muy mal visible
					c = Color.Lerp(positiveColor, positiveColor,0.5f);
				else
					c = Color.Lerp(neutralColor, neutralColor,0.5f);				//no se puede ver
				SetColor(x, y, c);
			}
		}
		
		mesh.colors = colorsArray;
		}
	}
}
