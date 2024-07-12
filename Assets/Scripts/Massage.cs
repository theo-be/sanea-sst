
using System;
using TMPro;
using UnityEngine;



public class Massage : MonoBehaviour
{
    
    public GameObject mainGauche;
    public GameObject mainDroite;
    public GameObject corps;
    
    public EtatMassage etatMassage = EtatMassage.HorsMassage;

    // tolerance d'ecart par rapport a la cible
    public GameObject zoneDeTolerance;
    public float toleranceDeRotation = 20f;
    


    // ecart entre les deux : 0.05y
    public float distanceImpulsion = 0.04f; 
    public GameObject debutImpulsion;
    public GameObject finImpulsion;

    private ChangeMaterial _changeMaterial;

    
    // ecran debug
    public GameObject ecranDebug;
    private  TMP_Text texteDebug;
    private const string mainsenPosition = "mains en position";
    private const string mainsPasEnPosition = "mains pas en position";

    public float tempsDePreparation = 5f;
    private float tempsAvantDebutMassage = 0f;

    private bool hautTouche = false;
    private bool basTouche = false;

    private int nombreImpulsion = 0;
    public int nombreImpulsionsParSerie = 20;

    private bool impulsionEnCours = false;
    private bool etatImpulsion = ALLER;

    private const bool ALLER = false;
    private const bool RETOUR = true;
    
    // temps par impulsion lors du massage
    private float intervalleMinPulsation = .4f;
    private float intervalleMaxPulsation = .7f;
    private float tempsDebutImpulsion;
    
    
    private float tempsImpulsionMin;
    private float tempsImpulsionMax;
    
    
    

    public void changerEtat(EtatMassage etat)
    {
        etatMassage = etat;
    }
    
    private bool MainsEnPosition()
    {
        // verifier si les mains sont environ dans la zone de tolerance avant le massage

        var posMainGauche = mainGauche.transform.position;
        var posMainDroite = mainDroite.transform.position;
        var collisionSphere = zoneDeTolerance.transform.localScale;
        var centreSphere = zoneDeTolerance.transform.position;

        // rotation cible : 270 gauche 90 droite axe z
        
        var rotMainGauche = mainGauche.transform.eulerAngles;
        var rotMainDroite = mainDroite.transform.eulerAngles;

        if (
            // main gauche
            Mathf.Abs((posMainGauche - centreSphere).magnitude) < collisionSphere.magnitude / 2f
            && Mathf.Abs(rotMainGauche.z - 270f) < toleranceDeRotation
            // main droite
            &&
            Mathf.Abs((posMainDroite - centreSphere).magnitude) < collisionSphere.magnitude / 2f
            && Mathf.Abs(rotMainDroite.z - 90f) < toleranceDeRotation
            )
        {
            return true;
        }
        
        
        return false;
    }

    private bool MainDansLaZone(GameObject main, GameObject zone)
    {
        Vector3 positionMain = main.transform.position;
        Vector3 tailleZone = zone.transform.localScale;
        Vector3 positionZone = zone.transform.position;

        if (
            positionMain.x >= positionZone.x - tailleZone.x / 2f
            && positionMain.x <= positionZone.x + tailleZone.x / 2f
            && positionMain.z >= positionZone.z - tailleZone.z / 2f
            && positionMain.z <= positionZone.z + tailleZone.z / 2f
        )
        {
            return true;
        }
        
        return false;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        _changeMaterial = zoneDeTolerance.GetComponent<ChangeMaterial>();
        texteDebug = ecranDebug.GetComponent<TMP_Text>();
    }
    
    // Update is called once per frame
    void Update()
    {
        
        if (etatMassage == EtatMassage.Preparation)
        {
            if (MainsEnPosition())
            {

                if (tempsAvantDebutMassage == 0f)
                {
                    tempsAvantDebutMassage = Time.time;
                }
                else
                {
                    texteDebug.text += "debut de l'exercice dans : " + (Time.time - tempsAvantDebutMassage);
                }
                
                
                debutImpulsion.transform.position = mainGauche.transform.position;
                finImpulsion.transform.position = debutImpulsion.transform.position + Vector3.down * distanceImpulsion;
                

                zoneDeTolerance?.SetActive(false);
                _changeMaterial?.SetOtherMaterial();
                texteDebug.text = mainsenPosition;

                if (Time.time - tempsAvantDebutMassage > tempsDePreparation)
                {
                    etatMassage = EtatMassage.EnCours;
                    texteDebug.text = "Massage en cours";
                }

            }
            else
            {
                zoneDeTolerance?.SetActive(true);
                _changeMaterial?.SetOriginalMaterial();
                tempsAvantDebutMassage = 0f;
                texteDebug.text = mainsPasEnPosition;
            }
        } 
        else if (etatMassage == EtatMassage.EnCours)
        {

            Vector3 positionDebutImpulsion = debutImpulsion.transform.position;
            Vector3 positionFinImpulsion = finImpulsion.transform.position;

            if (MainDansLaZone(mainGauche, debutImpulsion) && MainDansLaZone(mainDroite, debutImpulsion) || true)
            {
                // variables d'etat instantane
                hautTouche = mainGauche.transform.position.y > positionDebutImpulsion.y;
                basTouche = mainGauche.transform.position.y < positionFinImpulsion.y;


                if (hautTouche)
                {
                    if (etatImpulsion == RETOUR && impulsionEnCours)
                    {
                        float tempsImpulsion = Time.time - tempsDebutImpulsion;
                        nombreImpulsion++;
                        texteDebug.text = "Nombre impulsions : " + nombreImpulsion;
                        
                        if (tempsImpulsion > intervalleMinPulsation)
                        {
                            texteDebug.text += "\nTrop lent : ";
                        } else if (tempsImpulsion < intervalleMaxPulsation)
                        {
                            texteDebug.text += "\nTrop rapide : ";
                        }
                        texteDebug.text += tempsImpulsion + "s";
                    }
                    impulsionEnCours = false;
                    etatImpulsion = ALLER;
                    
                }
                else if (basTouche)
                {
                    etatImpulsion = RETOUR;
                    
                }
                // entre les deux plans
                else
                {
                    if (impulsionEnCours == false)
                    {
                        tempsDebutImpulsion = Time.time;
                    }
                    impulsionEnCours = true;
                    
                    texteDebug.text = "";
                    texteDebug.text += "\n" + (Time.time - tempsDebutImpulsion) + "s";
                }

            }
            else
            {
                impulsionEnCours = false;
                etatImpulsion = ALLER;
            }

            if (nombreImpulsion >= nombreImpulsionsParSerie + 1000000000)
            {
                etatMassage = EtatMassage.HorsMassage;
                texteDebug.text = "Exercice termine";
            }


        }

    }
    
}

public enum EtatMassage
{
    HorsMassage,
    Preparation,
    EnCours
}