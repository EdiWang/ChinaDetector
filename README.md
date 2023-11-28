# Edi.ChinaDetector

Detect if current code is running on China machine

## Install

```powershell
Install-Package Edi.ChinaDetector
```

```bash
dotnet add package Edi.ChinaDetector
```

## Examples

`Rank` in the detection result indicates the possiblity that current code is running on a China machine, the higher the more possible. When using multiple detectors, if `Rank >= 2` then you may consider the code is running on a China machine.

### Offline Detection


#### By time zone

```csharp
var service = new OfflineChinaDetectService();
var result = await service.Detect(DetectionMethod.TimeZone);
```

#### By culture

```csharp
var service = new OfflineChinaDetectService();
var result = await service.Detect(DetectionMethod.Culture);
```

#### Both

```csharp
var service = new OfflineChinaDetectService();
var result = await service.Detect(DetectionMethod.TimeZone | DetectionMethod.Culture);
```

or

```csharp
await service.Detect(DetectionMethod.AllOffline);
```

### Online Detection

#### By IP

TODO

#### By GFW test

TODO

### Dependency Injection

Coming in next release