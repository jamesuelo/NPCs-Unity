using UnityEngine;

public class Patrullar : Estado  {

    private bool patrulla;
    private float minDistance = 10f;
    public override void EntrarEstado(NPC npc) {
        move = false;
        patrulla = false;
        npc.GetComponent<PathFollowing>().patrol = true;
    }

    public override void SalirEstado(NPC npc) {
        npc.GetComponent<PathFollowing>().patrol = false;
        npc.GetComponent<Path>().ClearPath();
    }

    public override void Accion(NPC npc) {
        float principio = Vector3.Distance(npc.agentNPC.Position, npc.puntoPatrullaInicial.position);
        float fin = Vector3.Distance(npc.agentNPC.Position, npc.puntoPatrullaFin.position);
        if (!move) {
            if (principio < fin)
                npc.pf.EncontrarCaminoJuego(npc.nodoActual.Posicion, npc.puntoPatrullaInicial.position);
            else 
                npc.pf.EncontrarCaminoJuego(npc.nodoActual.Posicion, npc.puntoPatrullaFin.position);
            move = true;
        }
        else if (principio <= minDistance || fin <= minDistance) {
            if (!patrulla) {
                npc.GetComponent<PathFollowing>().patrol = true;
                if (principio <= minDistance)
                    npc.pf.EncontrarCaminoJuego(npc.puntoPatrullaInicial.position, npc.puntoPatrullaFin.position);
                else if (fin <= minDistance)
                    npc.pf.EncontrarCaminoJuego(npc.puntoPatrullaFin.position, npc.puntoPatrullaInicial.position);
                patrulla = true;
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