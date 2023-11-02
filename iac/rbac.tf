# refer to sub_owners ad group to assign as aks admins 
data "azuread_group" "sub_owners" {
  display_name     = "sub_owners"
  security_enabled = true
}