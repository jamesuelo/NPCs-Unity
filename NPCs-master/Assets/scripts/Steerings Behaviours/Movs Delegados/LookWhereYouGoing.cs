using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookWhereYouGoing : Align {
    
    // prueba de scripts
    [SerializeField]
    private GameObject goLook;
    void Start(){
         goLook = new GameObject("goLook");
         target = goLook.AddComponent<Agent>() as Agent;
    }
    
    public override Steering GetSteering(AgentNPC agent) {
        if (agent.Velocity.magnitude == 0){
            target.orientation = agent.orientation;
        }
        target.orientation = Mathf.Atan2(-agent.velocity.x, agent.velocity.z);

        return base.GetSteering(agent);
    }
}
