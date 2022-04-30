using UnityEngine;

public class Captura : Estado {

    private bool inutil;
    public override void EntrarEstado(NPC npc) {
        move = false;
        inutil = false;
    }

    public override void SalirEstado(NPC npc) {
        npc.GetComponent<Path>().ClearPath();
    }

    public override void Accion(NPC npc) {

        GameManager gameManager = npc.gameManager;
        
        //si ya esta en la base enemiga, que se quede quieto
        if (gameManager.NPCInWaypoint(npc, gameManager.waypointManager.GetRival(npc))) {
            if (move) {
                move = false;
                npc.GetComponent<Path>().ClearPath();
            }

            // mientras que no haya enemigos, seguira capturano la base
            if (!gameManager.EnemigosDefendiendo(npc)){}
                gameManager.waypointManager.Captura(npc);
        } 
        else if (!move) {
            //establecemos como objetivo uno de los waypoints de la base contraria
            npc.pf.EncontrarCaminoJuego(npc.nodoActual.Posicion, gameManager.waypointManager.GetNodoAleatorio(gameManager.waypointManager.GetRival(npc)).Posicion);
            move = true;
        } else {
            //si ya estaban atacando la base, compruebo si merece la pena seguir yendo a capturar o no
            if (gameManager.EnemigosCheckpoint(npc) > 0) {
                var baseAliada = gameManager.waypointManager.GetEquipo(npc).posicion;
                var baseEnemiga = gameManager.waypointManager.GetRival(npc).posicion;
                var posicion = npc.nodoActual.Posicion;
                var distanciaBaseEnemiga = Vector3.Distance(baseAliada, posicion);
                var distanciaBaseAliada = Vector3.Distance(baseEnemiga, posicion);
                if (distanciaBaseAliada <= distanciaBaseEnemiga) 
                    inutil = true;
            
            }
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
        if (ComprobarAtaqueRangoMedico(npc))
            return;
        if (!inutil && ComprobarCaptura(gameManager, npc))
            return;
        npc.CambiarEstado(npc.estadoAsignado);
    }

}
