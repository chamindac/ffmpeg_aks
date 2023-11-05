To run IaC
IaC is only setup to create AKS. Need further improvemeents.

cd iac
terraform init -backend-config='/backends/dev.cfg'
terraform plan -var-file='env.tfvars' -out='my.tfplan'
terraform apply my.tfplan

If sh files gives not found issues. rename current files. create new setup.sh and videoprocessor.sh with same content.
Then docker build works. To avoid this on windows run below command for git before cloning this repo.
git config --global core.autocrlf false

docker build --no-cache -t chmediaservice:dev --progress=plain -f Dockerfile .

docker run -v c:/temp/videos:/media/data -e MEDIA_PATH='/media/data' --name chmediaservice chmediaservice:dev



docker tag chmediaservice:dev chdemosharedacr.azurecr.io/media/chmediaservice:1.0
docker login chdemosharedacr.azurecr.io -u spnappid -p spnapppwd
docker push chdemosharedacr.azurecr.io/media/chmediaservice:1.0

# Test message for Q

{
    "assetContianerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
    "assetId":  "bb5ab2dd-f89c-4689-976b-0de2fce614ec",
    "originalAssetBlobName": "original",
    "sourceStorageAccount": "cheuw001assetsstcool",
    "destinationStorageAccount": "cheuw001assetssthot",
    "commandCount": 2,
    "commandArgs": [
        {
        "inFileOptions": "-vf fps=1/4",
        "outFileName": "walk_%04d.png"
        },
        {
        "inFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
        "outFileName": "walk_720p.mp4"
        }
    ]
}

# local test

winpty docker exec -it chmediaservice sh

cd /media/data
ls -l

expiry=$(date -u -d "120 minutes" '+%Y-%m-%dT%H:%MZ')
echo "SAS key expiry utc" $expiry

az storage queue generate-sas --account-key accKey --account-name chvideodeveuw001queuest -n demovideoqueue --permissions apru --expiry $expiry --https-only

curl -i -X GET -H "x-ms-version: 2020-04-08" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages?peekonly=true&SaSKey"

queueMessage=$(curl -s -X GET -H "x-ms-version: 2020-04-08" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages?peekonly=true&SaSKey")

echo $queueMessage
echo $queueMessage > queueMessage.xml
cat queueMessage.xml

yq --input-format xml -op .QueueMessagesList.QueueMessage.MessageId queueMessage.xml

messageContent=$(yq --input-format xml -op .QueueMessagesList.QueueMessage.MessageText queueMessage.xml)

echo $messageContent > messageContent.json

yq --input-format json -op .assetContianerName messageContent.json
yq --input-format json -op .assetId messageContent.json
yq --input-format json -op .originalAssetBlobName messageContent.json
yq --input-format json -op .sourceStorageAccount messageContent.json
yq --input-format json -op .destinationStorageAccount messageContent.json
yq --input-format json -op .commandCount messageContent.json

assetContianerName=$(yq --input-format json -op .assetContianerName messageContent.json)
assetId=$(yq --input-format json -op .assetId messageContent.json)
originalAssetBlobName=$(yq --input-format json -op .originalAssetBlobName messageContent.json)
sourceStorageAccount=$(yq --input-format json -op .sourceStorageAccount messageContent.json)
destinationStorageAccount=$(yq --input-format json -op .destinationStorageAccount messageContent.json)
commandCount=$(yq --input-format json -op .commandCount messageContent.json)

generatedDirName="generated"
mkdir $assetId
cd $assetId
mkdir $generatedDirName

az storage blob download --auth-mode login --max-connections 5 --blob-url https://$sourceStorageAccount.blob.core.windows.net/$assetContianerName/$assetId/$originalAssetBlobName -f $assetId

echo $commandCount

# for loop is in videoprocessor.sh
i=0

inFileOptions=$(yq --input-format json -op .commandArgs.$i.inFileOptions ../messageContent.json)

outFileName=$(yq --input-format json -op .commandArgs.$i.outFileName ../messageContent.json)

ffmpeg -i $assetId $inFileOptions $generatedDirName/$outFileName

# then i=1 and repeat above

az storage container create --auth-mode login --account-name $destinationStorageAccount --name video-$assetId
az storage blob upload-batch --auth-mode login --max-connections 5 --overwrite true --account-name $destinationStorageAccount -s $generatedDirName -d video-$assetId
cd ..
rm -rf $assetId

rm -f messageContent.json
rm -f queueMessage.xml

