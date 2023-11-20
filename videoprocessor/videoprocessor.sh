ls -l
echo "$MEDIA_PATH"
cd "$MEDIA_PATH"
ls -l

queueSaSKey=""
generatedDirName="generated"
maxProcessWaitTime=14400 # Max 4 hour wait for processing a file
currentDate=$(date '+%Y-%m-%d')

echo "============================================"
echo "ffmpeg service is running for $currentDate"

# Setting up queue storage SaS key - valid for 5 hours - should be higher than maxProcessWaitTime
queueStorageAccKey=$(az storage account keys list -g ch-video-dev-euw-001-rg -n chvideodeveuw001queuest --query [0].value -o tsv)
expirySaSKey=$(date -u -d "5 hours" '+%Y-%m-%dT%H:%MZ')
queueSaSKey=$(az storage queue generate-sas --account-key $queueStorageAccKey --account-name chvideodeveuw001queuest -n demovideoqueue --permissions apru --expiry $expirySaSKey --https-only -o tsv)

echo "Genrated SaS Key for queue."
echo "============================================"


started=$(date '+%s')
queueMessage=$(curl -s -X GET -H "x-ms-version: 2020-04-08" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages?visibilitytimeout=$maxProcessWaitTime&$queueSaSKey")
receivedMessageCount=$(echo $queueMessage | yq --input-format xml -op ".QueueMessagesList | length")

if [ $receivedMessageCount -gt 0 ]
then
    messageId=$(echo $queueMessage | yq --input-format xml -op ".QueueMessagesList.QueueMessage.MessageId")
    messagePopReceipt=$(echo $queueMessage | yq --input-format xml -op ".QueueMessagesList.QueueMessage.PopReceipt")
    messageDequeueCount=$(echo $queueMessage | yq --input-format xml -op ".QueueMessagesList.QueueMessage.DequeueCount")
    messageContent=$(echo $queueMessage | yq --input-format xml -op ".QueueMessagesList.QueueMessage.MessageText")

    # Pop receipt must be encoded to enable passing it via querystring
    encodedMessagePopReceipt=$(echo -n $messagePopReceipt | jq -sRr @uri)

    echo "Received below message from queue"
    echo "--------------------------------------------"
    echo $messageContent
    echo "--------------------------------------------"

    assetContianerName=$(echo $messageContent | jq -r ".assetContianerName")
    assetId=$(echo $messageContent | jq -r ".assetId")
    originalAssetBlobName=$(echo $messageContent | jq -r ".originalAssetBlobName")
    sourceStorageAccount=$(echo $messageContent | jq -r ".sourceStorageAccount")
    destinationStorageAccount=$(echo $messageContent | jq -r ".destinationStorageAccount")
    commandCount=$(echo $messageContent | jq -r ".commandArgs | length")

    rm -rf $assetId # clean up if any prevoius attempt data exist for the asset
    mkdir $assetId
    cd $assetId
    
    { # try
        downloadStarted=$(date '+%s')
        az storage blob download --auth-mode login --max-connections 5 --blob-url https://$sourceStorageAccount.blob.core.windows.net/$assetContianerName/$assetId/$originalAssetBlobName -f $assetId
        
        assetSize=$(du -h)
        mkdir $generatedDirName
        processStarted=$(date '+%s')
        
        for (( i=0; i<$commandCount; i++ ))
        do 
            outFileOptions=$(echo $messageContent | yq --input-format json -op .commandArgs.$i.outFileOptions)
            outFileName=$(echo $messageContent | yq --input-format json -op .commandArgs.$i.outFileName)

            echo "--------------------------------------------"
            echo "Processing... ffmpeg -i $assetId $outFileOptions $generatedDirName/$outFileName"
            ffmpeg -i $assetId $outFileOptions $generatedDirName/$outFileName
            echo "--------------------------------------------"
        done

        uploadStarted=$(date '+%s')
        echo "--------------------------------------------"
        echo "Uploading generated asset files..."
        az storage container create --auth-mode login --account-name $destinationStorageAccount --name video-$assetId
        az storage blob upload-batch --auth-mode login --max-connections 5 --overwrite true --account-name $destinationStorageAccount -s $generatedDirName -d video-$assetId
        echo "Uploading generated asset files completed."
        echo "--------------------------------------------"

        ended=$(date '+%s')
        totalElapsed=$(($ended - $started))
        uploadElapsed=$(($ended - $uploadStarted))
        processElapsed=$(($uploadStarted - $processStarted))
        downloadElapsed=$(($processStarted - $downloadStarted))

        echo "--------------------------------------------"
        echo "Successfully processed in $messageDequeueCount attempt(s). Removing message from the queue..."
        curl -i -X DELETE -H "x-ms-version: 2020-04-08" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages/$messageId?popreceipt=$encodedMessagePopReceipt&$queueSaSKey"
        echo "Message removed from the queue."
        echo "Asset size: $assetSize"
        echo "Total time: $totalElapsed"
        echo "Download time: $downloadElapsed"
        echo "Process time: $processElapsed"
        echo "Upload time: $uploadElapsed"
        echo -e "Asset size: $assetSize \nTotal time: $totalElapsed \nDowlnoad time: $downloadElapsed \nProcess time: $processElapsed \nUpload time: $uploadElapsed" > "$generatedDirName/processtime.txt"
        az storage blob upload --auth-mode login --overwrite true --account-name $destinationStorageAccount -f "$generatedDirName/processtime.txt" -n processtime.txt -c video-$assetId
        echo "--------------------------------------------"

    } || { # catch
        if [ $messageDequeueCount -ge 3 ] # 3 attempts made to process
        then
            echo "--------------------------------------------"
            echo "Failed to process in 3 attempts. Removing message from the queue..."
            curl -i -X DELETE -H "x-ms-version: 2020-04-08" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages/$messageId?popreceipt=$encodedMessagePopReceipt&$queueSaSKey"
            echo "Message removed from the queue."
            echo "--------------------------------------------"
        else
            echo "Failed to process in $messageDequeueCount attempt. Adding message back to the queue..."
            curl -i -X PUT -H "x-ms-version: 2020-04-08" -H "Content-Length: 0" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages/$messageId?popreceipt=$encodedMessagePopReceipt&visibilitytimeout=1&$queueSaSKey"
            echo "Message added back to the queue."
            echo "--------------------------------------------"
        fi
    }
    
    cd ..
    rm -rf $assetId
else
    echo "No messages in the queue. Job execution completed." 
fi
