# resource group for aks
resource "azurerm_resource_group" "instancerg" {
  name     = "${var.PREFIX}-${var.PROJECT}-${var.ENVNAME}-rg"
  location = var.REGION

  tags = merge(tomap({
    Service = "resource_group"
  }), local.tags)
}

# # Random fail simulation
# resource "time_rotating" "randomfail" {
#   rotation_minutes = 1 # make sure we run in every TF plan and apply

#   provisioner "local-exec" {
#     command = <<-SHELL
#       retval=$((${self.unix}%2))
#       echo exitcode is:$retval
#       return $retval
#     SHELL
#   }
# }