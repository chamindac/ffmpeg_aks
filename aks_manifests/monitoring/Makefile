.PHONY: install-monitoring delete-monitoring

install-monitoring:
	@kubectl apply --server-side -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0alertmanagerConfigCustomResourceDefinition.yaml
	@kubectl apply --server-side -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0alertmanagerCustomResourceDefinition.yaml
	@kubectl apply --server-side -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0podmonitorCustomResourceDefinition.yaml
	@kubectl apply --server-side -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0probeCustomResourceDefinition.yaml
	@kubectl apply --server-side -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0prometheusCustomResourceDefinition.yaml
	@kubectl apply --server-side -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0prometheusagentCustomResourceDefinition.yaml
	@kubectl apply --server-side -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0prometheusruleCustomResourceDefinition.yaml
	@kubectl apply --server-side -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0scrapeconfigCustomResourceDefinition.yaml
	@kubectl apply --server-side -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0servicemonitorCustomResourceDefinition.yaml
	@kubectl apply --server-side -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0thanosrulerCustomResourceDefinition.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/namespace.yaml
	
	@kubectl wait --for condition=Established --all CustomResourceDefinition --namespace=monitoring
	
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-alertmanager.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-networkPolicy.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-podDisruptionBudget.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-prometheusRule.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-secret.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-service.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-serviceAccount.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-serviceMonitor.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-clusterRole.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-clusterRoleBinding.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-configuration.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-deployment.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-networkPolicy.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-service.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-serviceAccount.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-serviceMonitor.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-config.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-dashboardDatasources.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-dashboardDefinitions.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-dashboardSources.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-deployment.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-networkPolicy.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-prometheusRule.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-service.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-serviceAccount.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-serviceMonitor.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubePrometheus-prometheusRule.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-clusterRole.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-clusterRoleBinding.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-deployment.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-networkPolicy.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-prometheusRule.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-service.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-serviceAccount.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-serviceMonitor.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubernetesControlPlane-prometheusRule.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubernetesControlPlane-serviceMonitorApiserver.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubernetesControlPlane-serviceMonitorCoreDNS.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubernetesControlPlane-serviceMonitorKubeControllerManager.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubernetesControlPlane-serviceMonitorKubeScheduler.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubernetesControlPlane-serviceMonitorKubelet.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-clusterRole.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-clusterRoleBinding.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-daemonset.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-networkPolicy.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-prometheusRule.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-service.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-serviceAccount.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-serviceMonitor.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-clusterRole.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-clusterRoleBinding.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-networkPolicy.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-podDisruptionBudget.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-prometheus.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-prometheusRule.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-roleBindingConfig.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-roleBindingSpecificNamespaces.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-roleConfig.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-roleSpecificNamespaces.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-service.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-serviceAccount.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-serviceMonitor.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-apiService.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-clusterRole.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-clusterRoleAggregatedMetricsReader.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-clusterRoleBinding.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-clusterRoleBindingDelegator.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-clusterRoleServerResources.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-configMap.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-deployment.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-networkPolicy.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-podDisruptionBudget.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-roleBindingAuthReader.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-service.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-serviceAccount.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-serviceMonitor.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-clusterRole.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-clusterRoleBinding.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-deployment.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-networkPolicy.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-prometheusRule.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-service.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-serviceAccount.yaml
	@kubectl apply -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-serviceMonitor.yaml

	@kubectl apply -f ingress.yaml

delete-monitoring:

	@kubectl apply -f ingress.yaml
	
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-serviceMonitor.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-serviceAccount.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-service.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-prometheusRule.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-networkPolicy.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-deployment.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-clusterRoleBinding.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusOperator-clusterRole.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-serviceMonitor.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-serviceAccount.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-service.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-roleBindingAuthReader.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-podDisruptionBudget.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-networkPolicy.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-deployment.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-configMap.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-clusterRoleServerResources.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-clusterRoleBindingDelegator.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-clusterRoleBinding.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-clusterRoleAggregatedMetricsReader.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-clusterRole.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheusAdapter-apiService.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-serviceMonitor.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-serviceAccount.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-service.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-roleSpecificNamespaces.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-roleConfig.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-roleBindingSpecificNamespaces.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-roleBindingConfig.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-prometheusRule.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-prometheus.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-podDisruptionBudget.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-networkPolicy.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-clusterRoleBinding.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/prometheus-clusterRole.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-serviceMonitor.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-serviceAccount.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-service.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-prometheusRule.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-networkPolicy.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-daemonset.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-clusterRoleBinding.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/nodeExporter-clusterRole.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubernetesControlPlane-serviceMonitorKubelet.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubernetesControlPlane-serviceMonitorKubeScheduler.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubernetesControlPlane-serviceMonitorKubeControllerManager.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubernetesControlPlane-serviceMonitorCoreDNS.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubernetesControlPlane-serviceMonitorApiserver.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubernetesControlPlane-prometheusRule.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-serviceMonitor.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-serviceAccount.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-service.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-prometheusRule.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-networkPolicy.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-deployment.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-clusterRoleBinding.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubeStateMetrics-clusterRole.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/kubePrometheus-prometheusRule.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-serviceMonitor.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-serviceAccount.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-service.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-prometheusRule.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-networkPolicy.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-deployment.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-dashboardSources.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-dashboardDefinitions.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-dashboardDatasources.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/grafana-config.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-serviceMonitor.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-serviceAccount.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-service.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-networkPolicy.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-deployment.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-configuration.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-clusterRoleBinding.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/blackboxExporter-clusterRole.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-serviceMonitor.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-serviceAccount.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-service.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-secret.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-prometheusRule.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-podDisruptionBudget.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-networkPolicy.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/alertmanager-alertmanager.yaml


	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/namespace.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0thanosrulerCustomResourceDefinition.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0servicemonitorCustomResourceDefinition.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0scrapeconfigCustomResourceDefinition.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0prometheusruleCustomResourceDefinition.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0prometheusagentCustomResourceDefinition.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0prometheusCustomResourceDefinition.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0probeCustomResourceDefinition.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0podmonitorCustomResourceDefinition.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0alertmanagerCustomResourceDefinition.yaml
	@kubectl delete -f https://raw.githubusercontent.com/prometheus-operator/kube-prometheus/main/manifests/setup/0alertmanagerConfigCustomResourceDefinition.yaml