variable "prefix" {
  description = "Organization prefix"
}

variable "project" {
  description = "Project name"
}

variable "devip" {
  description = "Developer IP"
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

variable "aks_subnet_id" {
  description = "Name of the Resource Group"
}

variable "tags" {
  description = "Map of tags to assign to the Cluster"
  type        = map(string)
}