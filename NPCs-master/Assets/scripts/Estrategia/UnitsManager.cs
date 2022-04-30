using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using System.Linq;

public static class UnitsManager {

    private static int rango = 30;        //rango que se establece para detectar colisiones

    // enemigos cerca que puede ver
    public static int EnemigosCerca(NPC npc) {
        int result = 0;
        List<string> nombresEnemigos= new List<string>();
        Collider[] hitColliders = Physics.OverlapSphere(npc.agentNPC.Position, 9);
        int i = 0;
        while (i < hitColliders.Length) {
            NPC actualNPC = hitColliders[i].GetComponent<NPC>();
            if (actualNPC != null && !nombresEnemigos.Contains(actualNPC.name) && actualNPC.team != npc.team && !actualNPC.IsDead && DirectLine(npc, actualNPC)){
                nombresEnemigos.Add(actualNPC.name);
                result++;
            }
            i++;
        }
        return result;
    }

    // comprobamos si hay algo en medio entre el atacante y el atacado
    public static bool DirectLine(NPC atacante, NPC atacado) {
        RaycastHit hit;
        var direction = atacado.agentNPC.Position - atacante.agentNPC.Position;
        if (Physics.Raycast(atacante.agentNPC.Position, direction, out hit))
            return hit.collider.GetComponent<NPC>() == atacado;
        return false;
    }
    // enemigos dentro del rango de un NPC
    public static List<NPC> EnemigosEnRango(NPC npc) {
        List<NPC> enemigos = new List<NPC>();
        Collider[] hitColliders = Physics.OverlapSphere(npc.agentNPC.Position, npc.rangedRange);
        int i = 0;
        while (i < hitColliders.Length) {
            NPC actualNPC = hitColliders[i].GetComponent<NPC>();
            if (actualNPC != null && actualNPC.team != npc.team && DirectLine(npc, actualNPC) && !actualNPC.IsDead) {
                enemigos.Add(actualNPC);
            }
            i++;
        }
        if (enemigos.Count > 0) {
            enemigos = enemigos.OrderBy(e => Vector3.Distance(e.agentNPC.Position, npc.agentNPC.Position)).ToList();
        }
        return enemigos;
    }

    // devuelve quien es el agente medico mas cercano (posiblemente metamos mas medicos)
    public static NPC MedicoCerca(NPC npc) {
        Collider[] hitColliders = Physics.OverlapSphere(npc.agentNPC.Position, rango);
        int i = 0;

        NPC seleccionado = null;
        while (i < hitColliders.Length) {
            NPC actualNPC = hitColliders[i].GetComponent<NPC>();
            if (actualNPC != null && actualNPC.team == npc.team && actualNPC.tipo == NPC.TipoUnidad.Medic && !actualNPC.IsDead) {
                seleccionado = actualNPC;       
            }
            i++;
        }
        return seleccionado;
    }
    
    // devuelve al aliado mas cercano de un NPC
    public static NPC AliadoCercano(NPC npc) {
        Collider[] hitColliders = Physics.OverlapSphere(npc.agentNPC.Position, rango);
        int i = 0;
        float minDistancia = float.MaxValue;
        NPC aliado = null;
        while (i < hitColliders.Length) {
            NPC actualNPC = hitColliders[i].GetComponent<NPC>();
            if (actualNPC != null && actualNPC.team == npc.team && !actualNPC.IsDead && npc.name != actualNPC.name) {
                float distancia = Vector3.Distance(actualNPC.agentNPC.Position, npc.agentNPC.Position);
                if (distancia < minDistancia) {
                    minDistancia = distancia;
                    aliado = actualNPC;
                }  
            }
            i++;
        }

        if (aliado != null)
            return aliado;
        return null;
    }
    
    //devuelve el aliado con menos vida
    public static NPC ElegirAliado(NPC npc) {
        List<NPC> lowHealth = new List<NPC>();
        Collider[] hitColliders = Physics.OverlapSphere(npc.agentNPC.Position, npc.rangedRange);
        int i = 0;
        while (i < hitColliders.Length) {
            NPC actualNPC = hitColliders[i].GetComponent<NPC>();
            if (actualNPC != null && actualNPC.team == npc.team && DirectLine(npc, actualNPC) && !actualNPC.IsDead) {
                if (actualNPC.health <= actualNPC.healthy)
                    lowHealth.Add(actualNPC);
            }
            i++;
        }
        if (lowHealth.Count > 0) {
            lowHealth = lowHealth.OrderBy(e => e.health).ToList();
            return lowHealth[0];
        }
        return null;
    }

}