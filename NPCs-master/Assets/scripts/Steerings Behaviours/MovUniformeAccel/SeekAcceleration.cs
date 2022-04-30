#pragma warning disable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekAcceleration : SteeringBehaviour
{
    Steering steer = new Steering();
    override public Steering GetSteering(AgentNPC agent)
    {
        //establecer a valores iniciales el steering que se debe retornar,
        
        steer.angular = 0;
        if (target == null){
            steer.linear = Vector3.zero;
            return steer;
        }
        //calculamos la distancia entre objetivo y el agente player (de un punto a otro)
        float distancia = Mathf.Sqrt(Mathf.Pow((target.transform.position.x - this.transform.position.x),2) + 
        0 +
        Mathf.Pow((target.transform.position.z - this.transform.position.z),2));

        //Calculamos la direccion del agente al target
        steer.linear = agent.directionToTarget(target.transform.position) ;
        //Si la distancia es menor que el radio interior, entonces ponemos la velocidad en sentido contrario para contrarrestar el mov uniforme
        if(distancia < target.intRadius){
            steer.linear = Vector3.zero;
            agent.Velocity = Vector3.zero;
        }
        
        steer.linear.Normalize();
        steer.linear *=agent.maxAcceleration;
        return steer;

       

    }
}
