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