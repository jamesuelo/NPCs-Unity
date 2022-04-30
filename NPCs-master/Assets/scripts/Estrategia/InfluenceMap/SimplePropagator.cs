using UnityEngine;
using System.Collections;

public interface IPropagator
{
	Vector2I GridPosition { get; }
	float Value { get; }
}

public class SimplePropagator : MonoBehaviour, IPropagator
{
	[SerializeField]
	float valor;			//valor del propagador
	public float Value {
		get => valor;
		set => valor = value;
	}

	[SerializeField]
	InfluenceMapControl map;

	public Vector2I GridPosition => map.GetGridPosition(transform.position);
	
}
