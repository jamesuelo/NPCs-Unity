using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{


    public Agent target;                //objetivo del comportamiento
    public float weight = 1f;           //peso del comportamiento para los arbitro

    public abstract Steering GetSteering(AgentNPC agent);
}
