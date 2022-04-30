using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : SteeringBehaviour {
    [SerializeField]
    private float umbral = 0f;          //distancia de seguridad

    public List<Agent> targets;         //lista de objetivos

    [SerializeField]
    private float coef = 1f;

    [SerializeField]
    private float fuerza;               //fuerza que se aplica
    private GameObject goSeparation;    
    void Start(){
        goSeparation = new GameObject("Separation");
        Agent invisible = goSeparation.AddComponent<Agent>() as Agent;
        target = invisible;
    }

    public override Steering GetSteering(AgentNPC agent) {
        Steering steering = new Steering();
        Vector3 direction;
        float distancia = 0f;
        foreach (Agent target in targets) {
            direction = agent.Position - target.Position; //calculamos la distacnia
            distancia = Mathf.Abs(direction.magnitude);

            if (distancia < umbral) {       //si es menor qeu el umbral establecido o distancia de seguridad, calculamos la fuerza que le vamos a aplicar para separarlos
                float f = coef/(distancia*distancia);
                fuerza = Mathf.Min(f, agent.maxAcceleration);
                steering.linear += direction.normalized * fuerza;
            }
        }
        return steering;
    }
}