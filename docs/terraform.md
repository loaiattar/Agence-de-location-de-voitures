# Terraform

## Ressources gérées

| Ressource | Description |
|-----------|-------------|
| `kubernetes_namespace` | Namespace `caragence` pour isoler l'application |
| `kubernetes_persistent_volume` | PV pour le stockage SQLite |
| `kubernetes_persistent_volume_claim` | PVC pour lier le volume à l'application |

## Variables

| Variable | Défaut | Description |
|----------|--------|-------------|
| `namespace` | `caragence` | Namespace Kubernetes |
| `app_name` | `caragence` | Nom de l'application |
| `storage_size` | `1Gi` | Taille du volume SQLite |
| `storage_class` | `standard` | Classe de stockage |
| `kubeconfig_path` | `~/.kube/config` | Chemin vers kubeconfig |
| `kubeconfig_context` | `minikube` | Contexte Kubernetes |

## Outputs

| Output | Description |
|--------|-------------|
| `namespace` | Namespace créé |
| `pv_name` | Nom du PersistentVolume |
| `pvc_name` | Nom du PersistentVolumeClaim |
| `storage_path` | Chemin hôte pour SQLite |

## Utilisation

```bash
cd terraform

# Initialiser
terraform init

# Valider
terraform validate

# Prévisualiser
terraform plan

# Appliquer
terraform apply

# Voir les outputs
terraform output
```

## Gestion de l'état

- L'état Terraform (`terraform.tfstate`) n'est **pas versionné**
- Le fichier `.gitignore` exclut tous les fichiers d'état
- En cas de perte, relancer `terraform apply` recrée les ressources
