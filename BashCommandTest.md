To run IaC
IaC is only setup to create AKS. Need further improvemeents.

cd iac
terraform init -backend-config='/backends/dev.cfg'
terraform plan -var-file='env.tfvars' -out='my.tfplan'
terraform apply my.tfplan

Set up helm in WSL following instructions in https://chamindac.blogspot.com/2023/12/setting-up-helm-in-wsl.html

Then do following

cd aks_manifests\prerequisites
run make file to setup prereuisites including keda namespace and keda-operator service account

cd aks_manifests\keda
intall keda with helm using ./installkeda.sh
then setup trigger authentications for keda by running make

If sh files gives not found issues. rename current files. create new setup.sh and videoprocessor.sh with same content.
Then docker build works. To avoid this on windows run below command for git before cloning this repo.
git config --global core.autocrlf false

docker build --no-cache -t chmediaservice:dev --progress=plain -f Dockerfile .

docker run -v c:/temp/videos:/media/data -e MEDIA_PATH='/media/data' --name chmediaservice chmediaservice:dev



docker tag chmediaservice:dev chdemosharedacr.azurecr.io/media/chmediaservice:1.1
docker login chdemosharedacr.azurecr.io -u spnappid -p spnapppwd
docker push chdemosharedacr.azurecr.io/media/chmediaservice:1.1

# Test message for Q
For this you need an asset (video) uploaded in cheuw001assetsstcool with below specified blob container having a folder named with assetId. The asset name set as original.

{
    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
    "assetId":  "bb5ab2dd-f89c-4689-976b-0de2fce614ec",
    "originalAssetBlobName": "original",
    "sourceStorageAccount": "cheuw001assetsstcool",
    "destinationStorageAccount": "cheuw001assetssthot",
    "commandArgs": [
        {
        "outFileOptions": "-vf fps=1/4",
        "outFileName": "walk_%04d.png"
        },
        {
        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
        "outFileName": "walk_720p.mp4"
        }
    ]
}

# local test

winpty docker exec -it chmediaservice sh

cd /media/data
ls -l


queueStorageAccKey=$(az storage account keys list -g ch-video-dev-euw-001-rg -n chvideodeveuw001queuest --query [0].value -o tsv)

echo $queueStorageAccKey

# Setting the expiry for SaS key for 48 hours to ensure it is available for the day (each day processor will create a new SaS key).
expirySaSKey=$(date -u -d "120 minutes" '+%Y-%m-%dT%H:%MZ')
echo "SAS key expiry utc" $expirySaSKey

queueSaSKey=$(az storage queue generate-sas --account-key $queueStorageAccKey --account-name chvideodeveuw001queuest -n demovideoqueue --permissions apru --expiry $expirySaSKey --https-only -o tsv)

echo $queueSaSKey

curl -i -X GET -H "x-ms-version: 2020-04-08" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages?peekonly=true&$queueSaSKey"

