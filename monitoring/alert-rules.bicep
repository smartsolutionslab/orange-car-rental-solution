// Azure Monitor Alert Rules for Orange Car Rental
// Deploy with: az deployment group create --resource-group <rg> --template-file alert-rules.bicep

@description('Application Insights resource name')
param appInsightsName string

@description('Action group for notifications')
param actionGroupId string

@description('Environment (production, staging, development)')
param environment string = 'production'

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

// High Error Rate Alert
resource highErrorRateAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'orange-car-rental-high-error-rate'
  location: 'global'
  properties: {
    description: 'Alert when error rate exceeds 5% of total requests'
    severity: 2
    enabled: true
    scopes: [
      appInsights.id
    ]
    evaluationFrequency: 'PT5M'
    windowSize: 'PT15M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'FailedRequests'
          metricName: 'requests/failed'
          operator: 'GreaterThan'
          threshold: 50
          timeAggregation: 'Total'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroupId
      }
    ]
  }
}

// Slow Response Time Alert
resource slowResponseTimeAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'orange-car-rental-slow-response'
  location: 'global'
  properties: {
    description: 'Alert when average response time exceeds 2 seconds'
    severity: 3
    enabled: true
    scopes: [
      appInsights.id
    ]
    evaluationFrequency: 'PT5M'
    windowSize: 'PT15M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'ResponseTime'
          metricName: 'requests/duration'
          operator: 'GreaterThan'
          threshold: 2000
          timeAggregation: 'Average'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroupId
      }
    ]
  }
}

// Low Availability Alert
resource lowAvailabilityAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'orange-car-rental-low-availability'
  location: 'global'
  properties: {
    description: 'Alert when availability drops below 99%'
    severity: 1
    enabled: true
    scopes: [
      appInsights.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'Availability'
          metricName: 'availabilityResults/availabilityPercentage'
          operator: 'LessThan'
          threshold: 99
          timeAggregation: 'Average'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroupId
      }
    ]
  }
}

// High Exception Count Alert
resource highExceptionAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'orange-car-rental-high-exceptions'
  location: 'global'
  properties: {
    description: 'Alert when exception count is abnormally high'
    severity: 2
    enabled: true
    scopes: [
      appInsights.id
    ]
    evaluationFrequency: 'PT5M'
    windowSize: 'PT15M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'Exceptions'
          metricName: 'exceptions/count'
          operator: 'GreaterThan'
          threshold: 100
          timeAggregation: 'Total'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroupId
      }
    ]
  }
}

// Database Connection Alert
resource databaseConnectionAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'orange-car-rental-database-slow'
  location: 'global'
  properties: {
    description: 'Alert when database queries are slow (> 500ms avg)'
    severity: 3
    enabled: true
    scopes: [
      appInsights.id
    ]
    evaluationFrequency: 'PT5M'
    windowSize: 'PT15M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'DatabaseDuration'
          metricName: 'dependencies/duration'
          operator: 'GreaterThan'
          threshold: 500
          timeAggregation: 'Average'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroupId
      }
    ]
  }
}

// Memory Usage Alert (for App Service)
resource highMemoryAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = if (environment == 'production') {
  name: 'orange-car-rental-high-memory'
  location: 'global'
  properties: {
    description: 'Alert when memory usage exceeds 85%'
    severity: 2
    enabled: true
    scopes: [
      appInsights.id
    ]
    evaluationFrequency: 'PT5M'
    windowSize: 'PT15M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'MemoryUsage'
          metricName: 'performanceCounters/memoryAvailableBytes'
          operator: 'LessThan'
          threshold: 500000000 // 500 MB
          timeAggregation: 'Average'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroupId
      }
    ]
  }
}

output alertRuleIds array = [
  highErrorRateAlert.id
  slowResponseTimeAlert.id
  lowAvailabilityAlert.id
  highExceptionAlert.id
  databaseConnectionAlert.id
]
