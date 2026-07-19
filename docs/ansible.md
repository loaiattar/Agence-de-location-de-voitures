# Ansible

## Rôle du playbook

Le playbook Ansible orchestre le déploiement complet de l'application sur minikube depuis votre machine locale.

## Étapes orchestrées

### 1. Vérification des préreis
- minikube installé et en cours d'exécution
- kubectl installé et configuré
- Terraform installé

### 2. Infrastructure Terraform
- `terraform init` — initialisation du provider
- `terraform plan` — prévisualisation des changements
- `terraform apply` — création des ressources
- Récupération des outputs (namespace, PV, PVC)

### 3. Déploiement Kubernetes
- Application de la namespace
- Application du PVC pour SQLite
- Application des secrets
- Application de la ConfigMap Nginx
- Déploiement de l'application
- Déploiement de Nginx
- Création des services

### 4. Validation
- Attente que les pods soient prêts
- Vérification du health endpoint
- Affichage de l'URL d'accès

## Préreis Ansible

```bash
# Installer Ansible
pip install ansible

# Installer les collections nécessaires
ansible-galaxy collection install community.general
ansible-galaxy collection install kubernetes.core
```

## Utilisation

```bash
# Vérifier la syntaxe
ansible-playbook ansible/playbook.yml --syntax-check

# Dry run
ansible-playbook ansible/playbook.yml --check

# Exécuter
ansible-playbook ansible/playbook.yml
```

## Variables

Les variables sont définies dans `ansible/defaults/main.yml` :

| Variable | Valeur | Description |
|----------|--------|-------------|
| `app_name` | `caragence` | Nom de l'application |
| `namespace` | `caragence` | Namespace Kubernetes |
| `app_image` | `ghcr.io/...` | Image Docker |
| `app_tag` | `latest` | Tag de l'image |
| `terraform_dir` | `../terraform` | Répertoire Terraform |
| `k8s_manifests_dir` | `../k8s` | Répertoire des manifests |
