using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : Align {

    public Agent aux;
    private GameObject goFace;
    void Start(){
        goFace = new GameObject("Face");
        Agent invisible = goFace.AddComponent<Agent>() as Agent;
        aux = target;
        invisible.transform.position = Vector3.zero;
        this.target = invisible;
    }
    public override Steering GetSteering(AgentNPC agent) {
        Steering steer= this.gameObject.GetComponent<Steering>();
        if (aux == null || target == null){
            return base.GetSteering(agent);
        }
        //Establecemos un steer que sera completamente sin resultados para devolverlo en caso de que la distancia sea 0 
        steer.linear = Vector3.zero;
        steer.angular = 0;
        target.transform.position = aux.transform.position;
        // Sacamos la direccion y la distancia
        float distancia = Mathf.Sqrt(Mathf.Pow((target.transform.position.x - agent.transform.position.x),2) + 
        0 +
        Mathf.Pow((target.transform.position.z - agent.transform.position.z),2));
        Vector3 direction = target.transform.position - agent.transform.position;

        // Si la distancia es 0 devolvemos lo que tenemos del steering "vacio"
        if (distancia == 0) {
            return steer;
        }
        //Establecemos como objetivo el agent que nos interesa y cambiamos la orientacion del agente
        target.Orientation = Mathf.Atan2(direction.x, direction.z);
        // Devolvemos el control al align
        return base.GetSteering(agent);
    }
}