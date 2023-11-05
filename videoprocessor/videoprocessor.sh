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

assetContianerName="originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3"
assetId="bb5ab2dd-f89c-4689-976b-0de2fce614ec"
assetNamePrefix="walk_"
originalAssetBlobName="original"
sourceStorageAccount="cheuw001assetsstcool"
destinationStorageAccount="cheuw001assetssthot"
generatedDirName="generated"

mkdir $assetId
cd $assetId
mkdir $generatedDirName
az storage blob download --auth-mode login --max-connections 5 --blob-url https://$sourceStorageAccount.blob.core.windows.net/$assetContianerName/$assetId/$originalAssetBlobName -f $assetId
ffmpeg -i $assetId -vf fps=1/4 $generatedDirName/$assetNamePrefix%04d.png
ffmpeg -i $assetId -vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720 $generatedDirName/"$assetNamePrefix"720p.mp4
az storage container create --auth-mode login --account-name $destinationStorageAccount --name video-$assetId
az storage blob upload-batch --auth-mode login --max-connections 5 --overwrite true --account-name $destinationStorageAccount -s $generatedDirName -d video-$assetId
cd ..
rm -rf $assetId


while :
do
    currentDate=$(date '+%Y-%m-%d')

    if [ "$logDate" != "$currentDate" ]
    then
        logDate=$currentDate
        echo "media service is running for $logDate"
    fi

    # This is for loop through commands when a message is received
    # commandCount=10

    # for (( i=0; i<$commandCount; i++ ))
    # do 
    #     echo "Welcome $i times"
    # done

    sleep 60
done