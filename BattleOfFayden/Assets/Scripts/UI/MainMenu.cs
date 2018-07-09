using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject inputText;
    public GameObject inputField;

	void Start () {
        inputField.GetComponent<InputField>().text = PlayerPrefs.GetString("PlayerName");
	}
	
	public void OnStartPressed()
    {
        PlayerPrefs.SetString("PlayerName", inputText.GetComponent<Text>().text);
        SceneManager.LoadSceneAsync((int)SceneAlias.MainMenu, LoadSceneMode.Additive);
    }
}
