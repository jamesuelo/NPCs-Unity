using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMatching : SteeringBehaviour
{
    public float TimeToTarget = 1f;
    override public Steering GetSteering(AgentNPC agent)
    {
        //establecer a valores iniciales el steering que se debe retornar,
        Steering steer = this.gameObject.GetComponent<Steering>();
        steer.angular = 0;
        if (target == null){
            steer.linear = Vector3.zero;
            return steer;
        }
        //calculamos la distancia entre objetivo y el agente player (de un punto a otro)
        float distancia = Mathf.Sqrt(Mathf.Pow((target.transform.position.x - this.transform.position.x),2) + 
        0 +
        Mathf.Pow((target.transform.position.z - this.transform.position.z),2));

            //establecemos el steer.linear en la misma direccion y velocidad que nuestro target

        steer.linear = target.velocity-agent.velocity;
        steer.linear /= TimeToTarget;

        if (distancia > agent.maxAcceleration)  //si la distancia es mayor que la aceleracion maxima, entonces lo multiplicamos por la misma.
        {
            steer.linear.Normalize();
            steer.linear *= agent.maxAcceleration;
        }
            

        return steer;
    }
}