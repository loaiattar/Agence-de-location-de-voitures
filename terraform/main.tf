resource "kubernetes_namespace" "app" {
  metadata {
    name = var.namespace

    labels = {
      app     = var.app_name
      managed = "terraform"
    }
  }
}

resource "kubernetes_persistent_volume" "sqlite" {
  metadata {
    name = "${var.app_name}-sqlite-pv"

    labels = {
      app     = var.app_name
      managed = "terraform"
    }
  }

  spec {
    capacity = {
      storage = var.storage_size
    }

    access_modes = ["ReadWriteOnce"]

    persistent_volume_reclaim_policy = "Retain"

    storage_class_name = var.storage_class

    persistent_volume_source {
      host_path {
        path = "/data/${var.app_name}-sqlite"
        type = "DirectoryOrCreate"
      }
    }
  }
}

resource "kubernetes_persistent_volume_claim" "sqlite" {
  metadata {
    name      = "${var.app_name}-sqlite-pvc"
    namespace = kubernetes_namespace.app.metadata[0].name

    labels = {
      app     = var.app_name
      managed = "terraform"
    }
  }

  spec {
    access_modes = ["ReadWriteOnce"]

    resources {
      requests = {
        storage = var.storage_size
      }
    }

    volume_name = kubernetes_persistent_volume.sqlite.metadata[0].name
  }
}
