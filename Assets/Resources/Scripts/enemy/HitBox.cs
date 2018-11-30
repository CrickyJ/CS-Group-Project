using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {
    public Enemy parent;

    public void ApplyDamage(float amount) {
        parent.ApplyDamage(amount);
    }
}
