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

### Online Detection

#### By IP

TODO

#### By GFW test

TODO

### Dependency Injection

Coming in next release