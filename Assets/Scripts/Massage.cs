using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Massage : MonoBehaviour
{
    private const bool ALLER = false;
    private const bool RETOUR = true;
    
    public GameObject mainGauche;
    public GameObject mainDroite;
    public GameObject cam;
    private GameObject corps;
    private GameObject mainPrincipale;
    public TypeCorps type = TypeCorps.Adulte;
    
    public EtatMassage etatMassage = EtatMassage.HorsMassage;

    // tolerance d'ecart par rapport a la cible
    public GameObject zoneDeTolerance;
    private float toleranceDeRotation = 20f;

    public GameObject zoneTete;

    // ecart entre les deux : 0.04 y
    public float distanceImpulsion = 0.04f; 
    public GameObject debutImpulsion;
    public GameObject finImpulsion;

    private ChangeMaterial _changeMaterial;

    public float tempsDePreparation = 3f;
    private float tempsAvantDebutMassage = 0f;

    private bool hautTouche = false;
    private bool basTouche = false;

    private int nombreImpulsions = 0;
    public int nombreImpulsionsParSerie = 30;

    private bool impulsionEnCours = false;
    private bool etatImpulsion = ALLER;
    

    // temps par impulsion lors du massage
    private float intervalleMinPulsation = .4f;
    private float intervalleMaxPulsation = .7f;
    private float tempsDebutImpulsion;
    
    private float tempsImpulsionMin;
    private float tempsImpulsionMax;
    
    // insufflations
    public int nombreInsufflationsParSerie = 3;
    private int nombreInsufflations;
    private EtatInsufflation etatInsufflation = EtatInsufflation.AFaire;
    private float tempsDebutInsufflation;
    
    // deplacement du corps
    private XRGrabInteractable XRComponent;
    
    // ecran exercice termine
    public GameObject ecranExerciceTermine;
    
    // ecran consignes
    public GameObject ecranConsignes;
    private ConsignesMassage _consignesMassage;
    
    // ecran debug
    public GameObject ecranDebug;
    private TMP_Text texteDebug;
    private const string mainsenPosition = "mains en position";
    private const string mainsPasEnPosition = "mains pas en position";

    public GameObject menuQCM;
    public GestionQcm gestion; 
    

    public void changerEtat(EtatMassage etat)
    {
        etatMassage = etat;
    }

    public void ResetMassage()
    {
        etatMassage = EtatMassage.Insufflation;
        etatInsufflation = EtatInsufflation.AFaire;
        nombreImpulsions = 0;
        nombreInsufflations = 0;
    }

    public void ActiverDeplacementCorps()
    {
        etatMassage = EtatMassage.HorsMassage;
        XRComponent.enabled = true;
    }
    
    public void DesactiverDeplacementCorps()
    {
        etatMassage = EtatMassage.Insufflation;
        XRComponent.enabled = false;
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

        switch (type)
        {
            case TypeCorps.Adulte:
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
                    mainPrincipale = mainGauche;
                    return true;
                }
                break;
            case TypeCorps.Enfant:
            case TypeCorps.Bebe:
                if (
                    // main gauche
                    Mathf.Abs((posMainGauche - centreSphere).magnitude) < collisionSphere.magnitude / 2f
                    && Mathf.Abs(rotMainGauche.z - 270f) < toleranceDeRotation)
                {
                    mainPrincipale = mainGauche;
                    return true;
                }
                 if (   
                    Mathf.Abs((posMainDroite - centreSphere).magnitude) < collisionSphere.magnitude / 2f
                    && Mathf.Abs(rotMainDroite.z - 90f) < toleranceDeRotation
                )
                {
                    mainPrincipale = mainDroite;
                    return true;
                }
                break;
            default:
                break;
        }
        return false;
    }
    
    private bool MainEnPosition(GameObject main)
    {
        // verifier si les mains sont environ dans la zone de tolerance avant le massage
        var posMain = main.transform.position;
        var collisionSphere = zoneDeTolerance.transform.localScale;
        var centreSphere = zoneDeTolerance.transform.position;

        // rotation cible : 270 gauche 90 droite axe z
        var rotMain = main.transform.eulerAngles;
        
        if (
            Mathf.Abs((posMain - centreSphere).magnitude) < collisionSphere.magnitude / 2f
            && (Mathf.Abs(rotMain.z - 270f) < toleranceDeRotation || Mathf.Abs(rotMain.z - 90f) < toleranceDeRotation)
        )
        {
            mainPrincipale = main;
            return true;
        }
        
        return false;
    }

    private static bool ObjetDansLaZone(GameObject objet, GameObject zone)
    {
        Vector3 positionMain = objet.transform.position;
        Vector3 tailleZone = zone.transform.localScale;
        Vector3 positionZone = zone.transform.position;

        if (
            positionMain.x >= positionZone.x - tailleZone.x / 2f
            && positionMain.x <= positionZone.x + tailleZone.x / 2f
            && positionMain.z >= positionZone.z - tailleZone.z / 2f
        )
        {
            return true;
        }
        
        return false;
    }
    
    void Start()
    {
        _changeMaterial = zoneDeTolerance.GetComponent<ChangeMaterial>();
        texteDebug = ecranDebug.GetComponent<TMP_Text>();
        corps = gameObject;
        XRComponent = gameObject.GetComponentInParent<XRGrabInteractable>();
        gestion = menuQCM.GetComponent<GestionQcm>();
        gestion.corps = type;
        
        _consignesMassage = ecranConsignes.GetComponent<ConsignesMassage>();
        _consignesMassage.typeCorps = type;
        _consignesMassage.nombreMaxInsufflations = nombreInsufflationsParSerie;
        _consignesMassage.nombreMaxImpulsions = nombreImpulsionsParSerie;
    }
    
    void Update()
    {
        if (etatMassage == EtatMassage.Insufflation)
        {
            texteDebug.text = "etat insufflation : " + etatInsufflation;
            texteDebug.text += "\nnb : " + nombreInsufflations + "/" + nombreInsufflationsParSerie;

            _consignesMassage.etatMassage = EtatMassage.Insufflation;
            _consignesMassage.nombreInsufflations = nombreInsufflations;

            // la camera doit etre au niveau de la tete du corps
            // mettre la cam 1 s au niveau de la tete du corps
            // relever la tete pour valider l'insufflation
            // rotation cible de la cam : x = 80
            Vector3 rotationCamera = cam.transform.eulerAngles;

            if (ObjetDansLaZone(cam, zoneTete) && rotationCamera.x + toleranceDeRotation >= 80f)
            {
                if (etatInsufflation == EtatInsufflation.AFaire)
                {
                    etatInsufflation = EtatInsufflation.EnCours;
                    tempsDebutInsufflation = Time.time;
                } else if (etatInsufflation == EtatInsufflation.EnCours)
                {
                    if (Time.time - tempsDebutInsufflation >= 1f)
                    {
                        etatInsufflation = EtatInsufflation.Fait;
                        nombreInsufflations++;
                    }
                }
            }
            else
            {
                if (nombreInsufflations >= nombreInsufflationsParSerie)
                {
                    etatMassage = EtatMassage.Preparation;
                    gestion.ecran8();
                }
                else
                {
                    etatInsufflation = EtatInsufflation.AFaire;
                }
                tempsDebutImpulsion = Time.time;
            }
        }
        else if (etatMassage == EtatMassage.Preparation)
        {
            _consignesMassage.etatMassage = EtatMassage.Preparation;
            
            if (type == TypeCorps.Adulte)
            {
                if (MainEnPosition(mainGauche) && MainEnPosition(mainDroite))
                {
                    if (tempsAvantDebutMassage == 0f)
                    {
                        tempsAvantDebutMassage = Time.time;
                    }
                    else
                    {
                        // texteDebug.text += "debut de l'exercice dans : " + (Time.time - tempsAvantDebutMassage);
                    }
                    
                    debutImpulsion.transform.position = mainPrincipale.transform.position;
                    finImpulsion.transform.position = debutImpulsion.transform.position + Vector3.down * distanceImpulsion;
                    
                    zoneDeTolerance?.SetActive(false);
                    _changeMaterial?.SetOtherMaterial();
                    texteDebug.text = mainsenPosition;

                    if (Time.time - tempsAvantDebutMassage > tempsDePreparation)
                    {
                        etatMassage = EtatMassage.EnCours;
                        tempsDebutImpulsion = Time.time;
                        // texteDebug.text = "Massage en cours";
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
            else
            {
                // verifier qu'il n'y a qu'une main pour faire le massage sur le corps enfant ou bebe
                bool mainG = MainEnPosition(mainGauche);
                bool mainD = MainEnPosition(mainDroite);
                if (!(mainD && mainG) && mainG || mainD)
                {
                    if (tempsAvantDebutMassage == 0f)
                    {
                        tempsAvantDebutMassage = Time.time;
                    }
                    else
                    {
                        // texteDebug.text += "debut de l'exercice dans : " + (Time.time - tempsAvantDebutMassage);
                    }
                    
                    debutImpulsion.transform.position = mainPrincipale.transform.position;
                    finImpulsion.transform.position = debutImpulsion.transform.position + Vector3.down * distanceImpulsion;
                    
                    zoneDeTolerance?.SetActive(false);
                    _changeMaterial?.SetOtherMaterial();
                    texteDebug.text = mainsenPosition;

                    if (Time.time - tempsAvantDebutMassage > tempsDePreparation)
                    {
                        etatMassage = EtatMassage.EnCours;
                        tempsDebutImpulsion = Time.time;
                        // texteDebug.text = "Massage en cours";
                    }
                }
                else
                {
                    zoneDeTolerance?.SetActive(true);
                    _changeMaterial?.SetOriginalMaterial();
                    tempsAvantDebutMassage = 0f;
                    texteDebug.text = mainsPasEnPosition;
                    texteDebug.text += "Une seule main doit être utilisée pour le massage";
                }
            }
        } 
        else if (etatMassage == EtatMassage.EnCours)
        {
            _consignesMassage.etatMassage = EtatMassage.EnCours;

            Vector3 positionDebutImpulsion = debutImpulsion.transform.position;
            Vector3 positionFinImpulsion = finImpulsion.transform.position;

            // verifie que les mains soient toujours sur le corps pendant le massage
            if (true || ObjetDansLaZone(mainPrincipale, debutImpulsion))
            {
                // variables d'etat instantane
                hautTouche = mainPrincipale.transform.position.y > positionDebutImpulsion.y;
                basTouche = mainPrincipale.transform.position.y < positionFinImpulsion.y;

                if (hautTouche)
                {
                    if (etatImpulsion == RETOUR && impulsionEnCours)
                    {
                        nombreImpulsions++;
                        _consignesMassage.nombreImpulsions = nombreImpulsions;
                        impulsionEnCours = false;
                    }
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
                        float tempsImpulsion = Time.time - tempsDebutImpulsion;
                        texteDebug.text = "Nombre impulsions : " + nombreImpulsions;

                        if (nombreImpulsions > 1) 
                        {
                            if (tempsImpulsion > intervalleMaxPulsation)
                            {
                                texteDebug.text += "\n\nTrop lent";
                            }
                            else if (tempsImpulsion < intervalleMinPulsation)
                            {
                                texteDebug.text += "\n\nTrop rapide";
                            }

                            // texteDebug.text += tempsImpulsion + "s";
                        }
                        
                        tempsDebutImpulsion = Time.time;
                    }
                    impulsionEnCours = true;
                    
                    // texteDebug.text = "";
                    // texteDebug.text += "\n" + (Time.time - tempsDebutImpulsion) + "s";
                }
            }
            else
            {
                impulsionEnCours = false;
                etatImpulsion = ALLER;
            }

            if (nombreImpulsions >= nombreImpulsionsParSerie)
            {
                etatMassage = EtatMassage.HorsMassage;
                ecranExerciceTermine?.SetActive(true);
                texteDebug.text = "Exercice termine";
                gestion.activerEcran65();
            }
        }
    }
}

public enum EtatMassage
{
    HorsMassage,
    Preparation,
    EnCours,
    Insufflation
}

public enum TypeCorps
{
    Adulte,
    Enfant,
    Bebe
}

public enum EtatInsufflation
{
    AFaire,
    EnCours,
    Fait
}
