#pragma warning disable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFollowing : SeekAcceleration
{
    public GameObject goWF;
    public int predictTime = 1;
    private Vector3 futurePos;
    public float distancia = 3;
    [SerializeField]
    private List<GameObject> walls;
    private List<Collider> col;

    void Start(){
        goWF = new GameObject("WallFollowing");
        target = goWF.AddComponent<Agent>() as Agent;

        col = new List<Collider>();
        foreach (GameObject b in walls)
        {
            Collider c = b.GetComponent<Collider>();
            if (c!=null)
                col.Add(c);
        }
    }

    public override Steering GetSteering(AgentNPC agent){
        //calculamos la posicion futura con el time de prediccion
        futurePos = agent.transform.position+agent.Velocity*predictTime;
        //establecemos los puntos mas cercanos y la distancia
        GameObject pared = null;
        float puntoMasCercano = 99999;
        Vector3 closestPoint = Vector3.zero;
        Vector3 closestPointActualWall = Vector3.zero;
        Vector3 distance = Vector3.zero;
        //comprobamos cual es el punto mas cercano segun el wall que tengamos en la lista de colliders
        for (int i = 0; i< col.Count; i++){
            closestPoint = col[i].ClosestPoint(futurePos);
            distance = futurePos - closestPoint;
            if(distance.magnitude < puntoMasCercano){
                puntoMasCercano = distance.magnitude;
                closestPointActualWall = closestPoint;
                pared = walls[i];
                
            }
        }
        //despues de proyectar dicho punto, sacamos la normal
        Vector3 normale = Vector3.zero;
        Vector3 dir = pared.transform.position - futurePos;
        RaycastHit hit;
        if (Physics.Raycast(futurePos, dir, out hit))
        {

            normale = hit.normal;
        }
        //establecemos la distancia exacta
        normale = closestPointActualWall + normale*distancia;
        normale.y = 0;
        target.transform.position = normale;
        return base.GetSteering(agent);
    }
}
