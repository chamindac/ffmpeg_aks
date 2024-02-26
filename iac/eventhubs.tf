module "eventhubs_blue" {
  count = var.SYS_BLUE_DEPLOY ? 1 : 0

  # DO NOT change below module source and the comment --> source = "./modules/eventhubs" # Blue
  # Used by pipeline setup_iac_for_blue_green.yml to apply below condition
  # (phase is deploy && SYS_GREEN_IS_LIVE is false && SYS_GREEN_DEPLOY is true) - blue is live now and deploying new green
  #   || (phase is switch) - live switch blue --> green or rollback green --> blue
  #   || (phase is destroy) - destroy blue or rollback green --> blue
  # then eventhubs_ignoreall (no changes to live blue)
  # else eventhubs (changes applied to blue)
  source = "./modules/eventhubs" # Blue

  prefix           = var.PREFIX
  project          = var.PROJECT
  devip            = local.dev_ip
  environment      = var.ENV
  environment_name = var.ENVNAME
  deployment_name  = local.deployment_name_blue
  location         = azurerm_resource_group.instancerg.location
  rg_name          = azurerm_resource_group.instancerg.name
  aks_subnet_id    = azurerm_subnet.aks.id
  tags             = local.tags
}

module "eventhubs_green" {
  count = var.SYS_GREEN_DEPLOY ? 1 : 0

  # DO NOT change below module source and the comment --> source = "./modules/eventhubs" # Green
  # Used by pipeline setup_iac_for_blue_green.yml to apply below condition
  # (phase is deploy && SYS_GREEN_IS_LIVE is true && SYS_BLUE_DEPLOY is true) - green is live now and deploying new blue
  #   || (phase is switch)  - live switch green --> blue or rollback blue --> green
  #   || (phase is destroy) - destroy green or rollback blue --> green
  # then eventhubs_ignoreall (no changes to live green)
  # else eventhubs (changes applied to green)
  source = "./modules/eventhubs" # Green

  prefix           = var.PREFIX
  project          = var.PROJECT
  devip            = local.dev_ip
  environment      = var.ENV
  environment_name = var.ENVNAME
  deployment_name  = local.deployment_name_green
  location         = azurerm_resource_group.instancerg.location
  rg_name          = azurerm_resource_group.instancerg.name
  aks_subnet_id    = azurerm_subnet.aks.id
  tags             = local.tags
}