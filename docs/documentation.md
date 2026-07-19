# CarAgence — DevOps WEB2

## Objectif

Ce dépôt reprend l'application CarAgence (agence de location de voitures, ASP.NET Core MVC + SQLite, réalisée en POO) pour y bâtir une chaîne DevOps complète : Pull Requests protégées, intégration continue GitHub Actions, publication d'image Docker, infrastructure Terraform, orchestration Ansible et déploiement Kubernetes sur minikube, avec Nginx en reverse proxy et une supervision Prometheus/Grafana.

GitHub Actions s'arrête volontairement après la publication de l'image : le déploiement cible votre machine locale (minikube), un runner GitHub ne peut pas y accéder. Le détail de cette limite est expliqué dans `docs/ci-cd.md`.

## Lien avec le projet de POO

Le code applicatif (`src/CarAgence.*`) est celui du projet de POO CarAgence : entités Marque → Modele → Voiture, Client, Reservation, persistées en SQLite via EF Core, contrôleurs CRUD, vues Razor. Aucune fonctionnalité métier n'a été réécrite ; les seuls ajouts concernent l'exploitation en conteneur :

- endpoint de santé `/health` et endpoint de métriques `/metrics`
- chaîne de connexion SQLite surchargeable par variable d'environnement (`ConnectionStrings__DefaultConnection`)
- `Dockerfile` pour la conteneurisation
- suite de tests `tests/CarAgence.Tests/` (contrôleurs + helpers d'affichage)
- tout ce qui est sous `.github/`, `terraform/`, `ansible/`, `k8s/`, `monitoring/` et `docs/`.

## Structure du dépôt

```
.
├── src/
│   ├── CarAgence.Domain/        # Entités métier (Marque, Modele, Voiture, Client, Reservation)
│   ├── CarAgence.Data/          # Contexte EF Core + SQLite + Migrations
│   ├── CarAgence.Services/      # Logique métier
│   └── CarAgence.Web/           # Application web MVC (Controllers, Views, Program.cs)
├── tests/
│   └── CarAgence.Tests/         # Tests automatisés (xUnit + Moq, ~119 tests)
├── CarAgence.sln
├── .github/
│   └── workflows/
│       └── ci.yml               # Pipeline CI/CD (build, test, Docker, Trivy, push ghcr.io)
├── terraform/
│   ├── main.tf                  # Namespace + PVC SQLite sur minikube
│   ├── variables.tf             # Variables configurables
│   ├── outputs.tf               # Outputs pour Ansible
│   └── providers.tf             # Configuration provider Kubernetes
├── ansible/
│   ├── playbook.yml             # Playbook d'orchestration du déploiement local
│   ├── defaults/main.yml        # Variables par défaut
│   └── inventory.ini            # Inventaire local
├── k8s/                         # Manifests Kubernetes
│   ├── namespace.yaml
│   ├── deployment-app.yaml
│   ├── deployment-nginx.yaml
│   ├── service-app.yaml
│   ├── service-nginx.yaml
│   ├── pvc-sqlite.yaml
│   ├── secret.yaml
│   └── configmap-nginx.yaml
├── monitoring/                  # Stack Prometheus/Grafana
│   ├── prometheus-*.yaml
│   └── grafana-*.yaml
├── docker/
│   └── nginx.conf               # Configuration Nginx pour reverse proxy
├── Dockerfile                   # Image Docker multi-stage
├── docker-compose.yml           # Développement local
├── docs/                        # Documentation détaillée
└── README.md
```

## Prérequis locaux

- .NET SDK 8.0
- Docker (build de l'image + minikube driver)
- minikube et kubectl
- Terraform >= 1.5
- Ansible (ansible-playbook)
- gh (GitHub CLI)

Vérification rapide :

```bash
dotnet --version && docker --version && minikube version && kubectl version --client && terraform --version && ansible --version
```

## Démarrage rapide

### 1. Lancer l'application en local (sans conteneur)

```bash
dotnet restore
dotnet build
dotnet run --project src/CarAgence.Web
```

L'application applique les migrations EF Core et seed la base au démarrage (`Program.cs`). Elle écoute par défaut sur le port configuré par `launchSettings.json`.

### 2. Lancer les tests

```bash
dotnet test CarAgence.sln
```

### 3. Construire et lancer l'image Docker

```bash
docker build -t caragence:local .
docker run --rm -p 8080:8080 caragence:local
curl http://localhost:8080/health
```

### 4. Déployer sur minikube (Terraform → Ansible → Kubernetes)

Résumé (détail complet dans `docs/deploiement-local.md`) :

```bash
minikube start
docker build -t ghcr.io/loaiattar/agence-de-location-de-voitures:latest .
minikube image load ghcr.io/loaiattar/agence-de-location-de-voitures:latest

ansible-playbook ansible/playbook.yml
```

Ansible déploie ensuite Nginx, l'application (derrière Nginx, avec son volume SQLite) et la stack Prometheus/Grafana sur le cluster minikube.

## Documentation

| Fichier | Contenu |
|---------|---------|
| `docs/architecture.md` | Vue d'ensemble de l'architecture et rôle de chaque composant |
| `docs/ci-cd.md` | Règles de branche, Pull Requests, pipeline GitHub Actions, limites |
| `docs/deploiement-local.md` | Ordre exact des actions locales, de l'image publiée au déploiement minikube |
| `docs/terraform.md` | Ressources Terraform, variables, outputs, gestion de l'état |
| `docs/ansible.md` | Rôle du playbook, étapes orchestrées, dépendance aux outputs Terraform |
| `docs/kubernetes.md` | Ressources Kubernetes, services exposés, stockage SQLite, Nginx |
| `docs/monitoring.md` | Services monitorés, métriques, accès Prometheus/Grafana, dashboard |
| `docs/exploitation.md` | Vérifications post-déploiement, logs, rollback, limites connues |
| `docs/helm.md` | Structure du chart Helm (bonus, si réalisé) |
| `docs/preuves/` | Captures et extraits de logs des étapes clés |

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

GitHub Actions s'arrête après la publication de l'image Docker. Le déploiement sur minikube est déclenché manuellement via Terraform + Ansible.

## Limites connues

Voir la section dédiée dans `docs/exploitation.md`.
