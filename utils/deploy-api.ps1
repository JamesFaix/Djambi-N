Invoke-Expression -Command (aws ecr get-login --no-include-email --profile dev)
docker build -t apex_api-prod -f api/Dockerfile .
docker tag apex_api-prod <account>.dkr.ecr.us-east-1.amazonaws.com/apex-api
docker push <account>.dkr.ecr.us-east-1.amazonaws.com/apex-api
aws elasticbeanstalk rebuild-environment --environment-name apex-api-dev --profile dev