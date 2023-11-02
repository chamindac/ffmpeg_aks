# Locals
locals {
  kubernetes_version = "1.27.3" # we decide when to upgrade
}

variable "prefix" {
  description = "Organization prefix"
}
variable "project" {
  description = "Project name"
}

variable "environment" {
  description = "Shortname of environment"

  validation {
    condition     = contains(["dev", "qa", "prd", "ps", "poc", "core"], var.environment)
    error_message = "Valid values for var.environment are: (dev, qa, prd, ps, poc, core)."
  }
}

variable "environment_name" {
  description = "Name of environment"
}

variable "deployment_name" {
  description = "Deployment name blue or green"

  validation {
    condition     = contains(["blue", "green"], var.deployment_name)
    error_message = "Valid values for var.deployment_name are: (blue, green)."
  }
}

variable "location" {
  description = "Location for the cluster"
}

variable "rg_name" {
  description = "Name of the Resource Group"
}

variable "subnet_id" {
  description = "The id of the subnet where the cluster will be located"
}

variable "ingress_agw_private_ip" {
  description = "Ingress app gateway private IP"
}

variable "ingress_agw_subnet_id" {
  description = "The id of the subnet where the cluster ingress agw will be located"
}

variable "acr_id" {
  description = "ID of the ACR to pull the images from"
}

variable "network_plugin" {
  description = "Type of networking for AKS"
  default     = "kubenet"

  validation {
    condition     = contains(["none", "kubenet", "azure"], var.network_plugin)
    error_message = "Valid values for var.network_plugin are: (none, kubenet, azure)."
  }
}

variable "tenant_id" {
  description = "Tenant ID"
}

variable "log_analytics_workspace_id" {
  description = "Log analytics workspace id"
}

variable "tags" {
  description = "Map of tags to assign to the Cluster"
  type        = map(string)
}

variable "sub_owners_objectid" {
  description = "AD group sub_owners object id"
}