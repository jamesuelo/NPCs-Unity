using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seleccion : MonoBehaviour
{
    public float tiempoVuelta =10;  //intervalo de tiempo entre timeOut
    private float timeRes;          //tiempo que queda para saltar el timeOut
    public List<GameObject> selectedUnits; //gameobject del grupo que se escoge
    public GameObject selectedUnit;     //gameobject del unico que se escoge
    private GameObject goSel;       //gameObject usado para establecer el lugar donde se moveran los agentes
    private bool mult = false;  //booleano para establecer cuando coger grupo de agentes
    private Agent t;        //agente invisible que sera usado para cuando se seleccione un grupo de agentes
    private Agent t1;       //agente invisible que sera usado para cuando se seleccione un solo agente

    private List<GameObject> agentesRetForms;   //lista con los agentes de formaciones que volveran a la normalidad
    private List<GameObject> agentesReturn; //lista con los gameObject que vuelven a la normalidad

    public List<GameObject> listPuntos = new List<GameObject>();    //variables para pathfinding
    public PathFinding pathFinding;     //Pathfinding para poder mover aquellos que vayan por grid
    public List<GameObject> camino;     //para establecer el camino en el grid

    void Start(){
        timeRes=tiempoVuelta;           //establecemos el tiempo inicial y las listas vacias de los agentes
        agentesReturn = new List<GameObject>();
        agentesRetForms = new List<GameObject>();
    }
    // Update is called once per frame
    void Update()
    {

        timeRes -= Time.deltaTime;      //vamos quitando tiempo hasta que lleguemos a 0 y el tiempo se resetee y llamemos a la funcion que devuelve todo a la normalidad
        if (timeRes <=0.0f){
            timeRes = tiempoVuelta;

            TimeUp();           //salta timeOut
        }
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
                if (hitInfo.collider != null && hitInfo.collider.CompareTag("NPC") && mult)
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
                    }
                    selectedUnit=null; 
                }
                //si colisiona con un agente y no esta siendo pulsado Control, cogemos para un solitario
                else if(hitInfo.collider != null && (hitInfo.collider.CompareTag("NPC") || hitInfo.collider.CompareTag("PathFinding") || hitInfo.collider.CompareTag("PathFindingAStar"))){
                    //cogemos solo un personaje como seleccionado y apagamos el resto
                    if (selectedUnit != null){
                        GameObject g = selectedUnit.transform.Find("Sel").gameObject;
                        g.SetActive(false);
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
                        }
                    }
                    selectedUnits.Clear();  
                    
                }
                //si no pulsamos en ningun NPC, simplemente apagamos todos los seleccionados
                else if (hitInfo.collider != null && !hitInfo.collider.CompareTag("NPC")){
                    if(selectedUnit != null){
                        GameObject s = selectedUnit.transform.Find("Sel").gameObject;
                        s.SetActive(false);                        
                    }
                    if(selectedUnits.Count>0){
                        foreach (GameObject u in selectedUnits)
                        {
                            GameObject p = u.transform.Find("Sel").gameObject;
                            p.SetActive(false);
                        }
                    }                    
                    selectedUnits.Clear();
                    selectedUnit=null;
                    
                }
        
            }
    }

    public void DirigirLugar(){
            //si seleccionamos un lugar y el elegido tiene pathfindig, lo realiza
            if (selectedUnit != null && (selectedUnit.tag == "PathFinding" || selectedUnit.tag == "PathFindingAStar") ){
                Pathfinding();
            } else{

            // Comprobamos si el ratón golpea a algo en el escenario.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {


                // Si lo que golpea es un punto del terreno entonces da la orden a todas las unidades NPC
                if (hitInfo.collider != null && hitInfo.collider.CompareTag("Terrain") && (selectedUnit != null || selectedUnits.Count > 0))
                {

                    //para el grupo de agentes, establecemos un nuevo target en el sitio escogido con un radio
                    if(selectedUnits.Count>0){
                        goSel = new GameObject("Seleccion Grupo");
                        Agent invisible = goSel.AddComponent<Agent>();
                        t = invisible;
                        Vector3 newTarget = hitInfo.point;
                        t.transform.position = newTarget;
                        t.transform.position += new Vector3 (0,1.3f,0);
                        t.extRadius=2;
                        t.intRadius=2;

                        timeRes = tiempoVuelta;                     //reseteamos tiempo
                        foreach (GameObject npc in selectedUnits)
                        {
                            //cogemos los nuevos comportamiento del arrive y de los arbitros
                            AgentNPC n = npc.GetComponent<AgentNPC>();
                            ArriveAcceleration d = npc.GetComponent<ArriveAcceleration>();
                            BlendedSteering l = npc.GetComponent<BlendedSteering>();
                            AgentNPC x = npc.GetComponent<AgentNPC>();
                            //si un esocgido es un lider, movemos a partir de el toda la formacion que tenga
                            if (x.form && npc.name == "lider"){
                                x.llegar = true;
                                d = npc.GetComponent<ArriveAcceleration>();
                                d.target = t;
                                Align c = npc.GetComponent<Align>();
                                c.target = t;
                            }// si no es el lider, rompe la formacion para ir al lugar destinado
                             else if (x.form && npc.name != "lider"){
                                x.form = false;
                                npc.transform.parent.gameObject.GetComponent<CircleFixed>().agentes.Remove(n);
                                agentesRetForms.Add(npc);
                            }
                            //si no tenian arrive antes, se les añade, se les apaga el resto de componentes de comporatmiento, sea de arbitro o no, y lo activamos
                            if(d == null){

                                if(l == null)       //se comprueba los steerings del arbitro ponderado y se eliminan temporalmente

                                {
                                foreach (SteeringBehaviour i in npc.GetComponents<SteeringBehaviour>())
                                {
                                    n.SteeringList.Remove(i);
                                }
                                }else {
                                    l.behaviours.Clear();
                                }

                                x.nuevoArrive=true;
                                npc.AddComponent<ArriveAcceleration>();
                                d = npc.GetComponent<ArriveAcceleration>();
                                d.target = t;
                                agentesReturn.Add(npc);

                                if(l != null){
                                    l.behaviours.Add(d);
                                }
                            }
                            //si ya lo teneian, simplemente se le establece el nuevo target
                            else
                            {
                                d.target = t;
                            }

                            if(!n.SteeringList.Contains(d) && l == null)
                            {
                                n.SteeringList.Add(d);
                            }
                        }                
                    } else{
                        //de la misma manera creamos todo lo anterior pero para la unica unidad escogida.
                            goSel = new GameObject("Seleccion Solo");
                            Agent invisible = goSel.AddComponent<Agent>();
                            t1 = invisible;
                            Vector3 newTarget1 = hitInfo.point;
                            t1.transform.position = newTarget1;
                            t1.transform.position += new Vector3 (0,1.3f,0);
                            t1.extRadius=2;
                            t1.intRadius=2;

                            timeRes = tiempoVuelta;                     //reseteamos tiempo

                            ArriveAcceleration e = selectedUnit.GetComponent<ArriveAcceleration>();     //sacamos el arrive y el agenteNPC
                            AgentNPC x = selectedUnit.GetComponent<AgentNPC>();
                            if (x.form && selectedUnit.name == "lider"){                                //si es el lider de la formacion dirigir la formacion entera
                                x.llegar = true;
                                e = selectedUnit.GetComponent<ArriveAcceleration>();
                                e.target = t1;
                                Align c = selectedUnit.GetComponent<Align>();
                                c.target = t1;
                            } else if (x.form && selectedUnit.name != "lider"){
                                x.form = false;
                                selectedUnit.transform.parent.gameObject.GetComponent<CircleFixed>().agentes.Remove(x);
                                agentesRetForms.Add(selectedUnit);
                            }                                                       //si pertenece a la formacion pero no es el lider, que se salga de la formacion

                            BlendedSteering m = selectedUnit.GetComponent<BlendedSteering>();
                            AgentNPC n = selectedUnit.GetComponent<AgentNPC>();         //se añade a la lista de steering behaviours paraque tengan ese comportamiento
                            if(e == null){                                                              //si no tiene arrive, se le anade con el nuevo target y se le pone nuevoArrive a true para restablecerlo despues
                                

                                if(m == null)       //se comprueba los steerings del arbitro ponderado y se eliminan temporalmente

                                {
                                    foreach (SteeringBehaviour i in selectedUnit.GetComponents<SteeringBehaviour>())
                                    {
                                        n.SteeringList.Remove(i);
                                    }
                                }else {
                                    m.behaviours.Clear();
                                }

                                
                                selectedUnit.AddComponent<ArriveAcceleration>();
                                e = selectedUnit.GetComponent<ArriveAcceleration>();
                                x.nuevoArrive=true;
                                e.target = t1;

                                if(m != null){
                                    m.behaviours.Add(e);
                                }

                                agentesReturn.Add(selectedUnit);

                            }
                            else                                     //si ya lo tenia, y nuevoArrive esta a true solo se le asigna el nuevo target, si no se le anade a la lista del target y el gameobject
                            {
                                e.target = t1;
                            }
                            if(!n.SteeringList.Contains(e) && m == null)
                            {
                                n.SteeringList.Add(e);
                            }  
                    }
                }
        
            }
    }

    }


    public void TimeUp(){

        //devolvemos primero cada uno de los agentes que eran de una formacion a sus respectivas formaciones
        foreach (GameObject h in agentesRetForms)
        {
            AgentNPC v = h.GetComponent<AgentNPC>();
            if(v.form ==false){
                h.transform.parent.gameObject.GetComponent<CircleFixed>().agentes.Add(v);
                v.form =true;
            }
        }
        agentesRetForms.Clear();
        //de igual manera devolvemos a aquellos que no tuvieran arrive como componentes, para ello, eliminando arrive y volviendo a activar el resto de comprotamientos
        foreach (GameObject g in agentesReturn)
        {
            BlendedSteering m = g.GetComponent<BlendedSteering>();
            AgentNPC n = g.GetComponent<AgentNPC>();
            ArriveAcceleration e = g.GetComponent<ArriveAcceleration>();
            Debug.Log(g.transform.name);
            if (m == null){                 //para los que no tuviesen arbitro ponderado
                if (n.nuevoArrive)
                {
                    n.SteeringList.Remove(e);
                    Destroy(e);
                    n.nuevoArrive = false;
                    foreach (SteeringBehaviour i in g.GetComponents<SteeringBehaviour>())
                    {
                        if(i != e)
                            n.SteeringList.Add(i);
                    }

                }
            } else{             //para los que tuviesen arbitro ponderado
                if (n.nuevoArrive)
                {
                    m.behaviours.Remove(e);
                    Destroy(e);
                    n.nuevoArrive = false;
                    foreach (SteeringBehaviour i in g.GetComponents<SteeringBehaviour>())
                    {
                        if(i != e && i != m)
                            m.behaviours.Add(i);
                    }
                }   
            }
        }
        agentesReturn.Clear();
        
    }

    void Pathfinding(){
        //simplemente ejecutamos la funcion pathfinding para que encuentre el camino mas corto hasta la posicion seleccionada usando LRTA*
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            PathFollowing pf = new PathFollowing() ;
            Path path  = new Path();
            pathFinding = selectedUnit.GetComponent<PathFinding>();
            
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                if (hit.transform != null && hit.transform.tag != "Muro" && hit.transform.tag != "Agua")        //establecemos como punto de destino el seleccionado por el raton, y calculamos camino hasta ese punto
                {
                    AgentNPC n = selectedUnit.GetComponent<AgentNPC>();
                    listPuntos = pathFinding.EstablecerNodoFinal(n);            //establecemos el ultimo nodo
                    if (selectedUnit.GetComponent<PathFollowing>() == null){
                        PathFinding pathFinding = selectedUnit.GetComponent<PathFinding>();
                        pf =  selectedUnit.AddComponent(typeof(PathFollowing)) as PathFollowing;        //establecemos todo el camino y asignamos a pathfollowing
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
    }
}


