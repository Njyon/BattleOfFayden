using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour {

    [SerializeField]
    private GameObject OptionsPanel, QuitPanel;

    public GameObject escKey, qKey, wKey, spaceKey;
    public Sprite escNormal, qNormal, wNormal, spaceNormal;
    public Sprite escHover, qHover, wHover, spaceHover;
    public GameObject DescriptionBox;
    public Color hoverColor;

    private void Start()
    {
    }

    public void OpenOptionsPanel()
    {
        OptionsPanel.SetActive(true);
        QuitPanel.SetActive(false);
    }

    public void OpenQuitPanel()
    {
        QuitPanel.SetActive(true);
        OptionsPanel.SetActive(false);
    }

    public void SwitchOptionsPanel()
    {
        if (OptionsPanel.activeSelf)
        {
            CloseAllPanels();
            OptionsPanel.SetActive(false);
        } else
        {
            CloseAllPanels();
            OptionsPanel.SetActive(true);
        }

    }



    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (OptionsPanel.activeSelf)
            {
                CloseAllPanels();
            } else
            {
                CloseAllPanels();
                OptionsPanel.SetActive(false);
            }
        }
        DescriptionBox.transform.position = Input.mousePosition;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit, 10000))
        //{
        //    Debug.Log("SUKA");
        //    if (hit.collider == escKey.GetComponent<Collider>())
        //    {
        //        Debug.Log("ESC SUKA");

        //        DescriptionBox.SetActive(true);
        //        DescriptionBox.GetComponent<TextMeshProUGUI>().text = "ESC - Closes the Options-Menu";
        //        DescriptionBox.transform.position = Input.mousePosition;
        //        escKey.GetComponent<Image>().sprite = escHover;

        //        qKey.GetComponent<Image>().sprite = qNormal;
        //        wKey.GetComponent<Image>().sprite = wNormal;
        //        spaceKey.GetComponent<Image>().sprite = spaceNormal;
        //    }
        //    else if (hit.collider == qKey.GetComponent<Collider>())
        //    {
        //        DescriptionBox.SetActive(true);
        //        DescriptionBox.GetComponent<TextMeshProUGUI>().text = "Q - Cast Ability 1";
        //        DescriptionBox.transform.position = Input.mousePosition;
        //        qKey.GetComponent<Image>().sprite = qHover;

        //        escKey.GetComponent<Image>().sprite = escNormal;
        //        wKey.GetComponent<Image>().sprite = wNormal;
        //        spaceKey.GetComponent<Image>().sprite = spaceNormal;
        //    }
        //    else if (hit.collider == wKey.GetComponent<Collider>())
        //    {
        //        DescriptionBox.SetActive(true);
        //        DescriptionBox.GetComponent<TextMeshProUGUI>().text = "W - Cast Ability 2";
        //        DescriptionBox.transform.position = Input.mousePosition;
        //        wKey.GetComponent<Image>().sprite = wHover;

        //        escKey.GetComponent<Image>().sprite = escNormal;
        //        qKey.GetComponent<Image>().sprite = qNormal;
        //        spaceKey.GetComponent<Image>().sprite = spaceNormal;
        //    }
        //    else if (hit.collider ==spaceKey.GetComponent<Collider>())
        //    {
        //        DescriptionBox.SetActive(true);
        //        DescriptionBox.GetComponent<TextMeshProUGUI>().text = "SPACE - Snap Camera to the Character";
        //        DescriptionBox.transform.position = Input.mousePosition;
        //        spaceKey.GetComponent<Image>().sprite = spaceHover;

        //        escKey.GetComponent<Image>().sprite = escNormal;
        //        qKey.GetComponent<Image>().sprite = qNormal;
        //        wKey.GetComponent<Image>().sprite = wNormal;
        //    } else
        //    {
        //        DescriptionBox.SetActive(false);
        //        escKey.GetComponent<Image>().sprite = escNormal;
        //        qKey.GetComponent<Image>().sprite = qNormal;
        //        wKey.GetComponent<Image>().sprite = wNormal;
        //        spaceKey.GetComponent<Image>().sprite = spaceNormal;
        //    }

        //}
    }

    public void escEnter()
    {

        DescriptionBox.SetActive(true);
        DescriptionBox.GetComponent<TextMeshProUGUI>().text = "ESC - Closes the Options-Menu";
        DescriptionBox.transform.position = Input.mousePosition;
        escKey.GetComponent<Image>().sprite = escHover;

        qKey.GetComponent<Image>().sprite = qNormal;
        wKey.GetComponent<Image>().sprite = wNormal;
        spaceKey.GetComponent<Image>().sprite = spaceNormal;

        escKey.GetComponent<Image>().color = hoverColor;
        qKey.GetComponent<Image>().color = Color.white;
        wKey.GetComponent<Image>().color = Color.white;
        spaceKey.GetComponent<Image>().color = Color.white;
    }

    public void qEnter()
    {
        DescriptionBox.SetActive(true);
        DescriptionBox.GetComponent<TextMeshProUGUI>().text = "Q - Cast Ability 1";
        DescriptionBox.transform.position = Input.mousePosition;
        qKey.GetComponent<Image>().sprite = qHover;

        escKey.GetComponent<Image>().sprite = escNormal;
        wKey.GetComponent<Image>().sprite = wNormal;
        spaceKey.GetComponent<Image>().sprite = spaceNormal;

        escKey.GetComponent<Image>().color = Color.white;
        qKey.GetComponent<Image>().color = hoverColor;
        wKey.GetComponent<Image>().color = Color.white;
        spaceKey.GetComponent<Image>().color = Color.white;
    }

    public void wEnter()
    {

        DescriptionBox.SetActive(true);
        DescriptionBox.GetComponent<TextMeshProUGUI>().text = "W - Cast Ability 2";
        DescriptionBox.transform.position = Input.mousePosition;
        wKey.GetComponent<Image>().sprite = wHover;

        escKey.GetComponent<Image>().sprite = escNormal;
        qKey.GetComponent<Image>().sprite = qNormal;
        spaceKey.GetComponent<Image>().sprite = spaceNormal;

        escKey.GetComponent<Image>().color = Color.white;
        qKey.GetComponent<Image>().color = Color.white;
        wKey.GetComponent<Image>().color = hoverColor;
        spaceKey.GetComponent<Image>().color = Color.white;
    }

    public void spaceEnter()
    {

        DescriptionBox.SetActive(true);
        DescriptionBox.GetComponent<TextMeshProUGUI>().text = "SPACE - Snap Camera to the Character";
        DescriptionBox.transform.position = Input.mousePosition;
        spaceKey.GetComponent<Image>().sprite = spaceHover;

        escKey.GetComponent<Image>().sprite = escNormal;
        qKey.GetComponent<Image>().sprite = qNormal;
        wKey.GetComponent<Image>().sprite = wNormal;

        escKey.GetComponent<Image>().color = Color.white;
        qKey.GetComponent<Image>().color = Color.white;
        wKey.GetComponent<Image>().color = Color.white;
        spaceKey.GetComponent<Image>().color = hoverColor;
    }

    public void AnyKeyExit()
    {
        DescriptionBox.SetActive(false);
        escKey.GetComponent<Image>().sprite = escNormal;
        qKey.GetComponent<Image>().sprite = qNormal;
        wKey.GetComponent<Image>().sprite = wNormal;
        spaceKey.GetComponent<Image>().sprite = spaceNormal;
        escKey.GetComponent<Image>().color = Color.white;
        qKey.GetComponent<Image>().color = Color.white;
        wKey.GetComponent<Image>().color = Color.white;
        spaceKey.GetComponent<Image>().color = Color.white;
    }

    public void LeaveGame()
    {
        Application.Quit();
    }

    public void CloseAllPanels()
    {
        OptionsPanel.SetActive(false);
        QuitPanel.SetActive(false);
    }

}
