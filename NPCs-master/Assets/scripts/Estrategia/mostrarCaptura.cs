using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mostrarCaptura : MonoBehaviour
{

    public Text spain;
    public Text france;

    private float p1 = 0;
    private float p2 = 0;






    // Update is called once per frame
    void Update()
    {
        GameObject[] zonas = GameObject.FindGameObjectsWithTag("Zona");

        foreach (GameObject g in zonas)
        {
            cambiarPorcentaje(g);
        }
    }

    public void cambiarPorcentaje(GameObject f){
        Waypoint n = f.GetComponent<Waypoint>();
        switch(f.name){
            case "ZonaFrawp":
                if(n.porcentajeCaptura != p1)
                {
                    if(n.porcentajeCaptura <= 0)
                        p1 = 0;
                    else if(n.porcentajeCaptura >= 500)
                        p1 = 500;
                    else
                        p1 = n.porcentajeCaptura;
                    france.text = p1.ToString();
                }
                break;
            case "ZonaEspwp":
                if(n.porcentajeCaptura != p2)
                {
                    if(n.porcentajeCaptura <= 0)
                        p2 = 0;
                    else if(n.porcentajeCaptura >= 500)
                        p2 = 500;
                    else
                        p2 = n.porcentajeCaptura;
                    spain.text = p2.ToString();
                } 
                break;
        }

    }
}
