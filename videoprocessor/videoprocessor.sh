logDate=""

ls -l
echo "$MEDIA_PATH"
cd "$MEDIA_PATH"
ls -l

#ffmpeg -i Earth.avi Earth.mp4
#ffmpeg -i Earth.avi -vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720 Earth_720p.mp4
#ffmpeg -i Earth.avi -vf fps=1/10 %04d.png

# ffmpeg -i friendseating.webm -vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720 generated/friendseating_720p.mp4
# ffmpeg -i friendseating.webm -vf fps=1/4 generated/%04d.png
# ffmpeg -i walk.mp4 -vf fps=1/4 generated/walk_%04d.png
# ffmpeg -i beach.mp4 -vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280 generated/beach_720p.mp4
# ffmpeg -i beach.mp4 -vf fps=1/2 generated/beach_%04d.png

# ffmpeg -i https://cheuw001assetsstcool.blob.core.windows.net/originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3/227ac968-7f98-41b5-806c-cd966f41128c/original -vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280 generated/beach_720p.mp4
# ffmpeg -i https://cheuw001assetsstcool.blob.core.windows.net/originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3/227ac968-7f98-41b5-806c-cd966f41128c/original -vf fps=1/2 generated/beach_%04d.png

# ffmpeg -i https://cheuw001assetsstcool.blob.core.windows.net/originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3/bb5ab2dd-f89c-4689-976b-0de2fce614ec/original -vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720 generated/walk_720p.mp4
# ffmpeg -i https://cheuw001assetsstcool.blob.core.windows.net/originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3/bb5ab2dd-f89c-4689-976b-0de2fce614ec/original -vf fps=1/4 generated/walk_%04d.png

#az storage container exists --auth-mode login --account-name cheuw001assetsstcool -n originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3

# expiry=$(date -u -d "30 minutes" '+%Y-%m-%dT%H:%MZ')
# echo "SAS key expiry utc" $expiry
# az storage container generate-sas --auth-mode login --as-user --account-name cheuw001assetsstcool -n originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3 --permissions r --expiry $expiry --https-only
# az storage container generate-sas --account-key storageacckey --account-name cheuw001assetsstcool -n originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3 --permissions r --expiry $expiry --https-only

#ffmpeg -i "https://cheuw001assetsstcool.blob.core.windows.net/originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3/bb5ab2dd-f89c-4689-976b-0de2fce614ec/original?se=2023-10-31T16%3A13Z&sp=r&spr=https&sv=2022-11-02&sr=c&sig=BmgfTMe1MzmFP2Nh1zkYHGhnHAceyW6fXG0csHX%2Bqd4%3D" -vf fps=1/4 generated/walk_%04d.png

# assetContianerName="originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3"
# assetId="bb5ab2dd-f89c-4689-976b-0de2fce614ec"
# assetNamePrefix="walk_"
# originalAssetBlobName="original"
# sourceStorageAccount="cheuw001assetsstcool"
# destinationStorageAccount="cheuw001assetssthot"
# generatedDirName="generated"

# mkdir $assetId
# cd $assetId
# mkdir $generatedDirName
# az storage blob download --auth-mode login --max-connections 5 --blob-url https://$sourceStorageAccount.blob.core.windows.net/$assetContianerName/$assetId/$originalAssetBlobName -f $assetId
# ffmpeg -i $assetId -vf fps=1/4 $generatedDirName/$assetNamePrefix%04d.png
# ffmpeg -i $assetId -vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720 $generatedDirName/"$assetNamePrefix"720p.mp4
# az storage container create --auth-mode login --account-name $destinationStorageAccount --name video-$assetId
# az storage blob upload-batch --auth-mode login --max-connections 5 --overwrite true --account-name $destinationStorageAccount -s $generatedDirName -d video-$assetId
# cd ..
# rm -rf $assetId

queueSaSKey=""
generatedDirName="generated"
maxProcessWaitTime=14400

