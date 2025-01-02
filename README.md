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

Not all regions in the +8 time zone use the name "China Standard Time" (CST). 

For example, the +8 time zone also includes the following time zone names:

- Australian Western Standard Time (AWST)
- Singapore Time (SGT)
- Hong Kong Time (HKT)
- Taiwan Standard Time (TST)
- Malaysia Time (MYT)
- Philippine Time (PHT)
- Western Indonesian Time (WIB)

```csharp
var service = new OfflineChinaDetectService();
var result = await service.Detect(DetectionMethod.TimeZone);
```

For Hong Kong and Taiwan, you can configure the `includeHKTW` option to consider them as China. So that you won't go to jail for calling them a country in China.

```csharp
var result = await service.Detect(DetectionMethod.TimeZone, includeHKTW: true);
```

#### By culture

```csharp
var service = new OfflineChinaDetectService();
var result = await service.Detect(DetectionMethod.Culture);
```

#### By behavior

```csharp
var service = new OfflineChinaDetectService();
var result = await service.Detect(DetectionMethod.Behavior);
```

#### Multiple

```csharp
var service = new OfflineChinaDetectService();
var result = await service.Detect(DetectionMethod.TimeZone | DetectionMethod.Culture);
```

or

```csharp
await service.Detect(DetectionMethod.AllOffline);
```

### Online Detection

Note

- In China, IP address or GFW test is not a reliable way to detect if the machine is in China mainland, because many users are using VPN or proxy to access the Internet. Please use it in conjunction with offline detection.

- In reality, please use HttpClientFactory or Typed HttpClient to create HttpClient as a best practice in .NET.

#### By IP

```csharp
var handler = new HttpClientHandler
{
    UseProxy = false
};

var httpClient = new HttpClient(handler)
{
    Timeout = TimeSpan.FromSeconds(5)
};

var service = new OnlineChinaDetectService(httpClient);
await service.Detect(DetectionMethod.IPAddress);
```

#### By GFW test

```csharp
var handler = new HttpClientHandler
{
    UseProxy = false
};

var httpClient = new HttpClient(handler)
{
    Timeout = TimeSpan.FromSeconds(5)
};

var service = new OnlineChinaDetectService(httpClient);
await service.Detect(DetectionMethod.GFWTest);
```

### Dependency Injection

Coming in next release

## 免责申明

此项目(ChinaDetector)及其配套组件均为免费开源的产品，仅用于学习交流，并且不直接向中国提供服务，中国访客请于下载后立即删除。

任何中国境内的组织及个人不得使用此项目(ChinaDetector)及其配套组件构建任何形式的面向中国境内访客的网站或服务。

不可用于任何违反中华人民共和国(含台湾省)或使用者所在地区法律法规的用途。

因为作者即本人仅完成代码的开发和开源活动(开源即任何人都可以下载使用)，从未参与访客的任何运营和盈利活动。

且不知晓访客后续将程序源代码用于何种用途，故访客使用过程中所带来的任何法律责任即由访客自己承担。

[《开源软件有漏洞，作者需要负责吗？是的！》](https://go.edi.wang/aka/os251)
