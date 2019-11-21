$version = 3 # Update this every time
$eb_app = "apex-api"
$eb_env = "apex-api-dev"
$s3_bucket = "apex-game-api-bundle"
$bundle_name = "eb-bundle.zip"
$bundle_location = "api/"+$bundle_name
$profile = "dev"

# Create zip file
Compress-Archive -Path api/Dockerrun.aws.json -DestinationPath $bundle_location -Force

# Upload zip file to S3
aws s3 cp $bundle_location s3://$s3_bucket/ --profile $profile

# Create new EB app version pointing to new zip file
aws elasticbeanstalk create-application-version `
    --application-name $eb_app `
    --version-label $version `
    --source-bundle S3Bucket=$s3_bucket,S3Key=$bundle_name `
    --profile $profile

# Update environment to use new app version
aws elasticbeanstalk update-environment `
    --application-name $eb_app `
    --environment-name $eb_env `
    --version-label $version `
    --profile $profile