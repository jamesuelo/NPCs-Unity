using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class LRTA : MonoBehaviour
{
    public List<Nodo> EncontrarCaminoLRTAStar(Nodo comienzo, Nodo objetivo, int distancia, Grid grid)       //funcion para poder usar LRTA* en el bloque 1
    {
        //Lista de nodos cerrados a.k.a nodos utilizados
        if (grid.Nodos == null)
            return null;
        List<Nodo> cerrados = new List<Nodo>();
        Nodo actual = comienzo;
        int coste;
        //Aplicamos la heuristica especificada entre el nodo actual (inicial) y el destino
        switch (distancia)
        {
            case 1:
                coste = Manhattan(actual, objetivo);
                break;
            case 2:
                coste = Chebychev(actual, objetivo);
                break;
            case 3:
                coste = Euclidea(actual, objetivo);
                break;
            default:
                coste = Manhattan(actual, objetivo);
                break;
        }

        //Establecemos el coste del camino
        actual.ihCost = coste;

        //Mientras que no se haya alcanzado el nodo objetivo
        while (actual != objetivo)
        {
            Nodo siguiente = null;
            List<Nodo> vecinos = grid.GetVecinos(actual);
            //Buscamos de entre sus nodos vecinos
            foreach (Nodo vecino in vecinos)
            {
                coste = 0;
                //Para cada vecino, calcula su coste hasta el objetivo
                switch (distancia)
                {
                    case 1:
                        coste = Manhattan(vecino, objetivo);
                        break;
                    case 2:
                        coste = Chebychev(vecino, objetivo);
                        break;
                    case 3:
                        coste = Euclidea(vecino, objetivo);
                        break;
                }
                vecino.ihCost = coste;

            }
            float costeMinimo = Mathf.Infinity;

            //Para cada vecino
            foreach (Nodo vecino in vecinos)
            {
                //Si no está en la lista de cerrados y no está aislado
                //establece el vecino como el siguiente nodo del camino
                //obteniendo el nodo cuyo coste es menor
                if (!cerrados.Contains(vecino))
                {
                    if (costeMinimo == Mathf.Infinity)
                    {
                        siguiente = vecino;
                        costeMinimo = vecino.FCost;
                    }
                    else if (vecino.FCost < siguiente.FCost)
                    {
                        siguiente = vecino;
                        costeMinimo = vecino.FCost;
                    }
                }
            }
            //Añade el nodo a la lista de cerrados
            cerrados.Add(actual);
            //Establecemos el nodo actual como el siguiente nodo del camino
            actual = siguiente;
            if (siguiente == null)
            {
                break;
            }
        }
        //Devolvemos el camino
        return cerrados;
    }

    //Encuentra un camino dado un nodo por el que comenzar, y un destino
    //Distancia se usa para especificar que tipo de heuristica utilizar para el bloque 2
    public List<Nodo> EncontrarCaminoAStar(Nodo comienzo, Nodo objetivo, int distancia, Grid grid, NPC npc, bool tactico, float multiplicadorTerreno, float multiplicadorInfluencia, float multiplicadorVisibilidad)
    {
        //En caso de que el grid no contenga nodos, no se hace nada
        if (grid.Nodos == null)
            return null;
        List<Nodo> openSet = new List<Nodo>();
        List<Nodo> closedSet = new List<Nodo>();
        int coste = 0;
        openSet.Add(comienzo);
        while (openSet.Count > 0)
        {
            Nodo actual = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                switch (distancia)
                {
                    case 1:
                        actual.ihCost = Manhattan(actual, objetivo);
                        openSet[i].ihCost = Manhattan(openSet[i], objetivo);
                        break;
                    case 2:
                        actual.ihCost = Chebychev(actual, objetivo);
                        openSet[i].ihCost = Chebychev(openSet[i], objetivo);
                        break;
                    case 3:
                        actual.ihCost = Euclidea(actual, objetivo);
                        openSet[i].ihCost = Euclidea(openSet[i], objetivo);
                        break;
                    default:
                        actual.ihCost = Manhattan(actual, objetivo);
                        openSet[i].ihCost = Manhattan(openSet[i], objetivo);
                        break;
                }
                if (openSet[i].FCost < actual.FCost || openSet[i].FCost == actual.FCost && openSet[i].ihCost < actual.ihCost)
                {
                    actual = openSet[i];
                }
            }
            openSet.Remove(actual);
            closedSet.Add(actual);
            if (actual == objetivo)
            {
                return RecalcularCamino(comienzo, objetivo);
            }
            List<Nodo> vecinos = grid.GetVecinos(actual);
            foreach (Nodo v in vecinos)
            {
                if (!v.walkable || closedSet.Contains(v)) continue;
                switch (distancia)
                {
                    case 1:
                        coste = Manhattan(actual, v);
                        break;
                    case 2:
                        coste = Chebychev(actual, v);
                        break;
                    case 3:
                        coste = Euclidea(actual, v);
                        break;
                    default:
                        coste = Manhattan(actual, v);
                        break;
                }
                float costeVecino = 0;
                if (tactico)
                {
                    costeVecino = costeVecinoTactico(actual, v, multiplicadorTerreno, grid, npc.tipo, npc.team, coste,multiplicadorInfluencia, multiplicadorVisibilidad);
                }
                else
                {
                    costeVecino = actual.igCost + coste;
                }
                if (costeVecino < v.igCost || !openSet.Contains(v))
                {
                    v.igCost = costeVecino;
                    switch (distancia)
                    {
                        case 1:
                            v.ihCost = Manhattan(actual, v);
                            break;
                        case 2:
                            v.ihCost = Chebychev(actual, v);
                            break;
                        case 3:
                            v.ihCost = Euclidea(actual, v);
                            break;
                        default:
                            v.ihCost = Manhattan(actual, v);
                            break;
                    }
                    v.NodoPadre = actual;
                    if (!openSet.Contains(v))
                        openSet.Add(v);
                }
            }
        }
        return RecalcularCamino(comienzo, objetivo);
    }   



    //funcion para ir calculando el coste tactico dado el terreno, la influencia y la visibilidad
    float costeVecinoTactico(Nodo actual, Nodo vecino, float multiplicadorTerreno,
     Grid grid, NPC.TipoUnidad tipo, NPC.Equipo team, int heuristica, float multiplicadorInfluencia, float multiplicadorVisibilidad)
    {
        float finalCoste = 0;
        //Puede ser que haya cmabiarlo a (multiplicadorTerreno * (costeNodoTactico + hCost))
        float terrainCost = multiplicadorTerreno * (grid.costeNodoTactico(actual, tipo, team) + grid.costeNodoTactico(vecino, tipo, team)) / 2;
        float currentInfluence;
        float adjacentInfluence;
        if (team == NPC.Equipo.Spain)
        {
            //Si pertenece a  un equipo especifico no tiene en cuenta las influencias de las zonas controladas por el mismo equipo y se centra en las del contrario
            if (actual.influence > 0)
                currentInfluence = 0;
            else
                currentInfluence = Mathf.Abs(actual.influence);

            if (vecino.influence > 0)
                adjacentInfluence = 0;
            else
                adjacentInfluence = Mathf.Abs(actual.influence);
        }
        else
        {
            if (actual.influence < 0)
                currentInfluence = 0;
            else
                currentInfluence = actual.influence;

            if (vecino.influence < 0)
                adjacentInfluence = 0;
            else
                adjacentInfluence = vecino.influence;
        }

        float influenceCoste = multiplicadorInfluencia * (currentInfluence + adjacentInfluence) / 2;
        float visibilityCoste = multiplicadorVisibilidad * actual.visibilidad;
        finalCoste += terrainCost + influenceCoste + visibilityCoste;
        return finalCoste;
    }

    List<Nodo> RecalcularCamino(Nodo comienzo, Nodo objetivo)
    {
        List<Nodo> camino = new List<Nodo>();
        Nodo actual = objetivo;
        while (actual != comienzo)
        {
            camino.Add(actual);
            if (actual.NodoPadre != null)
                actual = actual.NodoPadre;
            else 
                break;
        }
        camino.Reverse();
        return camino;
    }

    //Calcula la distancia Manhattan entre dos nodos
    int Manhattan(Nodo a, Nodo b)
    {
        int ix = Mathf.Abs(a.X - b.X);
        int iy = Mathf.Abs(a.Y - b.Y);
        return ix + iy;
    }

    //Calcula la distancia Chebychev entre dos nodos
    int Chebychev(Nodo a, Nodo b)
    {
        int ix = Mathf.Abs(b.X - a.X);
        int iy = Mathf.Abs(b.Y - a.Y);
        return Mathf.Max(ix, iy);
    }

    //Calcula la distancia Euclidea entre dos nodos
    int Euclidea(Nodo a, Nodo b)
    {
        int ix = (b.X - a.X) * (b.X - a.X);
        int iy = (b.Y - a.Y) * (b.Y - a.Y);
        return (int)Mathf.Sqrt(ix + iy);
    }
}
