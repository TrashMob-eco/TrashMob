param workspaces_law_trashmobdev_name string = 'law-trashmobdev'

resource workspaces_law_trashmobdev_name_resource 'microsoft.operationalinsights/workspaces@2020-10-01' = {
  name: workspaces_law_trashmobdev_name
  location: 'westus2'
  properties: {
    provisioningState: 'Succeeded'
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    features: {
      legacy: 0
      searchVersion: 1
      enableLogAccessUsingOnlyResourcePermissions: true
    }
    workspaceCapping: {
      dailyQuotaGb: -1
    }
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_General_AlphabeticallySortedComputers 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_General|AlphabeticallySortedComputers'
  properties: {
    displayName: 'All Computers with their most recent data'
    category: 'General Exploration'
    query: 'search not(ObjectName == "Advisor Metrics" or ObjectName == "ManagedSpace") | summarize AggregatedValue = max(TimeGenerated) by Computer | limit 500000 | sort by Computer asc\r\n// Oql: NOT(ObjectName="Advisor Metrics" OR ObjectName=ManagedSpace) | measure max(TimeGenerated) by Computer | top 500000 | Sort Computer // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_General_dataPointsPerManagementGroup 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_General|dataPointsPerManagementGroup'
  properties: {
    displayName: 'Which Management Group is generating the most data points?'
    category: 'General Exploration'
    query: 'search * | summarize AggregatedValue = count() by ManagementGroupName\r\n// Oql: * | Measure count() by ManagementGroupName // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_General_dataTypeDistribution 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_General|dataTypeDistribution'
  properties: {
    displayName: 'Distribution of data Types'
    category: 'General Exploration'
    query: 'search * | extend Type = $table | summarize AggregatedValue = count() by Type\r\n// Oql: * | Measure count() by Type // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_General_StaleComputers 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_General|StaleComputers'
  properties: {
    displayName: 'Stale Computers (data older than 24 hours)'
    category: 'General Exploration'
    query: 'search not(ObjectName == "Advisor Metrics" or ObjectName == "ManagedSpace") | summarize lastdata = max(TimeGenerated) by Computer | limit 500000 | where lastdata < ago(24h)\r\n// Oql: NOT(ObjectName="Advisor Metrics" OR ObjectName=ManagedSpace) | measure max(TimeGenerated) as lastdata by Computer | top 500000 | where lastdata < NOW-24HOURS // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_AllEvents 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|AllEvents'
  properties: {
    displayName: 'All Events'
    category: 'Log Management'
    query: 'Event | sort by TimeGenerated desc\r\n// Oql: Type=Event // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_AllSyslog 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|AllSyslog'
  properties: {
    displayName: 'All Syslogs'
    category: 'Log Management'
    query: 'Syslog | sort by TimeGenerated desc\r\n// Oql: Type=Syslog // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_AllSyslogByFacility 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|AllSyslogByFacility'
  properties: {
    displayName: 'All Syslog Records grouped by Facility'
    category: 'Log Management'
    query: 'Syslog | summarize AggregatedValue = count() by Facility\r\n// Oql: Type=Syslog | Measure count() by Facility // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_AllSyslogByProcessName 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|AllSyslogByProcessName'
  properties: {
    displayName: 'All Syslog Records grouped by ProcessName'
    category: 'Log Management'
    query: 'Syslog | summarize AggregatedValue = count() by ProcessName\r\n// Oql: Type=Syslog | Measure count() by ProcessName // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_AllSyslogsWithErrors 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|AllSyslogsWithErrors'
  properties: {
    displayName: 'All Syslog Records with Errors'
    category: 'Log Management'
    query: 'Syslog | where SeverityLevel == "error" | sort by TimeGenerated desc\r\n// Oql: Type=Syslog SeverityLevel=error // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_AverageHTTPRequestTimeByClientIPAddress 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|AverageHTTPRequestTimeByClientIPAddress'
  properties: {
    displayName: 'Average HTTP Request time by Client IP Address'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = avg(TimeTaken) by cIP\r\n// Oql: Type=W3CIISLog | Measure Avg(TimeTaken) by cIP // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_AverageHTTPRequestTimeHTTPMethod 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|AverageHTTPRequestTimeHTTPMethod'
  properties: {
    displayName: 'Average HTTP Request time by HTTP Method'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = avg(TimeTaken) by csMethod\r\n// Oql: Type=W3CIISLog | Measure Avg(TimeTaken) by csMethod // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_CountIISLogEntriesClientIPAddress 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|CountIISLogEntriesClientIPAddress'
  properties: {
    displayName: 'Count of IIS Log Entries by Client IP Address'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by cIP\r\n// Oql: Type=W3CIISLog | Measure count() by cIP // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_CountIISLogEntriesHTTPRequestMethod 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|CountIISLogEntriesHTTPRequestMethod'
  properties: {
    displayName: 'Count of IIS Log Entries by HTTP Request Method'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by csMethod\r\n// Oql: Type=W3CIISLog | Measure count() by csMethod // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_CountIISLogEntriesHTTPUserAgent 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|CountIISLogEntriesHTTPUserAgent'
  properties: {
    displayName: 'Count of IIS Log Entries by HTTP User Agent'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by csUserAgent\r\n// Oql: Type=W3CIISLog | Measure count() by csUserAgent // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_CountOfIISLogEntriesByHostRequestedByClient 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|CountOfIISLogEntriesByHostRequestedByClient'
  properties: {
    displayName: 'Count of IIS Log Entries by Host requested by client'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by csHost\r\n// Oql: Type=W3CIISLog | Measure count() by csHost // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_CountOfIISLogEntriesByURLForHost 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|CountOfIISLogEntriesByURLForHost'
  properties: {
    displayName: 'Count of IIS Log Entries by URL for the host "www.contoso.com" (replace with your own)'
    category: 'Log Management'
    query: 'search csHost == "www.contoso.com" | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by csUriStem\r\n// Oql: Type=W3CIISLog csHost="www.contoso.com" | Measure count() by csUriStem // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_CountOfIISLogEntriesByURLRequestedByClient 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|CountOfIISLogEntriesByURLRequestedByClient'
  properties: {
    displayName: 'Count of IIS Log Entries by URL requested by client (without query strings)'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by csUriStem\r\n// Oql: Type=W3CIISLog | Measure count() by csUriStem // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_CountOfWarningEvents 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|CountOfWarningEvents'
  properties: {
    displayName: 'Count of Events with level "Warning" grouped by Event ID'
    category: 'Log Management'
    query: 'Event | where EventLevelName == "warning" | summarize AggregatedValue = count() by EventID\r\n// Oql: Type=Event EventLevelName=warning | Measure count() by EventID // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_DisplayBreakdownRespondCodes 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|DisplayBreakdownRespondCodes'
  properties: {
    displayName: 'Shows breakdown of response codes'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by scStatus\r\n// Oql: Type=W3CIISLog | Measure count() by scStatus // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_EventsByEventLog 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|EventsByEventLog'
  properties: {
    displayName: 'Count of Events grouped by Event Log'
    category: 'Log Management'
    query: 'Event | summarize AggregatedValue = count() by EventLog\r\n// Oql: Type=Event | Measure count() by EventLog // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_EventsByEventsID 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|EventsByEventsID'
  properties: {
    displayName: 'Count of Events grouped by Event ID'
    category: 'Log Management'
    query: 'Event | summarize AggregatedValue = count() by EventID\r\n// Oql: Type=Event | Measure count() by EventID // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_EventsByEventSource 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|EventsByEventSource'
  properties: {
    displayName: 'Count of Events grouped by Event Source'
    category: 'Log Management'
    query: 'Event | summarize AggregatedValue = count() by Source\r\n// Oql: Type=Event | Measure count() by Source // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_EventsInOMBetween2000to3000 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|EventsInOMBetween2000to3000'
  properties: {
    displayName: 'Events in the Operations Manager Event Log whose Event ID is in the range between 2000 and 3000'
    category: 'Log Management'
    query: 'Event | where EventLog == "Operations Manager" and EventID >= 2000 and EventID <= 3000 | sort by TimeGenerated desc\r\n// Oql: Type=Event EventLog="Operations Manager" EventID:[2000..3000] // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_EventsWithStartedinEventID 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|EventsWithStartedinEventID'
  properties: {
    displayName: 'Count of Events containing the word "started" grouped by EventID'
    category: 'Log Management'
    query: 'search in (Event) "started" | summarize AggregatedValue = count() by EventID\r\n// Oql: Type=Event "started" | Measure count() by EventID // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_FindMaximumTimeTakenForEachPage 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|FindMaximumTimeTakenForEachPage'
  properties: {
    displayName: 'Find the maximum time taken for each page'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = max(TimeTaken) by csUriStem\r\n// Oql: Type=W3CIISLog | Measure Max(TimeTaken) by csUriStem // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_IISLogEntriesForClientIP 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|IISLogEntriesForClientIP'
  properties: {
    displayName: 'IIS Log Entries for a specific client IP Address (replace with your own)'
    category: 'Log Management'
    query: 'search cIP == "192.168.0.1" | extend Type = $table | where Type == W3CIISLog | sort by TimeGenerated desc | project csUriStem, scBytes, csBytes, TimeTaken, scStatus\r\n// Oql: Type=W3CIISLog cIP="192.168.0.1" | Select csUriStem,scBytes,csBytes,TimeTaken,scStatus // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_ListAllIISLogEntries 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|ListAllIISLogEntries'
  properties: {
    displayName: 'All IIS Log Entries'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | sort by TimeGenerated desc\r\n// Oql: Type=W3CIISLog // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_NoOfConnectionsToOMSDKService 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|NoOfConnectionsToOMSDKService'
  properties: {
    displayName: 'How many connections to Operations Manager\'s SDK service by day'
    category: 'Log Management'
    query: 'Event | where EventID == 26328 and EventLog == "Operations Manager" | summarize AggregatedValue = count() by bin(TimeGenerated, 1d) | sort by TimeGenerated desc\r\n// Oql: Type=Event EventID=26328 EventLog="Operations Manager" | Measure count() interval 1DAY // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_ServerRestartTime 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|ServerRestartTime'
  properties: {
    displayName: 'When did my servers initiate restart?'
    category: 'Log Management'
    query: 'search in (Event) "shutdown" and EventLog == "System" and Source == "User32" and EventID == 1074 | sort by TimeGenerated desc | project TimeGenerated, Computer\r\n// Oql: shutdown Type=Event EventLog=System Source=User32 EventID=1074 | Select TimeGenerated,Computer // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_Show404PagesList 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|Show404PagesList'
  properties: {
    displayName: 'Shows which pages people are getting a 404 for'
    category: 'Log Management'
    query: 'search scStatus == 404 | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by csUriStem\r\n// Oql: Type=W3CIISLog scStatus=404 | Measure count() by csUriStem // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_ShowServersThrowingInternalServerError 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|ShowServersThrowingInternalServerError'
  properties: {
    displayName: 'Shows servers that are throwing internal server error'
    category: 'Log Management'
    query: 'search scStatus == 500 | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by sComputerName\r\n// Oql: Type=W3CIISLog scStatus=500 | Measure count() by sComputerName // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_TotalBytesReceivedByEachAzureRoleInstance 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|TotalBytesReceivedByEachAzureRoleInstance'
  properties: {
    displayName: 'Total Bytes received by each Azure Role Instance'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = sum(csBytes) by RoleInstance\r\n// Oql: Type=W3CIISLog | Measure Sum(csBytes) by RoleInstance // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_TotalBytesReceivedByEachIISComputer 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|TotalBytesReceivedByEachIISComputer'
  properties: {
    displayName: 'Total Bytes received by each IIS Computer'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = sum(csBytes) by Computer | limit 500000\r\n// Oql: Type=W3CIISLog | Measure Sum(csBytes) by Computer | top 500000 // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_TotalBytesRespondedToClientsByClientIPAddress 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|TotalBytesRespondedToClientsByClientIPAddress'
  properties: {
    displayName: 'Total Bytes responded back to clients by Client IP Address'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = sum(scBytes) by cIP\r\n// Oql: Type=W3CIISLog | Measure Sum(scBytes) by cIP // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_TotalBytesRespondedToClientsByEachIISServerIPAddress 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|TotalBytesRespondedToClientsByEachIISServerIPAddress'
  properties: {
    displayName: 'Total Bytes responded back to clients by each IIS ServerIP Address'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = sum(scBytes) by sIP\r\n// Oql: Type=W3CIISLog | Measure Sum(scBytes) by sIP // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_TotalBytesSentByClientIPAddress 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|TotalBytesSentByClientIPAddress'
  properties: {
    displayName: 'Total Bytes sent by Client IP Address'
    category: 'Log Management'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = sum(csBytes) by cIP\r\n// Oql: Type=W3CIISLog | Measure Sum(csBytes) by cIP // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_WarningEvents 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|WarningEvents'
  properties: {
    displayName: 'All Events with level "Warning"'
    category: 'Log Management'
    query: 'Event | where EventLevelName == "warning" | sort by TimeGenerated desc\r\n// Oql: Type=Event EventLevelName=warning // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_WindowsFireawallPolicySettingsChanged 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|WindowsFireawallPolicySettingsChanged'
  properties: {
    displayName: 'Windows Firewall Policy settings have changed'
    category: 'Log Management'
    query: 'Event | where EventLog == "Microsoft-Windows-Windows Firewall With Advanced Security/Firewall" and EventID == 2008 | sort by TimeGenerated desc\r\n// Oql: Type=Event EventLog="Microsoft-Windows-Windows Firewall With Advanced Security/Firewall" EventID=2008 // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_law_trashmobdev_name_LogManagement_workspaces_law_trashmobdev_name_LogManagement_WindowsFireawallPolicySettingsChangedByMachines 'Microsoft.OperationalInsights/workspaces/savedSearches@2020-08-01' = {
  name: '${workspaces_law_trashmobdev_name_resource.name}/LogManagement(${workspaces_law_trashmobdev_name})_LogManagement|WindowsFireawallPolicySettingsChangedByMachines'
  properties: {
    displayName: 'On which machines and how many times have Windows Firewall Policy settings changed'
    category: 'Log Management'
    query: 'Event | where EventLog == "Microsoft-Windows-Windows Firewall With Advanced Security/Firewall" and EventID == 2008 | summarize AggregatedValue = count() by Computer | limit 500000\r\n// Oql: Type=Event EventLog="Microsoft-Windows-Windows Firewall With Advanced Security/Firewall" EventID=2008 | measure count() by Computer | top 500000 // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}