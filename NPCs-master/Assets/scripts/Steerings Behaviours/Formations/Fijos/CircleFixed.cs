#pragma warning disable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFixed : MonoBehaviour
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
    // Start is called before the first frame update
    private Wander w;
    void Start()
    {
            //incializamos el time
        timeRes=time;
        invisibles = new GameObject[tamañoGrid];
        puntoDestinoGO = new GameObject("punto destino");
        puntoDestinoGO.AddComponent<Agent>();

        int i = 0; 
        //vamos añadiendo a cada agente el GO al que haran arrive
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
        if (timeRes <=0.0f && agentes[0].llegar == false){  //si timeOut entonces añadimos Wander si no lo tiene, y si lo tiene, se le quita
            timeRes = time;
            Debug.Log("timeUp");
            if(agentes[0].SteeringList.Contains(w)){        //si tiene el wander, quitamos el mismo y añadimos arrive y face
                agentes[0].SteeringList.Add(agentes[0].GetComponent<Face>());
                agentes[0].GetComponent<Face>().aux = agentes[0].GetComponent<Wander>().target;
                agentes[0].GetComponent<Face>().target = agentes[0].GetComponent<Wander>().target;
                agentes[0].SteeringList.Remove(w);
                agentes[0].SteeringList.Add(agentes[0].GetComponent<ArriveAcceleration>());
                agentes[0].GetComponent<ArriveAcceleration>().target = agentes[0];
            } else {                                        //si no lo tiene, qitamos arrive y face, pero añadimos wander
                agentes[0].SteeringList.Add(w);
                Agent puntoDestinoInv = puntoDestinoGO.GetComponent<Agent>();
                puntoDestinoInv.transform.position = agentes[0].GetComponent<Wander>().target.transform.position;
                agentes[0].SteeringList.Remove(agentes[0].GetComponent<ArriveAcceleration>());
                agentes[0].SteeringList.Remove(agentes[0].GetComponent<Face>());
            }
        }

        if (agentes[0].llegar){     // si ha sido seleccionado y enviado a un lugar, entonces quitamos wander si lo tenia, y les indicamos la nueva localizacion
            timeRes=time;

            if(agentes[0].SteeringList.Contains(w)){
                agentes[0].SteeringList.Remove(w);
                agentes[0].SteeringList.Add(agentes[0].GetComponent<Face>());
            }
            agentes[0].GetComponent<Face>().aux = puntoDestinoGO.GetComponent<Agent>();
            agentes[0].GetComponent<Face>().target = puntoDestinoGO.GetComponent<Agent>();
            Agent puntoDestinoInv = puntoDestinoGO.GetComponent<Agent>();
            puntoDestinoInv.transform.position = agentes[0].GetComponent<ArriveAcceleration>().target.transform.position;
            agentes[0].llegar = false;
        }
        UpdateSlots();
    }
    public void UpdateSlots() {
        // para cada agente, vamos actualizadno su posicion y orientacion
        for (int i = 0; i < agentes.Count; i++) {
            Vector3 pos = GetPosition(i);
            GameObject invisibleGOActual = invisibles[i];
            Agent invisibleActual = invisibleGOActual.GetComponent<Agent>();
            invisibleActual.transform.position =pos;
            if (i != 0){
                agentes[i].GetComponent<ArriveAcceleration>().target = invisibleActual;
                agentes[i].GetComponent<Face>().aux = invisibleActual;
                agentes[i].GetComponent<Face>().target = invisibleActual;
            }
            
        }
    }
    // calcula la posicion
    public Vector3 GetPosition(int numero) {
        Vector3 agenteActual = grid[numero];
        AgentNPC lider = agentes[0];
        float distancia = 4;
        //establecemos la matriz de rotacion para poder hacer que cuando gire el lider, el resto gire en consecuencia
        float[] matrizRotacion = new float[4]{-Mathf.Cos(lider.orientation), Mathf.Sin(lider.orientation),
                                            -Mathf.Sin(lider.orientation), -Mathf.Cos(lider.orientation)};
        
        Vector3 pm = productoMatricial(matrizRotacion, grid[numero]);
        Vector3 resultado = grid[0] + pm;       //se lo sumamos al del lider para que cada uno tenga su posicion
        resultado = lider.transform.position + resultado * distancia;
        return resultado;
    }

    public Vector3 productoMatricial(float[] x,Vector3 posicionGrid ){          //calculamos el producto matricial para sacar la posicion que le corresponde
        Vector3 resultado = new Vector3(x[0] * posicionGrid.x + x[2] *posicionGrid.z,0, x[1] * posicionGrid.x + x[3]*posicionGrid.z);
        return resultado;
    }
}
