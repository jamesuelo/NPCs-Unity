using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveAcceleration : SteeringBehaviour
{
    public float TimeToTarget = 0.1f;
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
        Vector3 direction = target.directionToTarget(agent.transform.position);
        Vector3 targetVelocity;
        float distancia = Mathf.Sqrt(Mathf.Pow((target.transform.position.x - this.transform.position.x),2) + 
        0 +
        Mathf.Pow((target.transform.position.z - this.transform.position.z),2));
        //Si la distancia es mayor que el radio interior del target establece la
        //magnitud vectorial del steering como el vector cuya magnitud es la
        //velocidad máxima del agente y cuya dirección va del agente hacia el
        //target

        float targetSpeed;
        if(distancia > agent.intRadius){
               
            steer.linear = -agent.velocity;
            if (distancia > 0) {
                steer.linear.Normalize();
                steer.linear *= agent.maxAcceleration;
            }
            return steer;
        }

        if(distancia <= agent.extRadius)        //si la distancia es mayor que el radio exterior del objetivo
        {           
            
            targetSpeed = agent.maxSpeed;       //velocidad maxima
        }
        else { 
            
            targetSpeed = agent.maxSpeed* distancia/ agent.extRadius;   //reducimos la velocidad si esta dentro 
        }
        targetVelocity = direction;
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        steer.linear = targetVelocity-agent.velocity;  //acelearcion intenta llegar a la velocidad del target
        steer.linear /= TimeToTarget;

        if (steer.linear.magnitude > agent.maxAcceleration)  //si esta lejos todavia sigues haciendo "arrive"
        {   
            steer.linear.Normalize();
            steer.linear *= agent.maxAcceleration;
        }

    return steer;       //devolver el steering
    }

}