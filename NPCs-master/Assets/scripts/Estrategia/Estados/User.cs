using UnityEngine;

public class User : Estado
{
    public override void EntrarEstado(NPC npc) {
        npc.GetComponent<Path>().ClearPath();
        move = false;
    }

    public override void SalirEstado(NPC npc) {}

    public override void Accion(NPC npc) {}

    public override void Ejecutar(NPC npc) {
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
        if (npc.patrol)
            npc.CambiarEstado(npc.estadoPatrullar);
        else 
            npc.CambiarEstado(npc.estadoVagar);
        
    }
}
