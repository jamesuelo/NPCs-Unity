using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendedSteering : SteeringBehaviour
{

    public List<SteeringBehaviour> behaviours;      //lista de comportamientos del arbitro
    private Steering steer;

    void Start(){
        steer = this.gameObject.GetComponent<Steering>();       //cogemos los comportamientos
    }
    public override Steering GetSteering(AgentNPC agent) {
        Vector3 multAux = Vector3.zero;
        float multAng = 0;
        Steering m;
            //vamos acumulando linear y angular de todos los steerings para aplicarlos despues
        foreach (SteeringBehaviour s in behaviours) {
            m = s.GetSteering(agent);
            multAux += (s.weight * m.linear);
            multAng += (s.weight * m.angular);
        }
        steer.linear = multAux;
        steer.angular =  multAng;
        //cogemos el minimo para evitar pasarnos del limite establecido del propio agente
        float t= Mathf.Min(steer.linear.magnitude,agent.maxAcceleration);
        steer.linear = steer.linear * t;
        steer.angular = Mathf.Min(steer.angular, agent.maxAngularAcc);
        return steer;
    }
}
