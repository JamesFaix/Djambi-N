# AWS Architecture and Deployment

## Web client

### Architecture

- Static files hosted in S3 bucket
- Cloudfront distribution used as CDN

### Scripts

Run `utils/deploy-web.ps1` to

- Build the web client in production mode, with the right API url
- Upload all output files to S3
- Invalidate all files in Cloudfront

This script will not

- Run the Client Generator utility before building
- Prune old files in S3

## API

### Architecture

- Hosted as Elastic Beanstalk (EB) application
- Docker image is stored in ECR
- EB bundle is stored in separate S3 bucket from web client

### Scripts

Run `utils/deploy-api-image.ps1` to

- Build the API in production mode
- Upload new docker image to ECR
- Rebuild EB environment with new image

Run `utils/deploy-api-bundle.ps1` to

- Build the EB bundle
- Upload the bundle to S3
- Update EB environment to use new bundle
- Rebuild EB environment with new bundle

Both of these scripts have some secrets omitted from source control. Update these places before running.

## Database

### Architecture

- Hosted in RDS
- SQL Docker image is only used for local development
