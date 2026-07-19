# Kubernetes - Ressources déployées

## Namespace

`caragence` — isolation de toutes les ressources de l'application.

## Ressources applicatives

### Deployments

| Deployment | Replicas | Image | Port |
|------------|----------|-------|------|
| `caragence-app` | 2 | `ghcr.io/.../agence-de-location-de-voitures:latest` | 8080 |
| `caragence-nginx` | 2 | `nginx:alpine` | 80 |

### Services

| Service | Type | Port | Sélecteur |
|---------|------|------|-----------|
| `caragence-app` | ClusterIP | 8080 | `app=caragence, component=backend` |
| `caragence-nginx` | NodePort | 80 | `app=caragence, component=proxy` |

### Stockage

| Ressource | Type | Taille | Description |
|-----------|------|--------|-------------|
| `caragence-sqlite-pv` | PersistentVolume | 1Gi | hostPath `/data/caragence-sqlite` |
| `caragence-sqlite-pvc` | PersistentVolumeClaim | 1Gi | Bind au PV |

### Configuration

| Ressource | Type | Contenu |
|-----------|------|---------|
| `nginx-config` | ConfigMap | Configuration reverse proxy Nginx |
| `caragence-secrets` | Secret | Connection string SQLite |

## Ressources monitoring

### Deployments

| Deployment | Image | Port |
|------------|-------|------|
| `prometheus` | `prom/prometheus:latest` | 9090 |
| `grafana` | `grafana/grafana:latest` | 3000 |

### Services

| Service | Type | Port | NodePort |
|---------|------|------|----------|
| `prometheus` | NodePort | 9090 | 30090 |
| `grafana` | NodePort | 3000 | 30030 |

### Configuration

| Ressource | Type | Contenu |
|-----------|------|---------|
| `prometheus-config` | ConfigMap | Config scrape + alert rules |
| `grafana-datasource` | ConfigMap | Datasource Prometheus auto-provisionnée |
| `grafana-dashboard` | ConfigMap | Dashboard 8 panneaux |

## Configuration Nginx

Nginx sert de reverse proxy :
- `/` → `http://caragence-app:8080` (proxy_pass)
- `/health` → health check passthrough
- `/health/ready` → readiness check passthrough
- Headers : X-Forwarded-For, X-Forwarded-Proto, Host

## Probes

| Probe | Path | Port | Interval |
|-------|------|------|----------|
| Liveness | `/health` | 8080 | 20s |
| Readiness | `/health/ready` | 8080 | 10s |

## Ressources CPU/Mémoire

| Container | CPU Request | CPU Limit | Memory Request | Memory Limit |
|-----------|-------------|-----------|----------------|--------------|
| app | 100m | 500m | 128Mi | 256Mi |
| nginx | 100m | 250m | 64Mi | 128Mi |
| prometheus | 100m | 500m | 256Mi | 512Mi |
| grafana | 100m | 500m | 128Mi | 256Mi |

## Commandes utiles

```bash
# Voir toutes les ressources
kubectl get all -n caragence

# Décrire un pod
kubectl describe pod <pod-name> -n caragence

# Logs
kubectl logs <pod-name> -n caragence

# Port-forward
kubectl port-forward svc/prometheus 9090:9090 -n caragence
kubectl port-forward svc/grafana 3000:3000 -n caragence
```
