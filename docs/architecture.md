# Architecture

## Vue d'ensemble

CarAgence est une application ASP.NET Core MVC de location de voitures, déployée sur un cluster Kubernetes local (minikube) avec une chaîne DevOps complète.

```
GitHub ──> GitHub Actions ──> ghcr.io (image Docker)
                                    │
                                    ▼
                            ┌──────────────────┐
                            │   Machine locale  │
                            │                   │
                            │  Terraform ──> K8s │
                            │  Ansible ──> K8s   │
                            │                   │
                            │  ┌─────────────┐  │
                            │  │  minikube    │  │
                            │  │             │  │
                            │  │  Nginx ──> App │
                            │  │  Prometheus  │  │
                            │  │  Grafana     │  │
                            │  │  SQLite (PV) │  │
                            │  └─────────────┘  │
                            └──────────────────┘
```

## Composants

### Application (ASP.NET Core MVC)
- **Domain** : entités métier (Marque, Modele, Voiture, Client, Reservation)
- **Data** : contexte EF Core + SQLite + migrations
- **Web** : controllers MVC + vues Razor + Tailwind CSS
- **Tests** : ~119 tests unitaires (xUnit + Moq)

### Infrastructure
| Composant | Rôle |
|-----------|------|
| **GitHub Actions** | CI/CD : build, test, Docker, Trivy, push ghcr.io |
| **Terraform** | Prépare l'infrastructure K8s (namespace, PV, PVC) |
| **Ansible** | Orchestre le déploiement complet sur minikube |
| **Docker** | Image multi-stage, utilisateur non-root |
| **Kubernetes** | Orchestration des pods, services, stockage |

### Déploiement K8s
| Ressource | Description |
|-----------|-------------|
| **Deployment app** | 2 replicas de l'application ASP.NET |
| **Deployment nginx** | 2 replicas du reverse proxy |
| **Service app** | ClusterIP port 8080 |
| **Service nginx** | NodePort port 80 (point d'entrée) |
| **PVC SQLite** | Volume persistant 1Gi pour la base |
| **Secret** | Connection string SQLite |
| **ConfigMap nginx** | Configuration reverse proxy |

### Monitoring
| Composant | Port | Rôle |
|-----------|------|------|
| **Prometheus** | NodePort 30090 | Collecte des métriques (15s) |
| **Grafana** | NodePort 30030 | Visualisation (dashboard 8 panneaux) |

## Flux de déploiement

1. Développeur crée une branche `feature/*` depuis `develop`
2. Push → GitHub Actions exécute build + test
3. PR vers `develop` ou `main` → CI valide
4. Merge vers `main` → Docker build + Trivy scan + push ghcr.io
5. Sur la machine locale :
   - `terraform init && terraform apply` → namespace, PV, PVC
   - `ansible-playbook ansible/playbook.yml` → déploiement complet K8s
6. Application accessible via Nginx sur minikube

## Persistance SQLite

SQLite utilise un PersistentVolume (hostPath `/data/caragence-sqlite`) monté dans le pod applicatif au `/data`. Le fichier `caragence.db` survit aux redémarrages de pods.

## Réseau

```
Utilisateur ──> Nginx (NodePort:80) ──> App (ClusterIP:8080) ──> SQLite (/data/caragence.db)
```

Nginx est le seul point d'entrée exposé. L'application n'est pas accessible directement depuis l'extérieur.
