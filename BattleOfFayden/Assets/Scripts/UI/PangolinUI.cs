using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PangolinUI : Photon.MonoBehaviour
{
    public Color color_1;
    public Color color_2;

    public List<Skilll> skills;
    public List<GameObject> counters;

    public Character character;

    public GameObject uiPanel;

    public Color comboTextColor_0;
    public Color comboTextColor_1;
    public Color comboTextColor_2;
    
    Text text;
    bool canStack = true;

    [Header("Sprtie Change Ability 1")]
    public Sprite inActiveSlash;
    public Sprite activeSlach;

    [Header("Sprtie Change Ability 2")]
    public Sprite inAktivearthQuake;
    public Sprite activeEarthQuake;
    
    //Team
    GameObject gTeamSymbol;
    Image teamSymbol;

    [Header("TeamSprites")]
    public Sprite rightTeam;
    public Sprite leftTeam;
    
    [Header("Indiactors")]
    [SerializeField] private GameObject pangolinSlashIndicator;
    [SerializeField] private GameObject pangolinEarthquakeIndicator;
    private Vector3 skillDestination;

    void Start()
    {
        if (photonView.isMine)
        {
            uiPanel = GameObject.Find("UI Panel");
            uiPanel.SetActive(false);
            
            character = GetComponent<Character>();
            
            skills[0].skillCooldown = GameObject.Find("CoolDown1_CH2");
            skills[1].skillCooldown = GameObject.Find("CoolDown2_CH2");
            //skills[2].skillCooldown = GameObject.Find("CoolDown3_CH2");

            skills[0].gSkillIcon = GameObject.Find("FirstAbility_CH2");
            skills[1].gSkillIcon = GameObject.Find("SecondAbility_CH2");
            //skills[2].gSkillIcon = GameObject.Find("ThirtAbility_CH2");

            skills[0].cooldown = character.slashCooldown;
            skills[1].cooldown = character.earthquakeCooldown;

            counters[0] = GameObject.Find("ComboCounter_CH2");

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

            skills[1].skillIcon.sprite = activeEarthQuake;

            if (character.teamID == PunTeams.Team.red)
            {
                teamSymbol.sprite = leftTeam;
            }
            else
            {
                teamSymbol.sprite = rightTeam;
            }

            //Indicators
            pangolinSlashIndicator = Instantiate(pangolinSlashIndicator);
            pangolinEarthquakeIndicator = Instantiate(pangolinEarthquakeIndicator);
            //Set Indicators inAktive so they are only aktive when u need them
            pangolinSlashIndicator.SetActive(false);
            pangolinEarthquakeIndicator.SetActive(false);
        }
    }

    void Update()
    {
        if (photonView.isMine)
        {
            if (Input.GetButtonUp("Q") && skills[0].currentCooldown <= 0 && character.properties.mana >= character.slashCost)
            {
                skills[0].skillIcon.color = color_1;

                skills[0].currentCooldown = skills[0].cooldown;
                skills[0].skillCooldownIcon.fillAmount = skills[0].currentCooldown / skills[0].cooldown;
            }
            if (Input.GetButtonUp("W") && skills[1].currentCooldown <= 0 && character.properties.mana >= character.earthquakeCost)
            {
                skills[1].skillIcon.color = color_2;

                skills[1].currentCooldown = skills[1].cooldown;
                skills[1].skillCooldownIcon.fillAmount = skills[1].currentCooldown / skills[1].cooldown;
            }
            //if (Input.GetButtonUp("E") && skills[2].currentCooldown <= 0)
            //{
            //    skills[2].skillIcon.color = color_1;

            //    skills[2].currentCooldown = skills[2].cooldown;
            //    skills[2].skillCooldownIcon.fillAmount = skills[2].currentCooldown / skills[2].cooldown;
            //}

            if (Input.GetButton("Q") && skills[0].currentCooldown <= 0 && character.properties.mana >= character.slashCost)
            {
                skills[0].skillIcon.color = color_2;

                pangolinSlashIndicator.SetActive(true);
                pangolinSlashIndicator.transform.position = transform.position;

                skillDestination = character.MousePosition();

                pangolinSlashIndicator.transform.LookAt(new Vector3(skillDestination.x, transform.position.y, skillDestination.z));
            }
            else
            {
                pangolinSlashIndicator.SetActive(false);
            }
            if (Input.GetButton("W") && skills[1].currentCooldown <= 0 && character.properties.mana >= character.earthquakeCost)
            {
                skills[1].skillIcon.color = color_2;

                pangolinEarthquakeIndicator.SetActive(true);
                pangolinEarthquakeIndicator.transform.position = transform.position;

                skillDestination = character.MousePosition();

                pangolinEarthquakeIndicator.transform.LookAt(new Vector3(skillDestination.x, transform.position.y, skillDestination.z));
            }
            else
            {
                pangolinEarthquakeIndicator.SetActive(false);
            }
            //if (Input.GetButtonDown("E") && skills[2].currentCooldown <= 0)
            //{
            //    skills[2].skillIcon.color = color_2;
            //}

            text.text = character.properties.charges.ToString();

            if (character.properties.charges == 0)
            {
                text.color = comboTextColor_0;
            }
            if (character.properties.charges == 1)
            {
                text.color = comboTextColor_1;
            }
            if (character.properties.charges == 2)
            {
                text.color = comboTextColor_2;
            }

            //////////////////////////////////////////////////////////////////////////////////
            ////////////            Change Sprite from inActive to Normal            /////////
            //////////////////////////////////////////////////////////////////////////////////

            if (skills[0].currentCooldown >= 0 && skills[0].skillIcon.sprite != inActiveSlash)
            {
                skills[0].skillIcon.sprite = inActiveSlash;
            }
            else if (skills[0].currentCooldown <= 0 && skills[0].skillIcon.sprite != activeSlach)
            {
                skills[0].skillIcon.sprite = activeSlach;
            }

            if (skills[1].currentCooldown >= 0 && skills[1].skillIcon.sprite != inAktivearthQuake)
            {
                skills[1].skillIcon.sprite = inAktivearthQuake;
            }
            else if (skills[1].currentCooldown <= 0 && skills[1].skillIcon.sprite != activeEarthQuake)
            {
                skills[1].skillIcon.sprite = activeEarthQuake;
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

