helm repo add kedacore https://kedacore.github.io/charts
helm repo update

helm install keda kedacore/keda --namespace keda \
--set serviceAccount.create=false \
--set serviceAccount.name=keda-operator \
--set podIdentity.azureWorkload.enabled=true \
--set podIdentity.azureWorkload.clientId=${sys_aks_uai_client_id}$ \
--set podIdentity.azureWorkload.tenantId=${tenantid}$