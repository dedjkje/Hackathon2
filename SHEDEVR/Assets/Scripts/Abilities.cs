using System;
using Unity.VisualScripting;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    [SerializeField] private Animator hand;

    public GameObject[] abilitiesUI;
    public GameObject[] defaultUI;
    public GameObject[] decals;
    public bool changing = false;

    private int preChange;

    public Color[] abilityColors;
    public Material abilityMaterial;
    
    public enum Ability
    {
        ChangeGravity = 0,
        PullObject = 1,
        AddGravity = 2,
        Katana = 3
    };

    public Ability currentAbility = Ability.ChangeGravity;

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
        if (changing) foreach (GameObject i in decals) i.SetActive(true);
        else foreach (GameObject i in decals) i.SetActive(false);

        if (!hand.GetBool("cilinder")) AddCast();

        if (currentAbility == Ability.ChangeGravity) abilityMaterial.color = abilityColors[0];
        if (currentAbility == Ability.PullObject) abilityMaterial.color = abilityColors[1];
        if (currentAbility == Ability.AddGravity) abilityMaterial.color = abilityColors[2];
        if (currentAbility == Ability.Katana) abilityMaterial.color = abilityColors[3];

        if (currentAbility == Ability.PullObject) defaultUI[0].SetActive(false);
        if (castPull.PullStarted && !castPull.PullEnded) defaultUI[0].SetActive(false);
        if (castPull.PullStarted || castPull.stopAnimation) hand.SetBool("pull", false);
        if (currentAbility == Ability.PullObject && castPull.PullEnded && !castPull.onTarget) defaultUI[0].SetActive(false);
        if (currentAbility == Ability.PullObject && castPull.PullEnded && castPull.onTarget && !changing) defaultUI[0].SetActive(true);
        

        if (currentAbility == Ability.AddGravity && !castDeleted) defaultUI[0].SetActive(addGravity.animEnded);
        if (currentAbility == Ability.AddGravity) defaultUI[0].SetActive(addGravity.popal);
        if (changing) defaultUI[0].SetActive(false);
    }

    public void nextAbility()
    {
        currentAbility = (Ability)((int)(currentAbility + 1) % 4);

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
        if (preChange == -1) currentAbility = Ability.Katana;
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
        DeleteCast();
        hand.SetBool("change", true);
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
        if (currentAbility == Ability.Katana)
        {
            // code
        }
    }

    public void apply()
    {
        hand.SetBool("change", false);
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
