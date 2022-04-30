using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public Vector2 tamGrid; //Tamaño del grid (unidades reales)
    public float radioNodo;

    public Nodo[,] Nodos; //Nodos
    Transform[,] mapa;
    public GameObject cubos;
    public int mapaFila;
    public int mapaColumna;
    private Transform fila;
    private Transform columna;

    float diametroNodo;
    int tamGridX, tamGridY; //Tamaño del grid en relacion a los nodos

    public InfluenceMapControl mapaInfluencia;      //mapa de influencias del grid
    public visibilityMapControl mapaVisibilidad;       //mapa de visibilidad del grid
    public Transform abajoIzq;                      //sacamos los puntos necesarios para determinar las diemnsiones del grid
    public Transform arribaDcha;
    private void Awake()
    {
        mapa = new Transform[mapaFila, mapaColumna];
        for (int i = 0; i < mapaFila; i++)
        {
            fila = cubos.transform.GetChild(i);
            for (int y = 0; y < mapaColumna; y++)
            {
                columna = fila.transform.GetChild(y);
                mapa[y, i] = columna;
            }
        }
    }


    private void Start()
    {
        diametroNodo = radioNodo * 2;
        tamGridX = Mathf.RoundToInt(tamGrid.x / diametroNodo);
        tamGridY = Mathf.RoundToInt(tamGrid.y / diametroNodo);
        CrearGrid();
        if (mapaInfluencia != null)
        {
            float minX, maxX, minZ, maxZ;

            // x
            if (abajoIzq.position.x < arribaDcha.position.x)
            {
                minX = abajoIzq.position.x;
                maxX = arribaDcha.position.x;
            }
            else
            {
                maxX = abajoIzq.position.x;
                minX = arribaDcha.position.x;
            }

            // z
            if (abajoIzq.position.z < arribaDcha.position.z)
            {
                minZ = abajoIzq.position.z;
                maxZ = arribaDcha.position.z;
            }
            else
            {
                maxZ = abajoIzq.position.z;
                minZ = arribaDcha.position.z;
            }

            int x = Mathf.RoundToInt((maxX - minX) / radioNodo * 2) + 1;
            int z = Mathf.RoundToInt((maxZ - minZ) / radioNodo * 2) + 1;
            mapaInfluencia.Initialize(x, z);
            mapaVisibilidad.Initialize();
        }
    }

    //Inicializa el grid
    void CrearGrid()
    {
        int[,] matrizCostes = ObtenerMatrizCostes();
        Nodos = new Nodo[tamGridX, tamGridY];
        Vector3 esquina = transform.position - Vector3.right * tamGrid.x / 2 - Vector3.forward * tamGrid.y / 2;
        for (int x = 0; x < tamGridX; x++)
        {
            for (int y = 0; y < tamGridY; y++)
            {
                Vector3 pos = esquina + Vector3.right * (x * diametroNodo + radioNodo) + Vector3.forward * (y * diametroNodo + radioNodo);
                bool walkable = !isObject(pos);
                Nodo.TerrainType terreno = getTerrenoPosition(pos);
                Nodos[x, y] = new Nodo(walkable, pos, x, y,terreno);  //Creamos un nuevo nodo con las caracteristicas establecidas y calculadas a partir de lo que conocemos
                Nodos[x, y].igCost = (float) matrizCostes[x, y];
                Nodos[x, y].calculoVisibilidad();

            }
        }
    }

    //Obtiene los nodos vecinos a partir del nodo dado
    public List<Nodo> GetVecinos(Nodo z)
    {
        List<Nodo> vecinos = new List<Nodo>();
        int icheckX;
        int icheckY;

        icheckX = z.X + 1;          //derecho
        icheckY = z.Y;
        if (icheckX >= 0 && icheckX < tamGridX)
        {
            if (icheckY >= 0 && icheckY < tamGridY)
            {
                vecinos.Add(Nodos[icheckX, icheckY]);
            }
        }

        icheckX = z.X - 1;      //izquierdo
        icheckY = z.Y;
        if (icheckX >= 0 && icheckX < tamGridX)
        {
            if (icheckY >= 0 && icheckY < tamGridY)
            {
                vecinos.Add(Nodos[icheckX, icheckY]);
            }
        }

        icheckX = z.X;          //arriba
        icheckY = z.Y + 1;
        if (icheckX >= 0 && icheckX < tamGridX)
        {
            if (icheckY >= 0 && icheckY < tamGridY)
            {
                vecinos.Add(Nodos[icheckX, icheckY]);
            }
        }

        icheckX = z.X;          //abajo
        icheckY = z.Y - 1;
        if (icheckX >= 0 && icheckX < tamGridX)
        {
            if (icheckY >= 0 && icheckY < tamGridY)
            {
                vecinos.Add(Nodos[icheckX, icheckY]);
            }
        }

        return vecinos;
    }


    //Obtiene el nodo correspondiente a las coordenadas reales
    public Nodo GetNodoPosicionGlobal(Vector3 pos)
    {
        float x = ((pos.x + tamGrid.x / 2) / tamGrid.x);
        float y = ((pos.z + tamGrid.y / 2) / tamGrid.y);

        x = Mathf.Clamp01(x);
        y = Mathf.Clamp01(y);

        int ix = Mathf.RoundToInt((tamGridX - 1) * x);
        int iy = Mathf.RoundToInt((tamGridY - 1) * y);
        if (Nodos != null && ix < mapaFila && ix >= 0 && iy < mapaColumna && iy >= 0)
            return Nodos[ix, iy];
        return null;
    }
    
    //devuelve la posicion del nodo en la matriz de nodos
    public Vector3 GetIndicesNodos(Vector3 pos)
    {
        float x = ((pos.x + tamGrid.x / 2) / tamGrid.x);
        float y = ((pos.z + tamGrid.y / 2) / tamGrid.y);

        x = Mathf.Clamp01(x);
        y = Mathf.Clamp01(y);

        int ix = Mathf.RoundToInt((tamGridX - 1) * x);
        int iy = Mathf.RoundToInt((tamGridY - 1) * y);
        if (Nodos != null && ix < mapaFila && ix >= 0 && iy < mapaColumna && iy >= 0)
            return new Vector3(ix, 0, iy);
        return Vector3.zero;
    }

    //Obtiene el coste de un nodo
    int costeNodo(Transform nodo)
    {
        if (isObject(nodo.position))
        {
            return 99999;
        }
        return 1;
    }

    public float costeNodoTactico(Nodo nodo, NPC.TipoUnidad tipoUnidad, NPC.Equipo team)
    {

        switch (nodo.terrainType)
        {
            case Nodo.TerrainType.Bosque:
                switch (tipoUnidad)
                {
                    case NPC.TipoUnidad.Ranged:
                        return 18;
                    case NPC.TipoUnidad.Brawler:
                        return 20f;
                    case NPC.TipoUnidad.Medic:
                        return 15;
                }
                break;
            case Nodo.TerrainType.Pradera:
                switch (tipoUnidad)
                {
                    case NPC.TipoUnidad.Ranged:
                        return 10;
                    case NPC.TipoUnidad.Brawler:
                        return 10;
                    case NPC.TipoUnidad.Medic:
                        return 10;

                }
                break;
            case Nodo.TerrainType.Carretera:
                switch (tipoUnidad)
                {
                    case NPC.TipoUnidad.Ranged:
                        return 0.8f;
                    case NPC.TipoUnidad.Brawler:
                        return 0.66f;
                    case NPC.TipoUnidad.Medic:
                        return 0.66f;

                }
                break;
            case Nodo.TerrainType.FraCapturar:
                switch (team) {
                    case NPC.Equipo.Spain:
                        return 1000;
                    case NPC.Equipo.France:
                        return 1;
                }
                break;
            case Nodo.TerrainType.EspCapturar:
                switch (team) {
                    case NPC.Equipo.Spain:
                        return 1;
                    case NPC.Equipo.France:
                        return 1000;
                }
                break;
            case Nodo.TerrainType.NotWalkable:
                return float.MaxValue;
            default:
                return 1;
        }
        return 1;
    }


    //Obtiene la matriz de costes del grids
    public int[,] ObtenerMatrizCostes()
    {
        int[,] mapaCostes = new int[mapaFila, mapaColumna];
        for (int i = 0; i < mapaFila; i++)
        {
            for (int y = 0; y < mapaColumna; y++)
            {
                mapaCostes[i, y] = costeNodo(mapa[i, y]);
            }
        }
        return mapaCostes;
    }

    //Comprueba si hay un objeto que impide el paso    
    bool isObject(Vector3 position)
    {
        Collider[] intersecting = Physics.OverlapSphere(position, radioNodo);
        foreach (Collider i in intersecting)
        {
            if (i.gameObject.tag == "Muro" || i.gameObject.tag == "Agua")
                return true;
        }
        return false;
    }

    Nodo.TerrainType getTerrenoPosition(Vector3 position)
    {
        Collider[] intersecting = Physics.OverlapSphere(position, radioNodo);
        foreach (Collider i in intersecting)
        {
            switch(i.tag){
                case "Carretera":
                    return Nodo.TerrainType.Carretera;
                case "Puente":
                    return Nodo.TerrainType.Carretera;
                case "Bosque":
                    return Nodo.TerrainType.Bosque;
                case "Pradera":
                    return Nodo.TerrainType.Pradera;
                case "Base Fra":
                    return Nodo.TerrainType.FraCapturar;
                case "Base Esp":
                    return Nodo.TerrainType.EspCapturar;
                case "Muro":
                    return Nodo.TerrainType.NotWalkable;
                case "Agua":
                    return Nodo.TerrainType.NotWalkable;
                case "Zona Curacion Esp":
                    return Nodo.TerrainType.CurarEsp;
                case "Zona Curacion Fra":
                    return Nodo.TerrainType.CurarFra;

            }
        }
        return Nodo.TerrainType.Undefined;
    }
}