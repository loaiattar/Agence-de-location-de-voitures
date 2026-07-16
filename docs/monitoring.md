# Monitoring

## Services monitorés

| Service | Port | Métriques |
|---------|------|-----------|
| Prometheus | 9090 | Métriques système Prometheus |
| App CarAgence | 8080 | Métriques applicatives (si exposées) |
| Nginx | 9113 | Métriques nginx-exporter |
| Kubernetes | - | Métriques pods, nodes, ressources |

## Dashboard Grafana

Le dashboard `CarAgence Dashboard` contient 8 panneaux :

| Panneau | Description |
|---------|-------------|
| App Pods Ready | Nombre de pods app prêts |
| Nginx Pods Ready | Nombre de pods nginx prêts |
| CPU Usage | Utilisation CPU totale (%) |
| Memory Usage | Utilisation mémoire totale (MB) |
| HTTP Requests | Requêtes HTTP par méthode et status |
| Pod Restarts | Nombre de redémarrages par pod |
| Network Traffic | Trafic réseau entrant/sortant |
| SQLite Storage | Utilisation du volume SQLite |

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

## Alertes (bonus)

Des alertes simples peuvent être ajoutées dans Prometheus :

```yaml
rule_files:
  - "alert_rules.yml"

# alert_rules.yml
groups:
  - name: caragence
    rules:
      - alert: PodNotReady
        expr: kube_deployment_status_replicas_ready{namespace="caragence"} < 2
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "Pod not ready in caragence"

      - alert: HighMemoryUsage
        expr: container_memory_usage_bytes{namespace="caragence"} / 1024 / 1024 > 200
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "High memory usage in caragence"
```

## Vérification post-déploiement

```bash
# Vérifier que Prometheus collecte les données
kubectl port-forward svc/prometheus 9090:9090 -n caragence
# Ouvrir http://localhost:9090/targets

# Vérifier que Grafana affiche les données
kubectl port-forward svc/grafana 3000:3000 -n caragence
# Ouvrir http://localhost:3000 (admin/admin)
# Aller dans Dashboards > CarAgence
```
