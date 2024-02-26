output "live_aks_name" {
  value = var.SYS_GREEN_IS_LIVE ? (
    var.SYS_GREEN_DEPLOY ? module.aks_green[0].aks_cluster_name : "No live cluster"
    ) : (
    module.aks_blue[0].aks_cluster_name
  )
}

output "app_deploy_aks_name" {
  value = var.SYS_GREEN_IS_LIVE ? (
    var.SYS_DEPLOYMENT_PHASE == "deploy" ? (
      var.SYS_BLUE_DEPLOY ? module.aks_blue[0].aks_cluster_name : module.aks_green[0].aks_cluster_name
      ) : (
      module.aks_green[0].aks_cluster_name
    )
    ) : (
    var.SYS_DEPLOYMENT_PHASE == "deploy" ? (
      var.SYS_GREEN_DEPLOY ? module.aks_green[0].aks_cluster_name : module.aks_blue[0].aks_cluster_name
      ) : (
      module.aks_blue[0].aks_cluster_name
    )
  )
}

output "eventhub_storage_name" {
  value = var.SYS_GREEN_IS_LIVE ? (
    var.SYS_DEPLOYMENT_PHASE == "deploy" ? (
      var.SYS_BLUE_DEPLOY ? module.eventhubs_blue[0].storage_name : module.eventhubs_green[0].storage_name
      ) : (
      module.eventhubs_green[0].storage_name
    )
    ) : (
    var.SYS_DEPLOYMENT_PHASE == "deploy" ? (
      var.SYS_GREEN_DEPLOY ? module.eventhubs_green[0].storage_name : module.eventhubs_blue[0].storage_name
      ) : (
      module.eventhubs_blue[0].storage_name
    )
  )
}

output "app_deploy_dns_zone" {
  value = trimsuffix(replace((var.SYS_GREEN_IS_LIVE ? (
    var.SYS_DEPLOYMENT_PHASE == "deploy" ? (
      var.SYS_BLUE_DEPLOY ? azurerm_private_dns_a_record.aks_agw_blue.fqdn : azurerm_private_dns_a_record.aks_agw_green.fqdn
      ) : (
      azurerm_private_dns_a_record.aks_agw_green.fqdn
    )
    ) : (
    var.SYS_DEPLOYMENT_PHASE == "deploy" ? (
      var.SYS_GREEN_DEPLOY ? azurerm_private_dns_a_record.aks_agw_green.fqdn : azurerm_private_dns_a_record.aks_agw_blue.fqdn
      ) : (
      azurerm_private_dns_a_record.aks_agw_blue.fqdn
    )
  )), "*", ""), ".")
}

# Client Id of the workload (user assigned) identity - to be used in service account creation after AKS deployed
output "aks_uai_client_id" {
  value = azurerm_user_assigned_identity.aks.client_id
}