module "servicebus_blue" {
  count = var.SYS_BLUE_DEPLOY ? 1 : 0

  # DO NOT change below module source and the comment --> source = "./modules/servicebus" # Blue
  # Used by pipeline setup_iac_for_blue_green.yml to apply below condition
  # (phase is deploy && SYS_GREEN_IS_LIVE is false && SYS_GREEN_DEPLOY is true) - blue is live now and deploying new green
  #   || (phase is switch) - live switch blue --> green or rollback green --> blue
  #   || (phase is destroy) - destroy blue or rollback green --> blue
  # then servicebus_ignoreall (no changes to live blue)
  # else servicebus (changes applied to blue)
  source = "./modules/servicebus" # Blue

  environment      = var.ENV
  environment_name = var.ENVNAME
  deployment_name  = local.deployment_name_blue
  prefix           = var.PREFIX
  project          = var.PROJECT
  location         = azurerm_resource_group.instancerg.location
  rg_name          = azurerm_resource_group.instancerg.name

  sub_owners_objectid                     = data.azuread_group.sub_owners.object_id
  aks_user_assigned_identity_principal_id = azurerm_user_assigned_identity.aks.principal_id

  depends_on = [
    azurerm_user_assigned_identity.aks
  ]

  tags = local.tags
}

module "servicebus_green" {
  count = var.SYS_GREEN_DEPLOY ? 1 : 0

  # DO NOT change below module source and the comment --> source = "./modules/servicebus" # Green
  # Used by pipeline setup_iac_for_blue_green.yml to apply below condition
  # (phase is deploy && SYS_GREEN_IS_LIVE is true && SYS_BLUE_DEPLOY is true) - green is live now and deploying new blue
  #   || (phase is switch)  - live switch green --> blue or rollback blue --> green
  #   || (phase is destroy) - destroy green or rollback blue --> green
  # then servicebus_ignoreall (no changes to live green)
  # else servicebus (changes applied to green)
  source = "./modules/servicebus" # Green

  environment      = var.ENV
  environment_name = var.ENVNAME
  deployment_name  = local.deployment_name_green
  prefix           = var.PREFIX
  project          = var.PROJECT
  location         = azurerm_resource_group.instancerg.location
  rg_name          = azurerm_resource_group.instancerg.name

  sub_owners_objectid                     = data.azuread_group.sub_owners.object_id
  aks_user_assigned_identity_principal_id = azurerm_user_assigned_identity.aks.principal_id

  depends_on = [
    azurerm_user_assigned_identity.aks
  ]

  tags = local.tags
}