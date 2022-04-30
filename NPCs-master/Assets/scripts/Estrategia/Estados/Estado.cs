using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Estado 
{

    public NPC npcObjetivo;         //el objetivo cuando realice algun ataque
    public bool move = false;          //para indicar si se esta moviendo 

    public Estado(){
        npcObjetivo = null;
        move = false;
    }
    public abstract void Accion(NPC n);
    public abstract void EntrarEstado(NPC n);
    public abstract void SalirEstado(NPC n);
    public abstract void Ejecutar(NPC n);
    public abstract void ComprobarEstado(NPC n);

    public void SetObjective(NPC newObjective) {
        npcObjetivo = newObjective;
    }
    //funciones de comprobaciones que servirarn para saber si pasaremos a otro estado
    protected bool ComprobarMuerto(NPC npc) {
        if(npc.user)
            return false;
        if (npc.health <= 0) {
            npc.CambiarEstado(npc.estadoMuerto);
            return true;
        }
        return false;
    }

    protected bool ComprobarDefensa(GameManager gameManager, NPC npc) {
        if(npc.user)
            return false;
        if (gameManager.EnemigosCheckpoint(npc) > 0 && 
        (npc.health > npc.menosVida || gameManager.totalWarMode) 
        && !gameManager.NPCInWaypoint(npc, gameManager.waypointManager.GetEquipo(npc))) {
            npc.CambiarEstado(npc.estadoDefensa);
            return true;
        }
        return false;
    }

    protected bool ComprobarCaptura(GameManager gameManager, NPC npc) {
        if(npc.user)
            return false;
        if ((!npc.patrol || npc.patrol && gameManager.totalWarMode) && 
        UnitsManager.EnemigosCerca(npc) == 0 && npc.health > npc.menosVida) {

            if (gameManager.AliadosCapturando(npc) >= npc.minAliadosCaptura) {
                npc.CambiarEstado(npc.estadoCaptura);
                return true;
            }
        }
        return false;
    }

    protected bool ComprobarEscapar(NPC npc) {
        if(npc.user)
            return false;
        if (npc.gameManager.totalWarMode)
            return false;
        
        if (npc.health <= npc.menosVida || UnitsManager.EnemigosCerca(npc) >= npc.numEnemigosEscape) {
            npc.CambiarEstado(npc.estadoEscapar);
            return true;
        }
        return false;
    }

    protected bool ComprobarAtaqueRangoMedico(NPC npc) {
        if(npc.user)
            return false;
        if (npc.tipo == NPC.TipoUnidad.Medic) {
            NPC aliado = UnitsManager.ElegirAliado(npc);
            if (aliado != null) {
                npc.CambiarEstado(npc.estadoAtaqueRango, aliado);
                return true;
            }
        }
        return false;
    }

    protected bool ComprobarAtaqueRangoMelee(NPC npc) {
        if(npc.user)
            return false;
        List<NPC> enemigos = UnitsManager.EnemigosEnRango(npc);
        if (enemigos != null && enemigos.Count > 0) {
            if (npc.tipo == NPC.TipoUnidad.Brawler || npc.tipo == NPC.TipoUnidad.Medic) {
                    if (!npc.gameManager.InCuracion(npc) && !npc.gameManager.InCuracion(enemigos[0])) {
                        npc.CambiarEstado(npc.estadoAtaqueMelee, enemigos[0]);
                        return true;
                    }
            } else if (npc.tipo == NPC.TipoUnidad.Ranged) {

                foreach (NPC en in enemigos) {
                    float distancia = Vector3.Distance(npc.agentNPC.Position, en.agentNPC.Position);
                    if (distancia <= npc.rangedRange && !npc.gameManager.InCuracion(npc) && !npc.gameManager.InCuracion(en)) {

                        npc.CambiarEstado(npc.estadoAtaqueRango, en);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    protected bool ComprobarUser(NPC npc) {
        if(npc.user){
            npc.CambiarEstado(npc.estadoUsuario);
            return true;
        }   
        return false;
    }



}