queueMessage=$(curl -s -X GET -H "x-ms-version: 2020-04-08" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages?peekonly=true&$queueSaSKey")
queueMessage=$(curl -s -X GET -H "x-ms-version: 2020-04-08" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages?visibilitytimeout=300&$queueSaSKey")

messageFileId=$(uuidgen)
echo $messageFileId

echo $queueMessage
echo $queueMessage > "$messageFileId.xml"
cat "$messageFileId.xml"

receivedMessageCount=$(yq --input-format xml -op ".QueueMessagesList | length" "$messageFileId.xml")
echo $receivedMessageCount

messageId=$(yq --input-format xml -op ".QueueMessagesList.QueueMessage.MessageId" "$messageFileId.xml")
messagePopReceipt=$(yq --input-format xml -op ".QueueMessagesList.QueueMessage.PopReceipt" "$messageFileId.xml")

echo $messagePopReceipt
echo -n $messagePopReceipt | jq -sRr @uri

encodedMessagePopReceipt=$(echo -n $messagePopReceipt | jq -sRr @uri)
echo $encodedMessagePopReceipt

yq --input-format xml -op ".QueueMessagesList.QueueMessage.MessageId" "$messageFileId.xml"

curl -i -X DELETE -H "x-ms-version: 2020-04-08" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages/$messageId?popreceipt=$encodedMessagePopReceipt&$queueSaSKey"

### reset visibility
curl -i -X PUT -H "x-ms-version: 2020-04-08" -H "Content-Length: 0" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages/$messageId?popreceipt=$encodedMessagePopReceipt&visibilitytimeout=1&$queueSaSKey"


yq --input-format xml -op ".QueueMessagesList.QueueMessage.MessageText" "$messageFileId.xml"

messageContent=$(yq --input-format xml -op ".QueueMessagesList.QueueMessage.MessageText" "$messageFileId.xml")

echo $messageContent > $messageFileId.json

jq -r ".assetContainerName" "$messageFileId.json"
jq -r ".assetId" "$messageFileId.json"
jq -r ".originalAssetBlobName" "$messageFileId.json"
jq -r ".sourceStorageAccount" "$messageFileId.json"
jq -r ".destinationStorageAccount" "$messageFileId.json"

jq -r ".commandArgs | length" "$messageFileId.json"
jq -r '.commandArgs[0].outFileOptions' "$messageFileId.json"

i=0
jq -r ".commandArgs[$i].outFileOptions" "$messageFileId.json"


### below with yq works as well
yq --input-format json -op ".assetId" "$messageFileId.json"
yq --input-format json -op ".commandArgs | length" "$messageFileId.json"
yq --input-format json -op ".commandArgs.$i.outFileOptions" "$messageFileId.json"

### getting message contents to variables
assetContainerName=$(jq -r ".assetContainerName" "$messageFileId.json")
assetId=$(jq -r ".assetId" "$messageFileId.json")
originalAssetBlobName=$(jq -r ".originalAssetBlobName" "$messageFileId.json")
sourceStorageAccount=$(jq -r ".sourceStorageAccount" "$messageFileId.json")
destinationStorageAccount=$(jq -r ".destinationStorageAccount" "$messageFileId.json")
commandCount=$(jq -r ".commandArgs | length" "$messageFileId.json")

echo $commandCount

generatedDirName="generated"
mkdir $assetId
cd $assetId
mkdir $generatedDirName

az storage blob download --auth-mode login --max-connections 5 --blob-url https://$sourceStorageAccount.blob.core.windows.net/$assetContainerName/$assetId/$originalAssetBlobName -f $assetId

echo $commandCount

# for loop is in videoprocessor.sh
i=0

outFileOptions=$(yq --input-format json -op .commandArgs.$i.outFileOptions ../$messageFileId.json)

outFileName=$(yq --input-format json -op .commandArgs.$i.outFileName ../$messageFileId.json)

ffmpeg -i $assetId $outFileOptions $generatedDirName/$outFileName

# then i=1 and repeat above

az storage container create --auth-mode login --account-name $destinationStorageAccount --name video-$assetId
az storage blob upload-batch --auth-mode login --max-connections 5 --overwrite true --account-name $destinationStorageAccount -s $generatedDirName -d video-$assetId
cd ..
rm -rf $assetId

rm -f $messageFileId.json
rm -f $messageFileId.xml


start=$(date '+%s')
end=$(date '+%s')
elapsed=$(($end - $start))


$(($(stat --format=%s original)/1048576))

$(du -h original)

messageContent='{ "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3", "assetId": "bb5ab2dd-f89c-4689-976b-0de2fce614ec", "originalAssetBlobName": "original", "sourceStorageAccount": "cheuw001assetsstcool", "destinationStorageAccount": "cheuw001assetssthot", "commandArgs": [ { "inFileOptions": "-vf fps=1/4", "outFileName": "walk_%04d.png" }, { "inFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720", "outFileName": "walk_720p.mp4" } ] }'

echo $messageContent | jq -r ".assetContainerName"
echo $messageContent | jq -r ".commandArgs | length"

queueMessage='<?xml version="1.0" encoding="utf-8"?><QueueMessagesList><QueueMessage><MessageId>d1d1469b-ca41-4500-82ac-23ff1d0e8167</MessageId><InsertionTime>Sun, 05 Nov 2023 18:11:40 GMT</InsertionTime><ExpirationTime>Sun, 12 Nov 2023 18:11:40 GMT</ExpirationTime><PopReceipt>AgAAAAMAAAAAAAAAZMcOzMQQ2gE=</PopReceipt><TimeNextVisible>Mon, 06 Nov 2023 15:20:38 GMT</TimeNextVisible><DequeueCount>15</DequeueCount><MessageText>{ "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3", "assetId": "bb5ab2dd-f89c-4689-976b-0de2fce614ec", "originalAssetBlobName": "original", "sourceStorageAccount": "cheuw001assetsstcool", "destinationStorageAccount": "cheuw001assetssthot", "commandArgs": [ { "inFileOptions": "-vf fps=1/4", "outFileName": "walk_%04d.png" }, { "inFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720", "outFileName": "walk_720p.mp4" } ] }</MessageText></QueueMessage></QueueMessagesList>'

echo $queueMessage

echo $queueMessage | yq --input-format xml -op ".QueueMessagesList.QueueMessage.MessageId"
echo $queueMessage | yq --input-format xml -op ".QueueMessagesList | length"