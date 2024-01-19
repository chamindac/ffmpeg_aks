.PHONY: install-keda delete-keda

install-keda:
	@kubectl apply --server-side --force-conflicts -f keda-2.13.0.yaml
	@kubectl apply -f k8s.yaml

delete-keda:
	@kubectl delete -f k8s.yaml
	@kubectl delete -f keda-2.13.0.yaml