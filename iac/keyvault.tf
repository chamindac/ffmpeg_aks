resource "azurerm_key_vault" "instancekeyvault" {
  name                        = "${var.PREFIX}-${var.PROJECT}-${var.ENVNAME}-kv"
  location                    = azurerm_resource_group.instancerg.location
  resource_group_name         = azurerm_resource_group.instancerg.name
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  sku_name                    = "standard"
  enabled_for_deployment      = false
  enabled_for_disk_encryption = false
  purge_protection_enabled    = false # allow purge for drop and create in demos. else this should be set to true

  network_acls {
    bypass         = "AzureServices"
    default_action = "Deny"
    ip_rules       = ["${local.dev_ip}/32"]
    virtual_network_subnet_ids = [
      "${azurerm_subnet.aks.id}"
    ]
  }

  # Sub Owners
  access_policy {
    tenant_id          = var.TENANTID
    object_id          = data.azuread_group.sub_owners.object_id
    secret_permissions = ["Get", "List"]
  }

  # Infra Deployment Service Principal
  access_policy {
    tenant_id               = data.azurerm_client_config.current.tenant_id
    object_id               = data.azurerm_client_config.current.object_id
    key_permissions         = ["Get", "Purge", "Recover"]
    secret_permissions      = ["Get", "List", "Set", "Delete", "Purge", "Recover"]
    certificate_permissions = ["Create", "Get", "Import", "List", "Update", "Delete", "Purge", "Recover"]
  }

  # Containers in AKS via UAI
  access_policy {
    tenant_id          = var.TENANTID
    object_id          = azurerm_user_assigned_identity.aks.principal_id
    secret_permissions = ["Get", "List", ]
  }

  tags = merge(tomap({
    Service = "key_vault",
  }), local.tags)
}

# Secrets 
resource "azurerm_key_vault_secret" "secret" {
  for_each = {
    # AzureWebJobsStorage-blue  = var.SYS_BLUE_DEPLOY ? "${module.eventhubs_blue[0].storage_connection_string}" : "${local.dummy_secret}"
    # EventHubConsumer-1-blue   = var.SYS_BLUE_DEPLOY ? "${module.eventhubs_blue[0].consumer_1}" : "${local.dummy_secret}"
    # EventHubPublisher-1-blue  = var.SYS_BLUE_DEPLOY ? "${module.eventhubs_blue[0].publisher_1}" : "${local.dummy_secret}"
    # EventHubConsumer-2-blue   = var.SYS_BLUE_DEPLOY ? "${module.eventhubs_blue[0].consumer_2}" : "${local.dummy_secret}"
    # EventHubPublisher-2-blue  = var.SYS_BLUE_DEPLOY ? "${module.eventhubs_blue[0].publisher_2}" : "${local.dummy_secret}"
    # AzureWebJobsStorage-green = var.SYS_GREEN_DEPLOY ? "${module.eventhubs_green[0].storage_connection_string}" : "${local.dummy_secret}"
    # EventHubConsumer-1-green  = var.SYS_GREEN_DEPLOY ? "${module.eventhubs_green[0].consumer_1}" : "${local.dummy_secret}"
    # EventHubPublisher-1-green = var.SYS_GREEN_DEPLOY ? "${module.eventhubs_green[0].publisher_1}" : "${local.dummy_secret}"
    # EventHubConsumer-2-green  = var.SYS_GREEN_DEPLOY ? "${module.eventhubs_green[0].consumer_2}" : "${local.dummy_secret}"
    # EventHubPublisher-2-green = var.SYS_GREEN_DEPLOY ? "${module.eventhubs_green[0].publisher_2}" : "${local.dummy_secret}"
    DemoSecret = "Notarealsecret"
  }
  name         = each.key
  value        = each.value
  key_vault_id = azurerm_key_vault.instancekeyvault.id

  depends_on = [
    azurerm_key_vault.instancekeyvault
  ]
}