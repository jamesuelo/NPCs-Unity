using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//prueba de offset para formacion(Posiblemente no se use)
public class OffsetPursuit : ArriveAcceleration
{

    public float maxPredict;

    public Agent aux;
    private GameObject goOffsetPursuit;
    void Start(){
        goOffsetPursuit = new GameObject("OffsetPursuit");
        Agent invisible = goOffsetPursuit.AddComponent<Agent>() as Agent;
        aux = target;
        target = invisible;
    }
    public override Steering GetSteering(AgentNPC agent) {
        // Calculamos la distancia y la direccion hacia el objetivo
        
        float distancia = Mathf.Sqrt(Mathf.Pow(aux.transform.position.x - agent.transform.position.x,2) + 
        0 +
        Mathf.Pow(aux.transform.position.z - agent.transform.position.z,2));
        // Obtenemos la velocidad que lleva
        float speed = agent.Velocity.magnitude;
        
        // Comprobamos la velocidad en funcion de la prediccion que hemos hecho
        
        float prediction;
        
        if (speed <= (distancia / maxPredict)) {
            prediction = maxPredict;
        }
         // Calculamos la predcicion si falla
        else {
            prediction = distancia / speed;
        }
        
        return base.GetSteering(agent);
        
    }
}
