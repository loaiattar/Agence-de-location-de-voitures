variable "namespace" {
  description = "Kubernetes namespace for the application"
  type        = string
  default     = "caragence"
}

variable "app_name" {
  description = "Application name used for resource labeling"
  type        = string
  default     = "caragence"
}

variable "storage_size" {
  description = "Size of the persistent volume for SQLite"
  type        = string
  default     = "1Gi"
}

variable "storage_class" {
  description = "Storage class for the persistent volume"
  type        = string
  default     = "standard"
}

variable "kubeconfig_path" {
  description = "Path to kubeconfig file"
  type        = string
  default     = "~/.kube/config"
}

variable "kubeconfig_context" {
  description = "Kubeconfig context to use"
  type        = string
  default     = "minikube"
}
