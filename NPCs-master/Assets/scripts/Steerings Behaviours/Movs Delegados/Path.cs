using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] 
    private float radio; 
    [SerializeField] public List<GameObject> nodos = new List<GameObject>();
         
    public void nuevoNodo(GameObject go) {      //metemos un nuevo nodo al camino
            nodos.Add(go);
    }

    public void ClearPath() {       // limpiar el camino
        foreach (GameObject n in nodos){
            Destroy(n);
        }
        nodos.Clear();
    }

    public int Length() {           //numero nodos del camino
        return nodos.Count;
    }

    public float Radio{         //getter y setter para el radio
        get => radio;
        set {radio = value;}
    }
    public Vector3 GetPosition(int valor) {
        // si el camino esta vacio pues devolvemos la misma posicion
            if (nodos.Count == 0)
                return transform.position;
            
            // Devuelve el ultimo punto si no hay mas despues
            if (valor >= nodos.Count){
                return nodos[nodos.Count - 1].transform.position;
            }

            // Devuelve el primer punto si no hay mas antes de el
            if (valor < 0)
                return nodos[0].transform.position;


            return nodos[valor].transform.position;

    }
    public bool EndOfThePath(int actualPosicion){
        return actualPosicion >= nodos.Count;      //para ver si es el ultimo nodo
    }
    public int GetParam(Vector3 personaje, int actualPosicion) {

            // si no hay camino , devuelve la misma posicion
            if (nodos.Count <= 1) {
                return actualPosicion;
            }
            // si no hay camino , pero nos pasamos, simplemente devolvemos el ultimo para evitar problemas
            if (actualPosicion >= nodos.Count) {
                return nodos.Count - 1;
            }
            
            // encontramos el punto mas cercano, el siguiente y el anterior
            float anterior, actual, siguiente; // a, b, c

            // principio, no hay anteriores
            if (actualPosicion == 0) {
                anterior = float.MaxValue;
                siguiente = Vector3.Distance(personaje, nodos[actualPosicion + 1].transform.position);
            }
            // final, no hay posteriores
            else if (actualPosicion == nodos.Count - 1) {
                anterior = Vector3.Distance(personaje, nodos[actualPosicion - 1].transform.position);
                siguiente = float.MaxValue;
            }
            else {
                siguiente = Vector3.Distance(personaje, nodos[actualPosicion + 1].transform.position);
                anterior = Vector3.Distance(personaje, nodos[actualPosicion - 1].transform.position);
            }

            actual = Vector3.Distance(personaje, nodos[actualPosicion].transform.position);
            //en funcion de la distancia entre nodos devolvemos la posicion que seguira

            if (actual <= anterior && actual <= siguiente){
                return actualPosicion;
            }

            if (siguiente <= anterior && siguiente < radio){
                return actualPosicion + 1;
            }
            if (anterior <= siguiente && anterior < radio)
                return actualPosicion - 1;


            return actualPosicion;
        }
}

