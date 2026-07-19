# Helm (Bonus)

> Ce document décrit la structure Helm si le bonus est réalisé.

## Structure du chart

```
helm/
└── caragence/
    ├── Chart.yaml
    ├── values.yaml
    └── templates/
        ├── deployment-app.yaml
        ├── deployment-nginx.yaml
        ├── service-app.yaml
        ├── service-nginx.yaml
        ├── configmap-nginx.yaml
        ├── secret.yaml
        ├── pvc-sqlite.yaml
        ├── namespace.yaml
        ├── deployment-prometheus.yaml
        ├── service-prometheus.yaml
        ├── configmap-prometheus.yaml
        ├── deployment-grafana.yaml
        ├── service-grafana.yaml
        ├── configmap-grafana-datasource.yaml
        └── configmap-grafana-dashboard.yaml
```

## Valeurs configurables

```yaml
# values.yaml
app:
  name: caragence
  image: ghcr.io/loaiattar/agence-de-location-de-voitures
  tag: latest
  replicas: 2

nginx:
  replicas: 2
  nodePort: 30080

sqlite:
  storageSize: 1Gi
  storageClass: standard

monitoring:
  enabled: true
  prometheus:
    nodePort: 30090
  grafana:
    nodePort: 30030
    adminPassword: admin
```

## Utilisation

```bash
# Lint
helm lint helm/caragence

# Template (dry run)
helm template caragence helm/caragence

# Installer
helm install caragence helm/caragence -n caragence --create-namespace

# Mettre à jour
helm upgrade caragence helm/caragence -n caragence

# Statut
helm status caragence -n caragence

# Historique
helm history caragence -n caragence

# Rollback
helm rollback caragence <revision> -n caragence
```

## Intégration Ansible

Ansible peut lancer Helm au lieu de `kubectl apply` :

```yaml
- name: Deploy with Helm
  kubernetes.core.helm:
    name: caragence
    chart_ref: ../helm/caragence
    namespace: caragence
    create_namespace: true
    values:
      app:
        tag: "{{ app_tag }}"
      monitoring:
        enabled: true
```
