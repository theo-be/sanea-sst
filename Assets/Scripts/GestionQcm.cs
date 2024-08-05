using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionQcm : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public TypeCorps corps;   
    
    // ecran question6.5
    public GameObject ecranQuestion65;
    
    // ecran question8
    public GameObject ecranQuestion8;
    
    // ecran question6bebe
    public GameObject ecranQuestion6bebe;
    
    // ecran question7bebe
    public GameObject ecranQuestion7bebe;
    // ecran question6enfant
    public GameObject ecranQuestion6enfant;
    
    // ecran question7enfant
    public GameObject ecranQuestion7enfant;
    // ecran question6adulte
    public GameObject ecranQuestion6adulte;
    
    // ecran question7adulte
    public GameObject ecranQuestion7adulte;
    
    // ecran question7adulte
    public GameObject ecranQuestion8adulteetenfant;
    
    // ecran question7adulte
    public GameObject ecranQuestion8bebe;
    
    //méthode
    public void activerEcran65()
    {
        ecranQuestion65?.SetActive(true); // Activating the new UI panel
    }
    public void activerEcran8()
    {
        ecranQuestion8?.SetActive(true); // Activating the new UI panel
    }
    
    public void activerEcran6bebe()
    {
        ecranQuestion6bebe?.SetActive(true); // Activating the new UI panel
    }
    public void activerEcran7bebe()
    {
        ecranQuestion7bebe?.SetActive(true); // Activating the new UI panel
    }
    
    public void activerEcran6enfant()
    {
        ecranQuestion6enfant?.SetActive(true); // Activating the new UI panel
    }
    public void activerEcran7enfant()
    {
        ecranQuestion7enfant?.SetActive(true); // Activating the new UI panel
    }
    
    public void activerEcran6adulte()
    {
        ecranQuestion6adulte?.SetActive(true); // Activating the new UI panel
    }
    public void activerEcran7adulte()
    {
        ecranQuestion7adulte?.SetActive(true); // Activating the new UI panel
    }

    public void activerEcran8AdulteetEnfant()
    {
        ecranQuestion8adulteetenfant?.SetActive(true);
    }
    
    public void activerEcran8Bebe()
    {
        ecranQuestion8bebe?.SetActive(true);
    }
    

    public void ecran7()
    {
        if (corps == TypeCorps.Adulte)
        {
            activerEcran7adulte();
        }
        else if (corps == TypeCorps.Enfant)
        {
            activerEcran7enfant();
        }
        else if (corps == TypeCorps.Bebe)
        {
            activerEcran7bebe();
        }
    }
    
    public void ecran8()
    {
        if (corps == TypeCorps.Adulte)
        {
            activerEcran8AdulteetEnfant();
        }
        else if (corps == TypeCorps.Enfant)
        {
            activerEcran8AdulteetEnfant();
        }
        else if (corps == TypeCorps.Bebe)
        {
            activerEcran8Bebe();
        }
    }
    
}
