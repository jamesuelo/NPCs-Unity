using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : Face
{
    
    public float wanderOffset;
    public float wanderRadius;
    public float wanderRate;
    [SerializeField]
    private float wanderOrientation;
    private GameObject go;
    void Start(){
        go = new GameObject("Wander");
        Agent invisible = go.AddComponent<Agent>() as Agent;
        aux = target;
        target = invisible;
    }
    private Vector3 AsVector(float o) {
        return new Vector3(Mathf.Cos(o), 0, Mathf.Sin(o));
    }
    private float RandomBinomial() {
        //usamos la funcion para generar el el angulo del face
        return Random.Range(0.0f, 1.0f) - Random.Range(0.0f, 1.0f);
    }
    override public Steering GetSteering(AgentNPC agent)
    {
        aux = target;
        //Calcular el ángulo random para hacer face
        wanderOrientation += wanderRate * RandomBinomial(); 
        //Orientación futura del agente.
        target.Orientation = wanderOrientation + agent.Orientation;
        //Colocar target invisible en posición adelantada del agente.
        Vector3 targetPosition = agent.transform.position + wanderOffset * AsVector(agent.Orientation); 
        //convertimos la orientacion en un vector y multiplicamos por el radiop
        targetPosition += wanderRadius * AsVector(target.Orientation);
        target.transform.position = targetPosition;
        Steering steer;
        steer = base.GetSteering(agent);
        //Acelerar al agente
        steer.linear = target.transform.position - agent.transform.position;
        steer.linear.Normalize();
        steer.linear *= agent.maxAcceleration;
        return steer;
    }
}