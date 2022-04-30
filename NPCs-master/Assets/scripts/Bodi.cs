using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodi : MonoBehaviour
{
    public bool form;       //para indicar si forma parte de una formacion
    public bool llegar;     //booleano usado para el movimeinto de las formaciones
    public bool nuevoArrive;    //si se le añadio un nuevo arrive
    public float speed;         //velocidad que tiene
    public float mass;          //masa del agente
    public float maxSpeed;      //velocidad maxima
    public float maxRotation;      //rotacion maxima
    public float maxAcceleration;   //maxima aceleracion
    public float maxAngularAcc;     //maxima aceleracion angular
    [SerializeField]
    public float orientation;       // orientacion del personaje
    private float rotation;
    public Vector3 velocity;           //Vector de volcidad del agente
    public Vector3 acceleration;        //vector de aceleracion del agente

    public float Orientation        //getter y setter de la orientacion
    {
        get => orientation;
        set{ orientation = value;}
    }

    public Vector3 Velocity         //getter y setter de la velocidad
    {
        get
        {
            if (velocity.magnitude > maxSpeed)
                velocity = velocity.normalized * maxSpeed;
            if (velocity.magnitude < 0.1)
                velocity = Vector3.zero;
            return velocity;
        }
        set
        {
            velocity = value;
            if (velocity.magnitude > maxSpeed)
                velocity = velocity.normalized * maxSpeed;
            if (velocity.magnitude < 0.1)
                velocity = Vector3.zero;
        }
    }
    public float Rotation           //getter y setter de la rotacion
    {
        get
        {
            if (Mathf.Abs(rotation) < 0.1)
                rotation = 0f;
            
            return rotation;
        }
        set
        {
            if (Mathf.Abs(rotation) < 0.1)
                rotation = 0f;
            rotation = value;
        }
    }
    public Vector3 Position         //getter y setter de la posicion
    {
        get => transform.position;
        set => transform.position = value;
    }
    public float MaxAngularAcc => maxAngularAcc;

    public float MaxRotation => maxRotation;

    //funcion para pasar de posicion a angulo
    public float PositionToAngle(Vector3 pos)
    {
        return Mathf.Atan2(pos.x, pos.z) * Mathf.Rad2Deg;
    }
    //funcion para pasar de angulo a posicion
    public Vector3 AngleToPosition(float angle)
    {
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }
    //para sacar el vector director
    public Vector3 directionToTarget(Vector3 pos)
    {
        Vector3 vecDir;
        vecDir = pos - this.transform.position;
        return vecDir;
    }
    //para sacar el angulo dado el vector direccion
    public float Heading(Vector3 pos)
    {
        return PositionToAngle(directionToTarget(pos));
    }
    //funcion establecida para el kinematic que no se ha usado
    public float nuevaOrientacion(float orientacion, Vector3 velocidad)
    {
        if (velocidad != Vector3.zero)
            return Mathf.Atan2(-velocidad.x, velocidad.z);

        return orientacion;
    }
}
