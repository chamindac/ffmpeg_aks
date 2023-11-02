data "azurerm_container_registry" "shared_container_registry" {
  provider            = azurerm.mgmt
  name                = "${var.PREFIX}demosharedacr"
  resource_group_name = "${var.PREFIX}-demo-shared-rg"
}

module "aks_blue" {
  count = var.SYS_BLUE_DEPLOY ? 1 : 0

  # DO NOT change below module source and the comment --> source = "./modules/aks" # Blue
  # Used by pipeline setup_iac_for_blue_green.yml to apply below condition
  # (phase is deploy && SYS_GREEN_IS_LIVE is false && SYS_GREEN_DEPLOY is true) - blue is live now and deploying new green
  #   || (phase is switch) - live switch blue --> green or rollback green --> blue
  #   || (phase is destroy) - destroy blue or rollback green --> blue
  # then aks_ignoreall (no changes to live blue)
  # else aks (changes applied to blue)
  source = "./modules/aks" # Blue

  environment                = var.ENV
  environment_name           = var.ENVNAME
  deployment_name            = local.deployment_name_blue
  prefix                     = var.PREFIX
  project                    = var.PROJECT
  location                   = azurerm_resource_group.instancerg.location
  rg_name                    = azurerm_resource_group.instancerg.name
  subnet_id                  = azurerm_subnet.aks.id
  log_analytics_workspace_id = azurerm_log_analytics_workspace.aks_log.id
  sub_owners_objectid        = data.azuread_group.sub_owners.object_id
  acr_id                     = data.azurerm_container_registry.shared_container_registry.id
  ingress_agw_private_ip     = var.PRIVATE_IP_AKS_AGW
  ingress_agw_subnet_id      = azurerm_subnet.aks_agw.id

  tenant_id = var.TENANTID

  depends_on = [
    azurerm_subnet.aks_agw,
    azurerm_subnet.aks,
    azurerm_subnet_network_security_group_association.aks_nsg,
    # All below dependencies to make aks the last deployment
    azurerm_private_dns_a_record.aks_agw_blue,
  ]

  tags = local.tags
}

module "aks_green" {
  count = var.SYS_GREEN_DEPLOY ? 1 : 0

  # DO NOT change below module source and the comment --> source = "./modules/aks" # Green
  # Used by pipeline setup_iac_for_blue_green.yml to apply below condition
  # (phase is deploy && SYS_GREEN_IS_LIVE is true && SYS_BLUE_DEPLOY is true) - green is live now and deploying new blue
  #   || (phase is switch)  - live switch green --> blue or rollback blue --> green
  #   || (phase is destroy) - destroy green or rollback blue --> green
  # then aks_ignoreall (no changes to live green)
  # else aks (changes applied to green)
  source = "./modules/aks" # Green

  environment                = var.ENV
  environment_name           = var.ENVNAME
  deployment_name            = local.deployment_name_green
  prefix                     = var.PREFIX
  project                    = var.PROJECT
  location                   = azurerm_resource_group.instancerg.location
  rg_name                    = azurerm_resource_group.instancerg.name
  subnet_id                  = azurerm_subnet.aks.id
  log_analytics_workspace_id = azurerm_log_analytics_workspace.aks_log.id
  sub_owners_objectid        = data.azuread_group.sub_owners.object_id
  acr_id                     = data.azurerm_container_registry.shared_container_registry.id
  ingress_agw_private_ip     = var.PRIVATE_IP_AKS_AGW_GREEN
  ingress_agw_subnet_id      = azurerm_subnet.aks_agw.id

  tenant_id = var.TENANTID

  depends_on = [
    azurerm_subnet.aks_agw,
    azurerm_subnet.aks,
    azurerm_subnet_network_security_group_association.aks_nsg,
    # All below dependencies to make aks the last deployment
    azurerm_private_dns_a_record.aks_agw_green,
  ]

  tags = local.tags
}

# Log analytics workspace for AKS
resource "azurerm_log_analytics_workspace" "aks_log" {
  name                = "${var.PREFIX}-${var.PROJECT}-${var.ENVNAME}-log"
  location            = azurerm_resource_group.instancerg.location
  resource_group_name = azurerm_resource_group.instancerg.name
  retention_in_days   = 30

  tags = merge(tomap({
    Service = "aks_cluster_logs"
  }), local.tags)
}