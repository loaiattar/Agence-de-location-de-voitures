# Kubernetes

## Ressources déployées

### Namespace
- `caragence` — isolation de l'application

### Application
| Ressource | Nom | Description |
|-----------|-----|-------------|
| Deployment | `caragence-app` | 2 replicas, probes, resource limits |
| Service | `caragence-app` | ClusterIP, port 8080 |
| Secret | `caragence-secrets` | Connection string SQLite |
| PVC | `caragence-sqlite-pvc` | 1Gi pour SQLite |

### Nginx Reverse Proxy
| Ressource | Nom | Description |
|-----------|-----|-------------|
| Deployment | `caragence-nginx` | 2 replicas, probes |
| Service | `caragence-nginx` | NodePort, port 80 |
| ConfigMap | `nginx-config` | Configuration reverse proxy |

### Monitoring
| Ressource | Nom | Description |
|-----------|-----|-------------|
| Deployment | `prometheus` | Collecte des métriques |
| Service | `prometheus` | NodePort, port 9090 |
| Deployment | `grafana` | Visualisation |
| Service | `grafana` | NodePort, port 3000 |
| ConfigMap | `prometheus-config` | Configuration scraping |
| ConfigMap | `grafana-datasources` | Source Prometheus |
| ConfigMap | `grafana-dashboards` | Dashboard CarAgence |

## Architecture réseau

```
Utilisateur
    ↓
Nginx (NodePort 80)
    ↓
App (ClusterIP 8080)
    ↓
SQLite (/data/caragence.db)
```

## Probes de santé

### Application
- **Liveness** : `GET /health` (port 8080)
- **Readiness** : `GET /health/ready` (port 8080)

### Nginx
- **Liveness** : `GET /` (port 80)
- **Readiness** : `GET /` (port 80)

## Resources

| Ressource | Request | Limit |
|-----------|---------|-------|
| CPU App | 100m | 500m |
| Memory App | 128Mi | 256Mi |
| CPU Nginx | 50m | 200m |
| Memory Nginx | 64Mi | 128Mi |
| CPU Prometheus | 100m | 500m |
| Memory Prometheus | 128Mi | 256Mi |
| CPU Grafana | 100m | 500m |
| Memory Grafana | 128Mi | 256Mi |

## Commandes utiles

```bash
# Voir toutes les ressources
kubectl get all -n caragence

# Voir les pods avec leurs IPs
kubectl get pods -n caragence -o wide

# Voir les logs d'un pod
kubectl logs <pod-name> -n caragence

# Exec dans un pod
kubectl exec -it <pod-name> -n caragence -- /bin/sh

# Port-forward
kubectl port-forward svc/caragence-nginx 8080:80 -n caragence

# Voir les événements
kubectl get events -n caragence --sort-by='.lastTimestamp'
```
