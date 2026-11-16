using _193326;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public static MinimapController instance;
    public Camera minimapCamera;
    [SerializeField] GameObject minimap;
    public GameObject[] minimapAnchors;
    public int[] minimapSizes;
    public bool[] isAnchored;
    [NonSerialized] public bool minimapVisible = true;
    [NonSerialized] public bool minimapDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        minimapVisible = true;
        minimap.SetActive(minimapVisible);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnchorCamera()
    {
        int closestAnchorIndex = 0;
        float closestDistance = float.MaxValue;
        Vector2 playerPos = GameManager.instance.player.transform.position;
        for (int i = 0; i < minimapAnchors.Length; i++)
        {
            float distance = Vector2.Distance(playerPos, minimapAnchors[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAnchorIndex = i;
            }
        }
        Debug.Log("Anchoring minimap camera to closest anchor: " + minimapAnchors[closestAnchorIndex].name);
        Vector3 camPos = minimapCamera.transform.position;
        Vector3 anchorPos = minimapAnchors[closestAnchorIndex].transform.position;
        camPos.x = anchorPos.x;
        camPos.y = anchorPos.y;
        if (isAnchored[closestAnchorIndex])
        {
            minimapCamera.transform.position = camPos;
            minimapCamera.GetComponent<Camera2DFollow>().enabled = false;
        }
        else
        {
            minimapCamera.GetComponent<Camera2DFollow>().enabled = true;
        }
        minimapCamera.orthographicSize = minimapSizes[closestAnchorIndex];
        //minimapCamera.orthographicSize = 25;
    }

    public void ShowMinimap()
    {
        minimapVisible = true;
        minimap.SetActive(true);
    }

    public void HideMinimap()
    {
        minimapVisible = false;
        minimap.SetActive(false);
    }
    public void EnableMinimap()
    {
        minimapDisabled = false;
    }

    public void DisableMinimap()
    {
        minimapDisabled = true;
        HideMinimap();
    }
}
