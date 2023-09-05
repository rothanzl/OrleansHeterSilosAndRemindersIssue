param name string
param location string = resourceGroup().location
param containerAppEnvironmentId string
param repositoryImage string = 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
param envVars array = []
param registry string
param registryUsername string
param minReplicas int = 1
param maxReplicas int = 1
// param scalerUrl string
param allowExternalIngress bool = false
param targetIngressPort int = 80
@secure()
param registryPassword string

resource containerApp 'Microsoft.App/containerApps@2023-05-01' ={
  name: name
  location: location
  properties: {
    managedEnvironmentId: containerAppEnvironmentId
    configuration: {
      activeRevisionsMode: 'Single'
      secrets: [
        {
          name: 'container-registry-password'
          value: registryPassword
        }
      ]      
      registries: [
        {
          server: registry
          username: registryUsername
          passwordSecretRef: 'container-registry-password'
        }
      ]
      ingress: {
        external: allowExternalIngress
        targetPort: targetIngressPort
      }
    }
    template: {
      containers: [
        {
          image: repositoryImage
          name: name
          env: envVars
          resources:{
            cpu: json('2.0')
            memory: '4.0Gi'
          }
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      //   rules: [
      //     {
      //       name: 'scaler'
      //       custom: {
      //         type: 'external'
      //         metadata: {
      //           scalerAddress: '${scalerUrl}:80'
      //           graintype: 'sensortwin'
      //           siloNameFilter: 'silo'
      //           upperbound: '300'
      //         }
      //       }
      //     }
      //   ]
      }
    }
  }
}

output url string = containerApp.properties.configuration.ingress.fqdn
