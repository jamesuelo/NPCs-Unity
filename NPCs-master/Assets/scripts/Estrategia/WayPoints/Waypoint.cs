using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public float porcentajeCaptura = 0;

    public enum WayPointClase {
        BaseESP,
        BaseFRA,
        Cobertura,
        zonaESP,
        zonaFRA,
        curaESP,
        curaFRA
    }

    [SerializeField] private WayPointClase wpClase;

    public Vector3 posicion;
    public Transform[] posiciones;

    void Start() {
        posicion = transform.position;
        if (wpClase == WayPointClase.zonaESP || wpClase == WayPointClase.zonaFRA)
            porcentajeCaptura = 0;
    }
}
