terraform {
  backend "azurerm" {}
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.94.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "=2.47.0"
    }
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.SUBSCRIPTIONID
}

provider "azurerm" {
  alias           = "mgmt"
  subscription_id = var.MANAGEMENTSUBSCRIPTIONID
  features {}
}