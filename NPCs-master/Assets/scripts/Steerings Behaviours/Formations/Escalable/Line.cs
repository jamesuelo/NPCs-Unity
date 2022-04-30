using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : FormationManager
{

    /**[SerializeField]
    private float radio;

    [SerializeField]
    private float ranuras;
    //lista de los agentes seleccionados
    [SerializeField]
    private List<AgentNPC> agentes = new List<AgentNPC>();
    private GameObject centro;
    private List<AgentNPC> asignaciones;
**/
    void Start() {
        asignaciones = new List<AgentNPC>();
        centro = new GameObject("Center");
        centro.AddComponent<AgentNPC>();
        //metemos los agentes que podemos para la formacion
        foreach (AgentNPC a in agentes) {
            //if (asignaciones.Count<ranuras){
                asignaciones.Add(a);
                GameObject ForC = new GameObject("FLE " + asignaciones.Count);
                Agent invisible = ForC.AddComponent<Agent>() as Agent;
                invisible.extRadius=1f;
                invisible.intRadius=1f;
                a.form = true;
           // }
        }
        UpdateSlots();
    }
    void Update(){

    }
    public override void UpdateSlots() {

        AgentNPC anchor = GetAnchor();

        anchor.orientation *= -1; 
        
        for (int i = 0; i < asignaciones.Count; i++) {
            Vector3 pos = GetPosition(i);
            float ori = 0;

            var result = new Vector3(Mathf.Cos(anchor.orientation) * pos.x -  Mathf.Sin(anchor.orientation) * pos.z,
                0,
                Mathf.Sin(anchor.orientation) * pos.x + Mathf.Cos(anchor.orientation) * pos.z);

            GameObject a = GameObject.Find("FLE " + (i+1));
            Agent invisible = a.GetComponent<Agent>();

            //if(asignaciones[0].GetComponent<SeekAcceleration>().target != null){
             //   invisible.transform.position =asignaciones[0].GetComponent<SeekAcceleration>().target.transform.position + result;
            //}
            //else{
            invisible.transform.position =anchor.transform.position + result;
            //}
            invisible.orientation =-(anchor.orientation + ori);

            asignaciones[i].GetComponent<ArriveAcceleration>().target = invisible;
            asignaciones[i].GetComponent<Align>().target = invisible;
        }
    }


    // calcula la posicion
    public override Vector3 GetPosition(int numero) {

        Vector3 resultado;
        resultado = new Vector3(numero*radio,0,0);
        return resultado;
    }

    public override float GetOrientation(int numero) {
        return 0;
    }

    public AgentNPC GetAnchor(){
        AgentNPC anchor = centro.GetComponent<AgentNPC>();
        anchor.transform.position = Vector3.zero;
        anchor.orientation =0;

        Vector3 posBase = Vector3.zero;
        float oriBase = 0f;

        for (int i = 0; i < asignaciones.Count; i++) {
            Vector3 pos = GetPosition(i);
            float ori = 0;
            anchor.transform.position += pos;
            anchor.orientation += ori;

            posBase += asignaciones[i].transform.position;
            oriBase += asignaciones[i].orientation;
        }

        // Divide through to get the drift offset
        int num = asignaciones.Count;
        anchor.transform.position /= num;
        anchor.orientation /= num;

        posBase /= num;
        oriBase /= num;
        anchor.transform.position += posBase;
        anchor.orientation += oriBase;

        return anchor;
    }
    public override bool SupportsSlots(int slotCount) {   
        return true;
    }
}
