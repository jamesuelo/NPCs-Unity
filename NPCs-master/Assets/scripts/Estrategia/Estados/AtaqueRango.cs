using UnityEngine;

public class AtaqueRango : Estado  {

    private float time;
    private bool inutil;
    public override void EntrarEstado(NPC npc) {
        time = -1f;
        inutil = false;
        npc.GetComponent<Path>().ClearPath();
    }

    public override void SalirEstado(NPC npc) {}

    public override void Accion(NPC npc) {

        
        Face f = npc.GetComponent<Face>();
        if (f == null){
            npc.gameObject.AddComponent<Face>();
            npc.gameObject.GetComponent<Face>().target = npcObjetivo.gameObject.GetComponent<AgentNPC>();
            npc.gameObject.GetComponent<Face>().aux = npcObjetivo.gameObject.GetComponent<AgentNPC>();
        }
        else{
            f.target = npcObjetivo.gameObject.GetComponent<AgentNPC>();
            f.aux = npcObjetivo.gameObject.GetComponent<AgentNPC>();
        }
        // Nos aseguramos de que el personaje tiene a rango al objetivo
        // Si no es GerraTotal el personaje se va
        if (!UnitsManager.DirectLine(npc, npcObjetivo) || Vector3.Distance(npc.agentNPC.Position, npcObjetivo.agentNPC.Position) > npc.rangedRange || (!npc.gameManager.totalWarMode && npc.health <= npc.menosVida))
            inutil = true;
        if (!inutil) {
            //vamos atacando segun nuestro ratio de ataque
            if (time == -1) {
                time = Time.time;
            }

            if (Time.time - time >= npc.rangedAttackSpeed) {
                CombatManager.AtaqueRango(npc, npcObjetivo);
                time = -1;
            }
        }
    }

    public override void Ejecutar(NPC npc) {
        Accion(npc);
        ComprobarEstado(npc);
    }

    public override void ComprobarEstado(NPC npc) {
        if (ComprobarMuerto(npc)){
            Face f = npc.GetComponent<Face>();
            f.target = null;
            f.aux = null;
            return;
        }
        if (!inutil && (ComprobarAtaqueRangoMedico(npc) || ComprobarAtaqueRangoMelee(npc)))
            return;
        
        npc.CambiarEstado(npc.estadoAsignado);
    }
}