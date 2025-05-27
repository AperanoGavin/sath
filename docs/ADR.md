# ADR 0001: Architecture for Parking Reservation System

**Status:** In Progress  
**Date:** 2025-05-26  
**Authors:** Totally Spies!

## 1. Contexte et Problématique

Depuis la mise en place du travail hybride postpandémie, la gestion des places de parking est devenue inefficace :  
Réservation manuelle par e-mail et Excel, source d’erreurs et de surcharge pour les secrétaires.

- Besoin de réservations par demi-journée, check-in/check-out, et libération automatique si non confirmé.
- Différents profils utilisateurs (employé, secrétaire, manager) avec droits et vues spécifiques.
- Traçabilité obligatoire de toutes les actions pour audit et reporting.

## 2. Contexte et Contraintes

### 2.1 Contexte

- Une interface intuitive, accessible aux utilisateurs non techniques.
- Prise en compte de rôles avec actions privilégiées (employés, secrétaires, managers).

### 2.2 Traçabilité & Historique

- Conservation immuable des événements : création, modification, annulation, check-in, check-out, expiration.
- Stockage de chaque action validée dans l’historique pour audit, reporting et analyse d’usage.

### 2.3 Notifications en temps réel

- Notifications in-app après chaque action importante : confirmation de réservation, rappel de check-in, libération automatique, rappel de réservation la veille.

### 2.4 Intégration événementielle

- Émission de messages vers une file (queue) pour traitement asynchrone (e.g., service d’envoi d’e-mails ou service de notification SMS).

## 3. Drivers de la Décision

- Utilisateurs attendus : non-techniques, besoin de simplicité.
- Équipe actuellement familière avec TypeScript & Angular.
- Base de données choisie : SurrealDB (graphiques et documents, requêtes flexibles).
- Besoin de notifications push et de traitement asynchrone.
- Administration fine des accès et audit.

## 4. Options Considérées

| Option | Frontend | Backend                                  | Notifications                 | Auth & RBAC                       | Conteneurs     |
| ------ | -------- | ---------------------------------------- | ----------------------------- | --------------------------------- | -------------- |
| A      | Angular  | ASP.NET Core (C#)                        | WebSocket / SSE + Apache Iggy | AWS Cognito                       | Docker + K8s   |
| B      | React    | Node – Express (JavaScript / TypeScript) | Server-Sent Events + Kafka    | Database authentication SurrealDB | Docker + K8s   |
| C      | Vue.js   | Express.js (JS / TS)                     | Polling HTTP + Redis Pub/Sub  | JWT + custom RBAC                 | Docker + Swarm |

## 5. Décision

Nous retenons l’Option A :

- **Frontend : Angular**

  - Composants modulaires, formulaires et guards (route protection).
  - TypeScript unifié avec le backend NestJS.

- **Backend : ASP.NET Core (C#)**

  - Architecture DDD et hexagonale, modules, contrôleurs et services.
  - Support natif de WebSockets, microservices et queues (Apache Iggy).

- **Base de données : SurrealDB**

  - Modèle hybride (graph + document) pour historisation naturelle.
  - Requêtes flexibles pour reporting.

- **Messaging & Notifications**

  - Apache Iggy pour découplage et fiabilité.
  - WebSockets pour notifications en temps réel dans l’app.

- **Auth & RBAC**

  - AWS Cognito.
  - Synchronisation LDAP / Active Directory.

- **Conteneurisation & Orchestration**

  - Docker pour chaque service.
  - Kubernetes (Helm) pour déploiement, scaling et rollback.

- **CI/CD & Qualité**
  - GitHub Actions pour pipelines build/test/deploy.
  - SonarCloud pour qualité de code, tests automatisés et couverture.

## 6. Conséquences

- **Courbe d’apprentissage :** Angular et ASP.NET Core peuvent être plus lourds initialement, surtout en suivant les bonnes pratiques.
- **Infrastructure :** Besoin de cluster Kubernetes.
- **Complexité opérationnelle :** Monitoring (Prometheus + Grafana), logging (ELK).
- **Évolutivité :** Architecture découplée facilitant l’ajout de features (API, mobile PWA).

## 7. Alternatives Rejetées

- **Option B (React + Node Express)** : Puissante, mais déphasage avec la stack de l’équipe ; moins d’affinité, développement plus lent et moins agréable.
- **Option C (Vue + Express)** : Simple, mais moins robuste pour microservices et scalabilité.
- **Option D (Django)** : Self explanatory.
