$profile="dev"
$ecr_repo="<account>.dkr.ecr.us-east-1.amazonaws.com/apex-api"
$docker_tag = "apex_api-prod"
$eb_env = "apex-api-dev"

Invoke-Expression -Command (aws ecr get-login --no-include-email --profile $profile)
docker build -t $docker_tag -f api/Dockerfile .
docker tag $docker_tag $ecr_repo
docker push $ecr_repo
aws elasticbeanstalk rebuild-environment `
    --environment-name $eb_env `
    --profile $profile