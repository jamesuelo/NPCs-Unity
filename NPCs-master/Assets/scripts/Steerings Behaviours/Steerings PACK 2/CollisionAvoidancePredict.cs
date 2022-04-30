#pragma warning disable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAvoidancePredict : SeekAcceleration
{
    [SerializeField]
    public float Radius = 0.5f;
    [SerializeField]
    private List<Agent> targets;

    public void Start(){

        GameObject[] gs =  GameObject.FindGameObjectsWithTag("NPC");        //buscamos los diversos gameObjects que seran los que intentemos evitar
        foreach (GameObject g in gs)
        {
            targets.Add(g.GetComponent<Agent>());
        }

    }


    public override Steering GetSteering(AgentNPC agent) {
        Steering steering = this.gameObject.GetComponent<Steering>();
        float shortestTime = Mathf.Infinity;

        //establecemos el target final, velocidad, distancia y separacion iniciales
        Agent firstTarget = null;
        float firstMinSeparation = 0;
        Vector3 firstRelativePos = Vector3.zero;
        float firstDistance = 0;
        Vector3 firstRelativeVel = Vector3.zero;

        float relativeSpeed = 0;
        float timeToCollision = 0;
        //vamos comprobando para cada agente quien es el target mas cercano con las propiedades inciadas de antes
        foreach (Agent a in targets)
        {
            Vector3 relativePos = a.transform.position - agent.transform.position;
            Vector3 relativeVel = a.Velocity - agent.Velocity;
            relativeSpeed = relativeVel.magnitude;
            timeToCollision = Vector3.Dot(relativePos,relativeVel);
            timeToCollision /= (relativeSpeed * relativeSpeed * -1);

            float distancia = relativePos.magnitude;
            float minSeparation = distancia - relativeSpeed * timeToCollision;
            if (minSeparation > 2* Radius){     //calculamos la distancia de seguridad y si se ve comprometida sacamos los datos del agnete que colisionara 
                continue;
            }
            if(timeToCollision > 0 && timeToCollision< shortestTime){
                shortestTime = timeToCollision;
                firstTarget = a;
                firstMinSeparation = minSeparation;
                firstDistance = distancia;
                firstRelativePos = relativePos;
                firstRelativeVel = relativeVel;
            }

        }
        //si no encontramos a nadie que este cerca, seguimos moviendonos igual
        if(firstTarget == null)
            return steering;
        //si alguien va a colisionar con nosotros, redirigimos el personajes en consecuencia para poder llegar al punto evitando a los personajes
        if (firstMinSeparation <= 0 || firstDistance < 2*Radius){
            firstRelativePos = firstTarget.transform.position;
        } else {
            firstRelativePos = firstRelativePos + firstRelativeVel * shortestTime;
        }

        firstRelativePos.Normalize();
        steering.linear = -firstRelativePos * agent.maxAcceleration;
        steering.linear.y =0;
        return steering;
    }

}
