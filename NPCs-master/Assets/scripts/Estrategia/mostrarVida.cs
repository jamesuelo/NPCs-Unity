using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mostrarVida : MonoBehaviour
{

    public Text vida_melee1E;
    public Text vida_melee2E;
    public Text vida_ranged1E;
    public Text vida_ranged2E;
    public Text vida_medicE;
    private float vida1 = 1000;
    private float vida2 = 1000;
    private float vida3 = 1000;
    private float vida4 = 1000;
    private float vida5 = 1000;


    public Text vida_melee1F;
    public Text vida_melee2F;
    public Text vida_ranged1F;
    public Text vida_ranged2F;
    public Text vida_medicF;
    private float vida6 = 1000;
    private float vida7 = 1000;
    private float vida8 = 1000;
    private float vida9 = 1000;
    private float vida10 = 1000;




    // Update is called once per frame
    void Update()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("PathFindingAStar");

        foreach (GameObject g in npcs)
        {
            cambiarVida(g);
        }
    }

    public void cambiarVida(GameObject f){
        NPC n = f.GetComponent<NPC>();
        switch(f.name){
            case "RangedFra 1":
                if(n.health != vida8)
                {
                    if(n.health <= 0)
                        vida8 = 0;
                    else
                        vida8 = n.health;
                    vida_ranged1F.text = vida8.ToString();
                }
                break;
            case "RangedFra 2":
                if(n.health != vida9)
                {
                    if(n.health <= 0)
                        vida9 = 0;
                    else
                        vida9 = n.health;
                    vida_ranged2F.text = vida9.ToString();
                } 
                break;
            case "RangedEsp 1": 
                if(n.health != vida3)
                {
                    if(n.health <= 0)
                        vida3 = 0;
                    else
                        vida3 = n.health;
                    vida_ranged1E.text = vida3.ToString();
                }                
                break;
            case "RangedEsp 2":
                if(n.health != vida4)
                {
                    if(n.health <= 0)
                        vida4 = 0;
                    else
                        vida4 = n.health;
                    vida_ranged2E.text = vida4.ToString();
                } 
                break;
            case "MeleeFra 1":
                if(n.health != vida6)
                {
                    if(n.health <= 0)
                        vida6 = 0;
                    else
                        vida6 = n.health;
                    vida_melee1F.text = vida6.ToString();
                }
                break;
            case "MeleeFra 2":
                if(n.health != vida7)
                {
                    if(n.health <= 0)
                        vida7 = 0;
                    else
                        vida7 = n.health;
                    vida_melee2F.text = vida7.ToString();
                } 
                break;
            case "MeleeEsp 1":
                if(n.health != vida1)
                {
                    if(n.health <= 0)
                        vida1 = 0;
                    else                    
                        vida1 = n.health;
                    vida_melee1E.text = vida1.ToString();
                } 
                break;
            case "MeleeEsp 2":
                if(n.health != vida2)
                {
                    if(n.health <= 0)
                        vida2 = 0;
                    else
                        vida2 = n.health;
                    vida_melee2E.text = vida2.ToString();
                } 
                break;
            case "MedicoEsp":
                if(n.health != vida5)
                {
                    if(n.health <= 0)
                        vida5 = 0;
                    else
                        vida5 = n.health;
                    vida_medicE.text = vida5.ToString();
                } 
                break;
            case "MedicoFra":
                if(n.health != vida10)
                {
                    if(n.health <= 0)
                        vida10 = 0;
                    else
                        vida10 = n.health;
                    vida_medicF.text = vida10.ToString();
                } 
                break;
        }

    }
}
