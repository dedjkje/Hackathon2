using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{
    [SerializeField] private Animator hand;
    [SerializeField] private Animator newHand;
    public GameObject[] abilitiesUI;
    public GameObject[] defaultUI;
    //public GameObject[] decals;
    public bool changing = false;
    public bool changingNORMAL = false;
    [SerializeField] Joystick joystick;

    private int preChange;

    public Color[] abilityColors;
    public Material abilityMaterial;

    
    public enum Ability
    {
        ChangeGravity = 0,
        PullObject = 1,
        AddGravity = 2,
    };

    public Ability currentAbility = Ability.ChangeGravity;
    public GameObject[] particles;
    public GameObject[] textes;
    private CastPull castPull;
    private up addGravity;
    private ChangeGravity changeGravity;
    public bool castDeleted;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        castDeleted = false;
        castPull = GetComponent<CastPull>();
        addGravity = GetComponent<up>();
        changeGravity = GetComponent<ChangeGravity>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (changing) foreach (GameObject i in decals) i.SetActive(true);
        //else foreach (GameObject i in decals) i.SetActive(false);

        if (!hand.GetBool("cilinder")) AddCast();

        if (currentAbility == Ability.ChangeGravity)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (i == 0)
                {
                    particles[i].SetActive(true);
                }
                else particles[i].SetActive(false);
            }
        }
        if (currentAbility == Ability.PullObject)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (i == 1) particles[i].SetActive(true);
                else particles[i].SetActive(false);
            }
        }
        if (currentAbility == Ability.AddGravity)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (i == 2) particles[i].SetActive(true);
                else particles[i].SetActive(false);
            }
        }
        if (currentAbility == Ability.PullObject) defaultUI[0].SetActive(false);
        if (castPull.PullStarted && !castPull.PullEnded) defaultUI[0].SetActive(false);
        if (castPull.PullStarted || castPull.stopAnimation) hand.SetBool("pull", false);
        if (currentAbility == Ability.PullObject && castPull.PullEnded && !castPull.onTarget) defaultUI[0].SetActive(false);
        if (currentAbility == Ability.PullObject && castPull.PullEnded && castPull.onTarget && !changing) defaultUI[0].SetActive(true);
        

        if (currentAbility == Ability.AddGravity && !castDeleted) defaultUI[0].SetActive(addGravity.animEnded);
        if (currentAbility == Ability.AddGravity) defaultUI[0].SetActive(addGravity.popal);
        if (changing) defaultUI[0].SetActive(false);

        if (changing && currentAbility == Ability.ChangeGravity)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (i == 0) textes[i].SetActive(true);
                else textes[i].SetActive(false);
            }
        }
        else if (changing && currentAbility == Ability.PullObject)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (i == 1) textes[i].SetActive(true);
                else textes[i].SetActive(false);
            }
        }
        else if (changing && currentAbility == Ability.AddGravity)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (i == 2) textes[i].SetActive(true);
                else textes[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i <= 2; i++)
            {
                textes[i].SetActive(false);
            }
        }
    }
    private void LateUpdate()
    {

        if (currentAbility == Ability.ChangeGravity && changingNORMAL) defaultUI[0].SetActive(false);
    }
    public void nextAbility()
    {
        currentAbility = (Ability)((int)(currentAbility + 1) % 3);

        if (currentAbility == Ability.PullObject)
        {
            castPull.canPull = true;
        }
        else
        {
            castPull.canPull = false;
        }

        if (currentAbility == Ability.AddGravity)
        {
            addGravity.canCast = true;
        }
        else
        {
            addGravity.canCast = false;
        }
    }

    public void prevAbility()
    {

        preChange = (int)(currentAbility - 1);
        if (preChange == -1) currentAbility = Ability.AddGravity;
        else currentAbility = (Ability)preChange;

        if (currentAbility == Ability.PullObject)
        {
            castPull.canPull = true;
        }
        else
        {
            castPull.canPull = false;
        }

        if (currentAbility == Ability.AddGravity)
        {
            addGravity.canCast = true;
        }
        else
        {
            addGravity.canCast = false;
        }
    }
    public void changeAbility()
    {
        joystick.ResetJoystick();
        changingNORMAL = true;
        joystick.enabled = false;
        DeleteCast();
        hand.SetBool("change", true);
        newHand.SetBool("change", true);
        foreach (GameObject i in abilitiesUI) i.SetActive(true);
        foreach (GameObject i in defaultUI) i.SetActive(false);
    }

    public void useAbility()
    {
        if (currentAbility == Ability.ChangeGravity)
        {
            changeGravity.Change();
        }
        if (currentAbility == Ability.PullObject)
        {
            castPull.Pull();
            if (castPull.pullable != null) hand.SetBool("pull", true);
        }
        if (currentAbility == Ability.AddGravity)
        {
            if (addGravity.Predict.transform.position != new Vector3(0, -1000f, 0)) hand.SetBool("cilinder", true);
            addGravity.Cast();
        }
        
    }

    public void apply()
    {
        (joystick as FloatingJoystick).ResetFloatingJoystick();
        joystick.enabled = true;
        changingNORMAL = false;
        hand.SetBool("change", false);
        newHand.SetBool("change", false);
        foreach (GameObject i in abilitiesUI) i.SetActive(false);
        foreach (GameObject i in defaultUI) i.SetActive(true);
    }

    public void isChangingTrue()
    {

        changing = true;
    }
    public void isChangingFalse()
    {

        changing = false;
    }
    public void castEnded()
    {
        
        
            addGravity.canCast = true;
            hand.SetBool("cilinder", false);
            castDeleted = false;
        
    }
    public void DeleteCast()
    {
        
        
            defaultUI[0].SetActive(false);
            castDeleted = true;
       
    }
    public void AddCast() {
        
        
            defaultUI[0].SetActive(true);
            castDeleted = false;
        
    }
}
