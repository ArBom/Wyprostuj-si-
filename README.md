# Wyprostuj się
Program uses a Kinect sensor to help you hold a healthy body position while you working at the PC. It uses Windows service to run itself with system. Informing is pursue by toast notifications, which are removed if them unactual. Program have polish and english interface.
![Sitting of kinect sensor](https://github.com/user-attachments/assets/19e9b73e-1a58-4886-bccd-53339c4d0f57)

## Projects:
* [Wyprostuj sie](#Wyprostuj-si-)
* [WyprostujSieBackground](#WyprostujSieBackground)
* [ToastPresenter](#ToastPresenter)

![CodeMap](https://github.com/user-attachments/assets/5f5f26e5-eaae-47cd-b939-b8f7f179e7db)

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

![Windows service](https://github.com/user-attachments/assets/e7db07e2-bc06-4aaa-b6d3-251c8ec46c6e)

## ToastPresenter
```
PM> Install-Package Microsoft.Toolkit.Uwp.Notifications -Version 7.1.2
```
![Toast](https://github.com/user-attachments/assets/623c746e-e17a-4aba-a462-454db64cd5ae)
