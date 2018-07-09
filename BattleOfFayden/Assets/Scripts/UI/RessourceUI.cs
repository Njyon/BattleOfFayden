using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RessourceUI : MonoBehaviour
{
    public GameObject imageHealthBar;
    public GameObject imageEnergieBar;
    public GameObject imageHealthLose;
    public GameObject imageEnergieLose;
    public Image healthBar;
    public Image energieBar;
    public Image healthLoseBar;
    public Image energieLoseBar;
    public int maxHealth;
    public int currentHealth;
    public int maxEnergie;
    public int currentEnergie;

    public float healthLoseFloat;
    public float energieLoseFloat;
    public float calcHealth;
    public float calcEnergie;
    public float calsHealthLose;
    float smoothHealthLose;
    float smoothEnergieLose;

    public Character character;

    public PhotonView photonView;

    GameObject pointPanel;
    GameObject characterPanel;

    Animator pointPanelAnimator;
    Animator characterPanelAnimator;

    public GameObject playerName;
    public Text playerNameText;

    void Awake()
    {
        pointPanel = GameObject.FindGameObjectWithTag("PointPanel");
        characterPanel = GameObject.FindGameObjectWithTag("CharacterPanel");

        pointPanelAnimator = pointPanel.GetComponent<Animator>();
        characterPanelAnimator = characterPanel.GetComponent<Animator>();
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        character = GetComponent<Character>();

        if (character.type == Character.CharacterID.Ape)
        {
            this.maxHealth = ApePrototype.health;
            this.smoothHealthLose = ApePrototype.health;
            this.healthLoseFloat = ApePrototype.health;
            this.maxEnergie = ApePrototype.mana;
            this.smoothEnergieLose = ApePrototype.mana;
            this.energieLoseFloat = ApePrototype.mana;
        }
        else if(character.type == Character.CharacterID.Pangolin)
        {
            this.maxHealth = PangolinPrototype.health;
            this.smoothHealthLose = PangolinPrototype.health;
            this.healthLoseFloat = PangolinPrototype.health;
            this.maxEnergie = PangolinPrototype.mana;
            this.smoothEnergieLose = PangolinPrototype.mana;
            this.energieLoseFloat = PangolinPrototype.mana;
        }

        imageHealthBar = GameObject.FindGameObjectWithTag("Health");
        imageHealthLose = GameObject.FindGameObjectWithTag("HealthLose");
        imageEnergieBar = GameObject.FindGameObjectWithTag("Energie");

        if (imageHealthBar != null)
            healthBar = imageHealthBar.GetComponent<Image>();
        if (imageHealthLose != null)
            healthLoseBar = imageHealthLose.GetComponent<Image>();
        if (imageEnergieBar != null)
            energieBar = imageEnergieBar.GetComponent<Image>();
        healthBar.fillAmount = 1;
        energieBar.fillAmount = 1;

        playerName = GameObject.Find("PlayerName");
        playerNameText = playerName.GetComponent<Text>();

        if (photonView.isMine)
        {
            playerNameText.text = PhotonNetwork.player.NickName;
        }
    }

    void Update()
    {
        if (photonView.isMine)
        {
            this.currentHealth = character.properties.health;
            this.currentEnergie = character.properties.mana;

            ////////////////////////////
            //  Healthbar UI Effect   //
            ////////////////////////////

            // Green Bar
            if (smoothHealthLose > currentHealth)
            {
                smoothHealthLose -= Time.deltaTime * 50;
                healthBar.fillAmount = smoothHealthLose / (float)this.maxHealth;
            }
            if (smoothHealthLose < currentHealth)
            {
                smoothHealthLose += Time.deltaTime * 50;
                healthBar.fillAmount = smoothHealthLose / (float)this.maxHealth;
            }

            //Red Bar
            if (this.healthLoseFloat > this.currentHealth)
            {
                this.healthLoseFloat -= Time.deltaTime * 10;
                healthLoseBar.fillAmount = this.healthLoseFloat / (float)this.maxHealth;
            }
            if (this.healthLoseFloat < this.currentHealth)
            {
                this.healthLoseFloat = currentHealth;
                healthLoseBar.fillAmount = this.healthLoseFloat / (float)this.maxHealth;
            }

            ////////////////////////////
            //  Energiebar UI Effect  //
            ////////////////////////////

            if (smoothEnergieLose > currentEnergie)
            {
                smoothEnergieLose -= Time.deltaTime * 50;
                energieBar.fillAmount = smoothEnergieLose / (float)this.maxEnergie;
            }
            if (smoothEnergieLose < currentEnergie)
            {
                smoothEnergieLose += Time.deltaTime * 50;
                energieBar.fillAmount = smoothEnergieLose / (float)this.maxEnergie;
            }

        }
    }

    public IEnumerator UI()
    {
        pointPanelAnimator.SetBool("isReady", true);
        characterPanelAnimator.SetBool("isReady", true);
        yield return new WaitForSeconds(1.1f);
        characterPanelAnimator.SetBool("isReady", false);
        pointPanelAnimator.SetBool("isReady", false);
        pointPanelAnimator.enabled = false;
        characterPanelAnimator.enabled = false;
    }
}
