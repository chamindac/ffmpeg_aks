# Locals
locals {
  dummy_secret          = "NotSecret"
  deployment_name_blue  = "blue"
  deployment_name_green = "green"
  aks_dns_prefix_blue   = "aksblue"
  aks_dns_prefix_green  = "aksgreen"
  aks_dns_prefix_live   = var.SYS_GREEN_IS_LIVE ? local.aks_dns_prefix_green : local.aks_dns_prefix_blue

  tags = {
    Environment          = var.ENV
    Owner                = "Demo Team"
    System               = "Demo"
    SystemClassification = "Internal"
    CreatedBy            = "Azure DevOps - Demo Pipelines"
    EnvName              = var.ENVNAME
    ProvisionedWith      = "Terraform"
  }
}

variable "ENV" {
  description = "env name"
  type        = string
}

variable "ENVNAME" {
  description = "full env name"
  type        = string
}

variable "MANAGEMENTSUBSCRIPTIONID" {
  description = "azure subscription id for management"
  type        = string
}

variable "PREFIX" {
  description = "project prefix"
  type        = string
}

variable "PRIVATE_IP_AKS_AGW" {
  description = "aks ingress app gateway private IP"
  type        = string
}

variable "PRIVATE_IP_AKS_AGW_GREEN" {
  description = "aks ingress app gateway private IP"
  type        = string
}

variable "PROJECT" {
  description = "Project name or shortcode"
  type        = string
}

variable "REGION" {
  description = "What region the instance is running in."
  type        = string
}
variable "REGIONSHORTCODE" {
  description = "What shortcode-region the instance is running in."
  type        = string
}

variable "SUBNET_CIDR_AKS" {
  description = "subnet cidr range for AKS cluster"
  type        = string
}

variable "SUBNET_CIDR_AKS_AGW" {
  description = "subnet cidr range for AKS Ingress App Gateway"
  type        = string
}

variable "SUBSCRIPTIONID" {
  description = "azure subscription id"
  type        = string
}

variable "TENANTID" {
  description = "TenantID"
  type        = string
}

variable "VNET_CIDR" {
  description = "azure subscription id"
  type        = string
}

# Variables controlling blue-green deployments
variable "SYS_BLUE_DEPLOY" {
  #default = "true"
  type = bool
}

variable "SYS_GREEN_DEPLOY" {
  # default = "false"
  type = bool
}

variable "SYS_GREEN_IS_LIVE" {
  # default = "true"
  type = bool
}

variable "SYS_DEPLOYMENT_PHASE" {
  # default = "deploy"
  type = string

  validation {
    condition     = contains(["deploy", "switch", "destroy"], var.SYS_DEPLOYMENT_PHASE)
    error_message = "Valid values for var.deployment_phase are: (deploy, switch or destroy)."
  }
}