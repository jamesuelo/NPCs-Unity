using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seleccionEstrategia : MonoBehaviour
{
    public List<GameObject> selectedUnits; //gameobject del grupo que se escoge
    public GameObject selectedUnit;     //gameobject del unico que se escoge
    private bool mult = false;  //booleano para establecer cuando coger grupo de agentes

    public List<GameObject> listPuntos = new List<GameObject>();    //variables para pathfinding
    public PathFinding pathFinding;     //Pathfinding para poder mover aquellos que vayan por grid
    public List<GameObject> camino;     //para establecer el camino en el grid

    // Update is called once per frame
    void Update()
    {
        Selec();                //funcion de seleccion
    }

    private void Selec(){

        //Si pulsamos Control, sera para coger varios individuos para moverse
        mult = Input.GetKey(KeyCode.LeftControl); 
        // si clickamos el boton izq del raton, es para seleccionar individiuos. 
        if (Input.GetMouseButtonUp(0))
        {
            seleccionarPersonajes();
        }  
        //si pulsamos el boton derecho del raton es para mandarlos a algun lugar
        if (Input.GetMouseButtonUp(1)){
            DirigirLugar();
        }
    }


    public void seleccionarPersonajes(){
        
            // Comprobamos si el ratón golpea a algo en el escenario.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))  //el rayCast ha colisionado
            {
                //si colisiona con un agente y esta siendo pulsado Control, cogemos para un grupo
                if (hitInfo.collider != null &&  (hitInfo.collider.CompareTag("PathFinding") || hitInfo.collider.CompareTag("PathFindingAStar")) && mult)
                {
                    //anadimos cada agente que seleccionemos, indicando que estan siendo activados y apagando el agente solitario
                    selectedUnits.Add(hitInfo.collider.gameObject);
                    foreach (GameObject u in selectedUnits)
                    {
                        GameObject p = u.transform.Find("Sel").gameObject;
                        p.SetActive(true);
                        
                    }
                    if(selectedUnit != null){
                        GameObject s = selectedUnit.transform.Find("Sel").gameObject;
                        s.SetActive(false);
                        selectedUnit.GetComponent<NPC>().user = false;                 
                    }
                    selectedUnit=null; 
                }
                //si colisiona con un agente y no esta siendo pulsado Control, cogemos para un solitario
                else if(hitInfo.collider != null && hitInfo.collider.CompareTag("PathFinding") || hitInfo.collider.CompareTag("PathFindingAStar")){
                    //cogemos solo un personaje como seleccionado y apagamos el resto
                    if (selectedUnit != null){
                        GameObject g = selectedUnit.transform.Find("Sel").gameObject;
                        g.SetActive(false);
                        selectedUnit.GetComponent<NPC>().user = false; 
                        selectedUnit=null;
                    }
                    selectedUnit = hitInfo.collider.gameObject;
                    GameObject s = selectedUnit.transform.Find("Sel").gameObject;
                    s.SetActive(true);

                    if(selectedUnits.Count>0){
                        foreach (GameObject u in selectedUnits)
                        {
                            GameObject p = u.transform.Find("Sel").gameObject;
                            p.SetActive(false);

                            u.GetComponent<NPC>().user = false; 
                        }
                    }
                    selectedUnits.Clear();  
                    
                }
                //si no pulsamos en ningun NPC, simplemente apagamos todos los seleccionados
                else if (hitInfo.collider != null && !(hitInfo.collider.CompareTag("PathFinding") || hitInfo.collider.CompareTag("PathFindingAStar"))){
                    if(selectedUnit != null){
                        GameObject s = selectedUnit.transform.Find("Sel").gameObject;
                        s.SetActive(false);    
                        selectedUnit.GetComponent<NPC>().user = false;                      
                    }
                    if(selectedUnits.Count>0){
                        foreach (GameObject u in selectedUnits)
                        {
                            GameObject p = u.transform.Find("Sel").gameObject;
                            p.SetActive(false);
                            u.GetComponent<NPC>().user = false;
                        }
                    }
                    selectedUnits.Clear();
                    selectedUnit=null;
                    
                }
            }
    }

    public void DirigirLugar(){
        bool sameTeam = false;
        bool sameTeam1 = false;
        //si seleccionamos un lugar y el elegido tiene pathfinding, lo realiza
        if (selectedUnit != null && (selectedUnit.tag == "PathFinding" || selectedUnit.tag == "PathFindingAStar") ){
            Pathfinding();
        }
        if (selectedUnits != null){
            foreach (GameObject i in selectedUnits)
            {
                if(i.GetComponent<NPC>().team == NPC.Equipo.Spain){
                    sameTeam = true;
                } else if (i.GetComponent<NPC>().team == NPC.Equipo.France) {
                    sameTeam1 = true;
                }
            }
            if((sameTeam) && (sameTeam1)){}
            else {
                Pathfinding();
            }
        }
    }


    void Pathfinding(){
        //simplemente ejecutamos la funcion pathfinding para que encuentre el camino mas corto hasta la posicion seleccionada.
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            PathFollowing pf = new PathFollowing() ;
            Path path  = new Path();
            if(selectedUnit != null){
                pathFinding = selectedUnit.GetComponent<PathFinding>();
                selectedUnit.GetComponent<NPC>().user = true;
                if (Physics.Raycast(ray, out hit, 1000.0f))
                {
                    if (hit.transform != null && hit.transform.tag != "Muro" && hit.transform.tag != "Agua")
                    {
                        AgentNPC n = selectedUnit.GetComponent<AgentNPC>();
                        listPuntos = pathFinding.EstablecerNodoFinal(n);
                        if (selectedUnit.GetComponent<PathFollowing>() == null){
                            PathFinding pathFinding = selectedUnit.GetComponent<PathFinding>();
                            pf =  selectedUnit.AddComponent(typeof(PathFollowing)) as PathFollowing;
                            path = selectedUnit.AddComponent(typeof(Path)) as Path;
                            pf.path = selectedUnit.GetComponent<Path>();
                            pf.path.Radio = pathFinding.grid.radioNodo;
                            n.SteeringList.Add(pf);
                        }
                        path = selectedUnit.GetComponent<Path>();
                        path.ClearPath();
                        pf = selectedUnit.GetComponent<PathFollowing>();
                        pf.currentPos = 0;
                        for(int i =0 ; i< listPuntos.Count; i ++){
                            path.nuevoNodo(listPuntos[i]);
                        }
                    }
                }
            } else if(selectedUnits != null){
                foreach (GameObject v in selectedUnits)
                {
                    v.GetComponent<NPC>().user = true;
                    pathFinding = v.GetComponent<PathFinding>();
                    if (Physics.Raycast(ray, out hit, 1000.0f))
                    {
                        if (hit.transform != null && hit.transform.tag != "Muro" && hit.transform.tag != "Agua")
                        {
                            AgentNPC n = v.GetComponent<AgentNPC>();
                            listPuntos = pathFinding.EstablecerNodoFinal(n);
                            if (v.GetComponent<PathFollowing>() == null){
                                PathFinding pathFinding = v.GetComponent<PathFinding>();
                                pf =  v.AddComponent(typeof(PathFollowing)) as PathFollowing;
                                path = v.AddComponent(typeof(Path)) as Path;
                                pf.path = v.GetComponent<Path>();
                                pf.path.Radio = pathFinding.grid.radioNodo;
                                n.SteeringList.Add(pf);
                            }
                            path = v.GetComponent<Path>();
                            path.ClearPath();
                            pf = v.GetComponent<PathFollowing>();
                            pf.currentPos = 0;
                            for(int i =0 ; i< listPuntos.Count; i ++){
                                path.nuevoNodo(listPuntos[i]);
                            }
                        }
                    }                    
                }
            }
    }
}


