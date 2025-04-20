using UnityEngine;

public class loadAbility : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindAnyObjectByType<Abilities>().currentAbility = AbilityMember.ability;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
