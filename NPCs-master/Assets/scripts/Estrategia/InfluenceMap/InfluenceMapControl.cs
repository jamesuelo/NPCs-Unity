using UnityEngine;
using System.Collections.Generic;

public class InfluenceMapControl : MonoBehaviour
{
	[SerializeField]
	private Transform bottomLeft;
	
	[SerializeField]
	private Transform upperRight;
	
	[SerializeField]
	private float gridSize = 1;
	
	[SerializeField]
	private float decay = 0.1f;
	
	[SerializeField]
	private float momentum = 1f;
	
	[SerializeField]
	private float updateFrequency = 10;
	
	InfluenceMap influenceMap;

	[SerializeField]
	private GridDisplay display;

	[SerializeField] 
	private Grid gridMap;

	[SerializeField]
	private List<SimplePropagator> propagators;

	public void CreateMap(int x, int z) {
		int width = x;
		int height = z;
		
		influenceMap = new InfluenceMap(width, height, decay, momentum, gridMap);
		
		display.SetGridData(influenceMap);
		display.CreateMesh(bottomLeft.position, gridSize);
	}

	public Vector2I GetGridPosition(Vector3 pos)
	{
		var cellPosition =gridMap.GetNodoPosicionGlobal(pos);
		return new Vector2I((int)cellPosition.Posicion.x, (int)cellPosition.Posicion.z);
	}

	
	public void Initialize(int x, int z) {
		CreateMap(x, z);
		
		foreach (var propagator in propagators) {
			influenceMap.RegisterPropagator(propagator);
		}
		
		InvokeRepeating(nameof(PropagationUpdate), 0.001f, 1.0f/updateFrequency);
	}

	public void PropagationUpdate()
	{
		influenceMap.Propagate();
	}

	public void SetInfluence(int x, int y, float value)
	{
		influenceMap.SetInfluence(x, y, value);
	}

	public void SetInfluence(Vector2I pos, float value)
	{
		influenceMap.SetInfluence(pos, value);
	}

	void Update()
	{
		influenceMap.Decay = decay;
		influenceMap.Momentum = momentum;
		
	}
}
