# Agence de Location de Voitures (CarAgence)

Application ASP.NET Core MVC de location de voitures avec persistance SQLite, déployée sur Kubernetes via une chaîne DevOps complète.

## Objectif

Reprendre un projet de POO (Locatic) et y appliquer les bonnes pratiques DevOps :
- Gestion Git professionnelle avec Pull Requests
- Pipeline CI/CD automatisé (GitHub Actions)
- Infrastructure as Code (Terraform)
- Orchestration de déploiement (Ansible)
- Déploiement Kubernetes sur minikube
- Monitoring (Prometheus + Grafana)

## Prérequis

| Outil | Version minimale | Usage |
|-------|-----------------|-------|
| .NET SDK | 8.0 | Build et test |
| Docker | latest | Conteneurisation |
| minikube | latest | Cluster K8s local |
| kubectl | latest | CLI Kubernetes |
| Terraform | >= 1.0 | Infrastructure |
| Ansible | >= 2.12 | Orchestration |
| gh (GitHub CLI) | latest | Interaction GitHub |

## Structure du projet

```
├── src/
│   ├── CarAgence.Domain/        # Entités métier
│   ├── CarAgence.Data/          # Contexte EF Core + SQLite
│   ├── CarAgence.Services/      # Logique métier
│   └── CarAgence.Web/           # Application web MVC
├── tests/
│   └── CarAgence.Tests/         # Tests unitaires (~119 tests)
├── .github/workflows/           # CI/CD GitHub Actions
├── ansible/                     # Playbook d'orchestration
├── terraform/                   # Infrastructure K8s
├── k8s/                         # Manifests Kubernetes
├── monitoring/                  # Prometheus + Grafana
├── docker/                      # Configuration nginx pour docker-compose
├── docs/                        # Documentation
├── Dockerfile                   # Image Docker multi-stage
├── docker-compose.yml           # Développement local
└── README.md
```

## Démarrage rapide

### Développement local (docker-compose)

```bash
docker-compose up --build
# App accessible sur http://localhost:80
```

### Déploiement sur minikube

```bash
# 1. Démarrer minikube
minikube start

# 2. Exécuter le playbook Ansible
cd ansible/
ansible-playbook playbook.yml

# 3. Accéder aux services
minikube service caragence-nginx -n caragence --url    # App
minikube service grafana -n caragence --url             # Grafana (admin/admin)
minikube service prometheus -n caragence --url          # Prometheus
```

## Développement

```bash
dotnet restore
dotnet build
dotnet test
```

## Branche `main` protégée

La branche `main` est protégée :

- **Pas de push direct** — tous les changements passent par Pull Request
- **CI obligatoire** — le job `build-and-test` doit passer avant merge
- **1 approval requis** — au moins 1 revue obligatoire
- **Stale reviews** — les approvals sont révoquées si de nouveaux commits sont poussés

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
