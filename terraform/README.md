# Terraform - Infrastructure locale

Prépare l'infrastructure Kubernetes sur minikube pour le déploiement de l'application.

## Ressources gérées

- **Namespace** `caragence` — isolation de l'application
- **PersistentVolume** — stockage SQLite sur `/data/caragence/sqlite`
- **PersistentVolumeClaim** — revendication du stockage

## Prérequis

- Terraform >= 1.0
- kubectl configuré avec minikube
- Minikube démarré

## Utilisation

```bash
# Initialiser
terraform init

# Vérifier la syntaxe
terraform validate

# Prévisualiser
terraform plan

# Appliquer
terraform apply

# Récupérer les outputs
terraform output
```

## Nettoyage

```bash
terraform destroy
```
