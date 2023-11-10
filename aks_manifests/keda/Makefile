.PHONY: install-keda delete-keda

install-keda:
	@kubectl apply --server-side -f keda-2.12.0.yaml
	    
delete-keda:
	@kubectl delete -f keda-2.12.0.yaml