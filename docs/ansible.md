# Ansible - Orchestration locale

## Rôle

Le playbook Ansible orchestre le déploiement complet de CarAgence sur minikube depuis votre machine locale.

## Prérequis

```bash
pip install ansible
ansible-galaxy collection install community.general kubernetes.core
```

## Étapes orchestrées

Le playbook exécute dans l'ordre :

1. **Vérification des prérequis** — minikube, kubectl, terraform installés
2. **Vérification minikube** — cluster en cours d'exécution
3. **Contexte kubectl** — basculement sur minikube
4. **Terraform** — init, plan, apply (namespace, PV, PVC)
5. **Resssources K8s de base** — namespace, PVC, secret, configmap nginx
6. **Déploiement applicatif** — deployment app, deployment nginx, services
7. **Monitoring** — Prometheus (ConfigMap, Deployment, Service) + Grafana (Datasource, Dashboard, Deployment, Service)
8. **Vérification** — attente pods prêts, health check
9. **Résumé** — URLs d'accès à l'app, Prometheus, Grafana

## Dépendance aux outputs Terraform

Le playbook récupère automatiquement les outputs Terraform :
- `namespace` — utilisé pour cibler le namespace K8s
- `pv_name` — nom du PersistentVolume
- `pvc_name` — nom du PersistentVolumeClaim
- `storage_path` — chemin de stockage SQLite

## Variables

Définies dans `ansible/defaults/main.yml` :

| Variable | Valeur | Description |
|----------|--------|-------------|
| `app_name` | `caragence` | Nom de l'application |
| `namespace` | `caragence` | Namespace K8s |
| `app_image` | `ghcr.io/loaiattar/agence-de-location-de-voitures` | Image Docker |
| `app_tag` | `latest` | Tag de l'image |
| `terraform_dir` | `../terraform` | Chemin vers Terraform |
| `k8s_manifests_dir` | `../k8s` | Chemin vers manifests K8s |
| `monitoring_dir` | `../monitoring` | Chemin vers manifests monitoring |

## Utilisation

```bash
cd ansible/

# Vérifier la syntaxe
ansible-playbook playbook.yml --syntax-check

# Simuler (dry run)
ansible-playbook playbook.yml --check

# Exécuter
ansible-playbook playbook.yml
```

## Sortie attendue

```
=== Deployment Complete ===
Namespace: caragence
PV: caragence-sqlite-pv
PVC: caragence-sqlite-pvc
Storage: /data/caragence-sqlite
App URL: http://<minikube-ip>:<nodeport>
Health: http://<minikube-ip>:<nodeport>/health
Prometheus: http://<minikube-ip>:<nodeport>
Grafana: http://<minikube-ip>:<nodeport> (admin/admin)
```
