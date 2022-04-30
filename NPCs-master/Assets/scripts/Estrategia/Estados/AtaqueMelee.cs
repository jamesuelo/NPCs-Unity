using UnityEngine;

public class AtaqueMelee : Estado
{

    private float time;
    private bool intento;
    private bool inutil;
    public override void EntrarEstado(NPC npc)
    {
        move = false;
        intento = false;
        inutil = false;
        time = -1;
    }

    public override void SalirEstado(NPC npc)
    {
        npc.GetComponent<Path>().ClearPath();
    }

    public override void Accion(NPC npc)
    {
        Face f = npc.GetComponent<Face>();
        if (f == null)
        {
            npc.gameObject.AddComponent<Face>();
            npc.gameObject.GetComponent<Face>().target = npcObjetivo.gameObject.GetComponent<AgentNPC>();
        }
        else
        {
            f.target = npcObjetivo.gameObject.GetComponent<AgentNPC>();
        }
        float distance = Vector3.Distance(npc.agentNPC.Position, npcObjetivo.agentNPC.Position);
        Path camino = npc.GetComponent<Path>();
        int posicionActualCamino = npc.GetComponent<PathFollowing>().currentPos;
        bool isFinalCamino = camino.EndOfThePath(posicionActualCamino);
        if (distance <= npc.rangoMelee)
        {

            //que deje de moverse el personaje si esta a rango
            if (move)
            {
                move = false;
                npc.GetComponent<Path>().ClearPath();
            }

            if (time == -1)
            {
                //comprobamos que no se suicide si no estamos en Guerra Total
                if (!npc.gameManager.totalWarMode && npc.health <= npc.menosVida)
                {

                    inutil = true;
                    return;
                }
                time = Time.time;
            }

            //atacamos segun su ratio de ataque 
            if (Time.time - time >= npc.meleeAttackSpeed)
            {
                CombatManager.AtaqueMelee(npc, npcObjetivo);
                time = -1;
                intento = false;
            }
        }
        else
        {
            if (!npc.gameManager.totalWarMode && npc.health <= npc.menosVida)
            {
                inutil = true;
                return;
            }
            npc.pf.EncontrarCaminoJuego(npc.nodoActual.Posicion, npcObjetivo.nodoActual.Posicion);
            move = true;
            intento = true;
        }
    }

    public override void Ejecutar(NPC npc)
    {
        Accion(npc);
        ComprobarEstado(npc);
    }

    public override void ComprobarEstado(NPC npc)
    {
        Face f = npc.GetComponent<Face>();
        if (ComprobarMuerto(npc))
        {
            f.target = null;
            f.aux = null;
            return;
        }
        if (!inutil && ComprobarAtaqueRangoMelee(npc))
            return;
        npc.CambiarEstado(npc.estadoAsignado);
    }


}