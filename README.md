# Projet Fil Rouge - Météo VR 

## Description

Ce projet est une application de visualisation météo immersive en Réalité Virtuelle. L'utilisateur est plongé au cœur d'une **grande boule à neige** contenant un environnement **Low Poly** dynamique. L'originalité du projet réside dans sa capacité à traduire des données météorologiques en changements visuels et atmosphériques concrets au sein de cet univers miniature.

## Fonctionnalités

* **Immersion VR** : Expérience conçue pour la réalité virtuelle (compatible avec les standards XR).
* **Environnement Variable** : Le décor (ciel, éclairage, particules, environnement) s'adapte en temps réel ou selon des configurations prédéfinies.
* **Style Low Poly** : Une direction artistique épurée et performante, idéale pour la VR.
* **Concept de "Boule à Neige"** : Un monde miniature contenu qui renforce l'aspect contemplatif de l'application.

## Spécifications Techniques

* **Moteur** : Unity `6000.3.5f2`
* **Pipeline de rendu** : URP (Universal Render Pipeline)
* **Target SDK** : OpenXR / XR Interaction Toolkit.

## Installation & Utilisation

### Prérequis

* Unity Hub installé.
* Éditeur Unity version **6000.3.5f2** (ou supérieure dans la branche 6000).
* Un casque VR compatible (Oculus/Meta Quest, Valve Index, HP Reverb, etc.).

### Cloner le projet

```bash
git clone https://github.com/Ectobiologist80/ProjetFilRouge-Meteo.git

```

### Configuration

1. Ouvrez le projet via **Unity Hub**.
2. Assurez-vous que les paramètres **XR Plug-in Management** sont correctement configurés pour votre casque dans *Project Settings*.
3. Ouvrez la scène principale située dans `Assets/Scenes/`.
4. Appuyez sur **Play** ou compilez le projet via *Build Settings*.

## Structure du Projet 

* `/Assets/Models` : Assets 3D Low Poly (Environnement, Boule à neige).
* `/Assets/Prefabs` : Éléments réutilisables du décor.
* `/Assets/Materials` : Shaders et matériaux optimisés pour la VR.
*...(à suivre)

## Licence

Distribué sous la licence MIT. Voir `LICENSE` pour plus d'informations.

---

*Réalisé par [Ectobiologist80](https://github.com/Ectobiologist80) & [Gallium24](https://github.com/Gallium24)
