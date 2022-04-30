#pragma warning disable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPathfinding : MonoBehaviour
{
    [SerializeField]
    private float time = 10;
    private float timeRes;
    //Tamaño del grid fijo
    private int tamañoGrid = 9;
    //Grid de posiciones relativas de los agentes.
    [SerializeField]
    private Vector3[] grid;
    private Path[] pathsAgentes;

    private GameObject[] esferasAgentes;
    //Agentes invisibles posicionados en el grid.
    private GameObject[] invisibles;
    //Punto de movimiento.
    private GameObject puntoDestinoGO;
    //Agentes que forman parte del grid
    public List<AgentNPC> agentes;
    // Start is called before the first frame update
    private Wander w;
    public List<GameObject> listPuntos = new List<GameObject>();    //variables para pathfinding
    public List<GameObject> camino;

    public GameObject nodoEnd;
    void Start()
    {
        nodoEnd = new GameObject("Esfera");
        timeRes = time;
        pathsAgentes = new Path[tamañoGrid];
        invisibles = new GameObject[tamañoGrid];
        esferasAgentes = new GameObject[tamañoGrid];
        puntoDestinoGO = new GameObject("punto destino");
        puntoDestinoGO.AddComponent<Agent>();

        int i = 0;
        foreach (AgentNPC ag in agentes)
        {
            if (i == 0)
            {
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
       /* if (timeRes <= 0.0f && agentes[0].llegar == false)
        {
            timeRes = time;
            Debug.Log("timeUp");
            if (agentes[0].SteeringList.Contains(w))
            {
                agentes[0].SteeringList.Add(agentes[0].GetComponent<Face>());
                agentes[0].GetComponent<Face>().aux = agentes[0].GetComponent<Wander>().target;
                agentes[0].GetComponent<Face>().target = agentes[0].GetComponent<Wander>().target;
                agentes[0].SteeringList.Remove(w);
            }
            else
            {
                agentes[0].SteeringList.Add(w);
                Agent puntoDestinoInv = puntoDestinoGO.GetComponent<Agent>();
                puntoDestinoInv.transform.position = agentes[0].GetComponent<Wander>().target.transform.position;
                agentes[0].SteeringList.Remove(agentes[0].GetComponent<Face>());
            }
        }*/

        if (agentes[0].llegar)
        {
            timeRes = time;

            if (agentes[0].SteeringList.Contains(w))
            {
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
    public void UpdateSlots()
    {
        for (int k = 0; k < agentes.Count; k++)
        {
            if (esferasAgentes[k] != null)
            {
                DestroyImmediate(esferasAgentes[k]);
            }
        }
        for (int i = 0; i < agentes.Count; i++)
        {
            Vector3 pos = GetPosition(i);
            GameObject invisibleGOActual = invisibles[i];
            Agent invisibleActual = invisibleGOActual.GetComponent<Agent>();
            invisibleActual.transform.position = pos;
            if (i != 0)
            {
                //Creamos el punto destino de nuevo
                nodoEnd = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                nodoEnd.transform.localScale = new Vector3(4, 4, 4);
                nodoEnd.transform.position = new Vector3(invisibleActual.transform.position.x, 1, invisibleActual.transform.position.z);
                nodoEnd.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                //Reseteamos la esfera roja de cada npc
                esferasAgentes[i] = nodoEnd;
                //Creamos el camino final hasta el nodo invisible
                listPuntos = agentes[i].GetComponent<PathFinding>().nodoFinalFormaciones(agentes[i], esferasAgentes[i]);
                pathsAgentes[i] = agentes[i].GetComponent<Path>();
                pathsAgentes[i].ClearPath();
                //Creamos el nuevo camino para el agente si existe camino
                if (listPuntos != null)
                {
                    for (int j = 0; j < listPuntos.Count; j++)
                    {
                        pathsAgentes[i].nuevoNodo(listPuntos[j]);
                    }
                }
                pintarCamino();
                agentes[i].GetComponent<Face>().aux = invisibleActual;
                agentes[i].GetComponent<Face>().target = invisibleActual;
            }

        }
    }
    // calcula la posicion
    public Vector3 GetPosition(int numero)
    {
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

    public Vector3 productoMatricial(float[] x, Vector3 posicionGrid)
    {
        Vector3 resultado = new Vector3(x[0] * posicionGrid.x + x[2] * posicionGrid.z, 0, x[1] * posicionGrid.x + x[3] * posicionGrid.z);
        return resultado;
    }
    void pintarCamino()
    {
        if (listPuntos != null && listPuntos.Count != 0)
        {
            if (camino != null)
            {
                foreach (GameObject g in camino)
                {
                    Destroy(g);
                }
            }

            camino = new List<GameObject>();
            GameObject aux;
            foreach (GameObject v in listPuntos)
            {
                aux = GameObject.CreatePrimitive(PrimitiveType.Cube);
                aux.transform.localScale = new Vector3(1, 2, 1);
                aux.transform.position = v.transform.position;
                camino.Add(aux);
            }

        }
    }
}
