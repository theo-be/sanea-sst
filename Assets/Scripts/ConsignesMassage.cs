using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ConsignesMassage : MonoBehaviour
{
    public TMP_Text texteEcran;
    public EtatMassage etatMassage = EtatMassage.HorsMassage;
    public TypeCorps typeCorps = TypeCorps.Adulte;
    private string texteInsufflation = "Faire {0} insufflations";
    private string texteMassage = "Faire {0} impulsions";
    private string textePreparationAdulte = "Mettre les mains en position";
    private string textePreparationEnfant = "Mettre une seule main en position";
    private string textePreparationBebe = "Mettre deux doigts en position";
    public int nombreImpulsions;
    [FormerlySerializedAs("nomreMaxImpulsions")] public int nombreMaxImpulsions;

    public int nombreInsufflations;
    public int nombreMaxInsufflations;

    public bool exerciceTermine;
    
    
    void Start()
    {
        
    }

    void Update()
    {
        if (!exerciceTermine)
        {
            switch (etatMassage)
            {
                case EtatMassage.Insufflation:
                    texteEcran.text = string.Format(texteInsufflation, nombreInsufflations);
                    texteEcran.text += string.Format("\n({0}/{1})", nombreInsufflations, nombreMaxInsufflations);
                    break;
                case EtatMassage.Preparation:
                    switch (typeCorps)
                    {
                        case TypeCorps.Adulte:
                            texteEcran.text = textePreparationAdulte;
                            break;
                        case TypeCorps.Enfant:
                            texteEcran.text = textePreparationEnfant;
                            break;
                        case TypeCorps.Bebe:
                            texteEcran.text = textePreparationBebe;
                            break;
                        default:
                            break;
                    }

                    break;
                case EtatMassage.EnCours:
                    texteEcran.text = string.Format(texteMassage, nombreMaxImpulsions);
                    texteEcran.text += string.Format("\n({0}/{1})", nombreImpulsions, nombreMaxImpulsions);
                    break;
                default:
                    break;
            }
        }
        else
        {
            
        }
    }
}
