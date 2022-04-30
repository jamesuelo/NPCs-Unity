#pragma warning disable 0168 
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Grid grid;

    public GameManager gm;

    [SerializeField]
    private Waypoint baseESP;
    [SerializeField]
    private Waypoint baseFRA;
    [SerializeField]
    private Waypoint zonaESP;
    [SerializeField]
    private Waypoint zonaFRA;
    [SerializeField]
    private Waypoint curaESP;
    [SerializeField]
    private Waypoint curaFRA;
    [SerializeField]
    private Waypoint[] coberturas;

    public Waypoint vagarWaypointSPA;
    public Waypoint vagarWaypointFRA;

    public Nodo GetNodoAleatorio(Waypoint wp) {
        int random = Random.Range(0, wp.posiciones.Length);
        return grid.GetNodoPosicionGlobal(wp.posiciones[random].position);
    }

    //Devuelve waypoint de la base del equipo
    public Waypoint GetBase(NPC npc) {
        if (npc.team == NPC.Equipo.France)
            return baseFRA;
        return baseESP;
    }

    //Devuelve waypoint de la base de curación del equipo
    public Waypoint GetCuracion(NPC npc) {
        if (npc.team == NPC.Equipo.France)
            return curaFRA;
        return curaESP;
    }

    //Devuelve waypoint de la zona del equipo
    public Waypoint GetEquipo(NPC npc) {
        if (npc.team == NPC.Equipo.France)
            return zonaFRA;
        return zonaESP;
    }

    //Devuelve waypoint de la zona del equipo rival
    public Waypoint GetRival(NPC npc) {
        if (npc.team == NPC.Equipo.France)
            return zonaESP;
        return zonaFRA;
    }

    //Devuelve el punto de cobertura más cercano
    public Nodo GetCobertura(NPC npc) {
        float minDist = float.MaxValue;
        Vector3 coberturaCercana = Vector3.zero;
        foreach (Waypoint cobertura in coberturas) {
            float distancia = Vector3.Distance(npc.GetComponent<AgentNPC>().transform.position, cobertura.posicion);
            if (distancia < minDist) {
                minDist = distancia;
                coberturaCercana = cobertura.posicion;
            }
        }
        return grid.GetNodoPosicionGlobal(coberturaCercana);
    }
    //Establecemos que se esta capturando una base 
    public void Captura(NPC npc) {
        if (npc.team == NPC.Equipo.France) {
            zonaESP.porcentajeCaptura += 0.25f;
            if (zonaESP.porcentajeCaptura >= 500)
                gm.FranciaGana();
        }
        if (npc.team == NPC.Equipo.Spain){
            zonaFRA.porcentajeCaptura += 0.25f;
            if (zonaFRA.porcentajeCaptura >= 500)
                gm.EspanaGana();
        }
    }
                //si el equipo contrario esta capturando se le bajan puntos al otro equipo
    public void EspCapturando() {
        if (zonaFRA.porcentajeCaptura > 0)
            zonaFRA.porcentajeCaptura -= 2f;
    }

    public void FraCapturando() {
        if (zonaESP.porcentajeCaptura > 0)
            zonaESP.porcentajeCaptura -= 2f;
    }

}
