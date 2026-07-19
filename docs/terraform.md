# Terraform - Infrastructure locale

## Ressources gérées

| Ressource | Description |
|-----------|-------------|
| `kubernetes_namespace` | Namespace `caragence` pour l'application |
| `kubernetes_persistent_volume` | PV hostPath `/data/caragence-sqlite` |
| `kubernetes_persistent_volume_claim` | PVC 1Gi lié au PV |

## Variables

| Variable | Type | Défaut | Description |
|----------|------|--------|-------------|
| `namespace` | string | `caragence` | Namespace Kubernetes |
| `app_name` | string | `caragence` | Nom de l'application |
| `storage_size` | string | `1Gi` | Taille du volume SQLite |
| `storage_class` | string | `standard` | Storage class |
| `kubeconfig_path` | string | `~/.kube/config` | Chemin kubeconfig |
| `kubeconfig_context` | string | `minikube` | Contexte minikube |

## Outputs

| Output | Description |
|--------|-------------|
| `namespace` | Nom du namespace |
| `pv_name` | Nom du PersistentVolume |
| `pvc_name` | Nom du PersistentVolumeClaim |
| `storage_path` | Chemin hostPath pour SQLite |

## Utilisation

```bash
cd terraform/

# Initialiser
terraform init

# Vérifier la syntaxe
terraform validate

# Prévisualiser les changements
terraform plan

# Appliquer
terraform apply

# Récupérer les outputs
terraform output
```

## Gestion de l'état

- L'état Terraform (`terraform.tfstate`) n'est **pas versionné**
- Le `.gitignore` exclut `*.tfstate`, `.terraform/`, `terraform.tfvars`
- Pour un environnement partagé, utiliser un backend distant (S3, GCS, etc.)

## Nettoyage

```bash
terraform destroy
```
