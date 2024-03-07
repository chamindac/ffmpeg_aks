# output "storage_connection_string" {
#   value = azurerm_storage_account.instancestorageehn.primary_connection_string
# }

# output "consumer_1" {
#   value = "${azurerm_eventhub_namespace_authorization_rule.consumer["eventhub1consumer"].primary_connection_string};TransportType=Amqp"
# }

# output "publisher_1" {
#   value = "${azurerm_eventhub_namespace_authorization_rule.publisher["eventhub1publisher"].primary_connection_string};TransportType=Amqp"
# }

# output "consumer_2" {
#   value = "${azurerm_eventhub_namespace_authorization_rule.consumer["eventhub2consumer"].primary_connection_string};TransportType=Amqp"
# }

# output "publisher_2" {
#   value = "${azurerm_eventhub_namespace_authorization_rule.publisher["eventhub2publisher"].primary_connection_string};TransportType=Amqp"
# }

output "storage_name" {
  value = azurerm_storage_account.instancestorageehn.name
}

output "eventhubnamespace_1_name" {
  value = azurerm_eventhub_namespace.instanceeventhub[0].name
}

output "eventhubnamespace_2_name" {
  value = azurerm_eventhub_namespace.instanceeventhub[1].name
}