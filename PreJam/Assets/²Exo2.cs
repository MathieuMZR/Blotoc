using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Faire qu'un VFX suive des points en un temps donné.
public class Exo1 : MonoBehaviour
{
    // Déclarez un tableau de points que le VFX va suivre. 
    // Tu dois remplir ce tableau avec les positions à suivre.

    // Temps total en secondes pour parcourir tous les points.

    // Variable pour stocker l'effet visuel (VFX).

    // Variable interne pour garder la durée actuelle du mouvement.
    private float timer;

    // Variable interne pour stocker la position de départ.
    private Vector3 startPosition;

    // Index du point courant dans la liste des points.
    private int currentPointIndex = 0;
    
    void Start()
    {
        // TODO: Initialise la position de départ du VFX avec la position du premier point.
        // startPosition = ...

        // TODO: Assure-toi que le VFX commence à la position du premier point de la liste.
        // vfxObject.transform.position = ...
    }
    
    void Update()
    {
        // TODO: Incrémente le timer en fonction du temps écoulé depuis la dernière frame.
        // timer += ...

        // TODO: Calcule le pourcentage de progression entre les points en utilisant le timer et le temps total.
        // float progress = ...
        
        // TODO: Détermine la position actuelle du VFX en fonction du pourcentage de progression entre les points.
        // Si le pourcentage de progression atteint 1.0f, passe au point suivant.
        // Utilise Vector3.Lerp pour interpoler entre deux positions.
        // Exemple : Vector3.Lerp(startPosition, nextPosition, progress);
        // vfxObject.transform.position = ...

        // TODO: Lorsque le VFX atteint le dernier point, réinitialise le timer ou arrête le mouvement.
        // if (currentPointIndex >= points.Length)
        // {
        //     // Arrête ou réinitialise l'exécution du code.
        // }
    }
}

