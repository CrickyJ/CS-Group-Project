using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// every lateupdate, updates the attached SpriteMask object's sprite to be the same 
/// as the attached SpriteRenderer's sprite
/// </summary>
public class SpriteMaskDuplicator : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private SpriteMask spriteMask;
    
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteMask = GetComponent<SpriteMask>();
	}
	
	void LateUpdate () {
        spriteMask.sprite = spriteRenderer.sprite;
	}
}
