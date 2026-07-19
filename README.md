# Agence de Location de Voitures (CarAgence)

Application ASP.NET Core MVC de location de voitures avec persistance SQLite, déployée sur Kubernetes avec un pipeline DevOps complet.

## Objectif

Mettre en place une chaîne DevOps complète : gestion Git professionnelle, pipeline CI/CD GitHub, infrastructure locale automatisée et déploiement Kubernetes sur minikube.

## Prérequis locaux

- .NET 8.0 SDK
- Docker
- minikube
- kubectl
- Terraform >= 1.0
- Ansible (`pip install ansible`)
- GitHub CLI (`gh`)

## Structure du projet

```
├── src/
│   ├── CarAgence.Domain/        # Entités métier
│   ├── CarAgence.Data/          # Contexte EF Core + SQLite
│   ├── CarAgence.Services/      # Logique métier
│   └── CarAgence.Web/           # Application web MVC
├── tests/
│   └── CarAgence.Tests/       # Tests unitaires (160 tests)
├── .github/workflows/
│   └── ci.yml                 # Pipeline CI/CD
├── terraform/                 # Infrastructure K8s
├── ansible/                   # Orchestration déploiement
├── k8s/                       # Manifests Kubernetes
├── monitoring/                # Config Prometheus + Grafana
├── Dockerfile                 # Image Docker
└── docs/                      # Documentation
```

## Architecture

```
GitHub → GitHub Actions → ghcr.io → Terraform → Ansible → Kubernetes
                                                                    ├── Nginx (reverse proxy)
                                                                    ├── App (ASP.NET Core)
                                                                    ├── SQLite (volume persistant)
                                                                    ├── Prometheus
                                                                    └── Grafana
```

Voir [docs/architecture.md](docs/architecture.md) pour le détail.

## Démarrage rapide

```bash
# 1. Démarrer minikube
minikube start

# 2. Construire l'image Docker
docker build -t ghcr.io/loaiattar/agence-de-location-de-voitures:latest .

# 3. Charger l'image dans minikube
minikube image load ghcr.io/loaiattar/agence-de-location-de-voitures:latest

# 4. Déployer avec Ansible
ansible-playbook ansible/playbook.yml

# 5. Accéder à l'application
minikube service caragence-nginx -n caragence --url
```

## Documentation

| Document | Description |
|----------|-------------|
| [Documentation complète](docs/documentation.md) | Vue d'ensemble DevOps du projet |
| [Architecture](docs/architecture.md) | Vue d'ensemble de l'architecture |
| [CI/CD](docs/ci-cd.md) | Pipeline GitHub Actions |
| [Déploiement local](docs/deploiement-local.md) | Étapes de déploiement |
| [Terraform](docs/terraform.md) | Infrastructure automatisée |
| [Ansible](docs/ansible.md) | Orchestration locale |
| [Kubernetes](docs/kubernetes.md) | Manifests et ressources K8s |
| [Monitoring](docs/monitoring.md) | Prometheus et Grafana |
| [Exploitation](docs/exploitation.md) | Vérifications et diagnostic |

## Branche `main` protégée

- Pas de push direct — Pull Request obligatoire
- CI obligatoire — `build-and-test` doit passer
- 1 approval requis
- Stale reviews révoqués

## Documentation

| Document | Contenu |
|----------|---------|
| [Architecture](docs/architecture.md) | Vue d'ensemble, composants, flux |
| [CI/CD](docs/ci-cd.md) | Pipeline GitHub Actions, règles de branche |
| [Déploiement local](docs/deploiement-local.md) | Étapes complètes minikube |
| [Terraform](docs/terraform.md) | Infrastructure, variables, outputs |
| [Ansible](docs/ansible.md) | Playbook, étapes orchestrées |
| [Kubernetes](docs/kubernetes.md) | Ressources K8s déployées |
| [Monitoring](docs/monitoring.md) | Prometheus, Grafana, alertes |
| [Exploitation](docs/exploitation.md) | Diagnostic, rollback, problèmes |
| [Helm](docs/helm.md) | Chart bonus (si réalisé) |

## Architecture

```
Utilisateur ──> Nginx (NodePort:80) ──> App ASP.NET (ClusterIP:8080) ──> SQLite (PVC)
                                                                 │
                                                    Prometheus ────┘
                                                         │
                                                     Grafana (dashboard)
```

## Pipeline CI/CD

```
Push feature/* ──> build-and-test ──> PR
                                          │
Push main ──> build-and-test ──> docker-build ──> Trivy scan ──> push ghcr.io
```

## Licence

Projet pédagogique — Développement Web II
