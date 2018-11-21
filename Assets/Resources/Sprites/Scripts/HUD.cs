using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    public Sprite[] HealthSprites;
    public Image HealthUI;
    private PlayerController player;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        HealthUI.sprite = HealthSprites[(100-player.getHealth())/10];
	}
}
