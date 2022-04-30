using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpose : ArriveAcceleration
{
    public Agent a;
    public Agent b;
    public GameObject go;

    // Start is called before the first frame update
    void Start()
    {
        go = new GameObject("Interpose");
        target = go.AddComponent<Agent>() as Agent;
    }


    public override Steering GetSteering(AgentNPC agent){
        //calculamos el punto medio entre A y B
        Vector3 middlePoint = (a.transform.position + b.transform.position)/2;
        //calculamos el tiempo
        float time = ((agent.transform.position - middlePoint).magnitude)/ agent.maxSpeed;
        //calculamos los vectores A y B de prediccion segun el tiempo para ver donde se vana colorcar los objetivos
        Vector3 predictA = a.transform.position + a.Velocity * time;
        Vector3 predictB = b.transform.position + b.Velocity * time;

        middlePoint = (predictA + predictB)/2;
        //la posicion del target invisible sera entre los dos Vectores de posicion calculados
        target.transform.position = middlePoint;
        return base.GetSteering(agent);
    }
}