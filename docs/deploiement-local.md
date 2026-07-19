# Déploiement local

## Prérequis

- Docker installé et démarré
- minikube installé
- kubectl installé
- Terraform >= 1.0 installé
- Ansible installé (`pip install ansible`)
- Collections Ansible : `ansible-galaxy collection install community.general kubernetes.core`

## Étapes de déploiement

### 1. Démarrer minikube

```bash
minikube start
```

### 2. Construire l'image Docker

```bash
docker build -t ghcr.io/loaiattar/agence-de-location-de-voitures:latest .
```

### 3. Charger l'image dans minikube

```bash
minikube image load ghcr.io/loaiattar/agence-de-location-de-voitures:latest
```

### 4. Exécuter le playbook Ansible

```bash
ansible-playbook ansible/playbook.yml
```

Ce playbook exécute automatiquement :
- Vérification des prérequis
- Terraform init/plan/apply
- Déploiement des ressources K8s
- Validation de la santé

### 5. Vérifier le déploiement

```bash
kubectl get all -n caragence
```

### 6. Accéder à l'application

```bash
minikube service caragence-nginx -n caragence --url
```

### 7. Accéder au monitoring

```bash
# Prometheus
minikube service prometheus -n caragence --url

# Grafana (admin/admin)
minikube service grafana -n caragence --url
```

## Commandes utiles

```bash
# Voir les pods
kubectl get pods -n caragence

# Voir les logs
kubectl logs -l app=caragence,component=backend -n caragence

# Décrire une ressource
kubectl describe deployment caragence-app -n caragence

# Port-forward temporaire
kubectl port-forward svc/caragence-nginx 8080:80 -n caragence
```

## Nettoyage

```bash
# Détruire les ressources K8s
kubectl delete namespace caragence

# Détruire l'infrastructure Terraform
cd terraform && terraform destroy

# Arrêter minikube
minikube stop

# Supprimer minikube
minikube delete
```
