using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : Align {
    [SerializeField]
    private float umbral = 0f;

    public List<Agent> targets;
    private GameObject goAlignment;
    
    void Start(){
        goAlignment = new GameObject("Alignment");
        Agent invisible = goAlignment.AddComponent<Agent>() as Agent;
        target = invisible;
    }
    public override Steering GetSteering(AgentNPC agent) {
        Vector3 direction;
        float heading = 0f;
        float distancia = 0f;
        int i = 0;

        foreach (Agent target in targets) {
            direction = agent.Position - target.Position;       //calculamos la distancia y direccion
            distancia = Mathf.Abs(direction.magnitude);

            if (distancia < umbral) {
                heading += target.Orientation;      //de la misma manera que cohesion, vamos sumando las orientacions para despues modificarlo como si fuese un centro de masas
                i++;
            }
        }
        if (i > 0) {
            heading /= i;
        }

        target.Orientation = heading;

        return base.GetSteering(agent);
    }
}