data "azurerm_client_config" "current" {}

resource "azurerm_app_configuration" "appconf" {
  name                = "${var.PREFIX}-${var.PROJECT}-${var.ENVNAME}-appconfig-ac"
  resource_group_name = azurerm_resource_group.instancerg.name
  location            = azurerm_resource_group.instancerg.location
  sku                 = "standard"

  tags = merge(tomap({
    Service = "app_config"
  }), local.tags)
}

resource "azurerm_role_assignment" "appconf_dataowner" {
  scope                = azurerm_app_configuration.appconf.id
  role_definition_name = "App Configuration Data Owner"
  principal_id         = data.azurerm_client_config.current.object_id
}

# AKS user assigned identity as a reader
resource "azurerm_role_assignment" "appconf_datareader_aks" {
  scope                = azurerm_app_configuration.appconf.id
  role_definition_name = "App Configuration Data Reader"
  principal_id         = azurerm_user_assigned_identity.aks.principal_id
}

resource "azurerm_app_configuration_key" "config_vault" {
  for_each = {
    "DemoSecret" = azurerm_key_vault_secret.secret["DemoSecret"].versionless_id
  }
  configuration_store_id = azurerm_app_configuration.appconf.id
  key                    = each.key
  type                   = "vault" # keyvault reference
  label                  = azurerm_resource_group.instancerg.name
  vault_key_reference    = each.value
  depends_on = [
    azurerm_role_assignment.appconf_dataowner
  ]
}

resource "azurerm_app_configuration_key" "config_kv_blue" {
  for_each = {
    "EventHubStorageName"     = var.SYS_BLUE_DEPLOY ? module.eventhubs_blue[0].storage_name : local.dummy_value
    "EventHubNamespaceName-1" = var.SYS_BLUE_DEPLOY ? module.eventhubs_blue[0].eventhubnamespace_1_name : local.dummy_value
    "EventHubNamespaceName-2" = var.SYS_BLUE_DEPLOY ? module.eventhubs_blue[0].eventhubnamespace_2_name : local.dummy_value
  }
  configuration_store_id = azurerm_app_configuration.appconf.id
  key                    = each.key
  type                   = "kv" # key value
  label                  = "${azurerm_resource_group.instancerg.name}-${local.deployment_name_blue}"
  value                  = each.value
  depends_on = [
    azurerm_role_assignment.appconf_dataowner
  ]
}

resource "azurerm_app_configuration_key" "config_kv_green" {
  for_each = {
    "EventHubStorageName"     = var.SYS_GREEN_DEPLOY ? module.eventhubs_green[0].storage_name : local.dummy_value
    "EventHubNamespaceName-1" = var.SYS_GREEN_DEPLOY ? module.eventhubs_green[0].eventhubnamespace_1_name : local.dummy_value
    "EventHubNamespaceName-2" = var.SYS_GREEN_DEPLOY ? module.eventhubs_green[0].eventhubnamespace_2_name : local.dummy_value
  }
  configuration_store_id = azurerm_app_configuration.appconf.id
  key                    = each.key
  type                   = "kv" # key value
  label                  = "${azurerm_resource_group.instancerg.name}-${local.deployment_name_green}"
  value                  = each.value
  depends_on = [
    azurerm_role_assignment.appconf_dataowner
  ]
}

# resource "azurerm_app_configuration_key" "config_vault_blue" {
#   for_each = {
#     "AzureWebJobsStorage" = azurerm_key_vault_secret.secret["AzureWebJobsStorage-blue"].versionless_id
#     "EventHubConsumer1"   = azurerm_key_vault_secret.secret["EventHubConsumer-1-blue"].versionless_id
#     "EventHubConsumer2"   = azurerm_key_vault_secret.secret["EventHubConsumer-2-blue"].versionless_id
#     "EventHubPublisher1"  = azurerm_key_vault_secret.secret["EventHubPublisher-1-blue"].versionless_id
#     "EventHubPublisher2"  = azurerm_key_vault_secret.secret["EventHubPublisher-2-blue"].versionless_id
#   }
#   configuration_store_id = azurerm_app_configuration.appconf.id
#   key                    = each.key
#   type                   = "vault" # keyvault reference
#   label                  = "${azurerm_resource_group.instancerg.name}-${local.deployment_name_blue}"
#   vault_key_reference    = each.value
#   depends_on = [
#     azurerm_role_assignment.appconf_dataowner
#   ]
# }

# resource "azurerm_app_configuration_key" "config_vault_green" {
#   for_each = {
#     "AzureWebJobsStorage" = azurerm_key_vault_secret.secret["AzureWebJobsStorage-green"].versionless_id
#     "EventHubConsumer1"   = azurerm_key_vault_secret.secret["EventHubConsumer-1-green"].versionless_id
#     "EventHubConsumer2"   = azurerm_key_vault_secret.secret["EventHubConsumer-2-green"].versionless_id
#     "EventHubPublisher1"  = azurerm_key_vault_secret.secret["EventHubPublisher-1-green"].versionless_id
#     "EventHubPublisher2"  = azurerm_key_vault_secret.secret["EventHubPublisher-2-green"].versionless_id
#   }
#   configuration_store_id = azurerm_app_configuration.appconf.id
#   key                    = each.key
#   type                   = "vault" # keyvault reference
#   label                  = "${azurerm_resource_group.instancerg.name}-${local.deployment_name_green}"
#   vault_key_reference    = each.value
#   depends_on = [
#     azurerm_role_assignment.appconf_dataowner
#   ]
# }