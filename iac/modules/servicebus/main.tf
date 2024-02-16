resource "azurerm_servicebus_namespace" "demo" {
  name                = "${var.prefix}-${var.project}-${var.environment_name}-sbus-${var.deployment_name}"
  location            = var.location
  resource_group_name = var.rg_name
  sku                 = "Standard"

  tags = merge(tomap({
    Service = "service_bus"
  }), var.tags)

  lifecycle {
    ignore_changes = []
  }
}

resource "azurerm_servicebus_queue" "dotnetvideo" {
  name                         = "dotnetvideoqueue"
  namespace_id                 = azurerm_servicebus_namespace.demo.id
  lock_duration                = "PT5M" #https://stackoverflow.com/questions/77315762/terraform-stuck-at-still-creating-step-and-timeouts-while-creating-azure-service
  requires_duplicate_detection = true
  enable_partitioning          = true

  lifecycle {
    ignore_changes = []
  }
}

resource "azurerm_role_assignment" "sbq_contributor" {
  principal_id                     = var.aks_user_assigned_identity_principal_id
  role_definition_name             = "Azure Service Bus Data Owner"
  scope                            = azurerm_servicebus_namespace.demo.id
  skip_service_principal_aad_check = true

  lifecycle {
    ignore_changes = []
  }
}

# Assign permission to manage queue data for subscription owners group
resource "azurerm_role_assignment" "sbq_contributor_subowners" {
  principal_id         = var.sub_owners_objectid
  role_definition_name = "Azure Service Bus Data Owner"
  scope                = azurerm_servicebus_namespace.demo.id

  lifecycle {
    ignore_changes = []
  }
}