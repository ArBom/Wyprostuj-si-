# Wyprostuj się
Program uses a Kinect sensor to help you hold a healthy body position while you working at the PC. It uses Windows service to run itself with system. Informing is pursue by toast notifications, which are removed if them unactual. Program have polish and english interface.
![Sitting of kinect sensor](https://db5pap001files.storage.live.com/y4mbW7yFX3lJFAjXciAay9ES4Iv8lU0kqfiWM9ZQQTChLGOQ2FdxlKCj1eZIL9E16NxprfP2z5R2fnP_aJNyiSIFW8zGbgDRsWRa_2nufQjQS2-OM9WFLT4H_cYAbUv0Twz524awQ1i263yKTf2jppdgfJVuvXlzDw8lgq83VV4W_75t1JKZ52qin2liKXrikqL?width=1024&height=576&cropmode=none "Sitting of kinect sensor")

## Projects:
* [Wyprostuj sie](#Wyprostuj-si-)
* [WyprostujSieBackground](#WyprostujSieBackground)
* [ToastPresenter](#ToastPresenter)

![CodeMap](https://db5pap001files.storage.live.com/y4mwXFWFVfQ_f5mzmdkBJaughNcMKbCddYxRi8jfVtQvjqlBcojU43cd_fMtnX3p93DBGI68Fmit5906K9DEj8LyLfn6Gj1d_a3RCkCUgiuN9uctj0k1aLLlT7-CIPlSvb01X77trlDE2deYv7Dym602WB5gq5R8vBmH7hMvBVlNQQWv07X_CMr8kOqTogKUrL1?width=1675&height=901&cropmode=none "CodeMap")

## Wyprostuj-si-

```
PM> Install-Package Newtonsoft.Json -Version 13.0.1
```

![WinApp](https://github.com/ArBom/Wyprostuj-si-/assets/59375967/8c6591ee-750d-4911-b007-dd9b43cdec56)

|No.  | Sub No. | Descripion                                                         | Comment                                            |
|---  | ---     | ---                                                                | ---                                                |
|1.   |         |Run automatically at Windows startup                                |You have to run app as admin to change this option  |
|2    |a.       |Check it to have info about if you move forward your head to much   |
|     |b.       |Slider to change acceptable value of forward head position ratio    |It's possible to change if CheckBox 2a. is checked  |
|     |c.       |The current forward head position ratio                             |It's red while current position ratio is unaccepted |
|3    |a.       |Check it to have info about if you leaning forward to much          |
|     |b.       |Slider to change acceptable value of leaning body forward ratio     |It's possible to change if CheckBox 3a. is checked  |
|     |c.       |The current leaning body forward ratio                              |It's red while current position ratio is unaccepted |
|4    |a.       |Check it to have info about if you leaning to one side to much      |
|     |b.       |Slider to change acceptable value of leaning body to one side ratio |It's possible to change if CheckBox 4a. is checked  |
|     |c.       |The current leaning body to one side ratio                          |It's red while current position ratio is unaccepted |
|5.   |         |Check to getting information about the lack of detected person      |
|6.   |         |Check to getting information about detected more than one person    |
|7.   |         |Camera view                                                         |
|8.   |         |Status Bar                                                          |
|9.   |         |Detected bodies view                                                |
 
 ## WyprostujSieBackground

```
PM> Install-Package Newtonsoft.Json -Version 13.0.1
PM> Install-Package Microsoft.Kinect -Version 2.0.1410.19000
```

![Windows service](https://db5pap001files.storage.live.com/y4mbSzEgfD6GGa72hN48JTNjZFvzPJd2sFQvhEiGq-835Ed2gfQBrfY4zxIrpDzZb490znzXNi9IB8S9-GctmzqEtGQ8J2ZgVWX39SeL1CKs0DI_gplEeaysyox2y6znaTVZXKQYX-O_6tXa2_7ToVIIRa_MuETr7Cn1C-TGf1HJoEXsqF1jqhHf_6xTC4B3O61?width=1024&height=338&cropmode=none "Windows service")

## ToastPresenter
```
PM> Install-Package Microsoft.Toolkit.Uwp.Notifications -Version 7.1.2
```
![Toast](https://db5pap001files.storage.live.com/y4m1M5DvfOpMGcUc82pNzHDN9cpedVDImo8lpXGSLg0kDJaBurABh4dVUHZ0P1AkOABnxDCbJcJ7QAw6-ZgARlFiaOBcMpa2q-7eyZGUWDFwX0Zzvi_NPMbosfPf3709plD9IYUMb8pHFbX0p_-mA7ZPm-me2WPEQiIMq3aywj8gkZXCbhsetvDUbT_P60gEluM?width=827&height=465&cropmode=none "Showing examle a toast notification")
