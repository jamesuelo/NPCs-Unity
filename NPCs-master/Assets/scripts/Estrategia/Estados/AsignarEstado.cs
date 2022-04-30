using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AsignarEstado : Estado
{

    public override void EntrarEstado(NPC npc) { }

    public override void SalirEstado(NPC npc) { }

    public override void Accion(NPC npc) { }

    public override void Ejecutar(NPC npc)
    {
        Accion(npc);
        ComprobarEstado(npc);
    }

    public override void ComprobarEstado(NPC npc)
    {
        if (npc.GetComponent<Face>() != null)
        {
            npc.GetComponent<Face>().target = null;
            npc.GetComponent<Face>().aux = null;
        }
        GameManager gameManager = npc.gameManager;
        //comprobamos si esta muerto
        if (ComprobarMuerto(npc))
            return;
        //comprobamos si debe escapar
        if (ComprobarEscapar(npc))
            return;
        //comprobamos si siendo un medico puede hacer curar a alguien
        if (ComprobarAtaqueRangoMedico(npc))
            return;
        //comprobamos si debemos defender
        if (ComprobarDefensa(gameManager, npc))
            return;
        //comprobamos si podemos capturar la base contraria
        if (ComprobarCaptura(gameManager, npc))
            return;
        //comprobamos si podemos atacar a distancia larga o corta
        if (ComprobarAtaqueRangoMelee(npc))
            return;
        //comprobamos si podemos hacer patrulla
        if (npc.patrol)
            npc.CambiarEstado(npc.estadoPatrullar);
        else
            //si no vagamos por el mapa
            npc.CambiarEstado(npc.estadoVagar);

    }

}