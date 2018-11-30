using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotSpawner : MonoBehaviour {
    public GameObject projectilePrefab;

    public void SpawnProjectile(bool flipDirection) {
        GameObject go = (GameObject)Instantiate(projectilePrefab);
        go.transform.SetParent(null);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation * (flipDirection ? Quaternion.Euler(0, 180, 0) : Quaternion.identity);
    }
}
