#pragma warning disable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowingPredict : SeekAcceleration
{
    public Path path;           //el camino a seguir
    public int targetParam;
    public int currentPos;          //posicion actual
    public Vector3 futurePos;       //posicion futura basada en el predict
    public int currentParam;
    public float predictTime = 0.1f;
    public GameObject goPathFoll;
    void Start(){
        goPathFoll = new GameObject("PathFollowing");
        Agent invisible = goPathFoll.AddComponent<Agent>() as Agent;
        invisible.intRadius = path.Radio;
        invisible.extRadius = path.Radio + 0.5f;
        target = invisible;
        currentPos = 0;
    }
    

    public override Steering GetSteering(AgentNPC agent){
        //calculamos la posicion futura dada la pos del agente 
        futurePos = agent.transform.position+agent.Velocity*predictTime;
        //Actual posición en el camino dada la posiicon futura
        currentParam = path.GetParam(futurePos, currentPos);
        //Actualizamos la posición actual
        currentPos = currentParam;
        //Calculamos la posición del target en el camino.
        targetParam = currentParam + 1;
        //Calculamos la posición del keypoint target.
        base.target.transform.position = path.GetPosition(targetParam);
        return base.GetSteering(agent);
    }

}
