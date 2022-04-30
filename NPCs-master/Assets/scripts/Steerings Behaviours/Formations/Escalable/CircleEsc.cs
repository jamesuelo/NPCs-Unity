#pragma warning disable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleEsc : FormationManager
{

    private GameObject puntoSeleccion;
    void Start() {
        puntoSeleccion = new GameObject("F1");
        puntoSeleccion.AddComponent<Agent>();
        asignaciones = new List<AgentNPC>();
        centro = new GameObject("CenterCir");
        centro.AddComponent<AgentNPC>();
        //metemos los agentes que podemos para la formacion
        foreach (AgentNPC a in agentes) {
            asignaciones.Add(a);
            GameObject ForC = new GameObject("FCE " + asignaciones.Count);
            Agent invisible = ForC.AddComponent<Agent>() as Agent;
            invisible.extRadius=1.5f;
            invisible.intRadius=1.5f;
            a.form = true;
        }
        UpdateSlots();
    }

    void Update(){
        foreach (AgentNPC a in asignaciones)
        {
            if (a.llegar)
                puntoSeleccion.GetComponent<Agent>().transform.position = a.GetComponent<ArriveAcceleration>().target.transform.position;; 
            UpdateSlots();
        }

    }
    public override void UpdateSlots() {

        AgentNPC anchor = GetAnchor();

        anchor.orientation *= -1; 
        
        for (int i = 0; i < asignaciones.Count; i++) {
            Vector3 pos = GetPosition(i);
            float ori = GetOrientation(i);

            var result = new Vector3(Mathf.Cos(anchor.orientation) * pos.x -  Mathf.Sin(anchor.orientation) * pos.z,
                0,
                Mathf.Sin(anchor.orientation) * pos.x + Mathf.Cos(anchor.orientation) * pos.z);
            
            GameObject a = GameObject.Find("FCE " + (i+1));
            Agent invisible = a.GetComponent<Agent>();

            invisible.transform.position =anchor.transform.position + result;
            invisible.orientation =-(anchor.orientation + ori);
            AgentNPC lider = asignaciones[0];
            if((Mathf.Abs(lider.transform.position.x) - Mathf.Abs(puntoSeleccion.transform.position.x) < lider.intRadius) && (Mathf.Abs(a.transform.position.z) - Mathf.Abs(puntoSeleccion.transform.position.z) < lider.intRadius))
                lider.llegar = false;
            //Poner los steerings en tránsito y parados.
            if (lider.llegar && i != 0 && lider.velocity.magnitude >= 1){
                asignaciones[i].GetComponent<ArriveAcceleration>().target =lider;
                asignaciones[i].GetComponent<Align>().target = lider;
                lider.GetComponent<Face>().aux = puntoSeleccion.GetComponent<Agent>();
                lider.GetComponent<Face>().target = puntoSeleccion.GetComponent<Agent>();
            }else if (!lider.llegar && lider.velocity.magnitude < 1){
                asignaciones[i].GetComponent<ArriveAcceleration>().target = invisible;
                asignaciones[i].GetComponent<Align>().target = invisible;
                lider.GetComponent<Face>().target = null;
                lider.GetComponent<Face>().aux = null;
            }

        }
    }

        // calcula la orientacion
    public override float GetOrientation(int numero) {

        float anguloOri = numero / (float)asignaciones.Count * Mathf.PI * 2;
        float resultado = anguloOri- Mathf.PI/2;

        return resultado;
    }

    // calcula la posicion
    public override Vector3 GetPosition(int numero) {

        float anguloPos = numero / (float)asignaciones.Count * Mathf.PI * 2;
        float radioPos = radio / Mathf.Sin(Mathf.PI / asignaciones.Count);
        Vector3 resultado = new Vector3(radioPos * Mathf.Cos(anguloPos),0,radioPos * Mathf.Sin(anguloPos));
        return resultado;
    }

    public AgentNPC GetAnchor(){
        AgentNPC anchor = centro.GetComponent<AgentNPC>();
        anchor.transform.position = Vector3.zero;
        anchor.orientation =0;

        Vector3 posBase = Vector3.zero;

        for (int i = 0; i < asignaciones.Count; i++) {
            Vector3 pos = GetPosition(i);
            float ori = GetOrientation(i);
            anchor.transform.position += pos;
            anchor.orientation += ori;

            posBase += asignaciones[i].transform.position;
        }

        int num = asignaciones.Count;
        anchor.transform.position /= num;
        anchor.orientation /= num;

        posBase /= num;
        anchor.transform.position += posBase;


        return anchor;
    }

    public override bool SupportsSlots(int slotCount) {   
        return true;
    }
}
