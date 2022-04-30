using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FormationManager : MonoBehaviour {
    
    [SerializeField]
    protected float radio;

    //lista de los agentes seleccionados
    [SerializeField]
    protected List<AgentNPC> agentes = new List<AgentNPC>();
    protected GameObject centro;
    protected List<AgentNPC> asignaciones;
    protected Align align;
    protected Face face;



    public void ActualizaPuestos() {
        for (int i = 0; i <  asignaciones.Count; i++) {
            if(asignaciones[i] == null){
                asignaciones[i] = asignaciones[i+1];
            }
            else
                asignaciones[i] = asignaciones[i];
        }
    }

    public void RemoveCharacter(AgentNPC c) {

        asignaciones.Remove(c);
        //ActualizaPuestos();
    }


    public abstract void UpdateSlots();

    public abstract Vector3 GetPosition(int numero);

    public abstract float GetOrientation(int numero);

    public abstract bool SupportsSlots(int slotCount);
    
}
