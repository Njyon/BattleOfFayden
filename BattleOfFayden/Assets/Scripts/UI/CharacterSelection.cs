using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public GameObject characterSelectionMenu;
    public Color outlineColor;
    public float outlineSize;

    private void OnMouseDown()
    {
        var selectionMenu = characterSelectionMenu.GetComponent<CharacterSelectionMenu>();
        selectionMenu.SelectCharacter(this.gameObject.name == "Ape" ?
            CharacterSelectionMenu.CharacterID.Champion01 :
            CharacterSelectionMenu.CharacterID.Champion02);
    }
    
    private void OnMouseOver()
    {
        var rendererList = GetComponentsInChildren<Renderer>();
        foreach (var renderer in rendererList)
        {
            renderer.material.SetColor("_OutlineColor", outlineColor);
            renderer.material.SetFloat("_OutlineWidth", outlineSize);
        }
    }

    private void OnMouseExit()
    {
        var rendererList = GetComponentsInChildren<Renderer>();
        foreach (var renderer in rendererList)
        {
            renderer.material.SetFloat("_OutlineWidth", 0.0f);
        }
    }
}
