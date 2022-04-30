using UnityEngine;

public class Defender : Estado  {

    public override void EntrarEstado(NPC npc) {
        move = false;
    }

    public override void SalirEstado(NPC npc) {
        npc.GetComponent<Path>().ClearPath();
    }

    public override void Accion(NPC npc) {

        if (!move) {
            // Moverlos hacia su base para defenderla
            npc.pf.EncontrarCaminoJuego(npc.nodoActual.Posicion, npc.gameManager.waypointManager.GetNodoAleatorio(npc.gameManager.waypointManager.GetEquipo(npc)).Posicion);
            move = true;
        }

    }

    public override void Ejecutar(NPC npc) {
        Accion(npc);
        ComprobarEstado(npc);
    }

    public override void ComprobarEstado(NPC npc) {
        GameManager gameManager = npc.gameManager;
        //comprobar que esta muerto
        if (ComprobarMuerto(npc))
            return;
        //comprobar que este a rango de una medico
        if (ComprobarAtaqueRangoMedico(npc))
            return;
        //comprobar que deba defender
        if (ComprobarDefensa(gameManager, npc))
            return;
            
       npc.CambiarEstado(npc.estadoAsignado);
    }

}