# Orleans Performance Tests in Azure Container Apps

This project originates from fork of https://github.com/Azure-Samples/Orleans-Cluster-on-Azure-Container-Apps


## Tests

Each test is assigned by number and has own branch



1. Max limit grains in silo tests
2. Orleans broadcast channels tests


## Deploy

Deploy pipeline is triggered by push to `deploy` branch.

## Solution structure

Projects:
1. Silo
2. Tester
3. Dashboard
4. Abstraction

### Silo

System under test.

### Tester

Tester.
Implements NBomber.
Handling by REST API.

### Dashboard

Orleans Dashboard.

### Abstraction

Shared library.

## Common trouble and troubleshooting

### Cannot deploy due to credentials

This error happened almost any time when had been created new resources in Azure.

The solution is to retry `deploy` job until pass. (retry about 5 times)