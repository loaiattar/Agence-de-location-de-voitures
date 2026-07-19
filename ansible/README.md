# Ansible - Orchestration locale

Playbook Ansible pour orchestrer le déploiement de l'application sur minikube.

## Préreis

```bash
pip install ansible
ansible-galaxy collection install community.general kubernetes.core
```

## Utilisation

```bash
# Vérifier la syntaxe
ansible-playbook ansible/playbook.yml --syntax-check

# Simuler (dry run)
ansible-playbook ansible/playbook.yml --check

# Exécuter
ansible-playbook ansible/playbook.yml
```

## Étapes orchestrées

1. Vérification des prérequis (minikube, kubectl, terraform)
2. Initialisation et application Terraform
3. Déploiement des ressources Kubernetes
4. Vérification de la santé de l'application
5. Affichage de l'URL d'accès
