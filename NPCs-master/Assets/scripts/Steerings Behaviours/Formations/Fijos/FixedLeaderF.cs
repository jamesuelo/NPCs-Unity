#pragma warning disable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedLeaderF : MonoBehaviour
{
    [SerializeField]
    private float time = 10;
    private float timeRes;
    //Tamaño del grid fijo
    private int tamañoGrid = 9;
    //Grid de posiciones relativas de los agentes.
    [SerializeField]
    private Vector3[] grid;
    //Agentes invisibles posicionados en el grid.
    private GameObject[] invisibles;
    //Punto de movimiento.
    private GameObject puntoDestinoGO;
    //Agentes que forman parte del grid
    public List<AgentNPC> agentes;
    private Wander w;
    void Start()
    {
        timeRes=time;
        invisibles = new GameObject[tamañoGrid];
        puntoDestinoGO = new GameObject("punto destino");
        puntoDestinoGO.AddComponent<Agent>();

        int i = 0;
        foreach (AgentNPC ag in agentes)
        {
            if(i == 0){
                w = ag.GetComponent<Wander>();
                ag.SteeringList.Remove(w);
            }
            GameObject invisibleGO = new GameObject("FC " + agentes.Count);
            Agent invisible = invisibleGO.AddComponent<Agent>() as Agent;
            invisibles[i] = invisibleGO;
            invisible.extRadius = 1f;
            invisible.intRadius = 1f;
            ag.form = true;
            i++;

        }
        UpdateSlots();
    }

    // Update is called once per frame
    void Update()
    {
        timeRes -= Time.deltaTime;
        if (timeRes <=0.0f && agentes[0].llegar == false){
            timeRes = time;
            Debug.Log("timeUp");
            if(agentes[0].SteeringList.Contains(w)){
                agentes[0].SteeringList.Add(agentes[0].GetComponent<Face>());
                agentes[0].GetComponent<Face>().aux = agentes[0].GetComponent<Wander>().target;
                agentes[0].GetComponent<Face>().target = agentes[0].GetComponent<Wander>().target;
                agentes[0].SteeringList.Remove(w);
                agentes[0].SteeringList.Add(agentes[0].GetComponent<ArriveAcceleration>());
                agentes[0].GetComponent<ArriveAcceleration>().target = agentes[0];
            } else {
                agentes[0].SteeringList.Add(w);
                Agent puntoDestinoInv = puntoDestinoGO.GetComponent<Agent>();
                puntoDestinoInv.transform.position = agentes[0].GetComponent<Wander>().target.transform.position;
                agentes[0].SteeringList.Remove(agentes[0].GetComponent<ArriveAcceleration>());
                agentes[0].SteeringList.Remove(agentes[0].GetComponent<Face>());
            }
        }

        if (agentes[0].llegar){
            timeRes=time;

            if(agentes[0].SteeringList.Contains(w)){
                agentes[0].SteeringList.Remove(w);
                agentes[0].SteeringList.Add(agentes[0].GetComponent<Face>());
            }
            agentes[0].GetComponent<Face>().aux = puntoDestinoGO.GetComponent<Agent>();
            agentes[0].GetComponent<Face>().target = puntoDestinoGO.GetComponent<Agent>();
            Agent puntoDestinoInv = puntoDestinoGO.GetComponent<Agent>();
            puntoDestinoInv.transform.position = agentes[0].GetComponent<ArriveAcceleration>().target.transform.position;
            float diffX = Mathf.Abs(Mathf.Abs(agentes[0].transform.position.x) - Mathf.Abs(puntoDestinoInv.transform.position.x));
            float diffZ = Mathf.Abs(Mathf.Abs(agentes[0].transform.position.z) - Mathf.Abs(puntoDestinoInv.transform.position.z));
            if(diffX < agentes[0].intRadius && diffZ < agentes[0].intRadius) {
                agentes[0].llegar = false;
            }
        }
        UpdateSlots();
    }
    public void UpdateSlots() {
        
        for (int i = 0; i < agentes.Count; i++) {
            Vector3 pos = GetPosition(i);
            GameObject invisibleGOActual = invisibles[i];
            Agent invisibleActual = invisibleGOActual.GetComponent<Agent>();
            invisibleActual.transform.position =pos;
            if (i != 0 && !agentes[0].llegar){
                agentes[i].GetComponent<ArriveAcceleration>().target = invisibleActual;
                agentes[i].GetComponent<Face>().aux = invisibleActual;
                agentes[i].GetComponent<Face>().target = invisibleActual;
            }else if ( i !=0 && agentes[0].llegar){     //la unica diferencia con respecto a las formaciones fijas es que cuando el lider se mueve, rompemos las formaciones para que vayan al lugar establecido por el lider y desues volver a formar
                agentes[i].GetComponent<ArriveAcceleration>().target = agentes[0];
                agentes[i].GetComponent<Face>().aux = agentes[0];
                agentes[i].GetComponent<Face>().target = agentes[0];
            }

            
        }
    }
    // calcula la posicion
    public Vector3 GetPosition(int numero) {
        Vector3 agenteActual = grid[numero];
        AgentNPC lider = agentes[0];
        float distancia = 4;

        float[] matrizRotacion = new float[4]{-Mathf.Cos(lider.orientation), Mathf.Sin(lider.orientation),
                                            -Mathf.Sin(lider.orientation), -Mathf.Cos(lider.orientation)};
        
        Vector3 pm = productoMatricial(matrizRotacion, grid[numero]);
        Vector3 resultado = grid[0] + pm;
        resultado = lider.transform.position + resultado * distancia;
        return resultado;
    }

    public Vector3 productoMatricial(float[] x,Vector3 posicionGrid ){
        Vector3 resultado = new Vector3(x[0] * posicionGrid.x + x[2] *posicionGrid.z,0, x[1] * posicionGrid.x + x[3]*posicionGrid.z);
        return resultado;
    }
}