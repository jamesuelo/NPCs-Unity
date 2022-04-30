using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : FleeAcceleration
{

    public float maxPredict;
    public Agent aux;
    private GameObject goPursue;
    void Start(){
        goPursue = new GameObject("Pursue");
        Agent invisible = goPursue.AddComponent<Agent>() as Agent;
        aux = target;
        this.target = invisible;
        target.intRadius = aux.intRadius;
        target.extRadius = aux.extRadius;
    }
    public override Steering GetSteering(AgentNPC agent) {
        // Calculamos la distancia y la direccion hacia el objetivo
        Vector3 direction = aux.transform.position - agent.transform.position;
        float distancia = Mathf.Sqrt(Mathf.Pow(aux.transform.position.x - agent.transform.position.x,2) + 
        0 +
        Mathf.Pow(aux.transform.position.z - agent.transform.position.z,2));
        
        // Obtenemos la velocidad que lleva
        float speed = agent.Velocity.magnitude;
        
        // Comprobamos la velocidad en funcion de la prediccion que hemos hecho sobre su velocidad/posicion
        
        float prediction;
        
        if (speed <= (distancia / maxPredict)) {
            prediction = maxPredict;
        }
         // Calculamos la predcicion si falla
        else {
            prediction = distancia / speed;
        }
        
        // Calculamos la posicion del target
        target.transform.position = aux.transform.position;
        target.transform.position += aux.Velocity * prediction;
        
        return base.GetSteering(agent);
        
    }
}
