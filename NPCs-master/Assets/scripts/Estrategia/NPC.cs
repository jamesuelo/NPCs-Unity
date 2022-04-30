using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum TipoUnidad
    {
        Brawler,
        Ranged,
        Medic
    }

    public enum Equipo
    {
        Spain,
        France
    }

    public bool user;       // para saber si tiene que entrar a estado user

    //######REFERENCIAS A OBJETOS#########
    public Grid gridMap;
    public GameManager gameManager;
    public AgentNPC agentNPC;
    public SimplePropagator simplePropagator;
    public PathFinding pf;


    //##########INFO DE LA UNIDAD###########
    public TipoUnidad tipo;
    public Equipo team;
    public Vector3 startPosition;

    public Nodo nodoActual;
    public Nodo anteriorNodo;
    public int radio = 1;
    public float influencia = 1f;
    //#########CARACTERISTICAS DE LA UNIDAD###########
    public float health;
    public int meleeDamage;
    public int meleeDamageCrit;
    public float rangoMelee;
    public float meleeAttackSpeed;
    public int rangedDamage;
    public int rangedDamageCrit;
    public float rangedRange;
    public float rangedAttackSpeed;
    public float healthy;
    //#############Pesos segun los modos#############
    public int aliadosAtacantes;
    public float menosVida;
    public float maxVida;
    public int aliadosDefendiendo;
    public int capturar;
    public int numEnemigosEscape;
    public int minAliadosCaptura;
    public int maxEnemigosMelee;
    public int minAliadosMelee;
    public int enemigosEscapeBase;
    public int enemigosMeleeBase;
    public int aliadosCapturaBase;
    public int minAliadosMeleeBase;
    public int menosVidaBase;
    //############ PATROL ###############
    public bool patrol;
    public Transform puntoPatrullaInicial;
    public Transform puntoPatrullaFin;
    //######### Estados###########

    [SerializeField]
    private Estado currentState;
    public Captura estadoCaptura;
    public Defender estadoDefensa;
    public Escapar estadoEscapar;
    public Curar estadoCuracion;
    public AsignarEstado estadoAsignado;
    public AtaqueMelee estadoAtaqueMelee;
    public Patrullar estadoPatrullar;
    public AtaqueRango estadoAtaqueRango;
    public Muerto estadoMuerto;
    public Vagar estadoVagar;
    public User estadoUsuario;
    public bool IsDead => currentState == estadoMuerto;
    //Función que sirve para cambiar el estado del NPC
    void Start()
    {
        agentNPC = GetComponent<AgentNPC>();
        simplePropagator = GetComponent<SimplePropagator>();
        Initialize();
    }
    protected void Initialize() {       //inicializamos los estados 
        currentState = null;
        estadoCaptura = new Captura();
        estadoDefensa = new Defender();
        estadoEscapar = new Escapar();
        estadoCuracion = new Curar();
        estadoAsignado = new AsignarEstado();
        estadoAtaqueMelee = new AtaqueMelee();
        estadoPatrullar = new Patrullar();
        estadoAtaqueRango = new AtaqueRango();
        estadoMuerto = new Muerto();

        estadoUsuario = new User();
        estadoVagar = new Vagar();
        health = maxVida;

        startPosition = agentNPC.Position;
        CambiarEstado(estadoAsignado);
    }


    void Update()
    {
        if (currentState != null){
            currentState.Ejecutar(this);
        }
        agentNPC.maxSpeed =  gridMap.GetNodoPosicionGlobal(agentNPC.Position).SpeedMultiplier(tipo);
        nodoActual = gridMap.GetNodoPosicionGlobal(agentNPC.Position);

        if (nodoActual.walkable)
            anteriorNodo = nodoActual;
    }

    public void CambiarEstado(Estado newState, NPC objective = null)
    {
        if (currentState != null && currentState != newState)
            currentState.SalirEstado(this);

        newState.SetObjective(objective);
        if (currentState == null || currentState != newState)
        {
            currentState = newState;
            currentState.EntrarEstado(this);
        }
    }

    public void DispararModoOfensivo()       //establecer los cambios de las caracteristicas en modo ofensivo
    {
       numEnemigosEscape = enemigosEscapeBase + 1;
        menosVida = menosVidaBase - 50;
        maxEnemigosMelee = enemigosMeleeBase + 1;
        minAliadosCaptura = 0;
        if (minAliadosMelee > 0)
            minAliadosMelee = minAliadosMeleeBase - 1;
    }

    public void DispararModoDefensivo()     //establecer los cambios de las caracteristicas en modo defensivo
    {
        numEnemigosEscape = enemigosEscapeBase - 1;
        menosVida = menosVidaBase - 50;
        minAliadosCaptura = aliadosCapturaBase;
        if (maxEnemigosMelee > 0)
            maxEnemigosMelee = enemigosMeleeBase - 1;
        minAliadosMelee = minAliadosMeleeBase + 1;
    }

    public void DispararGuerraTotal()       //establecer los cambios de las caracteristicas en modo Guerra Total
    {
        numEnemigosEscape = enemigosEscapeBase + 2;
        menosVida = menosVidaBase - 50;
        maxEnemigosMelee = enemigosMeleeBase + 2;
        minAliadosCaptura = 0;
        if(minAliadosMelee > 0){
            minAliadosMelee = minAliadosMeleeBase - 1;
        }
    }
    public virtual float GetDropOff()
    {
        return influencia;
    }


}
