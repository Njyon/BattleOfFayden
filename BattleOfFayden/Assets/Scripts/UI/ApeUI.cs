using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApeUI : Photon.MonoBehaviour
{
    public Color color_1;
    public Color color_2;

    public List<Skilll> skills;
    public List<GameObject> counters;

    public Character character;

    public GameObject uiPanel;

    Text text;
    bool canStack = true;
    
    [Header("Sprtie Change Ability 1")]
    public Sprite inActiveShot;
    public Sprite activeShot;

    [Header("Sprtie Change Ability 2")]
    public Sprite inActiveBomb;
    public Sprite activeBomb;

    //Team
    GameObject gTeamSymbol;
    Image teamSymbol;

    [Header("TeamSprites")]
    public Sprite rightTeam;
    public Sprite leftTeam;
    
    [Header("Indiactors")]
    [SerializeField] private GameObject apeShotIndicator;
    [SerializeField] private GameObject apeBombIndicator;
    private Vector3 skillDestination;

    void Start()
    {
        if (photonView.isMine)
        {
            uiPanel = GameObject.Find("CH2 UI Panel");
            uiPanel.SetActive(false);

            character = GetComponent<Character>();

            skills[0].cooldown = character.shotCooldown;
            skills[1].cooldown = character.bombCooldown * 2;

            skills[0].skillCooldown = GameObject.Find("CoolDown1");
            skills[1].skillCooldown = GameObject.Find("CoolDown2");
            //skills[2].skillCooldown = GameObject.Find("CoolDown3");

            skills[0].gSkillIcon = GameObject.Find("FirstAbility");
            skills[1].gSkillIcon = GameObject.Find("SecondAbility");
            //skills[2].gSkillIcon = GameObject.Find("ThirtAbility");

            counters[1] = GameObject.Find("BombStackCounter");

            //Team Image Variables
            gTeamSymbol = GameObject.Find("TeamSymbol");
            teamSymbol = gTeamSymbol.GetComponent<Image>();

            foreach (Skilll skill in skills)
            {
                if (skill.skillCooldown != null)
                    skill.skillCooldownIcon = skill.skillCooldown.GetComponent<Image>();
                if (skill.gSkillIcon != null)
                    skill.skillIcon = skill.gSkillIcon.GetComponent<Image>();
            }

            foreach (GameObject gameObject in counters)
            {
                if (gameObject != null)
                    text = gameObject.GetComponent<Text>();
            }
            skills[0].skillIcon.color = color_1;
            skills[1].skillIcon.color = color_1;
            //skills[2].skillIcon.color = color_1;

            if(character.teamID == PunTeams.Team.red)
            {
                teamSymbol.sprite = leftTeam;
            }
            else
            {
                teamSymbol.sprite = rightTeam;
            }

            // Indicators
            apeShotIndicator = Instantiate(apeShotIndicator);
            apeBombIndicator = Instantiate(apeBombIndicator);
            //Set Indicators inAktive so they are only aktive when u need them
            apeShotIndicator.SetActive(false);
            apeBombIndicator.SetActive(false);
        }
    }

    void Update()
    {
        if (photonView.isMine)
        {
            if (Input.GetButtonUp("Q") && skills[0].currentCooldown <= 0 && character.properties.mana >= character.shootCost)
            {
                skills[0].skillIcon.color = color_1;

                skills[0].currentCooldown = skills[0].cooldown;
                skills[0].skillCooldownIcon.fillAmount = skills[0].currentCooldown / skills[0].cooldown;
            }
            if (character.properties.charges < ApePrototype.charges && skills[1].currentCooldown <= 0)
            {
                skills[1].currentCooldown = skills[1].cooldown;
                skills[1].skillCooldownIcon.fillAmount = skills[1].currentCooldown / skills[1].cooldown;
            }
            //if (Input.GetButtonUp("E") && skills[2].currentCooldown <= 0)
            //{
            //    skills[2].skillIcon.color = color_1;

            //    skills[2].currentCooldown = skills[2].cooldown;
            //    skills[2].skillCooldownIcon.fillAmount = skills[2].currentCooldown / skills[2].cooldown;
            //}
            if (character.properties.charges < ApePrototype.charges)
            {
                if (skills[1].skillCooldownIcon.fillAmount <= 0.01 && canStack == true)
                {
                    canStack = false;
                    character.properties.charges++;
                    StartCoroutine(BombStackDelay());
                }
            }

            if (Input.GetButton("Q") && skills[0].currentCooldown <= 0 && character.properties.mana >= character.shootCost)
            {
                skills[0].skillIcon.color = color_2;
                
                apeShotIndicator.SetActive(true);
                apeShotIndicator.transform.position = transform.position;

                skillDestination = character.MousePosition();

                apeShotIndicator.transform.LookAt(new Vector3(skillDestination.x, transform.position.y, skillDestination.z));
            }
            else
            {
                apeShotIndicator.SetActive(false);
            }
            if (Input.GetButton("W") && character.properties.charges > 0)
            {
                skills[1].skillIcon.color = color_2;

                apeBombIndicator.SetActive(true);
                apeBombIndicator.transform.position = transform.position;

                skillDestination = character.MousePosition();

                apeBombIndicator.transform.position = skillDestination;
            }
            else
            {
                apeBombIndicator.SetActive(false);
            }
            //if (Input.GetButtonDown("E") && skills[2].currentCooldown <= 0)
            //{
            //    skills[2].skillIcon.color = color_2;
            //}

            text.text = character.properties.charges.ToString();
            
            //////////////////////////////////////////////////////////////////////////////////
            ////////////            Change Sprite from inActive to Normal            /////////
            //////////////////////////////////////////////////////////////////////////////////

            if (skills[0].currentCooldown >= 0 && skills[0].skillIcon.sprite != inActiveShot)
            {
                skills[0].skillIcon.sprite = inActiveShot;
            }
            else if (skills[0].currentCooldown <= 0 && skills[0].skillIcon.sprite != activeShot)
            {
                skills[0].skillIcon.sprite = activeShot;
            }

            if (character.bombOnCooldwon == true && skills[1].skillIcon.sprite != inActiveBomb)
            {
                skills[1].skillIcon.sprite = inActiveBomb;
            }
            else if (character.bombOnCooldwon == false && skills[1].skillIcon.sprite != activeBomb && character.properties.charges > 0)
            {
                skills[1].skillIcon.sprite = activeBomb;
            }

            foreach (Skilll skill in skills)
            {
                if (skill.currentCooldown >= 0)
                {
                    Cooldown();
                }
            }
        }
    }
    void Cooldown()
    {
        foreach (Skilll skill in skills)
        {
            if (skill.currentCooldown >= 0)
            {
                skill.currentCooldown -= Time.deltaTime;
                skill.skillCooldownIcon.fillAmount = skill.currentCooldown / skill.cooldown;
            }
        }
    }
    IEnumerator BombStackDelay()
    {
        yield return new WaitForSeconds(1f);
        canStack = true;
    }
}

[System.Serializable]
public class Skilll
{
    public float cooldown;
    public Image skillCooldownIcon;
    public Image skillIcon;
    //[HideInInspector]
    public float currentCooldown = 0f;
    [HideInInspector]
    public GameObject skillCooldown;
    [HideInInspector]
    public GameObject gSkillIcon;

}
