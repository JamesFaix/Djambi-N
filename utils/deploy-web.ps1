$s3_bucket = "apex-game-web"
$profile = "dev"
$api_address = "apex-api-dev.us-east-1.elasticbeanstalk.com"
$here = (Get-Location).Path -replace "\\", "/"
$web_output = $here + "/web/dist/prod/"
$cloudfront_distro = "EKHPVA69D4CE9"

# Build web client
cd .\web
webpack `
    --env.NODE_ENV="production" `
    --env.APEX_apiAddress=$api_address
cd ..

# Upload to S3
$files = Get-ChildItem $web_output -Recurse -File
ForEach($f in $files) {
    $absolute_path = $f.FullName -replace "\\", "/"
    $relative_path = $absolute_path -replace $web_output, ""
    $destination =  "s3://" + $s3_bucket + "/" + $relative_path
    aws s3 cp $absolute_path $destination --profile $profile
}

# Invalidate Cloudfront distribution
aws cloudfront create-invalidation `
    --distribution-id $cloudfront_distro `
    --profile $profile `
    --paths '/*'

# TODO: Make filtering uploads and invalidations easier