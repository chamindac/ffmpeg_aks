# we dont use it but AKS App gateway need to have a public IP
resource "azurerm_public_ip" "aks_agw" {
  name                = "${var.prefix}-${var.project}-${var.environment_name}-aks-agw-pip-${var.deployment_name}"
  location            = var.location
  resource_group_name = var.rg_name
  allocation_method   = "Static"
  sku                 = "Standard"

  lifecycle {
    ignore_changes = []
  }
}

resource "azurerm_application_gateway" "aks" {
  name                = "${var.prefix}-${var.project}-${var.environment_name}-aks-agw-${var.deployment_name}"
  location            = var.location
  resource_group_name = var.rg_name

  sku {
    name = "Standard_v2"
    tier = "Standard_v2"
  }

  autoscale_configuration {
    min_capacity = 0
    max_capacity = 10
  }

  gateway_ip_configuration {
    name      = "appGatewayIpConfig"
    subnet_id = var.ingress_agw_subnet_id

  }

  frontend_port {
    name = "port_80"
    port = 80
  }

  # need to have a public IP for Standard_v2 AGW. will not be used with any listerners by AKS
  frontend_ip_configuration {
    name                 = "DemoAKSPublicFrontendIp"
    public_ip_address_id = azurerm_public_ip.aks_agw.id
  }

  frontend_ip_configuration {
    name                          = "DemoAKSPrivateFrontendIp"
    private_ip_address            = var.ingress_agw_private_ip
    private_ip_address_allocation = "Static"
    subnet_id                     = var.ingress_agw_subnet_id
  }

  backend_address_pool {
    name = "dummyBackend"
  }

  backend_http_settings {
    name                  = "dummyBackendSettings"
    cookie_based_affinity = "Disabled"
    path                  = "/path1/"
    port                  = 80
    protocol              = "Http"
    request_timeout       = 60
  }

  http_listener {
    name                           = "dummyListener"
    frontend_ip_configuration_name = "DemoAKSPrivateFrontendIp"
    frontend_port_name             = "port_80"
    protocol                       = "Http"
  }

  request_routing_rule {
    name                       = "dummyRule"
    rule_type                  = "Basic"
    http_listener_name         = "dummyListener"
    backend_address_pool_name  = "dummyBackend"
    backend_http_settings_name = "dummyBackendSettings"
    priority                   = 100
  }

  tags = merge(tomap({
    Service = "aks_agw"
  }), var.tags)

  lifecycle {
    ignore_changes = [
      backend_address_pool,
      backend_http_settings,
      http_listener,
      probe,
      request_routing_rule,
      url_path_map,
      rewrite_rule_set,
      frontend_port,
      tags
    ]
  }
}

resource "azurerm_kubernetes_cluster" "aks_cluster" {

  lifecycle {
    ignore_changes = [default_node_pool[0].node_count]
  }

  name                         = "${var.prefix}-${var.project}-${var.environment_name}-aks-${var.deployment_name}"
  kubernetes_version           = local.kubernetes_version
  sku_tier                     = "Standard"
  location                     = var.location
  resource_group_name          = var.rg_name
  dns_prefix                   = "${var.prefix}-${var.project}-${var.environment_name}-aks-${var.deployment_name}-dns"
  node_resource_group          = "${var.prefix}-${var.project}-${var.environment_name}-aks-${var.deployment_name}-rg"
  image_cleaner_enabled        = false # As this is a preview feature keep it disabled for now. Once feture is GA, it should be enabled.
  image_cleaner_interval_hours = 48

  network_profile {
    network_plugin    = "azure"
    load_balancer_sku = "standard"
  }

  storage_profile {
    file_driver_enabled = true
  }

  default_node_pool {
    name                 = "chlinux"
    orchestrator_version = local.kubernetes_version
    node_count           = 1
    enable_auto_scaling  = true
    min_count            = 1
    max_count            = 7
    vm_size              = "Standard_DS4_v2"
    os_sku               = "Ubuntu"
    vnet_subnet_id       = var.subnet_id
    max_pods             = 30
    type                 = "VirtualMachineScaleSets"
    scale_down_mode      = "Delete"
    zones                = ["1", "2", "3"]
  }

  identity {
    type = "SystemAssigned"
  }

  ingress_application_gateway {
    gateway_id = azurerm_application_gateway.aks.id
  }

  key_vault_secrets_provider {
    secret_rotation_enabled = false
  }

  azure_active_directory_role_based_access_control {
    azure_rbac_enabled = false
    managed            = true
    tenant_id          = var.tenant_id

    # add sub owners as cluster admin 
    admin_group_object_ids = [
    var.sub_owners_objectid] # azure AD group object ID
  }

  oms_agent {
    log_analytics_workspace_id = var.log_analytics_workspace_id
  }

  depends_on = [
    azurerm_application_gateway.aks
  ]

  tags = merge(tomap({
    Service = "aks_cluster"
  }), var.tags)
}

resource "azurerm_role_assignment" "acr_attach" {
  principal_id                     = azurerm_kubernetes_cluster.aks_cluster.kubelet_identity[0].object_id
  role_definition_name             = "AcrPull"
  scope                            = var.acr_id
  skip_service_principal_aad_check = true

  lifecycle {
    ignore_changes = []
  }
}

#---------------------
# Ingess agw for aks is not getting the managed identity assigned automatically when attached with TF
# need to get the user assigned managed id from node rg when it is available after cluster creation

# get node pool rg
data "azurerm_resource_group" "aks_node_rg" {
  name = azurerm_kubernetes_cluster.aks_cluster.node_resource_group

  depends_on = [
    azurerm_application_gateway.aks,
    azurerm_kubernetes_cluster.aks_cluster
  ]
}

# get user assigned manged id
data "azurerm_user_assigned_identity" "aks_agw" {
  resource_group_name = data.azurerm_resource_group.aks_node_rg.name
  name                = "ingressapplicationgateway-${azurerm_kubernetes_cluster.aks_cluster.name}"

  depends_on = [
    azurerm_application_gateway.aks,
    azurerm_kubernetes_cluster.aks_cluster,
    data.azurerm_resource_group.aks_node_rg
  ]
}

# assign user assigned ingress managed id of aks to ingress agw - required to allow AGIC to manage agw
resource "azurerm_role_assignment" "aks_agw" {
  principal_id         = data.azurerm_user_assigned_identity.aks_agw.principal_id
  role_definition_name = "Contributor"
  scope                = azurerm_application_gateway.aks.id

  depends_on = [
    azurerm_application_gateway.aks,
    azurerm_kubernetes_cluster.aks_cluster,
    data.azurerm_resource_group.aks_node_rg,
    data.azurerm_user_assigned_identity.aks_agw
  ]

  lifecycle {
    ignore_changes = []
  }
}

# assign user assigned ingress managed id of aks to agw subnet - required to allow AGIC to manage agw effectively
resource "azurerm_role_assignment" "aks_agw_snet" {
  principal_id         = data.azurerm_user_assigned_identity.aks_agw.principal_id
  role_definition_name = "Network Contributor"
  scope                = var.ingress_agw_subnet_id

  depends_on = [
    azurerm_kubernetes_cluster.aks_cluster,
    data.azurerm_resource_group.aks_node_rg,
    data.azurerm_user_assigned_identity.aks_agw
  ]

  lifecycle {
    ignore_changes = []
  }
}