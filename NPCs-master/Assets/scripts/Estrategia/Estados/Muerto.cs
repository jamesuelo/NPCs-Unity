using UnityEngine;

public class Muerto : Estado  {

    private float deadTime = 7f;
    private float time;

    public override void EntrarEstado(NPC npc) {
        npc.GetComponent<Path>().ClearPath();
        move = false;
        time = Time.time;
    }

    public override void SalirEstado(NPC npc) {}

    public override void Accion(NPC npc) {
        // respaweamos en la base cuando pase el tiempo
        if (Time.time - time >= deadTime) {
            npc.agentNPC.Position = npc.gameManager.waypointManager.GetNodoAleatorio(npc.gameManager.waypointManager.GetBase(npc)).Posicion;
            npc.health = npc.maxVida;
        }
    }

    public override void Ejecutar(NPC npc) {
        Accion(npc);
        ComprobarEstado(npc);
    }

    public override void ComprobarEstado(NPC npc) {
        //Comprobamos si pasar al estado Idle o no hacer nada
        if (npc.health > 0)
            npc.CambiarEstado(npc.estadoAsignado);
    }
}
