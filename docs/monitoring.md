# Monitoring - Prometheus & Grafana

## Services monitorés

| Service | Port | Métriques |
|---------|------|-----------|
| Prometheus | 9090 | Métriques système Prometheus |
| App CarAgence | 8080 | Métriques applicatives (/health) |
| Nginx | 9113 | Métriques nginx-exporter |
| Kubernetes | - | Métriques pods, nodes, ressources |

## Accès local

### Prometheus
```bash
minikube service prometheus -n caragence --url
# Interface : http://<minikube-ip>:<nodeport>
# Targets : http://<minikube-ip>:<nodeport>/targets
```

### Grafana
```bash
minikube service grafana -n caragence --url
# Login : admin / admin
# Dashboard : CarAgence > CarAgence Dashboard
```

## Configuration Prometheus

- **Scrape interval** : 15s (défaut), 10s (app)
- **Rétention** : 7 jours
- **Jobs** : prometheus, caragence-app, nginx, kubernetes-pods

### Scrape jobs

| Job | Target | Interval |
|-----|--------|----------|
| `prometheus` | localhost:9090 | 15s |
| `caragence-app` | caragence-app:8080/health | 10s |
| `nginx` | caragence-nginx:9113/metrics | 15s |
| `kubernetes-pods` | Pod annotations | 15s |

## Alertes

| Alerte | Sévérité | Condition |
|--------|----------|-----------|
| `PodNotReady` | warning | Deployment < 2 ready replicas pendant 5min |
| `HighMemoryUsage` | warning | Container > 200MB pendant 5min |
| `PodCrashLooping` | critical | Pod redémarré > 3 fois en 15min |
| `PVCNearFull` | warning | PVC > 85% utilisé pendant 5min |

## Dashboard Grafana

Le dashboard `CarAgence Dashboard` contient 8 panneaux :

| Panneau | Description | Métrique |
|---------|-------------|----------|
| App Pods Ready | Pods app prêts | `kube_deployment_status_replicas_ready` |
| Nginx Pods Ready | Pods nginx prêts | `kube_deployment_status_replicas_ready` |
| CPU Usage | Utilisation CPU (%) | `container_cpu_usage_seconds_total` |
| Memory Usage | Utilisation mémoire (MB) | `container_memory_usage_bytes` |
| HTTP Requests | Requêtes HTTP | `http_requests_total` |
| Pod Restarts | Redémarrages pods | `kube_pod_container_status_restarts_total` |
| Network Traffic | Trafic réseau | `container_network_receive_bytes_total` |
| SQLite Storage | Utilisation stockage | `node_filesystem_size_bytes` |

## Vérification post-déploiement

```bash
# Vérifier que Prometheus collecte les données
kubectl port-forward svc/prometheus 9090:9090 -n caragence
# Ouvrir http://localhost:9090/targets

# Vérifier que Grafana affiche les données
kubectl port-forward svc/grafana 3000:3000 -n caragence
# Ouvrir http://localhost:3000 (admin/admin)
# Aller dans Dashboards > CarAgence

# Vérifier les alertes
# Dans Prometheus : http://localhost:9090/alerts
```
