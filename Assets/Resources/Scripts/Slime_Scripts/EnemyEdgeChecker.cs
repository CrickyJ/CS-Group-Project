using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEdgeChecker : MonoBehaviour {
    public Enemy enemy;
    private BoxCollider2D col;

    private void Start() {
        col = GetComponent<BoxCollider2D>();
    }

    public int GetOverlaps(ContactFilter2D filter, Collider2D[] results) {
        return col.OverlapCollider(filter, results);
    }
}
