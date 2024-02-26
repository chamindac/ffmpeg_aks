resource "azurerm_storage_account" "instancestorageehn" {
  name                             = "${var.prefix}${var.project}${replace(var.environment_name, "-", "")}ehn${var.deployment_name}"
  location                         = var.location
  resource_group_name              = var.rg_name
  account_tier                     = "Standard"
  account_replication_type         = "LRS"
  account_kind                     = "StorageV2"
  access_tier                      = "Hot"
  allow_nested_items_to_be_public  = false
  min_tls_version                  = "TLS1_0"
  cross_tenant_replication_enabled = false

  network_rules {
    default_action = "Deny"
    bypass         = ["Metrics", "AzureServices", "Logging"]
    virtual_network_subnet_ids = [
      "${var.aks_subnet_id}"
    ]
    ip_rules = ["${var.devip}"]
  }

  lifecycle {
    ignore_changes = []
  }

  tags = merge(tomap({
    Service = "storage_account"
  }), var.tags)
}

resource "azurerm_eventhub_namespace" "instanceeventhub" {
  count                    = 2
  name                     = "${var.prefix}-${var.project}-${var.environment_name}-${count.index + 1}-${var.deployment_name}"
  location                 = var.location
  resource_group_name      = var.rg_name
  sku                      = "Standard"
  capacity                 = 1
  auto_inflate_enabled     = true
  maximum_throughput_units = 20

  network_rulesets {
    default_action                 = "Deny"
    trusted_service_access_enabled = true
    ip_rule = [
      {
        ip_mask = "${var.devip}/32"
        action  = "Allow"
      }
    ]

    virtual_network_rule {
      subnet_id = var.aks_subnet_id
    }
  }

  lifecycle {
    ignore_changes = []
  }

  tags = merge(tomap({
    Service = "eventhub_namespace"
  }), var.tags)
}


resource "azurerm_eventhub_namespace_authorization_rule" "consumer" {
  for_each = {
    eventhub1consumer = azurerm_eventhub_namespace.instanceeventhub[0].name
    eventhub2consumer = azurerm_eventhub_namespace.instanceeventhub[1].name
  }
  name                = "consumer"
  namespace_name      = each.value
  resource_group_name = var.rg_name

  listen = true
  send   = true
  manage = true

  lifecycle {
    ignore_changes = []
  }
}

resource "azurerm_eventhub_namespace_authorization_rule" "publisher" {
  for_each = {
    eventhub1publisher = azurerm_eventhub_namespace.instanceeventhub[0].name
    eventhub2publisher = azurerm_eventhub_namespace.instanceeventhub[1].name
  }
  name                = "publisher"
  namespace_name      = each.value
  resource_group_name = var.rg_name

  listen = true
  send   = true
  manage = true

  lifecycle {
    ignore_changes = []
  }
}

resource "azurerm_eventhub" "eventhub" {
  for_each = {
    videopreview    = azurerm_eventhub_namespace.instanceeventhub[0].name
    neworder        = azurerm_eventhub_namespace.instanceeventhub[0].name
    paidinvoice     = azurerm_eventhub_namespace.instanceeventhub[1].name
    newinvoice      = azurerm_eventhub_namespace.instanceeventhub[1].name
  }
  name                = each.key
  namespace_name      = each.value
  resource_group_name = var.rg_name
  partition_count     = 10
  message_retention   = 1

  lifecycle {
    ignore_changes = []
  }
}

resource "azurerm_eventhub_consumer_group" "consumer_group_0_0" {
  for_each = {
    "ch.demo.videopreview.eventhandler" = azurerm_eventhub.eventhub["videopreview"].name
    "ch.demo.customer.eventhandler"     = azurerm_eventhub.eventhub["neworder"].name
  }
  name                = each.key
  namespace_name      = azurerm_eventhub_namespace.instanceeventhub[0].name
  eventhub_name       = each.value
  resource_group_name = var.rg_name

  lifecycle {
    ignore_changes = []
  }
}


resource "azurerm_eventhub_consumer_group" "consumer_group_1_0" {
  for_each = {
    "ch.demo.invoice.eventhandler"  = azurerm_eventhub.eventhub["paidinvoice"].name
    "ch.demo.customer.eventhandler" = azurerm_eventhub.eventhub["newinvoice"].name
  }
  name                = each.key
  namespace_name      = azurerm_eventhub_namespace.instanceeventhub[1].name
  eventhub_name       = each.value
  resource_group_name = var.rg_name

  lifecycle {
    ignore_changes = []
  }
}