while :
do
    currentDate=$(date '+%Y-%m-%d')
    

    if [ "$logDate" != "$currentDate" ]
    then
        logDate=$currentDate
        echo "============================================"
        echo "ffmpeg service is running for $logDate"

        # Setting up queue storage SaS key - valid for 48 hours (Resets in every day)
        queueStorageAccKey=$(az storage account keys list -g ch-video-dev-euw-001-rg -n chvideodeveuw001queuest --query [0].value -o tsv)
        expirySaSKey=$(date -u -d "48 hours" '+%Y-%m-%dT%H:%MZ')
        queueSaSKey=$(az storage queue generate-sas --account-key $queueStorageAccKey --account-name chvideodeveuw001queuest -n demovideoqueue --permissions apru --expiry $expirySaSKey --https-only -o tsv)

        echo "Genrated SaS Key for queue."
        echo "============================================"
    fi

    started=$(date '+%s')
    queueMessage=$(curl -s -X GET -H "x-ms-version: 2020-04-08" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages?visibilitytimeout=$maxProcessWaitTime&$queueSaSKey")

    messageFileId=$(uuidgen)
    echo $queueMessage > "$messageFileId.xml"
    receivedMessageCount=$(yq --input-format xml -op ".QueueMessagesList | length" "$messageFileId.xml")

    if [ $receivedMessageCount -gt 0 ]
    then
        messageId=$(yq --input-format xml -op ".QueueMessagesList.QueueMessage.MessageId" "$messageFileId.xml")
        messagePopReceipt=$(yq --input-format xml -op ".QueueMessagesList.QueueMessage.PopReceipt" "$messageFileId.xml")
        messageDequeueCount=$(yq --input-format xml -op ".QueueMessagesList.QueueMessage.DequeueCount" "$messageFileId.xml")
        messageContent=$(yq --input-format xml -op ".QueueMessagesList.QueueMessage.MessageText" "$messageFileId.xml")

        # Pop receipt must be encoded to enable passing it via querystring
        encodedMessagePopReceipt=$(echo -n $messagePopReceipt | jq -sRr @uri)

        echo $messageContent > $messageFileId.json

        echo "Received below message from queue"
        echo "--------------------------------------------"
        echo $messageContent
        echo "--------------------------------------------"

        assetContianerName=$(jq -r ".assetContianerName" "$messageFileId.json")
        assetId=$(jq -r ".assetId" "$messageFileId.json")
        originalAssetBlobName=$(jq -r ".originalAssetBlobName" "$messageFileId.json")
        sourceStorageAccount=$(jq -r ".sourceStorageAccount" "$messageFileId.json")
        destinationStorageAccount=$(jq -r ".destinationStorageAccount" "$messageFileId.json")
        commandCount=$(jq -r ".commandArgs | length" "$messageFileId.json")

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
                inFileOptions=$(yq --input-format json -op .commandArgs.$i.inFileOptions ../$messageFileId.json)
                outFileName=$(yq --input-format json -op .commandArgs.$i.outFileName ../$messageFileId.json)

                echo "--------------------------------------------"
                echo "Processing... ffmpeg -i $assetId $inFileOptions $generatedDirName/$outFileName"
                ffmpeg -i $assetId $inFileOptions $generatedDirName/$outFileName
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
            echo "Downoad time: $downloadElapsed"
            echo "Process time: $processElapsed"
            echo "Upload time: $uploadElapsed"
            echo -e "Asset size: $assetSize \nTotal time: $totalElapsed \nDownoad time: $downloadElapsed \nProcess time: $processElapsed \nUpload time: $uploadElapsed" > "$generatedDirName/processtime.txt"
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
                echo "Message added to the queue."
                echo "--------------------------------------------"
            fi
        }
        
        cd ..
        rm -rf $assetId
        rm -f $messageFileId.json
        rm -f $messageFileId.xml
    else
        # echo "No messages in the queue." # later comment this to prevent excessive logs.
        rm -f $messageFileId.xml
    fi
    sleep 1
done