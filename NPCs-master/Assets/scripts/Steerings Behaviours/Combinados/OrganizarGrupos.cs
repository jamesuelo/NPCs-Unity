using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganizarGrupos : MonoBehaviour
{
    public Agent[] agentesCombinados;   //lista de agentes con los comportamietnos de flocking

    //las siguientes funciones sirven para indicar quienes van a ser objetivos de los comportamientos de flocking
    private void PonerAlignment(Alignment script, int quitar)
    {
        for(int i = 0; i < agentesCombinados.Length; i++)
        {
            if (i != quitar)
                script.targets.Add(agentesCombinados[i].GetComponent<Agent>());
        }
    }

    private void PonerCohesion(Cohesion script, int quitar)
    {
        for (int i = 0; i < agentesCombinados.Length; i++)
        {
            if (i != quitar)
                script.targets.Add(agentesCombinados[i].GetComponent<Agent>());
        }
    }

    private void PonerSeparation(Separation script, int quitar)
    {
        for (int i = 0; i < agentesCombinados.Length; i++)
        {
            if (i != quitar)
                script.targets.Add(agentesCombinados[i].GetComponent<Agent>());
        }
    }

    void Start()
    {
        int i = 0;
        foreach (Agent a in agentesCombinados)
        {
            agentesCombinados[i++] = a;
        }

        for (i = 0; i < agentesCombinados.Length; i++)
        {
            PonerAlignment(agentesCombinados[i].GetComponent<Alignment>(), i);
            PonerCohesion(agentesCombinados[i].GetComponent<Cohesion>(), i);
            PonerSeparation(agentesCombinados[i].GetComponent<Separation>(), i);
        }
    }
}