using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : SeekAcceleration {

    [SerializeField]
    private float umbral = 0f;

    public List<Agent> targets;
    private GameObject goCohesion;
    void Start(){
        goCohesion = new GameObject("Cohesion");
        Agent invisible = goCohesion.AddComponent<Agent>() as Agent;
        target = invisible;
    }
    public override Steering GetSteering(AgentNPC agent) {
        Vector3 direction;
        Vector3 centro = Vector3.zero;
        float distancia = 0f;
        int i = 0;

        //vamos calculando el centro de masas en funcion de lo cerca que estan los personajes unos de otros y segun se vaya moviendo
        foreach (Agent target in targets) {
            direction = agent.Position - target.Position;           //calculamos su direccion y distancia
            distancia = Mathf.Abs(direction.magnitude);

            if (distancia < umbral) {               //si es menor que la distancia de seguridad, se le añade al centro de masas para despues modificarlo
                centro += target.Position;
                i++;
            }
        }
        if (i == 0) {
            Steering steering = new Steering();
            return steering;
        }
        centro /= i;
        target.transform.position = centro;
        return base.GetSteering(agent);
    }
}