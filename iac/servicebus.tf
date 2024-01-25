resource "azurerm_servicebus_namespace" "demo" {
  name                = "${var.PREFIX}-${var.PROJECT}-${var.ENVNAME}-sbus"
  location            = azurerm_resource_group.instancerg.location
  resource_group_name = azurerm_resource_group.instancerg.name
  sku                 = "Standard"

  tags = merge(tomap({
    Service = "service_bus"
  }), local.tags)
}

resource "azurerm_servicebus_queue" "dotnetvideo" {
  name                         = "dotnetvideoqueue"
  namespace_id                 = azurerm_servicebus_namespace.demo.id
  lock_duration                = "PT5M" #https://stackoverflow.com/questions/77315762/terraform-stuck-at-still-creating-step-and-timeouts-while-creating-azure-service
  requires_duplicate_detection = true
  enable_partitioning          = true
}

resource "azurerm_role_assignment" "sbq_contributor" {
  principal_id                     = azurerm_user_assigned_identity.aks.principal_id
  role_definition_name             = "Azure Service Bus Data Owner"
  scope                            = azurerm_servicebus_queue.dotnetvideo.id
  skip_service_principal_aad_check = true
}

# Assign permission to manage queue data for subscription owners group
resource "azurerm_role_assignment" "sbq_contributor_subowners" {
  principal_id                     = data.azuread_group.sub_owners.object_id
  role_definition_name             = "Azure Service Bus Data Owner"
  scope                            = azurerm_servicebus_queue.dotnetvideo.id
}