output "namespace" {
  description = "Kubernetes namespace"
  value       = kubernetes_namespace.app.metadata[0].name
}

output "pv_name" {
  description = "Persistent Volume name"
  value       = kubernetes_persistent_volume.sqlite.metadata[0].name
}

output "pvc_name" {
  description = "Persistent Volume Claim name"
  value       = kubernetes_persistent_volume_claim.sqlite.metadata[0].name
}

output "storage_path" {
  description = "Host path for SQLite storage"
  value       = "/data/${var.app_name}-sqlite"
}
