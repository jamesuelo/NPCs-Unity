using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrioritySteeringAng : SteeringBehaviour
{
    public List<BlendedSteering> groups;        //lista de steerings

    public float epsilon;                       //umbral arbitro

    public override Steering GetSteering(AgentNPC character)
    {
        Steering steering = this.gameObject.GetComponent<Steering>(); 
        foreach (BlendedSteering group in groups)
        {
            steering = group.GetSteering(character);
            if(Mathf.Abs(steering.angular) > epsilon)   //si el angular cumple el umbral realiza el primero steering en si y despues realizara el resto con menos prioridad
            {
                return steering;
            }
        }

        return steering; 
    }
}
