# Private dns zone for AKS
resource "azurerm_private_dns_zone" "aks" {
  name                = "${var.PREFIX}-${var.PROJECT}-${var.ENVNAME}.net"
  resource_group_name = azurerm_resource_group.instancerg.name
}

# Link private dns zone for AKS to env vnet
resource "azurerm_private_dns_zone_virtual_network_link" "aks" {
  name                  = "environment"
  private_dns_zone_name = azurerm_private_dns_zone.aks.name
  resource_group_name   = azurerm_private_dns_zone.aks.resource_group_name
  virtual_network_id    = azurerm_virtual_network.env_vnet.id
  registration_enabled  = false
}

# Private dns a record for AKS AGW Private IP - blue
resource "azurerm_private_dns_a_record" "aks_agw_blue" {
  name                = "*.${local.aks_dns_prefix_blue}"
  zone_name           = azurerm_private_dns_zone.aks.name
  resource_group_name = azurerm_private_dns_zone.aks.resource_group_name
  ttl                 = 3600
  records             = [var.PRIVATE_IP_AKS_AGW]
}

# Private dns a record for AKS AGW Private IP - green
resource "azurerm_private_dns_a_record" "aks_agw_green" {
  name                = "*.${local.aks_dns_prefix_green}"
  zone_name           = azurerm_private_dns_zone.aks.name
  resource_group_name = azurerm_private_dns_zone.aks.resource_group_name
  ttl                 = 3600
  records             = [var.PRIVATE_IP_AKS_AGW_GREEN]
}