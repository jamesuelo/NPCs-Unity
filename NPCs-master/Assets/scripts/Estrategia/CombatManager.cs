using UnityEngine;

public static class CombatManager {
    
    public static void AtaqueMelee(NPC attacker, NPC target) {
        int critico = Random.Range(0, 50);
        if (critico == 45) {
            // ataque critico
            target.health -= attacker.meleeDamageCrit;
        }
        else 
            // ataque basico
            target.health -= attacker.meleeDamage;
    }

    public static void AtaqueRango(NPC attacker, NPC target) {
        int critico = Random.Range(0, 50);
        if (critico >= 45) {
            // ataque critico
            if (target.team == attacker.team) 
                target.health += attacker.rangedDamageCrit;
            
            else 
                target.health -= attacker.rangedDamageCrit;
            
        }
        else {
            // ataque basico
            if (target.team == attacker.team) 
                target.health += attacker.rangedDamage;
            else 
                target.health -= attacker.rangedDamage;
            
        }
    }


}