using UnityEngine;
using System.Collections.Generic;

public class visibilityMapControl : MonoBehaviour
{
	[SerializeField]
	private Transform esquinaDL;
	[SerializeField]
	private float gridSize = 1;
	
	VisibilityMap visibilityMap;

	[SerializeField]
	private GridDisplayV display;
	[SerializeField] 
	private Grid gridMap;


	public void CreateMap() {
		visibilityMap = new VisibilityMap(gridMap);
		display.SetGridData(visibilityMap);				//establecemos el grid para que el display pueda dibujar
		display.CreateMesh(esquinaDL.position, gridSize);			//creamos la malla a partir del mismo script
	}

	public void Initialize() {		//incializamos el mapa creandolo
		CreateMap();
		
	}

}
