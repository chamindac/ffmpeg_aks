output "storage_connection_string" {
  value = azurerm_storage_account.instancestorageehn.primary_connection_string
}

output "storage_name" {
  value = azurerm_storage_account.instancestorageehn.name
}

output "consumer_1" {
  value = "${azurerm_eventhub_namespace_authorization_rule.consumer["eventhub1consumer"].primary_connection_string};TransportType=Amqp"
}

output "publisher_1" {
  value = "${azurerm_eventhub_namespace_authorization_rule.publisher["eventhub1publisher"].primary_connection_string};TransportType=Amqp"
}

output "consumer_2" {
  value = "${azurerm_eventhub_namespace_authorization_rule.consumer["eventhub2consumer"].primary_connection_string};TransportType=Amqp"
}

output "publisher_2" {
  value = "${azurerm_eventhub_namespace_authorization_rule.publisher["eventhub2publisher"].primary_connection_string};TransportType=Amqp"
}