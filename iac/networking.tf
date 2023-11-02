# vnet
resource "azurerm_virtual_network" "env_vnet" {
  name                = "${var.PREFIX}-${var.PROJECT}-${var.ENVNAME}-vnet"
  resource_group_name = azurerm_resource_group.instancerg.name
  location            = azurerm_resource_group.instancerg.location
  address_space       = [var.VNET_CIDR]
}

resource "azurerm_network_security_group" "nsg" {
  name                = "${var.PREFIX}-${var.PROJECT}-${var.ENVNAME}-nsg"
  location            = azurerm_resource_group.instancerg.location
  resource_group_name = azurerm_resource_group.instancerg.name

  tags = merge(tomap({
    Service = "network_security_group"
  }), local.tags)
}

## AKS Ingress AppGateway Subnet
resource "azurerm_subnet" "aks_agw" {
  name                 = "${var.PREFIX}-${var.PROJECT}-${var.ENVNAME}-aks-agw-snet"
  resource_group_name  = azurerm_virtual_network.env_vnet.resource_group_name
  virtual_network_name = azurerm_virtual_network.env_vnet.name
  address_prefixes     = ["${var.SUBNET_CIDR_AKS_AGW}"]
}

# AKS Subnet
resource "azurerm_subnet" "aks" {
  name                 = "${var.PREFIX}-${var.PROJECT}-${var.ENVNAME}-aks-snet"
  resource_group_name  = azurerm_virtual_network.env_vnet.resource_group_name
  virtual_network_name = azurerm_virtual_network.env_vnet.name
  address_prefixes     = ["${var.SUBNET_CIDR_AKS}"]
  service_endpoints = [
    "Microsoft.AzureActiveDirectory",
    "Microsoft.AzureCosmosDB",
    "Microsoft.EventHub",
    "Microsoft.KeyVault",
    "Microsoft.Storage",
    "Microsoft.Sql",
    "Microsoft.ServiceBus",
    "Microsoft.Web"
  ]
}

# Associate AKS subnet with network security group
resource "azurerm_subnet_network_security_group_association" "aks_nsg" {
  subnet_id                 = azurerm_subnet.aks.id
  network_security_group_id = azurerm_network_security_group.nsg.id
}