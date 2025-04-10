using System;
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (changing) foreach (GameObject i in decals) i.SetActive(true);
        else foreach (GameObject i in decals) i.SetActive(false);

        if (currentAbility == Ability.ChangeGravity) abilityMaterial.color = abilityColors[0];
        if (currentAbility == Ability.PullObject) abilityMaterial.color = abilityColors[1];
        if (currentAbility == Ability.AddGravity) abilityMaterial.color = abilityColors[2];
        if (currentAbility == Ability.Katana) abilityMaterial.color = abilityColors[3];
    }

    public void nextAbility()
    {
        currentAbility = (Ability)((int)(currentAbility + 1) % 4);
    }

    public void prevAbility()
    {
        preChange = (int)(currentAbility - 1);
        if (preChange == -1) currentAbility = Ability.Katana;
        else currentAbility = (Ability)(preChange);
    }
    public void changeAbility()
    {
        hand.SetBool("change", true);
        foreach (GameObject i in abilitiesUI) i.SetActive(true);
        foreach (GameObject i in defaultUI) i.SetActive(false);
    }

    public void useAbility()
    {
        if (currentAbility == Ability.ChangeGravity)
        {
            // код
        }
        if (currentAbility == Ability.PullObject)
        {
            // код
        }
        if (currentAbility == Ability.AddGravity)
        {
            // код
        }
        if (currentAbility == Ability.Katana)
        {
            // код
        }
    }

    public void apply()
    {
        hand.SetBool("change", false);
        foreach (GameObject i in abilitiesUI) i.SetActive(false);
        foreach (GameObject i in defaultUI) i.SetActive(true);
    }

    public void isChanging()
    {

        changing = !changing;
        Debug.Log("Выбор");
    }
}
