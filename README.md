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

## 免责申明

此项目(ChinaDetector)及其配套组件均为免费开源的产品，仅用于学习交流，并且不直接向中国提供服务，中国用户请于下载后立即删除。

任何中国境内的组织及个人不得使用此项目(ChinaDetector)及其配套组件构建任何形式的面向中国境内用户的网站或服务。

不可用于任何违反中华人民共和国(含台湾省)或使用者所在地区法律法规的用途。

因为作者即本人仅完成代码的开发和开源活动(开源即任何人都可以下载使用)，从未参与用户的任何运营和盈利活动。

且不知晓用户后续将程序源代码用于何种用途，故用户使用过程中所带来的任何法律责任即由用户自己承担。

[《开源软件有漏洞，作者需要负责吗？是的！》](https://go.edi.wang/aka/os251)
