# Déploiement local

## Prérequis

- Docker installé et en cours d'exécution
- minikube installé
- kubectl installé
- Terraform >= 1.0 installé
- Ansible installé (`pip install ansible`)
- Collections Ansible : `ansible-galaxy collection install community.general kubernetes.core`
- Image Docker publiée sur ghcr.io (via pipeline CI sur `main`)

## Étapes de déploiement

### 1. Vérifier que minikube est prêt

```bash
minikube status
# Devrait afficher : Running
```

Si minikube n'est pas démarré :
```bash
minikube start
```

### 2. Configurer kubectl

```bash
kubectl config use-context minikube
kubectl get nodes
```

### 3. Exécuter le playbook Ansible

Le playbook enchaîne automatiquement :
1. Vérification des prérequis
2. Terraform init/plan/apply (namespace, PV, PVC)
3. Déploiement K8s (app, nginx, monitoring)
4. Vérification de santé
5. Affichage des URLs

```bash
cd ansible/
ansible-playbook playbook.yml
```

### 4. Vérifier le déploiement

```bash
# Tous les pods
kubectl get all -n caragence

# Pods de l'application
kubectl get pods -n caragence -l component=backend

# Pods nginx
kubectl get pods -n caragence -l component=proxy

# Monitoring
kubectl get pods -n caragence -l component=monitoring
kubectl get pods -n caragence -l component=grafana
```

### 5. Accéder aux services

```bash
# Application (via Nginx)
minikube service caragence-nginx -n caragence --url

# Prometheus
minikube service prometheus -n caragence --url

# Grafana (admin/admin)
minikube service grafana -n caragence --url
```

### 6. Vérifier la santé

```bash
# Health check
curl http://$(minikube service caragence-nginx -n caragence --url | sed 's|https\?://||')/health

# Readiness check
curl http://$(minikube service caragence-nginx -n caragence --url | sed 's|https\?://||')/health/ready
```

## Déploiement sans Ansible

Si vous préférez exécuter les étapes manuellement :

```bash
# Terraform
cd terraform/
terraform init
terraform apply

# K8s manifests
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/pvc-sqlite.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/configmap-nginx.yaml
kubectl apply -f k8s/deployment-app.yaml
kubectl apply -f k8s/deployment-nginx.yaml
kubectl apply -f k8s/service-app.yaml
kubectl apply -f k8s/service-nginx.yaml

# Monitoring
kubectl apply -f monitoring/prometheus-configmap.yaml
kubectl apply -f monitoring/prometheus-deployment.yaml
kubectl apply -f monitoring/prometheus-service.yaml
kubectl apply -f monitoring/grafana-datasource-configmap.yaml
kubectl apply -f monitoring/grafana-dashboard-configmap.yaml
kubectl apply -f monitoring/grafana-deployment.yaml
kubectl apply -f monitoring/grafana-service.yaml
```

## Développement local (sans minikube)

```bash
docker-compose up --build
# App accessible sur http://localhost:80
```

## Nettoyage

```bash
# Supprimer les ressources K8s
kubectl delete namespace caragence

# Détruire l'infrastructure Terraform
cd terraform/
terraform destroy

# Arrêter minikube
minikube stop

# Supprimer minikube
minikube delete
```
