# Architecture

## Vue d'ensemble

L'architecture suit un pipeline DevOps complet : code → CI/CD → infrastructure → déploiement → monitoring.

```
GitHub (code + CI)
    ↓
GitHub Actions (build + test + scan + push image)
    ↓
ghcr.io (registry Docker)
    ↓
Terraform (namespace + stockage K8s)
    ↓
Ansible (orchestration locale)
    ↓
Kubernetes (minikube)
    ├── Nginx (reverse proxy)
    ├── App (ASP.NET Core MVC)
    ├── SQLite (volume persistant)
    ├── Prometheus (métriques)
    └── Grafana (visualisation)
```

## Rôle de chaque composant

### GitHub
- Stockage central du code source
- Gestion des Pull Requests et revues de code
- Branch protection sur `main`
- Publication de l'image Docker

### GitHub Actions
- Exécution automatique des tests et du build
- Construction de l'image Docker
- Scan de sécurité avec Trivy
- Publication de l'image dans ghcr.io (uniquement sur `main`)

### Terraform
- Création du namespace Kubernetes
- Provisionnement du PersistentVolume pour SQLite
- Gestion de l'état d'infrastructure

### Ansible
- Vérification des prérequis (minikube, kubectl, terraform)
- Orchestration du déploiement complet
- Validation post-déploiement

### Kubernetes
- Orchestration des conteneurs
- Gestion du cycle de vie des pods
- Exposition des services
- Stockage persistant

### Nginx
- Point d'entrée utilisateur
- Reverse proxy vers l'application
- Terminaison SSL (si configuré)

### Application
- ASP.NET Core MVC
- SQLite pour la persistance
- Health endpoints pour Kubernetes

### Monitoring
- Prometheus : collecte des métriques
- Grafana : visualisation et dashboards
