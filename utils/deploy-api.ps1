Invoke-Expression -Command (aws ecr get-login --no-include-email --profile dev)
docker build -t djambi3_api-prod -f api/Dockerfile .
docker tag djambi3_api-prod 798135284442.dkr.ecr.us-east-1.amazonaws.com/djambi-api
docker push 798135284442.dkr.ecr.us-east-1.amazonaws.com/djambi-api
aws elasticbeanstalk rebuild-environment --environment-name djambi-api-dev --profile dev