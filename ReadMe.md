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

{
    "assetContianerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
    "assetId":  "bb5ab2dd-f89c-4689-976b-0de2fce614ec",
    "assetNamePrefix": "walk_",
    "originalAssetBlobName": "original",
    "sourceStorageAccount": "cheuw001assetsstcool",
    "destinationStorageAccount": "cheuw001assetssthot"
}

# local test

winpty docker exec -it chmediaservice sh

cd /media/data
ls -l

expiry=$(date -u -d "120 minutes" '+%Y-%m-%dT%H:%MZ')
echo "SAS key expiry utc" $expiry

az storage queue generate-sas --account-key 6PZA4RP00tnVKLy+gmbbJ5FlagOtb3Ip2IptOQ18vvKISwFwJfgSBu/ub29OoZdZxuYcPLcv7r24+ASthMRN+g== --account-name chvideodeveuw001queuest -n demovideoqueue --permissions apru --expiry $expiry --https-only

curl -i -X GET -H "x-ms-version: 2020-04-08" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages?peekonly=true&SaSKey"

queueMessage=$(curl -s -X GET -H "x-ms-version: 2020-04-08" "https://chvideodeveuw001queuest.queue.core.windows.net/demovideoqueue/messages?peekonly=true&SaSKey")

echo $queueMessage
echo $queueMessage > queueMessage.xml
cat queueMessage.xml

yq --input-format xml -op '.QueueMessagesList.QueueMessage.MessageId' 'queueMessage.xml'

messageContent=$(yq --input-format xml -op '.QueueMessagesList.QueueMessage.MessageText' 'queueMessage.xml')

echo $messageContent > messageContent.json

yq --input-format json -op '.assetContianerName' 'messageContent.json'
yq --input-format json -op '.assetId' 'messageContent.json'
yq --input-format json -op '.assetNamePrefix' 'messageContent.json'
yq --input-format json -op '.originalAssetBlobName' 'messageContent.json'
yq --input-format json -op '.sourceStorageAccount' 'messageContent.json'
yq --input-format json -op '.destinationStorageAccount' 'messageContent.json'


rm -f messageContent.json
rm -f queueMessage.xml
