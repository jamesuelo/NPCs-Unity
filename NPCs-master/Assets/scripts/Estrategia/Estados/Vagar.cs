using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Vagar : Estado {

    public override void EntrarEstado(NPC npc) {
        move = false;
    }

    public override void SalirEstado(NPC npc) {
        npc.GetComponent<Path>().ClearPath();
    }

    public override void Accion(NPC npc) {

        GameManager gameManager = npc.gameManager;
        if (!move) {
            if (npc.team == NPC.Equipo.Spain)
                npc.pf.EncontrarCaminoJuego(npc.nodoActual.Posicion, gameManager.waypointManager.GetNodoAleatorio(gameManager.waypointManager.vagarWaypointSPA).Posicion);
            else
                npc.pf.EncontrarCaminoJuego(npc.nodoActual.Posicion, gameManager.waypointManager.GetNodoAleatorio(gameManager.waypointManager.vagarWaypointFRA).Posicion);
            move = true;
        }
        Path pathNPC = npc.agentNPC.GetComponent<Path>();
        if (pathNPC.nodos.Count > 0 && (npc.GetComponent<PathFollowing>().EndOfThePath() || Vector3.Distance(npc.agentNPC.Position,pathNPC.nodos[pathNPC.nodos.Count-1].gameObject.transform.position) < 4)){
            move = false;
        }
    }

    public override void Ejecutar(NPC npc) {
        Accion(npc);
        ComprobarEstado(npc);
    }

    public override void ComprobarEstado(NPC npc) {

        GameManager gameManager = npc.gameManager;
        
        if (ComprobarMuerto(npc))
            return;
        if (ComprobarEscapar(npc))
            return;
        if (ComprobarAtaqueRangoMedico(npc))
            return;
        if (ComprobarDefensa(gameManager, npc))
            return;
        if (ComprobarCaptura(gameManager, npc))
            return;
        if (ComprobarAtaqueRangoMelee(npc))
            return;        
    }
}