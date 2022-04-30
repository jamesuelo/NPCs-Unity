using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// takes care of creating the mesh that will display the influence map

public interface GridData
{
	int Width { get; }
	int Height { get; }
	Nodo GetValue(int x, int y);
}

public class GridDisplay : MonoBehaviour
{
	MeshRenderer meshRenderer;
	MeshFilter meshFilter;
	Mesh mesh;

	GridData data;

	[SerializeField]
	Material material;
	
	[SerializeField]
	Color neutralColor = Color.white;
	
	[SerializeField]
	Color positiveColor = Color.red;
	
	[SerializeField]
	Color positive2Color = Color.red;
	
	[SerializeField]
	Color negativeColor = Color.blue;
	
	[SerializeField]
	Color negative2Color = Color.blue;
	
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
		
		// create squares starting at bottomLeftPos
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
		for (int x = 0; x < verts.Count; x+=4) {

			int bl = x;
			int br = x+1;
			int tl = x+2;
			int tr = x+3;

			
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
		int ix = ((y * data.Width) + x) * 4;
		colorsArray[ix] = c;
		colorsArray[ix+1] = c;
		colorsArray[ix+2] = c;
		colorsArray[ix+3] = c;
	}

	void Update()
	{
		for (int y = 0; y < data.Height; ++y)
		{
			for (int x = 0; x < data.Width; ++x)
			{
				Nodo nodo = data.GetValue(x, y);
				Color c = neutralColor;
				if (nodo.influence < -0.5f)
					c = Color.Lerp(negativeColor, negative2Color, -(nodo.influence+0.5f)/0.5f);
				else if (nodo.influence < 0)
					c = Color.Lerp(neutralColor, negativeColor, -nodo.influence/0.5f);
				else if (nodo.influence > 0.5f)
					c = Color.Lerp(positiveColor, positive2Color, (nodo.influence-0.5f)/0.5f);
				else 
					c = Color.Lerp(neutralColor, positiveColor, nodo.influence/0.5f);
				
				SetColor(x, y, c);
			}
		}
		
		mesh.colors = colorsArray;
	}
}
