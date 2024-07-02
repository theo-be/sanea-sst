using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class Massage : MonoBehaviour
{
    
    public GameObject corps;
    public XROrigin rig;
    public GameObject sphere;
    public GameObject mainGauche;
    public GameObject mainDroite;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private bool MainsEnPosition()
    {
        // verifier si les mains sont environ dans la bonne position pour le massage, avant et pendant
        return false;
    }



    // Update is called once per frame
    void Update()
    {
        Vector3 position = (mainGauche.transform.position - mainDroite.transform.position) / 2.0f;
        sphere.transform.position = mainGauche.transform.position + position;
    }
}
