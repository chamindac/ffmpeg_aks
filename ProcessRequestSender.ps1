$assetsFilename = 'assetsToProcess.json'

$queueStorageAccKey = az storage account keys list -g ch-video-dev-euw-001-rg -n chvideodeveuw001queuest --query [0].value -o tsv

$expirySaSKey = (Get-Date).AddMinutes(30).ToString('yyyy-MM-ddTHH:mmZ') 
$ErrorActionPreference = 'SilentlyContinue'
$queueSaSKey= az storage queue generate-sas --account-key $queueStorageAccKey --account-name chvideodeveuw001queuest -n demovideoqueue --permissions apru --expiry $expirySaSKey --https-only -o tsv
$ErrorActionPreference = 'Continue'

$assets = Get-Content -Path "$PSScriptRoot/$assetsFilename" | ConvertFrom-Json

foreach($asset in $assets)
{
    $assetMessage = '<QueueMessage><MessageText>' + ($asset | ConvertTo-Json) + '</MessageText></QueueMessage>'

    $head = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $head = @{}
    $head.Add("-ms-version","2020-04-08")

    $url = 'https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages?' + $queueSaSKey

    Invoke-WebRequest -Method Post -Uri $url -ContentType 'text/plain' -Body $assetMessage -Headers $head
}