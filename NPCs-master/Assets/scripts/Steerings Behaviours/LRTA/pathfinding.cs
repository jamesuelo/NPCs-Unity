using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    Nodo nodoActual;
    Nodo nodoFinal;
    public int heuristica = 1;
    GameObject nodoEnd; //Objeto visual
    public LRTA lrta =  new LRTA();
    [SerializeField]
    public Grid grid;
    List<GameObject> camino;
    public NPC npc;
    public bool tactico;

    public float multiplicadorTerreno;
    public float multiplicadorInfluencia;
    public float multiplicadorVisibilidad;
    //public bool pintar;
    private void Start()
    {
        grid = grid.GetComponent<Grid>();
        lrta = new LRTA();
    }

    //Establece el objetivo de un agente del pathfinding
    public List<GameObject> EstablecerNodoFinal(AgentNPC agent)
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        List<Nodo> nodos = new List<Nodo>();
        List<GameObject> keyPoints = new List<GameObject>();
        this.nodoActual = grid.GetNodoPosicionGlobal(agent.transform.position);
        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            if (hit.transform != null && hit.transform.tag != "Muro")
            {
                if (nodoEnd != null) Destroy(nodoEnd);
                nodoEnd = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                nodoEnd.transform.localScale = new Vector3(grid.radioNodo * 2, grid.radioNodo * 2, grid.radioNodo * 2);
                nodoEnd.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y + 1, hit.transform.position.z);
                nodoEnd.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                nodoFinal = grid.GetNodoPosicionGlobal(nodoEnd.transform.position);
                if (agent.tag == "PathFinding")
                    nodos = lrta.EncontrarCaminoLRTAStar(nodoActual, nodoFinal, heuristica, grid);
                else if (agent.tag == "PathFindingAStar")
                    nodos = lrta.EncontrarCaminoAStar(nodoActual, nodoFinal, heuristica, grid, npc, tactico, multiplicadorTerreno, multiplicadorInfluencia, multiplicadorVisibilidad);
                List<Vector3> aux = new List<Vector3>(nodos.Count);
                for (int i = 0; i < nodos.Count; i++)
                {
                    GameObject keyPoint = new GameObject("Keypoint");
                    keyPoint.transform.position = nodos[i].Posicion;
                    keyPoints.Add(keyPoint);

                }
                return keyPoints;

            }
        }

        return null;
    }

    //Establece objetivo respecto al lider de una formacion
    public List<GameObject> nodoFinalFormaciones(AgentNPC agent, GameObject esferaDestino)
    {
        List<Nodo> nodos = new List<Nodo>();
        List<GameObject> keyPoints = new List<GameObject>();
        this.nodoActual = grid.GetNodoPosicionGlobal(agent.transform.position);
        Collider[] colliders = Physics.OverlapSphere(esferaDestino.transform.position, grid.radioNodo);
        bool muro = false;
        foreach (Collider c in colliders)
        {
            if (c.tag == "Muro" )
            {
                muro = true;
            }
        }
        if (esferaDestino != null && !muro)
        {
            nodoFinal = grid.GetNodoPosicionGlobal(esferaDestino.transform.position);
            if (agent.tag == "PathFinding"){
                if (nodoActual != null && nodoFinal != null)
                    nodos = lrta.EncontrarCaminoLRTAStar(nodoActual, nodoFinal, heuristica, grid);
            }
            else if (agent.tag == "PathFindingAStar")
                nodos = lrta.EncontrarCaminoAStar(nodoActual, nodoFinal, heuristica, grid, npc, tactico, multiplicadorTerreno, multiplicadorInfluencia, multiplicadorVisibilidad);
            if (nodos != null)
            {
                List<Vector3> aux = new List<Vector3>(nodos.Count);
                for (int i = 0; i < nodos.Count; i++)
                {
                    GameObject keyPoint = new GameObject("Keypoint");
                    keyPoint.transform.position = nodos[i].Posicion;
                    keyPoints.Add(keyPoint);

                }
                return keyPoints;
            }

        }
        return null;
    }

    public void EncontrarCaminoJuego(Vector3 posicionInicial, Vector3 posicionFinal)
    {
        Nodo actual = grid.GetNodoPosicionGlobal(posicionInicial);
        Nodo final = grid.GetNodoPosicionGlobal(posicionFinal);
        List<Nodo> nodos = lrta.EncontrarCaminoAStar(actual, final, heuristica, grid, npc, tactico, multiplicadorTerreno, multiplicadorInfluencia, multiplicadorVisibilidad);
        List<GameObject> keyPoints = new List<GameObject>();
        if (nodos != null)
        {
            List<Vector3> aux = new List<Vector3>(nodos.Count);
            for (int i = 0; i < nodos.Count; i++)
            {
                GameObject keyPoint = new GameObject("Keypoint");
                keyPoint.transform.position = nodos[i].Posicion;
                keyPoints.Add(keyPoint);
            }

        }
        Path path;
        PathFollowing pf;
        if (this.GetComponent<PathFollowing>() == null)
        {
            pf = this.gameObject.AddComponent<PathFollowing>();
            path = this.gameObject.AddComponent(typeof(Path)) as Path;
            pf.path = this.gameObject.GetComponent<Path>();
            path.Radio = this.grid.radioNodo;
            this.gameObject.GetComponent<AgentNPC>().SteeringList.Add(pf);
        }
        path = this.gameObject.GetComponent<Path>();
        path.ClearPath();
        pf = this.gameObject.GetComponent<PathFollowing>();
        pf.currentPos = 0;
        for (int i = 0; i < keyPoints.Count; i++)
        {
            path.nuevoNodo(keyPoints[i]);
        }
            //pintarCamino(keyPoints);
    }


    void pintarCamino(List<GameObject> listPuntos)      //podemos dibujar el camino que seguiras los agente pathfinding si activamos el booleano
    {
        if (listPuntos.Count != 0)
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
                aux.transform.localScale = new Vector3(1, this.grid.radioNodo, 1);
                aux.transform.position = v.transform.position;
                camino.Add(aux);
            }

        }
    }
}