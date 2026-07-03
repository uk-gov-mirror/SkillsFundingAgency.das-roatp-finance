## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Roatp Finance Assessment

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status%2FApprenticeships%20Providers%2Fdas-roatp-finance?repoName=SkillsFundingAgency%2Fdas-roatp-finance&branchName=main)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2671&repoName=SkillsFundingAgency%2Fdas-roatp-finance&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-roatp-finance&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-roatp-finance)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

## About

A service which allows EFSA staff members to assess the finance part of RoATP applications.

This mini portal hooks into QNA & RoATP API to get the finanical information of a selected application.

The PMO admin then performs a number of checks and then grades the application appropriately.

In some cases clarification is required. This is an offline check, which the PMO does and then enters the outcome separately.

Once graded, this will show in Oversight when Gateway checks, Blind Assessment & Moderation are completed.

## 🚀 Installation

### Pre-Requisites

* A clone of this repository
* Visual Studio or similar IDE
* A storage emulator (for example Azurite)
* An Staff iDAMS ADFS Active Directory account with the appropriate roles

### Dependencies

* The [das-apply-service](https://github.com/SkillsFundingAgency/das-apply-service) API available either running locally or accessible in an Azure tenancy    
* The [das-qna-api](https://github.com/SkillsFundingAgency/das-qna-api) API available either running locally or accessible in an Azure tenancy

### Config

AppSettings.Development.json file
```json
{
    "Logging": {
      "LogLevel": {
        "Default": "Debug",
        "System": "Information",
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
    "ConnectionStrings": {
      "Redis": ""
    },
    "cdn": {
      "url": "https://das-prd-frnt-end.azureedge.net"
    },
    "EnvironmentName": "LOCAL",
    "Version": "1.0",
  }  
```

Azure Table Storage config

Row Key: SFA.DAS.RoatpFinance_1.0

Partition Key: LOCAL

Data:

```json
{
    "SessionRedisConnectionString": "localhost",
    "SessionCachingDatabase": "DefaultDatabase=0",
    "DataProtectionKeysDatabase": "DefaultDatabase=3",
    "StaffAuthentication":
    {
        "WtRealm": "https://localhost:45669",
        "MetadataAddress": "https://adfs.preprod.skillsfunding.service.gov.uk/FederationMetadata/2007-06/FederationMetadata.xml"
    },
    "RoatpApplicationApiAuthentication":
    {
        "Identifier": "https://tenant.onmicrosoft.com/das-at-applyapi-as-ar",
        "ApiBaseAddress": "https://localhost:6000"
    },
    "QnaApiAuthentication":
    {
        "Identifier": "https://tenant.onmicrosoft.com/das-at-qna-as-ar",
        "ApiBaseAddress": "https://localhost:5555"
    },
    "EsfaAdminServicesBaseUrl": "https://localhost:44347"
}
```

## Technologies

* .Net 10.0
* REDIS
* Refit
* Azure Table Storage
* NUnit
* Moq
* FluentAssertions
