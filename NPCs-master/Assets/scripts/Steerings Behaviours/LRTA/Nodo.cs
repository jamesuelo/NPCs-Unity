using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Nodos que componen los caminos del pathfinding y del algoritmo LRTA *
public class Nodo{
    public enum TerrainType {           // tipo de terreno para el bloque 2
        Carretera,
        Pradera,
        Bosque,
        FraCapturar,
        EspCapturar,
        CurarEsp,
        CurarFra,
        Undefined,
        NotWalkable
    }
    public int X;   //Posicion x con respecto a los nodos
    public int Y;   //Posicion y con respecto a los nodos

    public bool walkable;     //Aislamiento del nodo, si es posible continuar
    public Vector3 Posicion;   //posicion

    public Nodo NodoPadre;  //Nodo del que se accede al actual

    public float igCost;  //coste nodo siguiente
    public float ihCost;  //coste destino
    public float FCost { get { return igCost + ihCost; } } //Suma de costes
    
    public int radio = 10;
    public float visibilidad =0;

    public TerrainType terrainType;
    public float influence;

    public Nodo(bool a, Vector3 b, int c, int d, TerrainType terreno) //Constructor
    {
        walkable = a;
        Posicion = b;
        X = c;
        Y = d;
        terrainType = terreno;
    }

    public float SpeedMultiplier(NPC.TipoUnidad unitType) {         //funcion del bloque 2 que modifica la velocidad dado el personaje
        switch (terrainType) {
            case TerrainType.Bosque:
                switch (unitType) {
                    case NPC.TipoUnidad.Ranged:
                        return 2.5f; 
                    case NPC.TipoUnidad.Brawler: 
                        return 2f;
                    case NPC.TipoUnidad.Medic:
                        return 3;
                }
                break;
            case TerrainType.Pradera:
                switch (unitType) {
                    case NPC.TipoUnidad.Ranged:
                        return 2.75f; 
                    case NPC.TipoUnidad.Brawler:
                        return 2.5f;
                    case NPC.TipoUnidad.Medic:
                        return 2.5f;
                    
                }
                break;
            case TerrainType.Carretera:
                switch (unitType) {
                    case NPC.TipoUnidad.Ranged:
                        return 2.75f; 
                    case NPC.TipoUnidad.Brawler:
                        return 3;
                    case NPC.TipoUnidad.Medic:
                        return 3;
                    
                }
                break;
            default:
                return 2;
        }
        return 2;
    }

    public void calculoVisibilidad() {
        if (!walkable) {
            visibilidad = 1;
            return;
        }
        float vis= 0;
        int rayos = 10;
        float angulo = 0;
        for (int i = 0; i < rayos; i++) {
            float x = Mathf.Cos(angulo);
            float y = Mathf.Sin(angulo);
            angulo += 2 * Mathf.PI / rayos;
                    
            //Vector3 dir = new Vector3 (Posicion.x + x, 2.5f, Posicion.z + y);
            //Vector3 pos =Posicion;
            Vector3 dir = new Vector3 (x, 0f, y);
            Vector3 pos =Posicion + new Vector3 (0, 2.5f, 0); 

            RaycastHit hit;
            if(Physics.Raycast(pos, dir,out hit, radio))
            {
                if(hit.collider == null){

                    vis = vis + radio;
                }
                else{
                    vis = vis + hit.distance;
                }
            }

        }
        vis /= rayos;
        vis /= radio;

        visibilidad=vis;
            
    }



}