using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideColliderColor : MonoBehaviour
{
    private TilemapRenderer tilemapRender;
    void Start()
    {
        tilemapRender = GetComponent<TilemapRenderer>();
    }

    
    void Update()
    {
        tilemapRender.enabled = false;
    }
}
