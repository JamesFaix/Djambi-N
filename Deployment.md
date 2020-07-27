# AWS Architecture and Deployment

## Architecture

### Web client

- Static files hosted in S3 bucket
- Cloudfront distribution used as CDN

### API

- Hosted as Elastic Beanstalk (EB) application
- Docker image is stored in ECR
- EB bundle is stored in separate S3 bucket from web client

### Database

- Hosted in RDS
- SQL Docker image is only used for local development

## Deployment

Merges to `master` will trigger deployments via GitHub Actions.

- Changes to `web/*` will trigger a web deployment
- Changes to `api/*` will trigger an API deployment
- Changes to `api/api.db.model/*` will trigger a database migration

There are a few things the pipelines won't do

- Provision new resources in AWS
- Clean out obsolete files from S3 bucket for web
