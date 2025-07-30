<!--
SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>

SPDX-License-Identifier: CC0-1.0
-->

# Open Data Hub - LTS Api Importer Library

A library created for the Open Data Hub Project to import tourism data from the LTS Rest Api.

Use this library to retrieve Tourism Data from the LTS Rest Interface.  
  
In this library the Open Data Hub Tourism Datamodel is used Nuget Package (https://www.nuget.org/packages/opendatahub-datamodel-tourism)

usage Example

```
LtsApi ltsapi = new LtsApi(//Pass valid LTSCredentials here);
var qs = new LTSQueryStrings() { page_size = 1 }; //Add all parameters
var dict = ltsapi.GetLTSQSDictionary(qs); //Create Dictionary with all parameters
var data = await ltsapi.AccommodationDetailRequest(id, dict); //Create a request and pass all parameters
```