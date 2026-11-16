using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportController : Interactable
{
    public bool isAutomatic;
    [SerializeField] Transform destination;
    [SerializeField] Transform player;
    [SerializeField] string newLocationName;
    [SerializeField] int newLocationIndex;
    [SerializeField] AudioClip newMusic;
    [SerializeField] bool changeToFreeroamMusic;
    [SerializeField] bool hideMinimap;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isAutomatic)
            {
                Interact();
            }
            else
            {
                playerNearby = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    public override void Interact()
    {
        StartCoroutine(GameManager.instance.FadeToBlack(0.4f));
        StartCoroutine(TeleportDelayed());
    }

    IEnumerator TeleportDelayed()
    {
        yield return new WaitForSeconds(0.3f);
        if (newMusic != null)
        {
            GameManager.instance.musicSource.clip = newMusic;
            GameManager.instance.musicSource.loop = true;
            GameManager.instance.musicSource.Play();
        }
        else if (changeToFreeroamMusic)
        {
            GameManager.instance.PlayFreeroamMusic();
        }
        if (newLocationName != "") GameManager.instance.currentLocationText.text = newLocationName;
        GameManager.instance.currentLocationIndex = newLocationIndex;
        player.position = destination.position;
        if (hideMinimap)
        {
            MinimapController.instance.HideMinimap();
        }
        else if (!MinimapController.instance.minimapDisabled)
        {
            MinimapController.instance.ShowMinimap();
            MinimapController.instance.AnchorCamera();
        }
    }
}
