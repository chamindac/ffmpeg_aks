resource "azurerm_storage_account" "queue" {
  name                             = "${var.PREFIX}${var.PROJECT}${replace(var.ENVNAME, "-", "")}queuest"
  resource_group_name              = azurerm_resource_group.instancerg.name
  location                         = azurerm_resource_group.instancerg.location
  account_tier                     = "Standard"
  account_replication_type         = "LRS"
  account_kind                     = "StorageV2"
  access_tier                      = "Hot"
  allow_nested_items_to_be_public  = false
  min_tls_version                  = "TLS1_2"
  cross_tenant_replication_enabled = false
}

resource "azurerm_storage_queue" "video" {
  name                 = "demovideoqueue"
  storage_account_name = azurerm_storage_account.queue.name
}

resource "azurerm_storage_queue" "dotnet_video" {
  name                 = "dotnetvideoqueue"
  storage_account_name = azurerm_storage_account.queue.name
}

resource "azurerm_role_assignment" "storage_q_contributor" {
  principal_id                     = azurerm_user_assigned_identity.aks.principal_id
  role_definition_name             = "Storage Queue Data Contributor"
  scope                            = azurerm_storage_account.queue.id
  skip_service_principal_aad_check = true
}

data "azurerm_storage_account" "cool" {
  name                = "cheuw001assetsstcool"
  resource_group_name = "ch-euw-001-assets-rg"
}

data "azurerm_storage_account" "hot" {
  name                = "cheuw001assetssthot"
  resource_group_name = "ch-euw-001-assets-rg"
}

resource "azurerm_role_assignment" "coolstorage_blob_contributor" {
  principal_id                     = azurerm_user_assigned_identity.aks.principal_id
  role_definition_name             = "Storage Blob Data Contributor"
  scope                            = data.azurerm_storage_account.cool.id
  skip_service_principal_aad_check = true
}

resource "azurerm_role_assignment" "hotstorage_blob_contributor" {
  principal_id                     = azurerm_user_assigned_identity.aks.principal_id
  role_definition_name             = "Storage Blob Data Contributor"
  scope                            = data.azurerm_storage_account.hot.id
  skip_service_principal_aad_check = true
}