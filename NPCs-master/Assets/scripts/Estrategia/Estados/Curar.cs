using UnityEngine;

public class Curar : Estado {
    private bool curar;
    private bool inutil;
    private float timer;
    private float healingRate = 1;

    public override void EntrarEstado(NPC npc) {
        npc.GetComponent<Path>().ClearPath();
        timer = -1;
    }

    public override void SalirEstado(NPC npc) {
        curar = false;
        inutil = false;
    }

    public override void Accion(NPC npc) {
        
        // Si nuestro personaje esta en la zona de curacion entocnes que se vaya curando
        if (npc.gameManager.InCuracion(npc)) {
            if (timer == -1)
                timer = Time.time;

            if (Time.time - timer >= 1) {
                timer = -1;
                if (npc.maxVida - npc.health >= 100)
            
                    npc.health += 100;
                else {
                    curar = true;
                }
            }

        } else {            //si no buscamos al medico para que nos cure
            if (npc.health <= npc.healthy) {
                NPC medico = UnitsManager.MedicoCerca(npc);
                if (medico == null || Vector3.Distance(npc.agentNPC.Position, medico.agentNPC.Position) > medico.rangedRange) {
                    inutil = true;
                }
            } else
                curar = true;
        }
    }

    public override void Ejecutar(NPC npc) {
        Accion(npc);
        ComprobarEstado(npc);
    }

    public override void ComprobarEstado(NPC npc) {
        if (ComprobarMuerto(npc))
            return;
        
        if (inutil)
            npc.CambiarEstado(npc.estadoAsignado);
        
        if (curar) 
            npc.CambiarEstado(npc.estadoAsignado);

    }

}