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